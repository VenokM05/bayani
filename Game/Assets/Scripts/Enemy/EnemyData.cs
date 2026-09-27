// BAYANI — Enemy stat block (phasing.md 1.7/1.9: variants = data tweaks, not new code).
// Defaults match the Anino (basic Limot): fast, weak, dies in ~3 lights + heavy.

using UnityEngine;

namespace Bayani.Enemy
{
    [CreateAssetMenu(menuName = "BAYANI/Enemy Data", fileName = "EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public string enemyName = "Anino";

        [Header("Combat")]
        public float maxHP = 30f;
        public float attackDamage = 12f;       // player HP scale: ~5 hits = death
        public float attackRange = 1.7f;

        [Header("Fairness (phasing 1.7: telegraph >= 0.6s)")]
        public float moveSpeed = 3.2f;
        public float telegraphTime = 0.7f;     // visible wind-up before the swing
        public float attackActiveTime = 0.15f;
        public float recoverTime = 0.9f;       // the punish window — this is the fight's rhythm
        public float aggroRadius = 8f;

        [Header("Rewards (ggd §56.2/§56.4)")]
        public float diwaOnKill = 6f;          // +6 Diwa per kill
        public float xpOnKill = 40f;           // Phase 2 XP hooks in; stored now
        public float stabilityOnKill = 5f;     // +5 Memory Stability (ggd §56.3)
    }
}
