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

        private float _lastStaminaChangeTime = -10f;
        private bool _blocking;     // regen paused while blocking (Phase 1 later step)

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
            if (HP <= 0f) Debug.Log("[BAYANI] Kai is down — death/respawn flow arrives Phase 2.");
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
