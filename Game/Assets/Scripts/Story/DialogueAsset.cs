// BAYANI — Dialogue data: fully data-driven story lines (phasing rule: values in assets).
// Authoring happens via the Editor builders (Assets/Data/Story/*.asset); runtime is read-only.

using System;
using UnityEngine;

namespace Bayani.Story
{
    [Serializable]
    public class DialogueLine
    {
        public string speaker = "";       // "" or "NARRATION" renders as centered italic voice-over
        public string text = "";
    }

    [CreateAssetMenu(menuName = "BAYANI/Dialogue Sequence")]
    public class DialogueAsset : ScriptableObject
    {
        public DialogueLine[] lines;

        [Tooltip("Quest/objective XP granted when a StoryTrigger finishes this sequence (ggd §56.4: quests 250–800; 0 = none).")]
        public int xpReward;

        [Header("Quest guide (docs/prd-progression.md §9)")]
        [Tooltip("Shown in the quest sidebar when autoTrack is on — e.g. 'Find the cause of the tremors'.")]
        public string objectiveText = "";
        [Tooltip("Adds objectiveText to the quest guide when this sequence finishes.")]
        public bool autoTrack;
    }
}
