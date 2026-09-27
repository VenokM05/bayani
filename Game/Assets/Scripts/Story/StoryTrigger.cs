// BAYANI — Story zone/prop trigger: plays a DialogueAsset once, either on entry (auto)
// or via [E] prompt (requiresInteract). Optional staging hooks for the prologue:
// titleCard on start, flash on start, and finale behavior (fade to black + caption).
// Data-driven: the dialogue itself lives in Assets/Data/Story/*.asset.

using Bayani.Core;
using UnityEngine;

namespace Bayani.Story
{
    public class StoryTrigger : MonoBehaviour
    {
        public DialogueAsset dialogue;
        public bool requiresInteract;
        public string prompt = "[E/F] Examine";
        public string titleCard;            // shown when the sequence starts ("" = none)
        public bool flashOnStart;           // SEQ 04 vision flash
        public string finaleCaption;        // if set: after dialogue, fade to black and hold this text
        public Bayani.Enemy.WaveSpawner spawnOnFinish;   // story-sequenced fight (Mactan shrine clear)

        [Header("Rewards (PRD §9)")]
        [Tooltip("Item granted to the inventory when this beat finishes (null = none).")]
        public Bayani.Player.ItemData grantItem;
        [Tooltip("Quest guide id — completing this beat marks that quest Complete; with dialogue.autoTrack it also adds the objective.")]
        public string questId = "";

        private bool _done;
        private bool _inside;

        private void OnTriggerEnter(Collider other)
        {
            if (_done || dialogue == null) return;
            if (!other.CompareTag("Player")) return;
            _inside = true;
            if (!requiresInteract)
            {
                DialoguePlayer.Prompt = null;
                StartSequence();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _inside = false;
            if (DialoguePlayer.Prompt == prompt) DialoguePlayer.Prompt = null;
        }

        private void Update()
        {
            if (_done || requiresInteract == false || dialogue == null) return;
            if (!_inside || DialoguePlayer.IsActive || ScreenFader.IsBlack) return;

            DialoguePlayer.Prompt = prompt;
            if (BayaniInput.InteractPressed)
            {
                DialoguePlayer.Prompt = null;
                StartSequence();
            }
        }

        private void StartSequence()
        {
            _done = true;
            if (!string.IsNullOrEmpty(titleCard)) ScreenFader.TitleCard(titleCard, 3f);
            if (flashOnStart) ScreenFader.Flash();

            if (!string.IsNullOrEmpty(finaleCaption))
                DialoguePlayer.Instance.Play(dialogue, () =>
                {
                    Finish();
                    ScreenFader.BlackOut(finaleCaption, 1.5f, 4f, 1f);
                });
            else
                DialoguePlayer.Instance.Play(dialogue, Finish);

            void Finish()
            {
                // objective completion pays XP (ggd §56.4 quests 250–800) — 0 on old assets
                if (dialogue.xpReward > 0)
                    Bayani.Combat.PlayerProgression.Instance?.AwardXp(dialogue.xpReward, dialogue.name);
                // item grant (PRD §9 skeleton): the container state is static — no host needed
                if (grantItem != null)
                    Bayani.Player.Inventory.Add(grantItem);
                // quest guide (PRD §9): autoTrack adds the objective; questId closes a DIFFERENT beat
                string opened = null;
                if (dialogue.autoTrack && !string.IsNullOrEmpty(dialogue.objectiveText))
                    opened = Bayani.Story.QuestGuide.QuestJournal.AutoAdd(dialogue, questId);
                if (!string.IsNullOrEmpty(questId) && questId != opened)
                    Bayani.Story.QuestGuide.QuestJournal.Complete(questId);
                if (spawnOnFinish != null) spawnOnFinish.Begin();
            }
        }
    }
}
