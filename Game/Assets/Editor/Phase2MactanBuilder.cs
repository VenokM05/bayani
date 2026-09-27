// BAYANI — Phase 2 Mactan builder (Editor tool)
// One click: Tools → BAYANI → Phase 2: Build Mactan 1521 (SEQ 09-10)
// Creates scene Assets/Scenes/Chapter1/SC_04_Mactan.unity (LOC_0003 per
// content/locations/mactan-1521.md): village gate → forge row → longhouses →
// shore shrine (SEQ 09B: Balikan Edge unfold + first ARTIFACT SCAN) → headland
// Limot pack (story-triggered wave, kills raise stability) → fire circle (SEQ 10,
// ends on the 10B "collapse" caption hook). Dialogue verbatim docs/storyline.md.
// REQUIRES: Phase 1 builder (combat SOs) + Phase 1.10 (enemy prefabs). Re-runnable.
// Tone: SEQ 09/10 are about a community preparing — dignity, not panic (storyline).

using Bayani.Combat;
using Bayani.Core;
using Bayani.Enemy;
using Bayani.Player;
using Bayani.Story;
using Bayani.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Bayani.EditorTools
{
    public static class Phase2MactanBuilder
    {
        private const string ScenePath = "Assets/Scenes/Chapter1/SC_04_Mactan.unity";
        private const string StoryDir = "Assets/Data/Story";
        private const string CombatDir = "Assets/Data/Combat";
        private const string PrefabDir = "Assets/Prefabs/Enemies";

        [MenuItem("Tools/BAYANI/Phase 2 - Build Mactan 1521 (SEQ 09-10)")]
        public static void Build()
        {
            // ---------- 0) data prerequisites (Phase 1 + 1.10 builders authored these) ----------
            var light1 = AssetDatabase.LoadAssetAtPath<AttackData>($"{CombatDir}/Attack_Light1.asset");
            var light2 = AssetDatabase.LoadAssetAtPath<AttackData>($"{CombatDir}/Attack_Light2.asset");
            var light3 = AssetDatabase.LoadAssetAtPath<AttackData>($"{CombatDir}/Attack_Light3.asset");
            var heavy = AssetDatabase.LoadAssetAtPath<AttackData>($"{CombatDir}/Attack_Heavy.asset");
            var skill = AssetDatabase.LoadAssetAtPath<AttackData>($"{CombatDir}/Attack_DiwaBurst.asset");
            if (light1 == null || skill == null)
            {
                EditorUtility.DisplayDialog("Mactan 1521",
                    "Combat data missing — run 'Phase 1 - Build Combat Arena' first.", "OK");
                return;
            }
            var anino = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/Anino.prefab");
            var lingid = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/Lingid.prefab");
            if (anino == null)
                Debug.LogWarning("[BAYANI] Mactan: enemy prefabs missing — run 'Phase 1.10 - Build Wave Arena', then rebuild. Shrine fight will be skipped.");

            // ---------- 1) Dialogue — verbatim storyline.md SEQ 09 / 09B / 10 ----------
            var seq09 = GetOrCreate($"{StoryDir}/DLG_SEQ09_Mactan.asset", () => Seq("SEQ 09 — Mactan",
                Line("NARRATION", "Days pass. Kai reaches Mactan."),
                Line("NARRATION", "The atmosphere is different. The people are preparing — forges, net-mending, war canoes hauled up, elders teaching children the old refusal."),
                Line("NARRATION", "Lapu-Lapu stands among his warriors. Kai doesn't approach him. Instead, Kai watches."),
                Line("NARRATION", "I came here searching for my ancestor."),
                Line("NARRATION", "But I found something bigger."),
                Line("NARRATION", "A people protecting their home.")));

            var seq09b = GetOrCreate($"{StoryDir}/DLG_SEQ09B_EdgeAwakens.asset", () => Seq("SEQ 09B — The Edge Awakens",
                Line("NARRATION", "At the Mactan shore shrine, Kai's artifact drinks the tide."),
                Line("NARRATION", "The blade unfolds from it — the Balikan Edge."),
                Line("NARRATION", "A weapon made of returning, shaped by the family that hid it for 666 years."),
                Line("NARRATION", "The first person to bow to Kai holding it is the blacksmith who forged its ancestor."),
                Line("NARRATION", "The artifact was never a key only — it is an inherited weapon system, dormant until carried into the memory it was made in."),
                Line("NARRATION", "The shrine reads the village memory: 80%. Something beneath us is being forgotten. The shadows on the headland will make it brighter — and the edge will teach you to scan them.")));

            var seq10 = GetOrCreate($"{StoryDir}/DLG_SEQ10_NightBefore.asset", () => Seq("SEQ 10 — The Night Before",
                Line("KAI", "Are you afraid?"),
                Line("WARRIOR", "Of course."),
                Line("KAI", "Then why stay?"),
                Line("WARRIOR", "Because courage isn't the absence of fear."),
                Line("WARRIOR", "It is deciding what matters more."),
                Line("NARRATION", "Kai looks toward the village. Families. Children. Elders. Warriors."),
                Line("KAI", "This isn't a page in a textbook. These are people's lives.")));

            // ---------- 2) artifact + shrine-clear wave (SO data) ----------
            var edge = GetOrCreateAsset($"{StoryDir}/Artifact_BalikanEdge.asset", () =>
            {
                var a = ScriptableObject.CreateInstance<ArtifactData>();
                a.name = "Balikan Edge";
                a.artifactId = "AR_0002";
                a.displayName = "Balikan Edge";
                a.description = "TALA: Weapon-class memory, designation BALIKAN EDGE. Forging lineage predates contact records; the alloy reads local, the intent does not. This object answers to the bearer's bloodline. It was made to be found by you. Codex entry written.";
                a.historicalStatus = "fictional";        // §51 enum: invented artifact inside real history
                a.codexCategory = "HERITAGE";
                return a;
            });

            var shrineWave = GetOrCreateAsset($"{CombatDir}/Wave_MactanShrine.asset", () =>
            {
                var w = ScriptableObject.CreateInstance<WaveData>();
                w.name = "Wave_MactanShrine";
                w.waves = new[]
                {
                    new WaveDef
                    {
                        startDelay = 2f,
                        entries = new[]
                        {
                            new SpawnEntry { enemyPrefab = anino, count = 2, spacing = 1.2f },
                            new SpawnEntry { enemyPrefab = lingid, count = 1, spacing = 1f },
                        },
                    },
                };
                return w;
            });

            // ---------- 3) scene + Kai rig ----------
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var camGO = new GameObject("Main Camera") { tag = "MainCamera" };
            camGO.transform.position = new Vector3(0f, 2.6f, 0f);
            var cam = camGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            camGO.AddComponent<AudioListener>();

            var lightGO = new GameObject("Directional Light");
            var sun = lightGO.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.45f;                      // late-afternoon island light
            lightGO.transform.rotation = Quaternion.Euler(44f, -25f, 0f);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = new Vector3(0f, 0f, 35f);
            ground.transform.localScale = new Vector3(6f, 1f, 8f);      // 60 x 80 m village band

            var water = GameObject.CreatePrimitive(PrimitiveType.Plane);
            water.name = "Sea";
            water.transform.position = new Vector3(0f, -0.35f, 82f);
            water.transform.localScale = new Vector3(10f, 1f, 6f);
            Object.DestroyImmediate(water.GetComponent<MeshCollider>());

            var kai = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            kai.name = "Kai";
            kai.tag = "Player";
            kai.transform.position = new Vector3(0f, 1.2f, 2f);
            Object.DestroyImmediate(kai.GetComponent<CapsuleCollider>());
            var cc = kai.AddComponent<CharacterController>();
            cc.center = Vector3.zero; cc.radius = 0.5f; cc.height = 2f;
            var pc = kai.AddComponent<PlayerController>();
            pc.lookCamera = cam;
            camGO.AddComponent<ThirdPersonFollowCamera>().target = kai.transform;

            var resources = kai.AddComponent<CombatResources>();
            var hurtbox = kai.AddComponent<Hurtbox>();
            hurtbox.team = Hitbox.Team.Player;
            var combat = kai.AddComponent<PlayerCombat>();
            combat.resources = resources;
            combat.hurtbox = hurtbox;
            var hbGO = new GameObject("Hitbox");
            hbGO.transform.SetParent(kai.transform, false);
            hbGO.transform.localPosition = new Vector3(0f, 0.1f, 0.9f);
            var hb = hbGO.AddComponent<Hitbox>();
            hbGO.AddComponent<BoxCollider>().isTrigger = true;
            hb.team = Hitbox.Team.Player;
            hb.owner = kai;
            hbGO.SetActive(false);
            combat.hitbox = hb;
            combat.combo = new[] { light1, light2, light3, heavy };
            combat.skill = skill;

            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/Settings/DefaultVolumeProfile.asset");
            if (profile != null)
            {
                var volume = camGO.AddComponent<Volume>();
                volume.isGlobal = true;
                volume.sharedProfile = profile;
            }

            var story = new GameObject("Story (DialoguePlayer + ScreenFader)");
            story.AddComponent<DialoguePlayer>();
            story.AddComponent<ScreenFader>();
            if (Object.FindFirstObjectByType<CombatHUD>() == null)
                new GameObject("CombatHUD (Graybox)").AddComponent<CombatHUD>();

            var look = new GameObject("Stability (Zone + Look)");
            look.AddComponent<Volume>();
            look.AddComponent<StabilityLook>();
            var zone = look.AddComponent<MemoryStabilityZone>();
            zone.zoneName = "Mactan";                   // start 80 (§56.3); kills visibly restore

            // ---------- 4) village: gate → forges → longhouses (SEQ 09) ----------
            Box("Gate_L", new Vector3(-3f, 1.5f, 6f), new Vector3(0.5f, 3f, 0.5f));
            Box("Gate_R", new Vector3(3f, 1.5f, 6f), new Vector3(0.5f, 3f, 0.5f));
            for (int i = 0; i < 3; i++)
                Box($"Forge_{i + 1}", new Vector3(-6f, 0.6f, 14f + i * 4f), new Vector3(2f, 1.2f, 2f));
            for (int i = 0; i < 4; i++)
            {
                float side = i % 2 == 0 ? -1f : 1f;
                Box($"Longhouse_{i + 1}", new Vector3(side * 7.5f, 1.1f, 24f + i * 4f), new Vector3(4f, 2.2f, 6f));
            }
            Npc("Warrior_Headland", new Vector3(9f, 1.2f, 30f), 1.1f);      // Lapu-Lapu, watched not met (SEQ 09)
            Npc("Warrior_A", new Vector3(10.5f, 1.2f, 31.5f), 1f);
            Npc("Warrior_B", new Vector3(8f, 1.2f, 32f), 1f);
            Trigger("SEQ09_Gate", new Vector3(0f, 1.2f, 7f), new Vector3(8f, 3f, 6f), seq09,
                auto: true, titleCard: "1521 — MACATAN");

            // ---------- 5) shore shrine (SEQ 09B): unfold → scan → story-triggered pack ----------
            Box("Shrine_Platform", new Vector3(0f, 0.15f, 62f), new Vector3(8f, 0.3f, 6f));
            Box("Shrine_Stone", new Vector3(0f, 0.9f, 64f), new Vector3(1.6f, 1.5f, 0.8f));
            var vessel = Box("Vessel_Pedestal", new Vector3(0f, 0.75f, 61.5f), new Vector3(0.6f, 0.6f, 0.6f));
            var vCol = vessel.GetComponent<BoxCollider>();
            vCol.isTrigger = true;              // scanner uses OnTriggerStay — solid would push Kai off the prompt
            vCol.size = new Vector3(2f, 2f, 2f);

            var unfold = Trigger("SEQ09B_Shrine", new Vector3(0f, 1.2f, 62f), new Vector3(7f, 3f, 5f), seq09b,
                auto: true, titleCard: "SEQ 09B — THE EDGE AWAKENS");

            var scan = vessel.AddComponent<ArtifactScanner>();
            scan.data = edge;
            scan.prompt = "[E/F] Scan the Balikan Edge";

            if (anino != null)
            {
                var spawnerGO = new GameObject("ShrineClear (wave)");
                var spawner = spawnerGO.AddComponent<WaveSpawner>();
                spawner.data = shrineWave;
                spawner.autoStart = false;              // the pack answers the Edge unfolding — not scene load
                spawner.clearTitle = "THE HEADLAND IS CLEAR — the village remembers. (stability rising)";
                var center = new GameObject("ShrineCenter");
                center.transform.position = new Vector3(0f, 0f, 62f);
                spawner.arenaCenter = center.transform;
                var points = new System.Collections.Generic.List<Transform>();
                for (int i = 0; i < 4; i++)
                {
                    var p = new GameObject($"Spawn_{i + 1}");
                    p.transform.position = new Vector3(i < 2 ? -6f : 6f, 0.2f, i % 2 == 0 ? 56f : 68f);
                    points.Add(p.transform);
                }
                spawner.spawnPoints = points.ToArray();
                unfold.GetComponent<StoryTrigger>().spawnOnFinish = spawner;
            }

            // cave-mouth foreshadow (10B territory — geometry only, no beats yet)
            Box("Sea_Cave_Mouth", new Vector3(14f, 1f, 74f), new Vector3(5f, 2.5f, 3f));

            // ---------- 6) fire circle (SEQ 10) ----------
            Box("Fire_Circle", new Vector3(0f, 0.06f, 44f), new Vector3(7f, 0.12f, 7f));
            Box("Hearth_Fire", new Vector3(0f, 0.4f, 44f), new Vector3(1f, 0.7f, 1f));
            Npc("Old_Warrior", new Vector3(-2.5f, 1.2f, 44f), 1.05f);
            var seq10GO = Trigger("SEQ10_FireCircle", new Vector3(0f, 1.2f, 44f), new Vector3(6f, 2.6f, 6f), seq10,
                auto: false);
            var seq10T = seq10GO.GetComponent<StoryTrigger>();
            seq10T.prompt = "[E/F] Sit with the old warrior";
            seq10T.finaleCaption =
                "Before dawn, the stability beneath the village collapses to 0%.\nNot naturally. Something down there is feeding.\n(SEQ 10B — the cave beneath the shore — next in the Chapter 1 build)";

            // ---------- 7) save ----------
            EnsureFolder("Assets/Scenes", "Chapter1");
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();

            Debug.Log($"[BAYANI] Mactan 1521 built → {ScenePath}. Play: gate → shrine (blade unfolds, scan it, clear the headland) → fire circle.");
            EditorUtility.DisplayDialog("Mactan 1521 ready",
                "SEQ 09–10 playable:\n\n" +
                "1. enter through the gate — the village prepares (09)\n" +
                "2. shore shrine — the Balikan Edge unfolds, scan it, " +
                "the headland shadows come (09B)\n" +
                "3. fire circle — the night before (10)\n\n" +
                "Killing the pack raises the MEMORY bar — watch the world regain color.", "OK");
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
            EnsureFolder("Assets/Data", "Story");
            var asset = make();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static T GetOrCreateAsset<T>(string path, System.Func<T> make) where T : ScriptableObject
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

        private static GameObject Box(string name, Vector3 pos, Vector3 scale)
        {
            var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
            b.name = name;
            b.transform.position = pos;
            b.transform.localScale = scale;
            return b;
        }

        private static GameObject Npc(string name, Vector3 pos, float scale)
        {
            var c = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            c.name = name;
            c.transform.position = pos;
            c.transform.localScale = Vector3.one * scale;
            return c;
        }

        private static GameObject Trigger(string name, Vector3 pos, Vector3 size, DialogueAsset dialogue,
            bool auto, string titleCard = null)
        {
            var go = new GameObject(name);
            go.transform.position = pos;
            var col = go.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = size;
            var t = go.AddComponent<StoryTrigger>();
            t.dialogue = dialogue;
            t.requiresInteract = !auto;
            if (titleCard != null) t.titleCard = titleCard;
            return go;
        }

        private static GameObject Prop(string name, Vector3 pos, Vector3 size, DialogueAsset dialogue, string prompt)
        {
            var go = Box(name, pos, size);
            var col = go.GetComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = size * 1.6f;
            var t = go.AddComponent<StoryTrigger>();
            t.dialogue = dialogue;
            t.requiresInteract = true;
            t.prompt = prompt;
            return go;
        }
    }
}
