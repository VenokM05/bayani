// BAYANI — Phase 0 scene builder (Editor tool)
// One-click replacement for the manual Phase 0.5 setup:
//   Ground plane + Kai capsule (CharacterController + PlayerController)
//   + Main Camera (ThirdPersonFollowCamera wired) + light + URP global volume.
// Usage in Unity: menu  Tools → BAYANI → Phase 0: Build Prototype Scene
// Saves the scene to Assets/Scenes/Prototype/SC_00_Prototype.unity

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Bayani.EditorTools
{
    public static class Phase0SceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Prototype/SC_00_Prototype.unity";

        [MenuItem("Tools/BAYANI/Phase 0 - Build Prototype Scene")]
        public static void Build()
        {
            // --- New empty scene ---
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // --- Camera ---
            var camGO = new GameObject("Main Camera") { tag = "MainCamera" };
            camGO.transform.position = new Vector3(0f, 2.6f, -4f);
            var cam = camGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            camGO.AddComponent<AudioListener>();

            // --- Sun ---
            var lightGO = new GameObject("Directional Light");
            var light = lightGO.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 3.3f;
            lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // --- Ground ---
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(5f, 1f, 5f); // 50x50 m

            // --- Player (Kai graybox capsule) ---
            var kai = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            kai.name = "Kai";
            kai.transform.position = new Vector3(0f, 1.2f, 0f);

            // Replace the capsule collider with a CharacterController (origin-centered)
            Object.DestroyImmediate(kai.GetComponent<CapsuleCollider>());
            var cc = kai.AddComponent<CharacterController>();
            cc.center = Vector3.zero;
            cc.radius = 0.5f;
            cc.height = 2f;

            var pc = kai.AddComponent<Bayani.Player.PlayerController>();
            pc.lookCamera = cam;   // public [SerializeField] field — direct wiring (Unity 6 API-safe)

            // --- Follow camera wiring ---
            var tfc = camGO.AddComponent<Bayani.Core.ThirdPersonFollowCamera>();
            tfc.target = kai.transform;

            // --- URP global volume (tonemapping etc., if profile exists) ---
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/Settings/DefaultVolumeProfile.asset");
            if (profile != null)
            {
                var volume = camGO.AddComponent<Volume>();
                volume.isGlobal = true;
                volume.sharedProfile = profile;
            }

            // --- Save ---
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);

            Debug.Log($"[BAYANI] Phase 0 scene built → {ScenePath}. Press Play: WASD move, Shift run, Space jump, Mouse look.");
            EditorUtility.DisplayDialog("Phase 0 scene ready",
                "SC_00_Prototype created and saved.\n\nPress PLAY to test the exit gate:\n" +
                "• WASD moves Kai\n• Mouse orbits camera\n• Shift runs, Space jumps\n\n" +
                "If it walks at 60 FPS, Phase 0 is DONE.", "OK");
        }
    }
}
