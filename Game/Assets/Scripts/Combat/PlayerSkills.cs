// BAYANI — Active skills (docs/prd-progression.md §3–5).
// 5-slot bar on keys 1–5. The bar CONTENT depends on weapon state (§4):
// armed → the arnis weapon skills, stowed → the unarmed arts; slot 5 stays
// RESERVED for future skills/passives (only 4+4 exist so far).
// Rules honored: shared Stamina pool as mana, cooldowns per skill, cast only
// when affordable + off cooldown + unlocked by level, hitbox opens at hitDelay
// (script-side animation event), Animator trigger + optional clip, graybox VFX
// flash and sound cue placeholder on every cast.

using System.Collections;
using System.Collections.Generic;
using Bayani.Core;
using Bayani.Player;
using UnityEngine;

namespace Bayani.Combat
{
    public class PlayerSkills : MonoBehaviour
    {
        public const int SlotCount = 5;

        [Header("Wired by installer")]
        public PlayerCombat combat;
        public SkillData[] loadout;         // all owned skills; bar = filter by category

        [Tooltip("Skills per category bar — first 4 slots; slot 5 stays reserved (§4).")]
        public int skillsPerBar = 4;

        public bool Armed { get; private set; } = true;

        private readonly SkillData[] _bar = new SkillData[SlotCount];
        private readonly float[] _cooldown = new float[SlotCount];
        private PlayerController _controller;
        private Animator _animator;
        private CharacterController _cc;
        private Coroutine _castRoutine;
        private readonly HashSet<string> _warnedClips = new HashSet<string>();

        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _animator = GetComponentInChildren<Animator>();   // null until Kai gets a rig — all paths guard
            _cc = GetComponent<CharacterController>();
            RebuildBar();
        }

        private void Update()
        {
            for (int i = 0; i < SlotCount; i++)
                if (_cooldown[i] > 0f) _cooldown[i] = Mathf.Max(0f, _cooldown[i] - Time.deltaTime);

            if (BayaniInput.WeaponTogglePressed)
            {
                Armed = !Armed;
                RebuildBar();
                ProgressStore.Toast = Armed ? "Weapon ready — armed skills" : "Weapon stowed — unarmed arts";
                return;
            }

            if (combat == null || combat.resources == null) return;
            if (_castRoutine != null) return;                      // one cast at a time
            if (combat.InAction || combat.IsBlocking) return;      // don't stack with combo/dodge/block

            for (int i = 0; i < SlotCount; i++)
                if (BayaniInput.SkillSlotPressed(i)) { TryCast(i); break; }
        }

        private void RebuildBar()
        {
            for (int i = 0; i < SlotCount; i++) _bar[i] = null;
            if (loadout == null) return;
            var cat = Armed ? SkillCategory.Armed : SkillCategory.Unarmed;
            int n = 0;
            foreach (var s in loadout)
                if (s != null && s.category == cat && n < Mathf.Min(skillsPerBar, SlotCount - 1))
                    _bar[n++] = s;
            // _bar[SlotCount-1] stays null = the reserved slot (§4)
        }

        // ---- HUD surface ----
        public string SlotName(int i) => _bar[i] == null ? (i == SlotCount - 1 ? "RSVD" : "") : _bar[i].skillName;
        public bool SlotArmed(int i) => _bar[i] != null;
        public bool SlotLocked(int i) => _bar[i] != null && ProgressStore.Level < _bar[i].unlockLevel;
        public float SlotCooldown(int i) => _bar[i] == null ? 0f : _cooldown[i];
        public float SlotCooldownMax(int i) => _bar[i] == null ? 1f : _bar[i].cooldown;
        public int SlotUnlockLevel(int i) => _bar[i] == null ? 0 : _bar[i].unlockLevel;

