// BAYANI — Phase 2 battle builder (Editor tool): SEQ 11, THE BATTLE OF MACTAN.
// One click: Tools → BAYANI → Phase 2 - Build The Battle (SEQ 11)
// Patches Assets/Scenes/Chapter1/SC_04_Mactan.unity — adds (or rebuilds) the
// battle shore west of the village: saturated history (warriors, canoes, smoke)
// + three desaturated seam pockets + the Memory Devourer (no-HP-bar, three
// rift cycles, Diwa-only seals, expelled not killed). REQUIRES: Phase 2 Mactan
// builder + Phase 1.10 (Anino prefab). After building, RE-RUN
// "Install Progression + Skills" — builders erase installer patches on our roots.

using Bayani.Combat;
using Bayani.Enemy;
using Bayani.Story;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Bayani.EditorTools
{
    public static class Phase2BattleBuilder
    {
        private const string ScenePath = "Assets/Scenes/Chapter1/SC_04_Mactan.unity";
        private const string StoryDir = "Assets/Data/Story";
        private const string RootName = "SEQ11_Battle";

        [MenuItem("Tools/BAYANI/Phase 2 - Build The Battle (SEQ 11)")]
        public static void Build()
        {
            if (!System.IO.File.Exists(System.IO.Path.Combine(
                    Application.dataPath, ScenePath.Substring("Assets/".Length))))
            {
                EditorUtility.DisplayDialog("The Battle (SEQ 11)",
                    "SC_04_Mactan not found — run 'Phase 2 - Build Mactan 1521' first.", "OK");
                return;
            }
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("The Battle (SEQ 11)", "Exit Play mode first.", "OK");
                return;
            }

            var anino = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/Anino.prefab");
            if (anino == null)
                Debug.LogWarning("[BAYANI] SEQ 11: Anino.prefab missing — run 'Phase 1.10 - Build Wave Arena'. The seams will spawn no maws.");

            // ---------- data (storyline.md SEQ 11 + content/enemies/memory-devourer.md) ----------
            var arrival = GetOrCreate($"{StoryDir}/DLG_SEQ11_Arrival.asset", () =>
            {
                var a = Seq("SEQ 11 — The Battle of Mactan",
                    Line("NARRATION", "War drums. The beach at sunrise — the recorded day, exactly as the archive rebuilt it."),
                    Line("NARRATION", "Kai tries to help their ancestor. The battle is chaotic and enormous and utterly real."),
                    Line("NARRATION", "And then Kai sees what no history book records: above the tideline, a shape eating the battle itself. Warriors flicker where it passes. The cannon-smoke remembers going the wrong way."),
                    Line("KAI", "…only I can see it. Why can only I see it?"));
                a.xpReward = 300;             // §56.4 quest band — auto-granted by StoryTrigger
                return a;
            });

            var reaching = GetOrCreate($"{StoryDir}/DLG_SEQ11_Reaching.asset", () => Seq("SEQ 11 — The Reaching",
                Line("NARRATION", "The last seam closes — and the shape turns from the fighters and reaches for the event itself."),
                Line("NARRATION", "The artifact burns against Kai's chest. The pull-back has begun, with or without consent."),
                Line("KAI", "Not yet!"),
                Line("NARRATION", "Kai cannot stay. The last burst is all that is left of choice.")));

            var victory = GetOrCreate($"{StoryDir}/DLG_SEQ11_Victory.asset", () => Seq("SEQ 11 — Driven Out",
                Line("NARRATION", "Every ambient sound rushes back into the hole at once. Drums. Surf. Screams. The battle keeps its shape."),
                Line("NARRATION", "The shape is driven out of this memory — down, deeper, into the bloodline. Not killed. Never killed."),
                Line("TALA", "Codex: ANOMALY — apex of the Limot family. The entry is 90% blank. Flagging the missing data as itself the anomaly."),
                Line("NARRATION", "On the tideline, Kai's ancestor is still standing. Wounded — alive. (SEQ 12 — the goodbye — continues.)")));

            // ---------- patch the scene: rebuild our root, leave village/cave untouched ----------
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            foreach (var go in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))
                if (go.name == RootName && go.parent == null) { Object.DestroyImmediate(go.gameObject); break; }

            var root = new GameObject(RootName);

            // battle shore west of the village — sand, shallow water, canoes offshore
            Box(root, "Battle_Floor", new Vector3(-16f, -0.05f, 66f), new Vector3(28f, 0.2f, 22f));
            Box(root, "Shallow_Water", new Vector3(-16f, 0.03f, 78f), new Vector3(28f, 0.08f, 8f));
            for (int i = 0; i < 4; i++)
                Box(root, $"Canoe_{i}", new Vector3(-24f + i * 5.5f, 0.25f, 82.5f), new Vector3(3.4f, 0.5f, 1f));
            Box(root, "Smoke_Column_A", new Vector3(-21f, 3.2f, 70f), new Vector3(2.2f, 5f, 2.2f));
            Box(root, "Smoke_Column_B", new Vector3(-9f, 3.8f, 68f), new Vector3(1.8f, 6f, 1.8f));

            // the recorded battle, static for graybox: defenders vs invaders on the tideline
            for (int i = 0; i < 5; i++)
            {
                Figure(root, $"Defender_{i}", new Vector3(-23f + i * 2.4f, 0.9f, 62f + (i % 2) * 2.2f), 0.22f, new Color(0.75f, 0.32f, 0.2f));
                Figure(root, $"Invader_{i}", new Vector3(-24f + i * 2.6f, 0.9f, 67.4f + (i % 2) * 1.6f), 180f, new Color(0.3f, 0.32f, 0.4f));
            }

            // three seam pockets — the desaturated stage edges (history stays intact between them)
            var seams = new[]
            {
                Seam(root, "Seam_A", new Vector3(-24f, 0f, 64f)),
                Seam(root, "Seam_B", new Vector3(-16f, 0f, 70.5f)),
                Seam(root, "Seam_C", new Vector3(-8f, 0f, 64f)),
            };

            // ---------- the Devourer: a shape where a crowd used to be ----------
            var bossGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            bossGO.name = "Memory_Devourer";
            bossGO.transform.SetParent(root.transform, false);
            bossGO.transform.position = new Vector3(-16f, 1.9f, 74.5f);
            bossGO.transform.localScale = new Vector3(2.4f, 2f, 2.4f);
            bossGO.AddComponent<Hurtbox>().team = Hitbox.Team.Enemy;   // hit-able so "blades pass through" lands

            var devourer = bossGO.AddComponent<MemoryDevourer>();
            devourer.player = Object.FindAnyObjectByType<PlayerCombat>();
            devourer.aninoPrefab = anino;
            devourer.riftPoints = new[] { seams[0].transform, seams[1].transform, seams[2].transform };
            devourer.reachingDialogue = reaching;
            devourer.victoryDialogue = victory;
            if (devourer.player == null)
                Debug.LogWarning("[BAYANI] SEQ 11: no PlayerCombat in SC_04_Mactan — rebuild Mactan first; the Devourer has no witness.");

            // arrival beat where the battle becomes visible from the village
            var trigGO = new GameObject("SEQ11_BattleEntrance");
            trigGO.transform.SetParent(root.transform, false);
            trigGO.transform.position = new Vector3(-14f, 1.2f, 55f);
            var tCol = trigGO.AddComponent<BoxCollider>();
            tCol.isTrigger = true;
            tCol.size = new Vector3(7f, 3f, 4f);
            var trig = trigGO.AddComponent<StoryTrigger>();
            trig.dialogue = arrival;
            trig.requiresInteract = false;
            trig.titleCard = "SEQ 11 — THE BATTLE OF MACTAN";

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();

            Debug.Log("[BAYANI] SEQ 11 battle shore built inside SC_04_Mactan. RE-RUN 'Install Progression + Skills', then play: village west edge -> the tideline.");
            EditorUtility.DisplayDialog("The Battle (SEQ 11)",
                "Battle shore built west of the village in SC_04_Mactan.\n\n" +
                "Memory Devourer — no HP bar, three seam cycles:\n" +
                "step INTO a seam pocket -> survive the maws -> [Q] Diwa (>=25) seals it.\n" +
                "Blades pass through; only Diwa bites.\n" +
                "After the third seam: THE REACHING -> last burst = expulsion.\n" +
                "Delay is allowed, skipping is not — the seams drain stability while open.\n\n" +
                "IMPORTANT: re-run 'Install Progression + Skills' now.", "OK");
        }

        // ---------- helpers ----------
        private static DialogueLine Line(string speaker, string text) =>
            new DialogueLine { speaker = speaker, text = text };

        private static DialogueAsset Seq(string title, params DialogueLine[] lines)
        {
            var a = ScriptableObject.CreateInstance<DialogueAsset>();
            a.name = title;
            a.lines = lines;
            return a;
        }

        private static T GetOrCreate<T>(string path, System.Func<T> make) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;
            string parent = path.Substring(0, path.LastIndexOf('/'));
            if (!AssetDatabase.IsValidFolder(parent))
                EnsureFolder("Assets/Data", parent.Substring(parent.LastIndexOf('/') + 1));
            var asset = make();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void EnsureFolder(string parent, string folder)
        {
            string path = $"{parent}/{folder}";
            if (AssetDatabase.IsValidFolder(path)) return;
            if (!AssetDatabase.IsValidFolder(parent))
            {
                string grand = parent.Substring(0, parent.LastIndexOf('/'));
                AssetDatabase.CreateFolder(grand, parent.Substring(parent.LastIndexOf('/') + 1));
            }
            AssetDatabase.CreateFolder(parent, folder);
        }

        private static GameObject Box(GameObject parent, string name, Vector3 pos, Vector3 scale)
        {
            var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
            b.name = name;
            b.transform.SetParent(parent.transform, false);
            b.transform.position = pos;
            b.transform.localScale = scale;
            return b;
        }

        private static GameObject Figure(GameObject parent, string name, Vector3 pos, float yaw, Color tint)
        {
            var c = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            c.name = name;
            c.transform.SetParent(parent.transform, false);
            c.transform.position = pos;
            c.transform.localScale = new Vector3(0.7f, 0.9f, 0.7f);
            c.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            var r = c.GetComponent<Renderer>();
            var mpb = new MaterialPropertyBlock();
            mpb.SetColor("_BaseColor", tint);
            r.SetPropertyBlock(mpb);
            return c;
        }

        private static GameObject Seam(GameObject parent, string name, Vector3 pos)
        {
            // graybox rift: a standing dark slab at the pocket's center; the pocket
            // itself is invisible until Kai chooses to step in (Watching phase)
            var s = Box(parent, name, pos + Vector3.up * 1.3f, new Vector3(0.35f, 2.6f, 1.8f));
            var r = s.GetComponent<Renderer>();
            var mpb = new MaterialPropertyBlock();
            mpb.SetColor("_BaseColor", new Color(0.06f, 0.03f, 0.1f));
            r.SetPropertyBlock(mpb);
            s.name = name + "_Slab";
            var pocket = new GameObject(name);   // the transform the boss tracks as the pocket
            pocket.transform.SetParent(parent.transform, false);
            pocket.transform.position = pos;
            return pocket;
        }
    }
}
