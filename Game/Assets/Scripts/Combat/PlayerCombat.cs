// BAYANI — Player combat core (phasing.md 1.3–1.6): L→L→L→H combo chain with a
// 0.25s input buffer, dodge roll with i-frames + PERFECT DODGE reward, Diwa skill.
// Desktop mapping (ggd §41 subset): LMB attack · RMB dodge · Q skill. Block/parry: next step.
// All timings come from AttackData ScriptableObjects — nothing hand-tuned in code (§56 exit gate).

using System.Collections;
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
        public AttackData skill;          // Q — Diwa burst

        [Header("Dodge (ggd §56.2)")]
        [SerializeField] private float dodgeStaminaCost = 20f;
        [SerializeField] private float dodgeDuration = 0.35f;
        [SerializeField] private float dodgeSpeed = 8f;
        [SerializeField] private float dodgeCooldown = 0.5f;
        [SerializeField] private float perfectDodgeWindow = 0.25f;  // hit blocked this soon into roll
        [SerializeField] private float perfectDodgeDiwa = 8f;

        [Header("Feel (phasing 1.6)")]
        [SerializeField] private float attackDriftSpeed = 1.6f;     // slight forward step on swings
        [SerializeField] private float hitStopOnLanding = 0.06f;

        public bool InAction => _attackRoutine != null || _rolling;
        public string StateText { get; private set; } = "Ready";

        private PlayerController _controller;
        private Coroutine _attackRoutine;
        private bool _rolling;
        private float _lastDodge = -10f;
        private float _rollStartedAt = -10f;
        private bool _perfectScoredThisRoll;
        private int _comboIndex;              // next attack in chain
        private bool _attackPressBuffered;

        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            if (hitbox != null) hitbox.OnLanded += OnAttackLanded;
            if (hurtbox != null) hurtbox.OnBlockedByIFrame += OnBlockedByIFrame;
        }

        private void OnDestroy()
        {
            if (hitbox != null) hitbox.OnLanded -= OnAttackLanded;
            if (hurtbox != null) hurtbox.OnBlockedByIFrame -= OnBlockedByIFrame;
        }

        private void Update()
        {
            var mouse = Mouse.current;
            var kb = Keyboard.current;
            if (mouse == null || kb == null) return;

            // --- Buffer attack input (phasing 1.3: forgiving windows) ---
            if (mouse.leftButton.wasPressedThisFrame) _attackPressBuffered = true;

            if (InAction) return;   // chains handled inside AttackRoutine

            // --- Start a new chain or advance buffered ---
            if (_attackPressBuffered)
            {
                _attackPressBuffered = false;
                TryStartAttack(_comboIndex);
            }

            // --- Dodge roll ---
            if (mouse.rightButton.wasPressedThisFrame && Time.time - _lastDodge >= dodgeCooldown
                && resources != null && resources.CanAffordStamina(dodgeStaminaCost))
            {
                _attackRoutine = null;   // cancel recovery — dodge is your get-out
                _lastDodge = Time.time;
                _rolling = true;
                _rollStartedAt = Time.time;
                _perfectScoredThisRoll = false;
                if (hitbox != null) hitbox.DeactivateWindow();
                SetMoveEnabled(false);
                resources.SpendStamina(dodgeStaminaCost);
                _attackRoutine = StartCoroutine(DodgeRoutine());
            }

            // --- Diwa skill (Q) ---
            if (skill != null && kb.dKey.wasPressedThisFrame && resources != null
                && resources.CanAffordDiwa(skill.diwaCost))
            {
                resources.SpendDiwa(skill.diwaCost);
                _attackRoutine = StartCoroutine(AttackRoutine(skill));
            }
        }

        /// <summary>Single place that picks, validates and advances the combo index.</summary>
        private void TryStartAttack(int index)
        {
            if (combo == null || combo.Length == 0) return;
            var a = combo[Mathf.Clamp(index, 0, combo.Length - 1)];
            if (!CanStart(a)) return;
            // heavy finisher resets the chain; otherwise step forward
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
            _rolling = false;
            SetMoveEnabled(false);
            if (resources != null && a.staminaCost > 0f) resources.SpendStamina(a.staminaCost);

            var cc = GetComponent<CharacterController>();
            Vector3 drift = transform.forward * attackDriftSpeed * (a.isHeavy ? 1.5f : 1f);

            // WINDUP — buffered press during windup queues the next chain link early
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

            // RECOVERY — chain window: buffered or fresh press advances the combo
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
        private bool _bufferHeldEarly;

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

        private void OnAttackLanded(Hurtbox target)
        {
            resources?.AddDiwa(2f);                    // hit connected: +2 Diwa (§56.2)
            HitStop.Play(this, hitStopOnLanding);      // the FEEL (§1.6)
        }

        private void OnBlockedByIFrame()
        {
            if (_rolling && Time.time - _rollStartedAt <= perfectDodgeWindow && !_perfectScoredThisRoll)
            {
                _perfectScoredThisRoll = true;
                resources?.AddDiwa(perfectDodgeDiwa);  // perfect dodge: +8 Diwa
                StateText = "PERFECT DODGE";
                Debug.Log("[BAYANI] Perfect dodge! +8 Diwa");
            }
        }

        /// <summary>Kill reward hook for enemies (ggd §56.2: kill = +6 Diwa).</summary>
        public void OnKillConfirmed(Enemy.EnemyData data)
        {
            resources?.AddDiwa(data.diwaOnKill);
        }

        private void SetMoveEnabled(bool value)
        {
            if (_controller != null) _controller.ControlsEnabled = value;
        }
    }
}
