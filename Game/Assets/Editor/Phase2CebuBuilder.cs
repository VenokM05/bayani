// BAYANI — Phase 2 Cebu builder (Editor tool)
// One click: Tools → BAYANI → Phase 2: Build Cebu 1521 (SEQ 06-08)
// Creates scene Assets/Scenes/Chapter1/SC_03_Cebu.unity (LOC_0002 per content/locations/cebu-1521.md):
//   beach arrival (SEQ 06) → Sugbu market strip (SEQ 07) → night shelter (SEQ 07B
//   scripted Limot tutorial) → datu court (SEQ 08 "The Name"). Dialogue SOs in
//   Assets/Data/Story/, lines verbatim from docs/storyline.md (narrative canon).
// REQUIRES: "Phase 1 - Build Combat Arena" run once (loads Attack_/Enemy_ SOs from
// Assets/Data/Combat/) and ideally "Phase 1.10" (Anino prefab for the tutorial).
// Re-runnable: existing dialogue assets are reused; the scene is rebuilt.
// SEQ 07 tone rule (storyline, binding): organized society — trade, diplomacy,
// craftsmanship. Zero "primitive" framing in every line authored here.

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
    public static class Phase2CebuBuilder
    {
        private const string ScenePath = "Assets/Scenes/Chapter1/SC_03_Cebu.unity";
        private const string StoryDir = "Assets/Data/Story";
        private const string CombatDir = "Assets/Data/Combat";

        [MenuItem("Tools/BAYANI/Phase 2 - Build Cebu 1521 (SEQ 06-08)")]
        public static void Build()
        {
            // ---------- 0) combat data prerequisite (Phase 1 builder authored it) ----------
            var light1 = AssetDatabase.LoadAssetAtPath<AttackData>($"{CombatDir}/Attack_Light1.asset");
            var light2 = AssetDatabase.LoadAssetAtPath<AttackData>($"{CombatDir}/Attack_Light2.asset");
            var light3 = AssetDatabase.LoadAssetAtPath<AttackData>($"{CombatDir}/Attack_Light3.asset");
            var heavy = AssetDatabase.LoadAssetAtPath<AttackData>($"{CombatDir}/Attack_Heavy.asset");
            var skill = AssetDatabase.LoadAssetAtPath<AttackData>($"{CombatDir}/Attack_DiwaBurst.asset");
            if (light1 == null || skill == null)
            {
                EditorUtility.DisplayDialog("Cebu 1521",
                    "Combat data assets missing — run 'Phase 1 - Build Combat Arena' once first.", "OK");
                return;
            }

            // ---------- 1) Dialogue data — verbatim storyline.md SEQ 06–08 ----------
            var seq06 = GetOrCreate($"{StoryDir}/DLG_SEQ06_Arrival.asset", () => Seq("SEQ 06 — Cebu, 1521",
                Line("NARRATION", "Kai wakes up on a beach. Not the ruin-coast — a different shoreline, an older sea."),
                Line("NARRATION", "I had read about this moment."),
                Line("NARRATION", "Ships from another world arrived in our islands."),
                Line("NARRATION", "But history had always been words on a page."),
                Line("NARRATION", "Now..."),
                Line("NARRATION", "I was standing inside it."),
                Line("VILLAGER", "The strangers have arrived."),
                Line("KAI", "This is Cebu.")));

            var seq07 = GetOrCreate($"{StoryDir}/DLG_SEQ07_PeopleOfCebu.asset", () => Seq("SEQ 07 — The People of Cebu",
                Line("NARRATION", "Kai is discovered by locals. Their eyes catch the strange clothing — fabrics with no year they know."),
                Line("NARRATION", "Kai tries to explain who he is. Nobody believes him."),
                Line("NARRATION", "He is brought toward the settlement. Foreign visitors. Local leaders. Trade. Gifts. Different languages. Different beliefs."),
                Line("NARRATION", "A port at full work: bangka fleets, hammered outriggers, gold and cotton, courts that speak in trade-metaphors."),
                Line("KAI", "This isn't simply a battle. It is a moment when two worlds are meeting for the first time.")));

            var seq07bIntro = GetOrCreate($"{StoryDir}/DLG_SEQ07B_Intro.asset", () => Seq("SEQ 07B — The First Shadow",
                Line("NARRATION", "Night. Kai is shown to a shelter by a Cebuano family."),
                Line("NARRATION", "The firelight flickers wrong. The shadows peel off the walls."),
                Line("NARRATION", "Something formless comes for Kai."),
                Line("ELDER", "The anito of forgotten places grow bold when strangers carry old light."),
                Line("NARRATION", "The artifact flares — and holds the shadow at bay. Fight it. Learn it. You cannot fall here.")));

            var seq07bRescue = GetOrCreate($"{StoryDir}/DLG_SEQ07B_Rescue.asset", () => Seq("SEQ 07B — Naming",
                Line("NARRATION", "The elder's blade finds the shadow. It unravels like smoke pulled out of a flame."),
                Line("VILLAGER", "Limot."),
                Line("NARRATION", "That which is forgotten."),
                Line("NARRATION", "The Limot are what memory becomes when it is eaten. They swarm where the past is being forgotten."),
                Line("NARRATION", "And the artifact answered to Kai's blood. This is the first proof it chose him.")));

            var seq08 = GetOrCreate($"{StoryDir}/DLG_SEQ08_TheName.asset", () => Seq("SEQ 08 — The Name",
                Line("NARRATION", "In the days that follow, Kai hears a name over and over."),
                Line("TRADER", "Humabon."),
                Line("NARRATION", "Then another name, spoken lower, near the war canoes."),
                Line("WARRIOR", "Lapu-Lapu."),
                Line("KAI", "Mactan."),
                Line("NARRATION", "History is moving toward that battle. Kai wants to stop it."),
                Line("ELDER", "You speak as if you already know tomorrow."),
                Line("KAI", "I do."),
                Line("ELDER", "Then perhaps you should be afraid of tomorrow.")));

            // ---------- 2) scene + Kai rig (prologue recipe + Phase 1 combat wiring) ----------
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var camGO = new GameObject("Main Camera") { tag = "MainCamera" };
            camGO.transform.position = new Vector3(0f, 2.6f, 2f);
            var cam = camGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            camGO.AddComponent<AudioListener>();

            var lightGO = new GameObject("Directional Light");
            var sun = lightGO.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.5f;                       // tropical 1521 sun, not the 2187 overcast
            lightGO.transform.rotation = Quaternion.Euler(52f, -30f, 0f);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = new Vector3(0f, 0f, 30f);
            ground.transform.localScale = new Vector3(6f, 1f, 9f);      // 60 x 90 m settlement strip

            var water = GameObject.CreatePrimitive(PrimitiveType.Plane);
            water.name = "Sea";
            water.transform.position = new Vector3(0f, -0.35f, -25f);
            water.transform.localScale = new Vector3(10f, 1f, 5f);
            Object.DestroyImmediate(water.GetComponent<MeshCollider>());

            var kai = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            kai.name = "Kai";
            kai.tag = "Player";
            kai.transform.position = new Vector3(0f, 1.2f, 4f);
            Object.DestroyImmediate(kai.GetComponent<CapsuleCollider>());
            var cc = kai.AddComponent<CharacterController>();
            cc.center = Vector3.zero; cc.radius = 0.5f; cc.height = 2f;
            var pc = kai.AddComponent<PlayerController>();
            pc.lookCamera = cam;
            camGO.AddComponent<Bayani.Core.ThirdPersonFollowCamera>().target = kai.transform;

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

            // Memory Stability zone (ggd §56.3): Sugbu arrival = 100 − 20 = 80
            var look = new GameObject("Stability (Zone + Look)");
            look.AddComponent<Volume>();
            look.AddComponent<StabilityLook>();
            var zone = look.AddComponent<MemoryStabilityZone>();
            zone.zoneName = "Sugbu";

            // ---------- 3) Beach: ships offshore + arrival (SEQ 06) ----------
            for (int i = 0; i < 3; i++)
                Box($"Ship_{i + 1}", new Vector3(-10f + i * 9f, 0.6f, -36f - i * 3f), new Vector3(4f, 1.6f, 9f));
            Trigger("SEQ06_Arrival", new Vector3(0f, 1.2f, 4f), new Vector3(10f, 3f, 10f), seq06,
                auto: true, titleCard: "1521 — SUGBU (CEBU)");

            // ---------- 4) Market strip (SEQ 07): stalls + bangka + villagers ----------
            Box("Bangka_1", new Vector3(6f, 0.3f, 10f), new Vector3(1.4f, 0.5f, 7f));
            Box("Bangka_2", new Vector3(8.5f, 0.3f, 11f), new Vector3(1.4f, 0.5f, 7f));
            for (int i = 0; i < 4; i++)
            {
                float z = 18f + i * 5f;
                Box($"Stall_W{i}", new Vector3(-4.5f, 1f, z), new Vector3(2.4f, 2f, 2.4f));
                Box($"Stall_E{i}", new Vector3(4.5f, 1f, z + 2.5f), new Vector3(2.4f, 2f, 2.4f));
            }
            Npc("Villager_Market", new Vector3(-2.5f, 1.2f, 21f), 0.9f);
            Trigger("SEQ07_People", new Vector3(0f, 1.2f, 26f), new Vector3(9f, 3f, 12f), seq07, auto: true);

            // ---------- 5) Night shelter: SEQ 07B scripted tutorial ----------
            Box("Shelter_Floor", new Vector3(0f, 0.05f, 41f), new Vector3(9f, 0.1f, 8f));
            Box("Shelter_W", new Vector3(-4.5f, 1.2f, 41f), new Vector3(0.3f, 2.4f, 8f));
            Box("Shelter_E", new Vector3(4.5f, 1.2f, 41f), new Vector3(0.3f, 2.4f, 8f));
            Box("Shelter_N", new Vector3(0f, 1.2f, 45f), new Vector3(9f, 2.4f, 0.3f));
            Box("Hearth", new Vector3(0f, 0.3f, 40f), new Vector3(1f, 0.6f, 1f));
            var elder = Npc("Elder_SHelter", new Vector3(-3f, 1.2f, 39f), 1f);

            var ambushGO = new GameObject("SEQ07B_Ambush (trigger)");
            ambushGO.transform.position = new Vector3(0f, 1.2f, 40.5f);
            var ambushCol = ambushGO.AddComponent<BoxCollider>();
            ambushCol.isTrigger = true;
            ambushCol.size = new Vector3(7f, 3f, 6f);
            var ambush = ambushGO.AddComponent<TutorialAmbush>();
            ambush.intro = seq07bIntro;
            ambush.rescue = seq07bRescue;
            ambush.enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/Anino.prefab");
            var spawn = new GameObject("AninoSpawn");
            spawn.transform.position = new Vector3(2.5f, 1.2f, 42.5f);
            ambush.enemySpawnPoint = spawn.transform;
            ambush.elderPos = elder.transform;
            if (ambush.enemyPrefab == null)
                Debug.LogWarning("[BAYANI] Cebu: Anino.prefab missing — SEQ 07B plays dialogue-only. Run 'Phase 1.10' once, then rebuild Cebu.");

            // ---------- 6) Datu court: SEQ 08 ----------
            Box("Court_Platform", new Vector3(0f, 0.25f, 55f), new Vector3(12f, 0.5f, 8f));
            Box("Court_Back", new Vector3(0f, 2f, 60f), new Vector3(10f, 3.5f, 0.4f));
            Npc("Warrior_08", new Vector3(2.5f, 1.2f, 54f), 1.05f);
            var humabon = Npc("Datu_Humabon", new Vector3(-2.5f, 1.2f, 56.5f), 1.1f);   // stands; words come later (dialogue pass)
            Prop("Elder_Court", new Vector3(-3.6f, 1.2f, 54.5f), new Vector3(1f, 2f, 1f), seq08,
                prompt: "[E/F] Ask about the names being spoken");

            // ---------- 7) Save ----------
            EnsureFolder("Assets/Scenes", "Chapter1");
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();

            Debug.Log($"[BAYANI] Cebu 1521 built → {ScenePath}. Play: wake on the beach → market → night shelter → court.");
            EditorUtility.DisplayDialog("Cebu 1521 ready",
                "SEQ 06–08 playable:\n\n" +
                "1. wake on the beach — ships offshore (SEQ 06)\n" +
                "2. walk the market strip (SEQ 07)\n" +
                "3. the night shelter — first Limot, no death possible (SEQ 07B)\n" +
                "4. [E/F] the elder at the datu court — 'Humabon'... 'Lapu-Lapu' (SEQ 08)\n\n" +
                "All lines verbatim storyline.md. Run Kai - Swap In Model after, to dress this Kai too.", "OK");
        }

        // ---------- helpers (same recipes as the prologue builder) ----------
        private static DialogueLine Line(string speaker, string text) =>
            new DialogueLine { speaker = speaker, text = text };

        private static DialogueAsset Seq(string title, params DialogueLine[] lines)
        {
            var a = ScriptableObject.CreateInstance<DialogueAsset>();
            a.name = title;
            a.lines = lines;
            return a;
        }

        private static DialogueAsset GetOrCreate(string path, System.Func<DialogueAsset> make)
        {
            var existing = AssetDatabase.LoadAssetAtPath<DialogueAsset>(path);
            if (existing != null) return existing;
            EnsureFolder("Assets/Data", "Story");
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
