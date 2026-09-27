// BAYANI — Phase 2 prologue builder (Editor tool)
// One click: Tools → BAYANI → Phase 2: Build 2187 Prologue (SEQ 01-05)
// Creates scene Assets/Scenes/Chapter1/SC_00_FutureManila.unity:
//   house (SEQ 01 book + SEQ 02 argument) → rain road (SEQ 03) → ruins cave
//   (SEQ 04 wall vision + SEQ 05 artifact) with dialogue assets in Assets/Data/Story/.
// Storyline source: docs/storyline.md SEQ 01-05 — lines quoted verbatim (narrative canon).
// Re-runnable: dialogue assets are skipped if they already exist; scene is rebuilt.

using Bayani.Story;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Bayani.EditorTools
{
    public static class Phase2PrologueBuilder
    {
        private const string ScenePath = "Assets/Scenes/Chapter1/SC_00_FutureManila.unity";
        private const string StoryDir = "Assets/Data/Story";

        [MenuItem("Tools/BAYANI/Phase 2 - Build 2187 Prologue (SEQ 01-05)")]
        public static void Build()
        {
            // ---------- 1) Dialogue data (canon lines from storyline.md) ----------
            var seq01 = GetOrCreate($"{StoryDir}/DLG_SEQ01_TheQuestion.asset", () => Seq("SEQ 01 — The Question",
                Line("NARRATION", "I knew the names of the people who came before me."),
                Line("NARRATION", "But I didn't know their story."),
                Line("KAI", "Why don't we have anything before Lolo?"),
                Line("PARENT", "Some things are better left in the past."),
                Line("KAI", "Why?"),
                Line("NARRATION", "No answer."),
                Line("NARRATION", "And the more I asked... the more my family wanted me to forget.")));

            var seq02 = GetOrCreate($"{StoryDir}/DLG_SEQ02_TheArgument.asset", () => Seq("SEQ 02 — The Argument",
                Line("KAI", "You're hiding something from me."),
                Line("PARENT", "Kai, stop asking questions."),
                Line("KAI", "It's my family."),
                Line("PARENT", "You don't understand."),
                Line("KAI", "Then explain it to me!"),
                Line("NARRATION", "The parent grabs the artifact. For the first time, Kai sees fear in their parent's eyes."),
                Line("PARENT", "Never use this."),
                Line("KAI", "Why?"),
                Line("PARENT", "Because it doesn't bring people back."),
                Line("KAI", "Then what does it do?"),
                Line("NARRATION", "Silence.")));

            var seq03 = GetOrCreate($"{StoryDir}/DLG_SEQ03_RunningAway.asset", () => Seq("SEQ 03 — Running Away",
                Line("NARRATION", "I thought I was running away from my family."),
                Line("NARRATION", "I didn't know I was running toward them.")));

            var seq04 = GetOrCreate($"{StoryDir}/DLG_SEQ04_TheArtifact.asset", () => Seq("SEQ 04 — The Artifact",
                Line("NARRATION", "Inside the ruins, Kai discovers ancient markings. One symbol matches the artifact."),
                Line("NARRATION", "Visions — ocean. Ancient boats. Warriors. A village. Foreign ships. Fire."),
                Line("NARRATION", "A man standing on the shore."),
                Line("NARRATION", "Kai drops the artifact. Then it starts moving by itself. THUMP. THUMP. Like a heartbeat.")));

            var seq05 = GetOrCreate($"{StoryDir}/DLG_SEQ05_TheFall.asset", () => Seq("SEQ 05 — The Fall Through Time",
                Line("NARRATION", "The artifact activates. The entire environment fractures."),
                Line("NARRATION", "2187... then... 1521."),
                Line("NARRATION", "The sound of the future becomes distorted. Then — black.")));

            // ---------- 2) Fresh scene + player rig (Phase 0 pattern) ----------
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var camGO = new GameObject("Main Camera") { tag = "MainCamera" };
            camGO.transform.position = new Vector3(0f, 2.6f, -2f);
            var cam = camGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            camGO.AddComponent<AudioListener>();

            var lightGO = new GameObject("Directional Light");
            var light = lightGO.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 2.6f;                    // overcast 2187 coast
            lightGO.transform.rotation = Quaternion.Euler(48f, 25f, 0f);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = new Vector3(0f, 0f, 40f);
            ground.transform.localScale = new Vector3(5f, 1f, 14f);   // 50 x 140 m corridor

            var kai = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            kai.name = "Kai";
            kai.tag = "Player";                        // StoryTriggers listen for this tag
            kai.transform.position = new Vector3(0f, 1.2f, 1.5f);
            Object.DestroyImmediate(kai.GetComponent<CapsuleCollider>());
            var cc = kai.AddComponent<CharacterController>();
            cc.center = Vector3.zero; cc.radius = 0.5f; cc.height = 2f;
            var pc = kai.AddComponent<Bayani.Player.PlayerController>();
            pc.lookCamera = cam;
            camGO.AddComponent<Bayani.Core.ThirdPersonFollowCamera>().target = kai.transform;

            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/Settings/DefaultVolumeProfile.asset");
            if (profile != null)
            {
                var volume = camGO.AddComponent<Volume>();
                volume.isGlobal = true;
                volume.sharedProfile = profile;
            }

            // Story system host
            var story = new GameObject("Story (DialoguePlayer + ScreenFader)");
            story.AddComponent<DialoguePlayer>();
            story.AddComponent<ScreenFader>();

            // ---------- 3) House (SEQ 01-02) ----------
            Box("House_Floor", new Vector3(0f, 0.05f, 6f), new Vector3(12f, 0.1f, 12f));
            Box("Wall_W", new Vector3(-6f, 1.5f, 6f), new Vector3(0.3f, 3f, 12f));
            Box("Wall_E", new Vector3(6f, 1.5f, 6f), new Vector3(0.3f, 3f, 12f));
            Box("Wall_S", new Vector3(0f, 1.5f, 0f), new Vector3(12f, 3f, 0.3f));
            Box("Wall_N_L", new Vector3(-3.75f, 1.5f, 12f), new Vector3(4.5f, 3f, 0.3f));
            Box("Wall_N_R", new Vector3(3.75f, 1.5f, 12f), new Vector3(4.5f, 3f, 0.3f));   // 3 m door gap
            Box("Table", new Vector3(0f, 0.45f, 4.5f), new Vector3(1.6f, 0.9f, 1f));

            Prop("Family_Book", new Vector3(0f, 1.05f, 4.5f), new Vector3(0.5f, 0.08f, 0.36f), seq01,
                prompt: "[E] Open the family book");

            Trigger("SEQ02_DoorArgument", new Vector3(0f, 1.2f, 10.5f), new Vector3(3f, 2.4f, 1.6f), seq02, auto: true);

            // ---------- 4) Rain road (SEQ 03) ----------
            for (int i = 0; i < 5; i++)
            {
                float z = 18f + i * 8f;
                Box($"Building_W{i}", new Vector3(-9f, 2f + (i % 2), z), new Vector3(4f, 4f + 2f * (i % 2), 4f));
                Box($"Building_E{i}", new Vector3(9f, 3f - (i % 2), z + 4f), new Vector3(4f, 6f - 2f * (i % 2), 4f));
            }
            Trigger("SEQ03_RoadTransit", new Vector3(0f, 1.2f, 34f), new Vector3(6f, 2.4f, 3f), seq03,
                auto: true, titleCard: "2187 — NO SERVICE");

            // ---------- 5) Ruins cave (SEQ 04-05) ----------
            Box("Ruin_Back", new Vector3(0f, 2f, 74f), new Vector3(14f, 4f, 1f));
            Box("Ruin_W", new Vector3(-6.5f, 2f, 69f), new Vector3(1f, 4f, 11f));
            Box("Ruin_E", new Vector3(6.5f, 2f, 69f), new Vector3(1f, 4f, 11f));
            Box("Ruin_Roof", new Vector3(0f, 4.2f, 70f), new Vector3(14f, 0.6f, 9f));

            Prop("Ancient_Wall", new Vector3(0f, 1.6f, 73.3f), new Vector3(3f, 2.4f, 0.4f), seq04,
                prompt: "[E] Touch the marked wall", flash: true);

            Prop("Artifact", new Vector3(0f, 0.7f, 68f), new Vector3(0.6f, 0.6f, 0.6f), seq05,
                prompt: "[E] Take the artifact",
                finaleCaption: "Kai wakes up on a beach.\n(SEQ 06 — CEBU, 1521 — next in the Chapter 1 build)");

            // ---------- 6) Save ----------
            EditorSceneManager.MarkSceneDirty(scene);
            EnsureFolder("Assets/Scenes", "Chapter1");
            EditorSceneManager.SaveScene(scene, ScenePath);

            Debug.Log($"[BAYANI] Phase 2 prologue built → {ScenePath}. Press Play: read the book, then follow the road.");
            EditorUtility.DisplayDialog("2187 Prologue ready",
                "SEQ 01–05 playable:\n\n" +
                "1. [E] the family book (SEQ 01)\n2. walk to the door (SEQ 02)\n" +
                "3. run the road (SEQ 03)\n4. in the ruins: touch the wall + take the artifact (SEQ 04–05)\n\n" +
                "Dialogue advances with E / Enter / gamepad A.\nAll lines are verbatim storyline.md.", "OK");
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

        private static GameObject Prop(string name, Vector3 pos, Vector3 size, DialogueAsset dialogue,
            string prompt, bool flash = false, string finaleCaption = null)
        {
            var go = Box(name, pos, size);
            var col = go.GetComponent<BoxCollider>();
            col.isTrigger = true;              // prop is walk-into + [E]
            col.size = size * 1.6f;
            var t = go.AddComponent<StoryTrigger>();
            t.dialogue = dialogue;
            t.requiresInteract = true;
            t.prompt = prompt;
            t.flashOnStart = flash;
            if (finaleCaption != null) t.finaleCaption = finaleCaption;
            return go;
        }
    }
}
