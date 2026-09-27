// BAYANI — Level/XP progression (docs/prd-progression.md §1) + the persistent
// run state that death and scene reloads must survive (§2): level, XP, skill
// points, and the last-touch checkpoint. ProgressStore is a STATIC — it rides
// through SceneManager.LoadScene; components re-apply themselves on Awake.
// Kill XP flows from EnemyData.xpOnKill, quest XP from DialogueAsset.xpReward,
// artifact XP from ArtifactData.xpReward — all inside the ggd §56.4 budget.

using UnityEngine;

namespace Bayani.Combat
{
    /// <summary>Run state that survives death and scene loads. Single source of truth.</summary>
    public static class ProgressStore
    {
        public static int Level = 1;
        public static int Xp;                           // XP inside the current level
        public static int SkillPoints;
        public static string Toast;                     // one-shot HUD message

        // checkpoint: last visited Checkpoint (or spawn) — respawn point per PRD §2
        public static string CheckpointScene = "";
        public static Vector3 CheckpointPosition;
        public static Quaternion CheckpointRotation;
        public static bool PendingPlacement;            // set on death-with-scene-load

        public static void SetCheckpoint(string scene, Vector3 pos, Quaternion rot)
        {
            CheckpointScene = scene; CheckpointPosition = pos; CheckpointRotation = rot;
        }
    }

    [DefaultExecutionOrder(-50)]   // apply saved level before anything reads the meters
    public class PlayerProgression : MonoBehaviour
    {
        public XpTable table;
        public CombatResources resources;
        public PlayerCombat combat;

        public static PlayerProgression Instance { get; private set; }

        public int Level => ProgressStore.Level;
        public int Xp => ProgressStore.Xp;
        public int XpNeeded => table != null ? table.Needed(ProgressStore.Level) : int.MaxValue;

        private void Awake()
        {
            Instance = this;
            if (resources != null)
            {
                // re-apply grown stats after scene reload (serialized base + stored levels)
                int grown = ProgressStore.Level - 1;
                if (table != null && grown > 0)
                {
                    resources.GrowMaxHP(table.hpPerLevel * grown);
                    resources.SetStaminaRegen(table.baseStaminaRegen + table.staminaRegenPerLevel * grown);
                }
            }
            if (combat != null) combat.OnKill += HandleKill;
        }

        private void OnDestroy()
        {
            if (combat != null) combat.OnKill -= HandleKill;
            if (Instance == this) Instance = null;
        }

        private void HandleKill(Enemy.EnemyData data) => AwardXp(data.xpOnKill, data.enemyName);

        /// <summary>The one XP door — kills, quest beats and artifact scans all route here.</summary>
        public void AwardXp(float amount, string reason)
        {
            if (table == null || amount <= 0f) return;
            ProgressStore.Xp += Mathf.RoundToInt(amount);
            Debug.Log($"[BAYANI] +{amount:0} XP ({reason}) — Lvl {ProgressStore.Level}, {ProgressStore.Xp}/{XpNeeded}");

            int levels = 0;
            while (ProgressStore.Level < table.MaxLevel && ProgressStore.Xp >= table.Needed(ProgressStore.Level))
            {
                ProgressStore.Xp -= table.Needed(ProgressStore.Level);
                ProgressStore.Level++;
                levels++;
                if (table.grantSkillPointPerLevel) ProgressStore.SkillPoints++;
                resources?.GrowMaxHP(table.hpPerLevel);
                resources?.SetStaminaRegen(table.baseStaminaRegen + table.staminaRegenPerLevel * (ProgressStore.Level - 1));
            }
            if (levels > 0)
            {
                resources?.Refill();    // leveling up feels good — full restore
                ProgressStore.Toast = $"LEVEL {ProgressStore.Level}  ·  +{table.hpPerLevel:0} MAX HP · +1 skill point (§56.4)";
                Bayani.Story.ScreenFader.Flash();
                Debug.Log($"[BAYANI] LEVEL UP → {ProgressStore.Level} ({ProgressStore.SkillPoints} skill points banked)");
            }
        }
    }
}
