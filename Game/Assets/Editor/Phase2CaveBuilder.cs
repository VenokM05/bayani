// BAYANI — Phase 2 cave builder (Editor tool): SEQ 10B, the Corrupted Anito.
// One click: Tools → BAYANI → Phase 2 - Build The Cave (SEQ 10B)
// Patches Assets/Scenes/Chapter1/SC_04_Mactan.unity — adds (or rebuilds) a
// "SEQ10B_Cave" root behind the Sea_Cave_Mouth foreshadow: tunnel → tidal
// chamber → seam-wall. Boss = CorruptedAnito (expel-not-kill, 3 phases) +
// intro/victory dialogue SOs. REQUIRES: Phase 2 Mactan builder + Phase 1.10
// (Anino prefab). The village stability zone IS the fight's stake: aggro
// collapses it to 0%, expulsion restores 100% — the player watches the color
// come back (ggd §56.3 proof-of-concept).

using Bayani.Combat;
using Bayani.Enemy;
using Bayani.Story;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Bayani.EditorTools
{
    public static class Phase2CaveBuilder
    {
        private const string ScenePath = "Assets/Scenes/Chapter1/SC_04_Mactan.unity";
        private const string StoryDir = "Assets/Data/Story";
        private const string CombatDir = "Assets/Data/Combat";
        private const string RootName = "SEQ10B_Cave";

        [MenuItem("Tools/BAYANI/Phase 2 - Build The Cave (SEQ 10B)")]
        public static void Build()
        {
            if (!System.IO.File.Exists(System.IO.Path.Combine(
                    Application.dataPath, ScenePath.Substring("Assets/".Length))))
            {
                EditorUtility.DisplayDialog("The Cave (SEQ 10B)",
                    "SC_04_Mactan not found — run 'Phase 2 - Build Mactan 1521' first.", "OK");
                return;
            }
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("The Cave (SEQ 10B)", "Exit Play mode first.", "OK");
                return;
            }

            var anino = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/Anino.prefab");
            if (anino == null)
                Debug.LogWarning("[BAYANI] 10B: Anino.prefab missing — run 'Phase 1.10 - Build Wave Arena'. Shadow-call will summon nothing.");

            // ---------- data (verbatim storyline.md SEQ 10B + content/enemies/corrupted-anito.md) ----------
            var intro = GetOrCreate($"{StoryDir}/DLG_SEQ10B_Intro.asset", () => Seq("SEQ 10B — The Cave Beneath the Shore",
                Line("NARRATION", "Before dawn, the stability beneath the village collapses. Not naturally. Something down there is feeding."),
                Line("NARRATION", "In the cave under the shore, Kai finds it: an anito — a guardian image of the village — inverted, gnawing the memory of the people it once protected."),
                Line("KAI", "You're one of them… what did they do to you?"),
                Line("NARRATION", "It answers by coming.")));

            var victory = GetOrCreate($"{StoryDir}/DLG_SEQ10B_Victory.asset", () => Seq("SEQ 10B — The Village Wakes",
                Line("NARRATION", "One clean bell-strike, from somewhere behind the wall."),
                Line("NARRATION", "The anito is driven back. Not deleted."),
                Line("NARRATION", "The village memory re-anchors at 100%. The people wake like sleepers."),
                Line("TALA", "Codex written — SPIRITS. The corruption is not the anito's nature. Something older is feeding through it."),
                Line("NARRATION", "A shrine bell token lies in the tide where it stood. (Codex HERITAGE — a key for later.)")));

            var bossData = GetOrCreate($"{CombatDir}/EnemyData_CorruptedAnito.asset", () =>
            {
                var d = ScriptableObject.CreateInstance<EnemyData>();
                d.name = "EnemyData_CorruptedAnito";
                d.enemyName = "Corrupted Anito";
                d.behavior = EnemyData.Behavior.Melee;
                d.maxHP = 120f;                 // phase-gated; expel prompt at 50%
                d.attackDamage = 14f;           // shrine sweep (content spec table)
                d.attackRange = 2.1f;
                d.staggerKnockback = 2.5f;      // heavy interrupts the Shadow-call; lights don't
                d.moveSpeed = 2.7f;             // drifting, not charging — readability over speed
                d.telegraphTime = 0.9f;         // windup per spec table
                d.attackActiveTime = 0.15f;
                d.recoverTime = 1.1f;           // the punish rhythm
                d.aggroRadius = 10f;
                d.diwaOnKill = 6f;              // post-expel Diwa per spec
                d.xpOnKill = 500f;              // §56.4 boss band: 500–1,200 (mini-boss = floor)
                d.stabilityOnKill = 0f;         // the restore IS the reward
                return d;
            });

            // ---------- patch the scene: rebuild our root, leave the village untouched ----------
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            foreach (var go in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))
                if (go.name == RootName && go.parent == null) { Object.DestroyImmediate(go.gameObject); break; }

            var root = new GameObject(RootName);

            // the village's solid foreshadow cube IS the doorway now — remove it, walls frame the gap
            foreach (var go in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))
                if (go.name == "Sea_Cave_Mouth") { Object.DestroyImmediate(go.gameObject); break; }

            // chamber geometry — tunnel mouth at (14,·,74) from the village, sea beyond
            // floor reaches z=75 so it meets the village ground flush (no pit at the threshold)
            Box(root, "Cave_Floor", new Vector3(14f, -0.1f, 85f), new Vector3(20f, 0.2f, 20f));
            Box(root, "Tide_Pool", new Vector3(14f, 0.05f, 86f), new Vector3(19.4f, 0.06f, 17.4f));   // waist-deep flavor
            Box(root, "Cave_Ceiling", new Vector3(14f, 3.4f, 86f), new Vector3(20f, 0.4f, 18f));
            Box(root, "Wall_S_L", new Vector3(8.7f, 1.6f, 76.7f), new Vector3(8.6f, 3.4f, 0.6f));
            Box(root, "Wall_S_R", new Vector3(19.6f, 1.6f, 76.7f), new Vector3(7.8f, 3.4f, 0.6f));
            Box(root, "Wall_N", new Vector3(14f, 1.6f, 95.3f), new Vector3(20f, 3.4f, 0.6f));
            Box(root, "Wall_W", new Vector3(3.7f, 1.6f, 86f), new Vector3(0.6f, 3.4f, 18f));
            Box(root, "Wall_E", new Vector3(24.3f, 1.6f, 86f), new Vector3(0.6f, 3.4f, 18f));
            var seam = Box(root, "Seam_Wall", new Vector3(14f, 1.6f, 94.9f), new Vector3(6f, 3.2f, 0.25f));
            var seamPoint = new GameObject("Seam_Point");
            seamPoint.transform.SetParent(root.transform, false);
            seamPoint.transform.position = new Vector3(14f, 1f, 93.6f);
            var shadowA = new GameObject("Shadow_A");
            shadowA.transform.SetParent(root.transform, false);
            shadowA.transform.position = new Vector3(6f, 0.2f, 82f);
            var shadowB = new GameObject("Shadow_B");
            shadowB.transform.SetParent(root.transform, false);
            shadowB.transform.position = new Vector3(22f, 0.2f, 91f);

            var torch = new GameObject("Cave_Light");
            torch.transform.SetParent(root.transform, false);
            torch.transform.position = new Vector3(14f, 2.6f, 86f);
            var tl = torch.AddComponent<Light>();
            tl.type = LightType.Point; tl.range = 16f; tl.intensity = 0.9f;
            tl.color = new Color(0.7f, 0.75f, 1f);     // cold ink-wash, per art brief

            // ---------- the boss ----------
            var boss = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            boss.name = "Corrupted_Anito";
            boss.transform.SetParent(root.transform, false);
            boss.transform.position = new Vector3(14f, 1.5f, 88f);
            boss.transform.localScale = new Vector3(1.7f, 1.6f, 1.7f);
            var bh = boss.AddComponent<Hurtbox>();
            bh.team = Hitbox.Team.Enemy;
            var bHitGO = new GameObject("Hitbox");
            bHitGO.transform.SetParent(boss.transform, false);
            bHitGO.transform.localPosition = new Vector3(0f, 0.15f, 1.1f);
            var bHit = bHitGO.AddComponent<Hitbox>();
            bHitGO.AddComponent<BoxCollider>().isTrigger = true;
            bHitGO.GetComponent<BoxCollider>().size = new Vector3(1.8f, 1.6f, 1.4f);
            bHitGO.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, 0.5f);
            bHit.team = Hitbox.Team.Enemy;
            bHit.owner = boss;
            bHitGO.SetActive(false);

            var anito = boss.AddComponent<CorruptedAnito>();
            anito.data = bossData;
            anito.hurtbox = bh;
            anito.hitbox = bHit;
            anito.player = Object.FindAnyObjectByType<PlayerCombat>();
            anito.aninoPrefab = anino;
            anito.shadowPoints = new[] { shadowA.transform, shadowB.transform };
            anito.seamWall = seam.transform;
            anito.victoryDialogue = victory;
            if (anito.player == null)
                Debug.LogWarning("[BAYANI] 10B: no PlayerCombat in SC_04_Mactan — rebuild Mactan first; boss has no target.");

            // intro beat at the cave mouth (auto-fire, like every story trigger)
            var introGO = new GameObject("SEQ10B_CaveEntrance");
            introGO.transform.SetParent(root.transform, false);
            introGO.transform.position = new Vector3(14f, 1.2f, 74f);
            var iCol = introGO.AddComponent<BoxCollider>();
            iCol.isTrigger = true;
            iCol.size = new Vector3(5f, 3f, 4f);
            var iT = introGO.AddComponent<StoryTrigger>();
            iT.dialogue = intro;
            iT.requiresInteract = false;
            iT.titleCard = "SEQ 10B — THE CAVE BENEATH THE SHORE";

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();

            Debug.Log("[BAYANI] SEQ 10B cave built inside SC_04_Mactan. Play: fire circle -> walk NE to the cave mouth.");
            EditorUtility.DisplayDialog("The Cave (SEQ 10B)",
                "Built inside SC_04_Mactan behind the cave mouth.\n\n" +
                "Expel-not-kill mini-boss:\n" +
                "phases Veiled -> Reaching (Shadow-call: heavy to interrupt)\n" +
                "-> Wounded: it turns to the seam-wall — SPEND DIWA [Q].\n\n" +
                "Aggro collapses village memory to 0%. Expulsion restores 100%\n" +
                "— watch the color come back.", "OK");
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
    }
}
