// BAYANI — Quest data (docs/prd-progression.md §9, quest-guide skeleton).
// Sequential tracking only — no branching trees, no timers (Phase 2 scope).
// A QuestData registers itself with QuestJournal when a scene reference loads
// it; StoryTrigger.questId is the match key for DialogueFinished beats.

using UnityEngine;

namespace Bayani.Story.QuestGuide
{
    public enum QuestCondition { DialogueFinished, ArtifactScanned, EnemyKilledCount }

    [CreateAssetMenu(menuName = "BAYANI/Quest Data", fileName = "QuestData")]
    public class QuestData : ScriptableObject
    {
        public string questId = "quest";
        public string title = "New Memory";
        [TextArea] public string objectiveText = "Do the thing";

        [Header("Completion")]
        public QuestCondition completionCondition = QuestCondition.DialogueFinished;
        [Tooltip("Enemy name for EnemyKilledCount (empty = any enemy); artifact category for ArtifactScanned (empty = any scan).")]
        public string target = "";
        public int targetCount = 1;              // EnemyKilledCount goal (also the tracker "x/y")

        [Header("Rewards on completion (ggd §56.4 bands)")]
        public int rewardXp;
        public int rewardDiwa;

        private void OnEnable() => QuestJournal.Register(this);   // scene/SO load registers
    }
}
