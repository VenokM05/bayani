// BAYANI — XP curve + level bonuses (docs/prd-progression.md §1).
// Budget follows ggd §56.4: L1→L10 total = 5,000 XP (xpToNext below sums to exactly
// 5,000), tuned so Chapter 1's scripted content lands ~L8 with codex/exploration
// closing the gap. Sources per §56.4: kills 20–150, quests 250–800, artifacts 50,
// bosses 500–1,200. All numbers live here — the Inspector is the tuning surface.

using UnityEngine;

namespace Bayani.Combat
{
    [CreateAssetMenu(menuName = "BAYANI/XP Table", fileName = "XpTable")]
    public class XpTable : ScriptableObject
    {
        [Tooltip("XP needed to go from level (i+1) to (i+2). Index 0 = L1→L2. Sum must equal the §56.4 budget (5,000 for L1→L10).")]
        public int[] xpToNext = { 190, 240, 300, 380, 475, 590, 740, 925, 1160 };

        [Header("Per-level bonuses (applied at each level-up)")]
        public float hpPerLevel = 10f;
        public float staminaRegenPerLevel = 2f;
        [Tooltip("Base stamina regen the bonuses stack on (matches CombatResources default).")]
        public float baseStaminaRegen = 25f;

        [Header("Skill points (ggd §56.4: +1 per level from L2 → 9 across 15 nodes)")]
        public bool grantSkillPointPerLevel = true;

        public int MaxLevel => xpToNext.Length + 1;

        /// <summary>XP required to advance from `level`; int.MaxValue at cap.</summary>
        public int Needed(int level) =>
            level >= MaxLevel || level < 1 ? int.MaxValue : xpToNext[Mathf.Clamp(level - 1, 0, xpToNext.Length - 1)];
    }
}
