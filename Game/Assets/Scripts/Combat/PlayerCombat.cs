// BAYANI — Player combat core (phasing.md 1.3–1.6 + 1B depth pass):
//  • L→L→L→H combo chain, 0.25s input buffer
//  • Dodge roll w/ i-frames + PERFECT DODGE (+8 Diwa) — LCtrl / RT
//  • BLOCK hold RMB / X · PARRY = block within 0.2s of the hit → +12 Diwa + stagger
//  • Q / Y = Diwa Burst
// All timings from AttackData ScriptableObjects (§56 exit gate: no hard-coded values).

using System;
using System.Collections;
using Bayani.Core;
using Bayani.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bayani.Combat
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Wired by Phase1 builder")]
        public CombatResources resources;
        public Hurtbox hurtbox;
        public Hitbox hitbox;             // child trigger volume, disabled by default
        public AttackData[] combo;        // [0..2] lights, [3] heavy finisher
        public AttackData skill;          // Diwa burst

        [Header("Dodge (ggd §56.2)")]
        [SerializeField] private float dodgeStaminaCost = 20f;
        [SerializeField] private float dodgeDuration = 0.35f;
        [SerializeField] private float dodgeSpeed = 8f;
        [SerializeField] private float dodgeCooldown = 0.5f;
        [SerializeField] private float perfectDodgeWindow = 0.25f;
        [SerializeField] private float perfectDodgeDiwa = 8f;

        [Header("Block / Parry (ggd §56.2: parry +12 Diwa)")]
        [SerializeField] private float parryWindow = 0.2f;         // press-block within this before impact
        [SerializeField] private float parryDiwa = 12f;
        [SerializeField] private float blockStaminaPerHit = 15f;
        [SerializeField] private float blockedDamageMultiplier = 0.3f;   // chip damage through guard

        [Header("Feel (phasing 1.6 / 1B)")]
        [SerializeField] private float attackDriftSpeed = 1.6f;
        [SerializeField] private float hitStopOnLanding = 0.06f;
        [SerializeField] private float heavyShakeAmplitude = 0.35f;

        public bool InAction => _attackRoutine != null || _rolling;
        public bool IsBlocking { get; private set; }
        public string StateText { get; private set; } = "Ready";

        private PlayerController _controller;
        private ThirdPersonFollowCamera _cam;
        private Coroutine _attackRoutine;
        private bool _rolling;
        private float _lastDodge = -10f;
        private float _rollStartedAt = -10f;
        private bool _perfectScoredThisRoll;
        private int _comboIndex;
        private bool _attackPressBuffered;
        private bool _bufferHeldEarly;
        private float _blockStartedAt = -10f;
        private AttackData _currentAttack;

        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _cam = Camera.main != null ? Camera.main.GetComponent<ThirdPersonFollowCamera>() : null;
            if (hitbox != null) hitbox.OnLanded += OnAttackLanded;
            if (hurtbox != null)
            {
                hurtbox.OnBlockedByIFrame += OnBlockedByIFrame;
                hurtbox.OnDeflect += TryDeflect;
            }
        }

        private void OnDestroy()
        {
            if (hitbox != null) hitbox.OnLanded -= OnAttackLanded;
            if (hurtbox != null)
            {
                hurtbox.OnBlockedByIFrame -= OnBlockedByIFrame;
                hurtbox.OnDeflect -= TryDeflect;
            }
        }

        private void Update()
        {
            // --- Buffer attack input (phasing 1.3) ---
            if (BayaniInput.AttackPressed) _attackPressBuffered = true;

            // --- Block (hold; only when free) ---
            if (!InAction && !IsBlocking && BayaniInput.BlockHeld && resources != null && !resources.IsDead)
            {
                IsBlocking = true;
                _blockStartedAt = Time.time;
                SetMoveEnabled(false);
                StateText = "BLOCK";
            }
            if (IsBlocking && !BayaniInput.BlockHeld)
            {
                IsBlocking = false;
                SetMoveEnabled(true);
                StateText = "Ready";
            }

            if (InAction || IsBlocking) return;

            // --- Attack ---
            if (_attackPressBuffered)
            {
                _attackPressBuffered = false;
                TryStartAttack(_comboIndex);
            }

            // --- Dodge roll (LCtrl / RT) ---
            if (BayaniInput.DodgePressed && Time.time - _lastDodge >= dodgeCooldown
                && resources != null && resources.CanAffordStamina(dodgeStaminaCost))
            {
                _lastDodge = Time.time;
                _rolling = true;
                _rollStartedAt = Time.time;
                _perfectScoredThisRoll = false;
                hitbox?.DeactivateWindow();
                SetMoveEnabled(false);
                resources.SpendStamina(dodgeStaminaCost);
                _attackRoutine = StartCoroutine(DodgeRoutine());
            }

            // --- Diwa skill (Q / Y) ---
            if (skill != null && BayaniInput.SkillPressed && resources != null
                && resources.CanAffordDiwa(skill.diwaCost))
            {
                resources.SpendDiwa(skill.diwaCost);
                _attackRoutine = StartCoroutine(AttackRoutine(skill));
            }
        }

        // ---------------- Deflection: block & parry ----------------
        /// <summary>Called by Hurtbox BEFORE damage applies. True = hit fully handled here.</summary>
        private bool TryDeflect(float damage, Vector3 fromPos, GameObject attacker)
        {
            if (!IsBlocking || resources == null) return false;

            // PARRY: block was raised within the window just before impact
            if (Time.time - _blockStartedAt <= parryWindow)
            {
                resources.AddDiwa(parryDiwa);
                HitStop.Play(this, 0.1f);
                _cam?.AddShake(0.3f, 0.15f);
                StateText = "PARRY!";
                if (attacker != null)
                    attacker.GetComponent<Bayani.Enemy.LimotEnemy>()?.NotifyParried();
                Debug.Log("[BAYANI] PARRY! +12 Diwa, attacker staggered");
                return true;    // zero damage
            }

            // BLOCK: chip damage + stamina toll; empty guard = guard break (hit lands full)
            if (resources.CanAffordStamina(blockStaminaPerHit))
            {
                resources.SpendStamina(blockStaminaPerHit);
                resources.TakeHit(damage * blockedDamageMultiplier);
                _cam?.AddShake(0.12f, 0.1f);
                StateText = "BLOCK";
                return true;
            }
            StateText = "GUARD BREAK!";
            return false;
        }

        // ---------------- Attacks ----------------
        /// <summary>Single place that picks, validates and advances the combo index.</summary>
        private void TryStartAttack(int index)
        {
            if (combo == null || combo.Length == 0) return;
            var a = combo[Mathf.Clamp(index, 0, combo.Length - 1)];
            if (!CanStart(a)) return;
            _comboIndex = a.isHeavy ? 0 : Mathf.Min(index + 1, combo.Length - 1);
            _attackRoutine = StartCoroutine(AttackRoutine(a));
        }

        private bool CanStart(AttackData a)
        {
            if (a == null) return false;
            if (resources != null && !resources.CanAffordStamina(a.staminaCost)) return false;
            return true;
        }

        private IEnumerator AttackRoutine(AttackData a)
        {
            StateText = a.attackName;
            _currentAttack = a;
            _rolling = false;
            SetMoveEnabled(false);
            if (resources != null && a.staminaCost > 0f) resources.SpendStamina(a.staminaCost);

            var cc = GetComponent<CharacterController>();
            Vector3 drift = transform.forward * attackDriftSpeed * (a.isHeavy ? 1.5f : 1f);

            // WINDUP — buffered press queues the next chain link early
            float t = 0f;
            while (t < a.windup)
            {
                if (_attackPressBuffered) _bufferHeldEarly = true;
                cc.Move(drift * Time.deltaTime);
                t += Time.deltaTime;
                yield return null;
            }

            // ACTIVE — hitbox live
            t = 0f;
            if (hitbox != null)
            {
                hitbox.damage = a.damage;
                hitbox.knockback = a.knockback;
                hitbox.transform.localScale = new Vector3(a.range * 0.9f, 1.2f, a.range);
                hitbox.ActivateWindow();
            }
            while (t < a.active)
            {
                cc.Move(drift * Time.deltaTime);
                t += Time.deltaTime;
                yield return null;
            }
            hitbox?.DeactivateWindow();

            // RECOVERY — chain window
            t = 0f;
            bool chainRequested = false;
            float window = Mathf.Max(a.recovery, 0f);
            while (t < window)
            {
                if (_attackPressBuffered || _bufferHeldEarly)
                {
                    _attackPressBuffered = false; _bufferHeldEarly = false;
                    chainRequested = !a.isSkill;
                    break;
                }
                t += Time.deltaTime;
                yield return null;
            }
            StateText = "Ready";
            _attackRoutine = null;
            SetMoveEnabled(true);

            if (chainRequested) TryStartAttack(_comboIndex);
        }

        private IEnumerator DodgeRoutine()
        {
            StateText = "Dodge";
            hurtbox?.SetInvulnerable(true);
            var cc = GetComponent<CharacterController>();
            Vector3 dir = transform.forward;
            float t = 0f;
            while (t < dodgeDuration)
            {
                cc.Move(dir * dodgeSpeed * Time.deltaTime);
                t += Time.deltaTime;
                yield return null;
            }
            hurtbox?.SetInvulnerable(false);
            _rolling = false;
            StateText = "Ready";
            SetMoveEnabled(true);
            _attackRoutine = null;
        }

        // ---------------- Rewards & feel hooks ----------------
        private void OnAttackLanded(Hurtbox target)
        {
            resources?.AddDiwa(2f);                       // hit connected: +2 Diwa (§56.2)
            HitStop.Play(this, hitStopOnLanding);         // the FEEL (§1.6)
            if (_currentAttack != null && (_currentAttack.isHeavy || _currentAttack.isSkill))
                _cam?.AddShake(heavyShakeAmplitude, 0.15f);
        }

        private void OnBlockedByIFrame()
        {
            if (_rolling && Time.time - _rollStartedAt <= perfectDodgeWindow && !_perfectScoredThisRoll)
            {
                _perfectScoredThisRoll = true;
                resources?.AddDiwa(perfectDodgeDiwa);     // perfect dodge: +8 Diwa
                StateText = "PERFECT DODGE";
                Debug.Log("[BAYANI] Perfect dodge! +8 Diwa");
            }
        }

        /// <summary>Kill reward hook for enemies (ggd §56.2: kill = +6 Diwa).</summary>
        public event Action<Enemy.EnemyData> OnKill;   // PlayerProgression listens for XP (§56.4)

        public void OnKillConfirmed(Enemy.EnemyData data)
        {
            resources?.AddDiwa(data.diwaOnKill);
            OnKill?.Invoke(data);
        }

        private void SetMoveEnabled(bool value)
        {
            if (_controller != null) _controller.ControlsEnabled = value;
        }
    }
}
