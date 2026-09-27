// BAYANI — Auto-heal (docs/prd-progression.md §9). A slow bedside manner, not a
// potion: +2 HP per 1.5s at level 1, +1 more per three levels, hard-capped.
// Never touches Diwa or Stamina (three non-overlapping meters, ggd §56.2) —
// every point goes through CombatResources.Heal, which clamps to maxHP.
// Rides the DialoguePlayer host (same GO as DeathManager); installer wires it.

using Bayani.Combat;
using UnityEngine;

namespace Bayani.Player
{
    public class PlayerAutoHeal : MonoBehaviour
    {
        public CombatResources resources;     // installer wires; lazy FindFirst fallback

        [Header("Tuning (Inspector — no code edits)")]
        public float tickSeconds = 1.5f;
        public int baseHeal = 2;              // HP per tick at level 1
        public int healPerThreeLevels = 1;    // +1 per full 3 levels above 1
        public int maxHeal = 8;               // cap, whatever the level

        private float _nextTick;

        private void Update()
        {
            if (resources == null) resources = FindFirstObjectByType<CombatResources>();
            if (resources == null || Time.time < _nextTick) return;
            _nextTick = Time.time + Mathf.Max(0.25f, tickSeconds);

            if (resources.IsDead || resources.HP >= resources.MaxHP) return;

            int heal = Mathf.Min(maxHeal,
                baseHeal + (Bayani.Combat.ProgressStore.Level - 1) / 3 * healPerThreeLevels);
            if (heal > 0) resources.Heal(heal);
        }
    }
}
