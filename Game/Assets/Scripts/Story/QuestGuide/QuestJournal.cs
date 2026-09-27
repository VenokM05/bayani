// BAYANI — Quest journal (docs/prd-progression.md §9, quest-guide skeleton).
// A STATIC entry list — survives death and scene loads like ProgressStore.
// Dialogue.autoTrack adds entries; StoryTrigger.questId closes them; kills and
// scans advance the counter conditions. Rewards (XP/Diwa) exist only when a
// QuestData SO is registered under the same id — plain autoTrack quests are
// tracked for free. Renders the top-right sidebar through CombatHUD.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bayani.Story.QuestGuide
{
    public static class QuestJournal
    {
        public class Entry
        {
            public string id;
            public string text;
            public bool done;
            public int progress;
            public int goal = 1;
            public QuestData data;               // null = plain autoTrack objective
        }

        private static readonly List<Entry> _entries = new List<Entry>();
        private static readonly Dictionary<string, QuestData> _registry = new Dictionary<string, QuestData>();

        public static event Action OnChanged;
        public static IReadOnlyList<Entry> Entries => _entries;

        public static void Register(QuestData q)
        {
            if (q == null || string.IsNullOrEmpty(q.questId)) return;
            if (!_registry.ContainsKey(q.questId)) _registry.Add(q.questId, q);
        }

        /// <summary>DialogueAsset.autoTrack path: objective shown when the sequence ends.
        /// Id = explicit questId when the beat carries one, else the dialogue asset name.
        /// Returns the id used (null when nothing was added) so the caller can avoid
        /// completing a quest in the same breath that opened it.</summary>
        public static string AutoAdd(DialogueAsset dialogue, string questId)
        {
            if (dialogue == null || string.IsNullOrEmpty(dialogue.objectiveText)) return null;
            string id = string.IsNullOrEmpty(questId) ? dialogue.name : questId;
            Add(id, dialogue.objectiveText);
            return id;
        }

        public static void Add(string id, string text)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(text)) return;
            foreach (var e in _entries) if (e.id == id) return;    // one entry per id, ever

            _registry.TryGetValue(id, out var data);
            _entries.Add(new Entry
            {
                id = id,
                text = text,
                data = data,
                goal = data != null && data.completionCondition == QuestCondition.EnemyKilledCount
                    ? Math.Max(1, data.targetCount) : 1,
            });
            Debug.Log($"[BAYANI] QUEST ADD '{text}' ({id})");
            OnChanged?.Invoke();
        }

        /// <summary>The story-beat door: a finished StoryTrigger with this questId closes it.
        /// Unknown ids only materialise when a QuestData SO is registered under them —
        /// a stray beat never invents junk sidebar entries.</summary>
        public static void Complete(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            var e = Find(id);
            if (e == null)
            {
                if (!_registry.TryGetValue(id, out var q)) return;
                Add(id, !string.IsNullOrEmpty(q.objectiveText) ? q.objectiveText : q.title);
                e = Find(id);
                if (e == null) return;
            }
            if (e.done) return;
            e.done = true;
            e.progress = e.goal;
            Grant(e);
            Debug.Log($"[BAYANI] QUEST COMPLETE '{e.text}' ({id})");
            OnChanged?.Invoke();
        }

        public static void OnEnemyKilled(Bayani.Enemy.EnemyData data)
        {
            if (data == null) return;
            foreach (var e in _entries)
            {
                if (e.done || e.data == null) continue;
                if (e.data.completionCondition != QuestCondition.EnemyKilledCount) continue;
                if (!string.IsNullOrEmpty(e.data.target) && e.data.target != data.enemyName) continue;
                e.progress++;
                if (e.progress >= e.goal) { e.done = true; Grant(e); Debug.Log($"[BAYANI] QUEST COMPLETE '{e.text}' ({e.id})"); }
                OnChanged?.Invoke();
            }
        }

        public static void OnArtifactScanned(string category)
        {
            foreach (var e in _entries)
            {
                if (e.done || e.data == null) continue;
                if (e.data.completionCondition != QuestCondition.ArtifactScanned) continue;
                if (!string.IsNullOrEmpty(e.data.target) && e.data.target != category) continue;
                e.done = true;
                e.progress = e.goal;
                Grant(e);
                Debug.Log($"[BAYANI] QUEST COMPLETE '{e.text}' ({e.id})");
                OnChanged?.Invoke();
            }
        }

        private static Entry Find(string id)
        {
            foreach (var e in _entries) if (e.id == id) return e;
            return null;
        }

        private static void Grant(Entry e)
        {
            if (e.data == null) return;   // plain tracked objective — no ledger entries
            if (e.data.rewardXp > 0)
                Bayani.Combat.PlayerProgression.Instance?.AwardXp(e.data.rewardXp, $"quest: {e.text}");
            if (e.data.rewardDiwa > 0)
            {
                var res = UnityEngine.Object.FindFirstObjectByType<Bayani.Combat.CombatResources>();
                if (res != null) res.AddDiwa(e.data.rewardDiwa);
            }
            Bayani.Combat.ProgressStore.Toast = $"QUEST COMPLETE — {e.text.ToUpper()}";
        }

        // ---------- graybox sidebar (called by CombatHUD, top-right under FPS) ----------
        public static void Draw()
        {
            if (_entries.Count == 0) return;

            var head = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold };
            head.normal.textColor = new Color(0.92f, 0.72f, 0.25f, 0.85f);
            var line = new GUIStyle(GUI.skin.label) { fontSize = 12, wordWrap = true, alignment = TextAnchor.UpperRight };
            var doneStyle = new GUIStyle(line);
            line.normal.textColor = new Color(0.9f, 0.9f, 0.94f, 0.95f);
            doneStyle.normal.textColor = new Color(0.45f, 0.75f, 0.5f, 0.9f);

            float w = 290f, x = Screen.width - w - 12f, y = 40f;   // under the FPS readout (y 14..36)
            GUI.Label(new Rect(x, y, w, 16), "QUEST GUIDE", head);
            y += 18;
            foreach (var e in _entries)
            {
                if (y > Screen.height * 0.5f) break;               // sidebar budget: half a screen
                string tracker = !e.done && e.goal > 1 ? $" — {e.progress}/{e.goal}" : "";
                string status = e.done ? "Complete" : "Active";
                float h = (line.CalcHeight(new GUIContent($"{e.text}{tracker} — {status}"), w));
                GUI.Label(new Rect(x, y, w, h + 2), $"{e.text}{tracker} — {status}", e.done ? doneStyle : line);
                y += h + 2;
            }
        }
    }
}
