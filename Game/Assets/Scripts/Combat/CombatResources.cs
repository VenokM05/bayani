// BAYANI — HP / Stamina / Diwa per ggd §56.2 (three non-overlapping meters).
// Stamina = rhythm meter (auto-regen with delay). Diwa = earned meter (combat-gated ONLY).
// Attach to Kai. HUD reads via the public events/properties.

using System;
using UnityEngine;

namespace Bayani.Combat
{
    public class CombatResources : MonoBehaviour
    {
        [Header("Caps (ggd §56.2)")]
        [SerializeField] private float maxHP = 100f;
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float maxDiwa = 100f;

        [Header("Stamina regen")]
        [SerializeField] private float staminaRegenPerSec = 25f;
        [SerializeField] private float regenDelay = 0.8f;   // after spend OR hit taken

        public float HP { get; private set; }
        public float Stamina { get; private set; }
        public float Diwa { get; private set; }
        public float MaxHP => maxHP;
        public float MaxStamina => maxStamina;
        public float MaxDiwa => maxDiwa;
        public bool IsDead => HP <= 0f;

        public event Action<string, float> OnResourceChanged;   // HUD hook ("diwa", +8)
        public event Action OnDeath;                             // fires once when HP hits 0 — DeathManager listens

        private float _lastStaminaChangeTime = -10f;
        private bool _blocking;     // regen paused while blocking (Phase 1 later step)
        private bool _deathFired;

        private void Awake()
        {
            HP = maxHP; Stamina = maxStamina; Diwa = 0f;   // Diwa starts empty — you EARN it
        }

        private void Update()
        {
            if (_blocking) return;
            if (Time.time - _lastStaminaChangeTime >= regenDelay && Stamina < maxStamina)
            {
                Stamina = Mathf.Min(maxStamina, Stamina + staminaRegenPerSec * Time.deltaTime);
            }
        }

        public bool CanAffordStamina(float cost) => Stamina >= cost;
        public bool CanAffordDiwa(float cost) => Diwa >= cost;

        public void SpendStamina(float cost)
        {
            Stamina = Mathf.Max(0f, Stamina - cost);
            _lastStaminaChangeTime = Time.time;
        }

        public void TakeHit(float damage)
        {
            HP = Mathf.Max(0f, HP - damage);
            _lastStaminaChangeTime = Time.time;   // being hit pauses your regen too
            OnResourceChanged?.Invoke("hp", -damage);
            if (HP <= 0f && !_deathFired)
            {
                _deathFired = true;
                OnDeath?.Invoke();
            }
        }

        // ---- progression hooks (docs/prd-progression.md §1–2) ----

        /// <summary>Permanent max-HP growth (level-up / checkpoint restore base). Heals the same amount.</summary>
        public void GrowMaxHP(float delta)
        {
            maxHP = Mathf.Max(1f, maxHP + delta);
            HP = Mathf.Min(maxHP, HP + Mathf.Max(0f, delta));
            OnResourceChanged?.Invoke("hp", delta);
        }

        /// <summary>Flat heal, clamped to maxHP, dead players don't mend (auto-heal + consumables route here).</summary>
        public void Heal(float amount)
        {
            if (amount <= 0f || IsDead || HP >= maxHP) return;
            float before = HP;
            HP = Mathf.Min(maxHP, HP + amount);
            OnResourceChanged?.Invoke("hp", HP - before);
        }

        /// <summary>Permanent Diwa-cap growth (stat upgrades) — the earned meter gets a bigger bucket.</summary>
        public void GrowMaxDiwa(float delta)
        {
            maxDiwa = Mathf.Max(1f, maxDiwa + delta);
            Diwa = Mathf.Min(maxDiwa, Diwa + Mathf.Max(0f, delta));
            OnResourceChanged?.Invoke("diwa", delta);
        }

        public float StaminaRegenPerSec => staminaRegenPerSec;
        public void SetStaminaRegen(float perSec) => staminaRegenPerSec = Mathf.Max(0f, perSec);

        /// <summary>Respawn / level-up restore: full HP + stamina, Diwa untouched (earned meter).</summary>
        public void Refill()
        {
            HP = maxHP; Stamina = maxStamina;
            _deathFired = false;
            _lastStaminaChangeTime = -10f;
        }

        /// <summary>The ONLY way Diwa grows (plus scan, Phase 2): hit +2, perfect dodge +8, parry +12, kill +6.</summary>
        public void AddDiwa(float amount)
        {
            if (amount <= 0f) return;
            Diwa = Mathf.Min(maxDiwa, Diwa + amount);
            OnResourceChanged?.Invoke("diwa", amount);
        }

        public void SpendDiwa(float cost) => Diwa = Mathf.Max(0f, Diwa - cost);

        public void SetBlocking(bool value) => _blocking = value;
    }
}
