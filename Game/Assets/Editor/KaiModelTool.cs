// BAYANI — One-click Kai model swap (Tools → BAYANI → Kai: Swap In Model (kai.fbx)).
// Replaces the graybox capsule mesh with Assets/Art/Characters/kai.fbx in both
// Chapter 1 graybox scenes (prototype arena + 2187 prologue):
//   • importer: stripBones OFF (preserve the skeleton), FBX scaled to fit the
//     2 m controller (feet at capsule bottom, y = -1)
//   • new "Visual" child (root yaw stays mouse-only — do not parent the controller)
//   • PlayerController.visual wired to it + KaiWalkBob procedural motion
//   • old capsule renderer disabled, never deleted (reversible)
// Re-runnable: swaps cleanly over a previous install.

using Bayani.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Bayani.EditorTools
{
    [InitializeOnLoad]
    public static class KaiModelTool
    {
        private const string FbxPath = "Assets/Art/Characters/kai.fbx";
        private const string FallbackMatPath = "Assets/Art/Characters/Mat_Kai_Fallback.mat";
        private const float TargetHeight = 1.9f;    // user verdict: graybox-fit felt small — fill the 2 m controller

        // Spawn positions per scene — the first buggy install zeroed Kai's root transform;
        // the self-repair in SwapInScene restores it.
        private static readonly (string Path, Vector3 Spawn)[] Scenes =
        {
            ("Assets/Scenes/Prototype/SC_00_Prototype.unity", new Vector3(0f, 1.2f, 0f)),
            ("Assets/Scenes/Chapter1/SC_00_FutureManila.unity", new Vector3(0f, 1.2f, 1.5f)),
            ("Assets/Scenes/Chapter1/SC_03_Cebu.unity", new Vector3(0f, 1.2f, 4f)),
            ("Assets/Scenes/Chapter1/SC_04_Mactan.unity", new Vector3(0f, 1.2f, 2f)),
        };

        // Menu validation: only show when the FBX is actually in the project,
        // and never while playing — EditorSceneManager.OpenScene throws in Play mode.
        [MenuItem("Tools/BAYANI/Kai - Swap In Model (kai.fbx)", true, 30)]
        private static bool CanSwap() => !Application.isPlaying
            && AssetDatabase.LoadAssetAtPath<GameObject>(FbxPath) != null;

        [MenuItem("Tools/BAYANI/Kai - Swap In Model (kai.fbx)", false, 30)]
        private static void Swap()
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Kai model", "Exit Play mode first — this tool edits and re-saves scenes.", "OK");
                return;
            }
            ConfigureImporter();
            int done = 0;
            foreach (var (scenePath, spawn) in Scenes)
            {
                if (!System.IO.File.Exists(Abs(scenePath)))
                {
                    Debug.Log($"[BAYANI] Kai swap: scene not found on disk — {scenePath}");
                    continue;
                }
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                if (SwapInScene(spawn))
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                    done++;
                }
                else Debug.Log($"[BAYANI] Kai swap: opened {scenePath} but no active 'Kai' GameObject in it.");
            }
            Debug.Log($"[BAYANI] Kai model swap finished on {done} scene(s). The capsule mesh is disabled, not deleted.");
            EditorUtility.DisplayDialog("Kai model",
                done > 0
                    ? $"Model installed in {done} scene(s):\n{FbxPath}\n\nScale auto-fit to {TargetHeight} m, walk-bob added.\nPress Play in either scene to see Kai wearing the model."
                    : "No scene contained a 'Kai' — run the Phase 0/2 builders first.",
                "OK");
        }

        [MenuItem("Tools/BAYANI/Kai - Restore Graybox Capsule", false, 31)]
        private static void Restore()
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Kai model", "Exit Play mode first — this tool edits and re-saves scenes.", "OK");
                return;
            }
            foreach (var (scenePath, spawn) in Scenes)
            {
                if (!System.IO.File.Exists(Abs(scenePath))) continue;
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                var kai = FindKai();
                if (kai == null) continue;
                kai.SetActive(true);
                if (kai.transform.position.y < 0.5f) kai.transform.position = spawn;
                var vis = kai.transform.Find("Visual");
                if (vis != null) Object.DestroyImmediate(vis.gameObject);
                var stray = kai.transform.Find("CapsuleGraybox");
                if (stray != null) Object.DestroyImmediate(stray.gameObject);
                var mr = kai.GetComponent<MeshRenderer>();
                if (mr != null) mr.enabled = true;                 // graybox capsule back on
                var pc = kai.GetComponent<PlayerController>();
                if (pc != null) pc.visual = null;                  // runtime rebuilds a proper Visual child
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
            Debug.Log("[BAYANI] Kai restored to graybox capsule in all found scenes.");
        }

        // ---------------- internals ----------------

        // Relative "Assets/..." paths resolve against the launcher's cwd — always probe the absolute one.
        private static string Abs(string assetPath) =>
            System.IO.Path.Combine(Application.dataPath, assetPath.Substring("Assets/".Length));

        private static void ConfigureImporter()
        {
            // Unity 6 churned the ModelImporter skeleton APIs (skeletonInfo → animationConfigInfo,
            // stripBones → optimizeTransformHierarchy across versions) — resolve by reflection so
            // this tool compiles on every 6000.x and simply no-ops if nothing matches.
            var importer = AssetImporter.GetAtPath(FbxPath) as ModelImporter;
            if (importer == null) return;
            var flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;

            foreach (var infoName in new[] { "skeletonInfo", "animationConfigInfo", "animationConfigurationInfo" })
            {
                var infoProp = typeof(ModelImporter).GetProperty(infoName, flags);
                if (infoProp == null || !infoProp.CanWrite) continue;
                object info = infoProp.GetValue(importer);
                if (info == null) continue;

                foreach (var stripName in new[] { "stripBones", "optimizeTransformHierarchy" })
                {
                    var stripProp = info.GetType().GetProperty(stripName, flags);
                    if (stripProp == null || stripProp.PropertyType != typeof(bool)) continue;
                    bool current = (bool)stripProp.GetValue(info);
                    const bool wantKeep = false;   // both flags named for REMOVING bones → false keeps them
                    if (current != wantKeep)
                    {
                        stripProp.SetValue(info, wantKeep);
                        infoProp.SetValue(importer, info);
                        importer.SaveAndReimport();
                        Debug.Log($"[BAYANI] kai.fbx importer: bones preserved via {infoName}.{stripName}.");
                    }
                    return;
                }
            }
            Debug.LogWarning("[BAYANI] Could not set 'Strip Bones' via script on this Unity version — if the model has no skeleton, uncheck Import Settings > Strip Bones manually once.");
        }

        private static bool SwapInScene(Vector3 spawn)
        {
            // Inactive-aware find: the first buggy install saved Kai's root DEACTIVATED,
            // and GameObject.Find can't see inactive objects.
            var kai = FindKai();
            if (kai == null) return false;

            // Self-repair of that broken state: re-activate + restore spawn if the root was zeroed.
            kai.SetActive(true);
            if (kai.transform.position.y < 0.5f) kai.transform.position = spawn;

            var model = AssetDatabase.LoadAssetAtPath<GameObject>(FbxPath);

            // Re-run safety: drop a previously installed Visual and the stray empty
            // "CapsuleGraybox" child the old reparent approach left behind.
            var oldVis = kai.transform.Find("Visual");
            if (oldVis != null) Object.DestroyImmediate(oldVis.gameObject);
            var stray = kai.transform.Find("CapsuleGraybox");
            if (stray != null) Object.DestroyImmediate(stray.gameObject);

            // The capsule MeshRenderer sits ON Kai's root — reparenting it moves the whole
            // controller (the exact bug that broke this tool first). Disable it in place.
            var capsule = kai.GetComponent<MeshRenderer>();
            if (capsule != null) capsule.enabled = false;

            // Install: Kai/Visual/model — the Visual node stays unit-scaled;
            // only the model instance carries the fit-scale.
            var vis = new GameObject("Visual");
            vis.transform.SetParent(kai.transform, false);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(model, vis.transform);
            instance.name = "kai_model";
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;

            AutoUpright(instance);
            FitAndGround(instance);
            FixMissingMaterials(instance);

            // Bob goes on the MODEL instance, not the Visual node — PlayerController
            // owns Visual's rotation (movement facing); bob layers local motion on top.
            instance.AddComponent<KaiWalkBob>();
            var pc = kai.GetComponent<PlayerController>();
            if (pc != null) pc.visual = vis.transform;
            return true;
        }

        // ---------- quick tune menus (operate on the installed model, save scenes) ----------

        [MenuItem("Tools/BAYANI/Kai - Tune: Turn Around 180", false, 32)]
        private static void TuneTurn() => TuneEach(inst => inst.transform.localRotation *= Quaternion.Euler(0f, 180f, 0f), "turned 180°");

        [MenuItem("Tools/BAYANI/Kai - Tune: Grow 15%", false, 33)]
        private static void TuneGrow() => TuneEach(inst => ScaleAroundFeet(inst, 1.15f), "grown 15%");

        [MenuItem("Tools/BAYANI/Kai - Tune: Shrink 15%", false, 34)]
        private static void TuneShrink() => TuneEach(inst => ScaleAroundFeet(inst, 1f / 1.15f), "shrunk 15%");

        private static void TuneEach(System.Action<GameObject> tune, string what)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Kai model", "Exit Play mode first — this tool edits and re-saves scenes.", "OK");
                return;
            }
            int done = 0;
            foreach (var (scenePath, _) in Scenes)
            {
                if (!System.IO.File.Exists(Abs(scenePath))) continue;
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                var kai = FindKai();
                var vis = kai != null ? kai.transform.Find("Visual") : null;
                var inst = vis != null ? vis.Find("kai_model") : null;
                if (inst == null) continue;
                tune(inst.gameObject);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                done++;
            }
            Debug.Log($"[BAYANI] Kai model {what} in {done} scene(s).");
        }

        /// <summary>Scale the model but keep its feet planted (pivot is usually at the ankles, not the soles).</summary>
        private static void ScaleAroundFeet(GameObject instance, float factor)
        {
            var t = instance.transform;
            var rs = instance.GetComponentsInChildren<Renderer>();
            if (rs.Length == 0) return;
            var b = CombineBounds(rs);
            Vector3 foot = new Vector3((b.min.x + b.max.x) * 0.5f, b.min.y, (b.min.z + b.max.z) * 0.5f);
            Vector3 pivot = t.position;
            t.localScale *= factor;
            // Scaling about the pivot drags the foot point with it — slide the root back so feet stay put.
            t.position = pivot + (foot - pivot) * (1f - factor);
        }

        // Root lookup that includes INACTIVE root objects (GameObject.Find skips those).
        private static GameObject FindKai() => FindRoot("Kai");

        private static GameObject FindRoot(string name)
        {
            foreach (var go in EditorSceneManager.GetActiveScene().GetRootGameObjects())
                if (go.name == name) return go;
            return null;
        }

        /// <summary>Scale to TargetHeight and place feet at the controller bottom (local y = -1).</summary>
        private static void FitAndGround(GameObject instance)
        {
            var renderers = instance.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return;

            float height = Mathf.Max(CombineBounds(renderers).size.y, 0.01f);
            float scale = TargetHeight / height;
            instance.transform.localScale = Vector3.one * scale;

            // Recompute post-scale (world space), then convert the foot offset to Kai-local.
            var bounds = CombineBounds(renderers);

            var kai = instance.transform.parent.parent;   // model -> Visual -> Kai
            Vector3 c = bounds.center;
            Vector3 footWorld = new Vector3(c.x, bounds.min.y, c.z);   // bottom-CENTER of the bbox
            Vector3 targetFoot = kai.TransformPoint(new Vector3(0f, -1f, 0f));  // capsule bottom
            Vector3 offset = targetFoot - footWorld;
            offset.y = Mathf.Max(offset.y, 0f);           // models are usually sunken, rarely floating
            instance.transform.position += offset;

            Debug.Log($"[BAYANI] Kai model fit: raw height {height:F2} m → scale {scale:F3}, feet grounded.");
        }

        /// <summary>
        /// AI mesh tools export Z-up models: in Unity the character lies prone (the "plank
        /// position" the playtest reported). Try upright candidates and keep the tallest
        /// silhouette; head-up vs head-down is settled by vertex mass above vs below the
        /// bbox mid-height (an upright human's heavy torso sits above center).
        /// </summary>
        private static void AutoUpright(GameObject instance)
        {
            var renderers = instance.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return;
            var t = instance.transform;
            t.localScale = Vector3.one;   // measure pose at neutral scale

            Quaternion best = Quaternion.identity;
            float bestScore = -1f;
            foreach (var rot in new[] { Quaternion.identity, Quaternion.Euler(-90f, 0f, 0f), Quaternion.Euler(90f, 0f, 0f) })
            {
                t.localRotation = rot;
                var b = CombineBounds(renderers);
                float upBias = VertexMassUpperHalf(renderers, b) ? 1f : 0.35f;   // penalize upside-down
                float score = b.size.y * upBias;
                if (score > bestScore) { bestScore = score; best = rot; }
            }
            t.localRotation = best;
            Debug.Log($"[BAYANI] Kai auto-upright: pitch {best.eulerAngles.x:F0}° → standing height {CombineBounds(renderers).size.y:F2} m.");
        }

        private static Bounds CombineBounds(Renderer[] renderers)
        {
            var b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) b.Encapsulate(renderers[i].bounds);
            return b;
        }

        private static bool VertexMassUpperHalf(Renderer[] renderers, Bounds b)
        {
            float above = 0f, below = 0f;
            float mid = b.center.y;
            foreach (var r in renderers)
            {
                var mf = r.GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null) continue;
                var verts = mf.sharedMesh.vertices;
                var m = mf.transform.localToWorldMatrix;
                for (int i = 0; i < verts.Length; i++)
                {
                    float dy = m.MultiplyPoint3x4(verts[i]).y - mid;
                    if (dy >= 0f) above += dy; else below -= dy;
                }
            }
            return above >= below;
        }

        /// <summary>AI-generated FBXs often ship without materials → URP shows them pink. Give them a fallback.</summary>
        private static void FixMissingMaterials(GameObject instance)
        {
            Material fallback = null;
            foreach (var r in instance.GetComponentsInChildren<Renderer>())
            {
                var mats = r.sharedMaterials;
                bool broken = mats.Length == 0 || mats.Length == 1 && string.IsNullOrEmpty(mats[0]?.name);
                if (!broken && mats.Length > 0 && mats[0] != null && mats[0].shader != null &&
                    mats[0].shader.name.Contains("Legacy")) broken = true;   // Standard/unlit leak from DCC
                if (!broken) continue;

                if (fallback == null) fallback = GetOrCreateFallback();
                r.sharedMaterials = new[] { fallback };
            }
        }

        private static Material GetOrCreateFallback()
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(FallbackMatPath);
            if (mat != null) return mat;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            mat = new Material(shader) { name = "Mat_Kai_Fallback", color = new Color(0.64f, 0.45f, 0.31f) };
            AssetDatabase.CreateAsset(mat, FallbackMatPath);
            Debug.Log("[BAYANI] Created URP fallback material (kai.fbx had no usable materials).");
            return mat;
        }
    }
}
