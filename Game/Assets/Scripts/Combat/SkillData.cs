// BAYANI — Active skill definition (docs/prd-progression.md §3–5).
// One SO per skill; the whole arsenal is Inspector-tunable (data-driven gate:
// no hard-coded combat values). Keys 1–5 bind by BAR POSITION (see PlayerSkills),
// not per-asset — the armed/unarmed category swaps which skills fill the bar.
// hitDelay/hitDuration are the script-side equivalent of animation events: the
// hitbox opens at clip frame ≈ hitDelay so timing survives clip changes.

using UnityEngine;

namespace Bayani.Combat
{
    public enum SkillCategory { Armed, Unarmed }        // §4: bar shown depends on weapon state
    public enum SkillCostType { Stamina, Diwa }         // §3: shared pool, only ever cast when affordable

    [CreateAssetMenu(menuName = "BAYANI/Skill", fileName = "Skill")]
    public class SkillData : ScriptableObject
    {
        [Header("Identity")]
        public string skillName = "";                   // e.g. "Solo Baston"
        public SkillCategory category = SkillCategory.Armed;
        [Min(1)] public int unlockLevel = 1;            // level gate (§1: level-ups unlock skill slots)

        [Header("Cast")]
        public SkillCostType costType = SkillCostType.Stamina;
        public float cost = 15f;
        public float cooldown = 4f;
        public float damage = 12f;
        public float range = 2f;
        public float knockback = 2f;
        [Tooltip("Seconds from cast until the hitbox opens — align with the clip's contact frame.")]
        public float hitDelay = 0.2f;
        public float hitDuration = 0.12f;
        public float lungeSpeed = 3f;                   // short root-motion drive during the cast

        [Header("Animation (§5)")]
        public string animatorParam = "";               // e.g. "SkillArmed1" — SetTrigger if the Animator has it
        public AnimationClip clip;                      // optional; null = procedural lunge fallback (graybox)

        [Header("Feedback (§5 placeholders)")]
        public Color castVfxColor = new Color(1f, 0.85f, 0.4f);  // graybox flash tint
        public AudioClip castSound;                     // null = silent + logged once
    }
}
