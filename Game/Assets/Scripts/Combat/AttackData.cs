// BAYANI — Per-attack data (phasing.md 1.4). One asset per swing in the combo chain.
// Values follow ggd §56.2: dodge −20 stam, heavy −15 stam; hit lands +2 Diwa.

using UnityEngine;

namespace Bayani.Combat
{
    [CreateAssetMenu(menuName = "BAYANI/Attack Data", fileName = "AttackData")]
    public class AttackData : ScriptableObject
    {
        [Header("Identity")]
        public string attackName = "Light 1";
        public bool isHeavy = false;          // heavy = finisher of the L-L-L chain
        public bool isSkill = false;          // Diwa-powered move

        [Header("Timing (seconds) — the combo IS the timing")]
        public float windup = 0.18f;          // before hitbox activates
        public float active = 0.12f;          // hitbox live window
        public float recovery = 0.25f;        // whiff punish window; next combo input accepted after windup starts

        [Header("Combat")]
        public float damage = 10f;
        public float staminaCost = 0f;        // lights are free, heavy 15 (ggd §56.2)
        public float diwaCost = 0f;           // skill attacks only
        public float knockback = 2f;
        public float range = 1.6f;            // hitbox reach in meters

        [Header("Input buffer (phasing 1.3)")]
        public float comboWindowAfterActive = 0.3f;  // chain window: next press buffered/accepted
    }
}