        // ---- casting ----
        private void TryCast(int slot)
        {
            var s = _bar[slot];
            if (s == null)
            {
                ProgressStore.Toast = slot == SlotCount - 1 ? "Slot reserved — future skill/passive" : "Empty skill slot";
                return;
            }
            if (ProgressStore.Level < s.unlockLevel)
            {
                ProgressStore.Toast = $"{s.skillName} locked — unlocks at Lvl {s.unlockLevel}";
                return;
            }
            if (_cooldown[slot] > 0f) return;   // silent: the HUD shows the timer

            var res = combat.resources;
            bool canPay = s.costType == SkillCostType.Diwa
                ? res.CanAffordDiwa(s.cost) : res.CanAffordStamina(s.cost);
            if (!canPay)   // spend never exceeds the pool — only cast when affordable (§3)
            {
                ProgressStore.Toast = $"Not enough {(s.costType == SkillCostType.Diwa ? "DIWA" : "STAMINA")}";
                return;
            }

            if (s.costType == SkillCostType.Diwa) res.SpendDiwa(s.cost); else res.SpendStamina(s.cost);
            _cooldown[slot] = s.cooldown;
            _castRoutine = StartCoroutine(CastRoutine(s));
        }

        private IEnumerator CastRoutine(SkillData s)
        {
            // Animator hook (§5): trigger if the controller has the param; clip if assigned.
            if (_animator != null && !string.IsNullOrEmpty(s.animatorParam))
            {
                foreach (var p in _animator.parameters)
                    if (p.name == s.animatorParam && p.type == AnimatorControllerParameterType.Trigger)
                    { _animator.SetTrigger(s.animatorParam); break; }
            }
            if (_animator != null && s.clip != null) _animator.Play(s.clip.name, 0, 0f);
            else if (s.clip != null && _animator == null && !_warnedClips.Contains(s.skillName))
            {
                _warnedClips.Add(s.skillName);
                Debug.Log($"[BAYANI] '{s.skillName}' has a clip but Kai has no Animator yet (Mixamo rig pending) — procedural fallback.");
            }

            SpawnCastFx(s);
            if (s.castSound != null) AudioSource.PlayClipAtPoint(s.castSound, transform.position + Vector3.up);

            // WINDUP → hit at hitDelay (script-side animation event), driving a short lunge.
            if (_controller != null) _controller.ControlsEnabled = false;
            float t = 0f;
            while (t < s.hitDelay)
            {
                if (_cc != null) _cc.Move(transform.forward * s.lungeSpeed * Time.deltaTime);
                t += Time.deltaTime;
                yield return null;
            }

            // ACTIVE — reuse the player hitbox; same damage pipeline as combo attacks.
            var hb = combat.hitbox;
            if (hb != null)
            {
                hb.damage = s.damage;
                hb.knockback = s.knockback;
                hb.transform.localScale = new Vector3(s.range * 0.9f, 1.2f, s.range);
                hb.ActivateWindow();
            }
            t = 0f;
            while (t < s.hitDuration)
            {
                if (_cc != null) _cc.Move(transform.forward * s.lungeSpeed * 0.4f * Time.deltaTime);
                t += Time.deltaTime;
                yield return null;
            }
            hb?.DeactivateWindow();

            if (_controller != null) _controller.ControlsEnabled = true;
            _castRoutine = null;
        }

        /// <summary>Graybox VFX placeholder: a fading glow quad at the chest (PRD §5).</summary>
        private void SpawnCastFx(SkillData s)
        {
            var fx = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(fx.GetComponent<Collider>());
            fx.name = "SkillFx";
            fx.transform.position = transform.position + transform.forward * 0.8f + Vector3.up * 1.1f;
            fx.transform.localScale = Vector3.one * 0.4f;
            fx.transform.rotation = Quaternion.LookRotation(-transform.forward);
            var r = fx.GetComponent<MeshRenderer>();
            var mpb = new MaterialPropertyBlock();
            mpb.SetColor("_BaseColor", s.castVfxColor);
            r.SetPropertyBlock(mpb);
            StartCoroutine(FadeFx(fx));
        }

        private IEnumerator FadeFx(GameObject fx)
        {
            float t = 0f;
            while (fx != null && t < 0.35f)
            {
                t += Time.deltaTime;
                fx.transform.localScale = Vector3.one * Mathf.Lerp(0.4f, 1.6f, t / 0.35f);
                yield return null;
            }
            if (fx != null) Destroy(fx);
        }
    }
}
