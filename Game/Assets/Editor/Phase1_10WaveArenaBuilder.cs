// BAYANI — Phase 1.10 Wave Arena builder (Editor tool)
// One click: Tools → BAYANI → Phase 1.10: Build Wave Arena
//  1) Creates enemy PREFABS (Assets/Prefabs/Enemies/) from the same graybox recipe
//     the Phase 1 builder uses for scene enemies.
//  2) Creates WaveData_Arena.asset — 3 waves ramping to 10 alive (exit-gate stress).
//  3) Opens SC_00_Prototype, adds a spawn ring + WaveSpawner (+ ScreenFader host for
//     wave title cards), saves. Re-runnable.
// NOTE: run "Phase 1" builder first (needs Kai's combat rig + EnemyData assets).
// The 4 hand-placed arena enemies stay; delete them in the scene for a pure wave run.

using Bayani.Combat;
using Bayani.Enemy;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Bayani.EditorTools
{
    public static class Phase1_10WaveArenaBuilder
    {
        private const string ScenePath = "Assets/Scenes/Prototype/SC_00_Prototype.unity";
        private const string PrefabDir = "Assets/Prefabs/Enemies";
        private const string DataDir = "Assets/Data/Combat";

        [MenuItem("Tools/BAYANI/Phase 1.10 - Build Wave Arena")]
        public static void Build()
        {
            // ---------- gate: Phase 1 assets must exist ----------
            var anino = AssetDatabase.LoadAssetAtPath<EnemyData>($"{DataDir}/Enemy_Anino.asset");
            var lingid = AssetDatabase.LoadAssetAtPath<EnemyData>($"{DataDir}/Enemy_Lingid.asset");
            var bantay = AssetDatabase.LoadAssetAtPath<EnemyData>($"{DataDir}/Enemy_Bantay.asset");
            if (anino == null || lingid == null || bantay == null)
            {
                EditorUtility.DisplayDialog("Wave arena", "Enemy data assets missing — run 'Phase 1 - Build Combat Arena' first.", "OK");
                return;
            }

            // ---------- 1) enemy prefabs ----------
            EnsureFolder("Assets/Prefabs", "Enemies");
            var aninoGO   = GetOrCreatePrefab("Anino", anino, Vector3.one);
            var lingidGO  = GetOrCreatePrefab("Lingid", lingid, Vector3.one * 0.85f);
            var bantayGO  = GetOrCreatePrefab("Bantay", bantay, Vector3.one * 1.3f);

            // ---------- 2) wave table (10 alive at peak — stress test) ----------
            var waves = GetOrCreate($"{DataDir}/WaveData_Arena.asset", () =>
            {
                var d = ScriptableObject.CreateInstance<WaveData>();
                d.waves = new[]
                {
                    new WaveDef { startDelay = 3f, entries = new[] { Entry(aninoGO, 2, 1.2f) } },
                    new WaveDef { entries = new[] { Entry(aninoGO, 3, 1f), Entry(lingidGO, 1, 1f) } },
                    new WaveDef { entries = new[] { Entry(aninoGO, 4, 0.8f), Entry(lingidGO, 2, 1f), Entry(bantayGO, 1, 1f) } },
                };
                d.interWaveDelay = 5f;
                return d;
            });

            // ---------- 3) scene rig ----------
            if (!System.IO.File.Exists(System.IO.Path.Combine(Application.dataPath, "Scenes/Prototype/SC_00_Prototype.unity")))
            {
                EditorUtility.DisplayDialog("Wave arena", $"Scene not found at {ScenePath} — run the Phase 0/1 builders first.", "OK");
                return;
            }
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var old = GameObject.Find("WaveArena");
            if (old != null) Object.DestroyImmediate(old);

            var arena = new GameObject("WaveArena");
            var spawner = arena.AddComponent<WaveSpawner>();
            spawner.data = waves;

            // 8-point spawn ring, r=11 m around origin (prototype ground is 50×50)
            var points = new Transform[8];
            for (int i = 0; i < points.Length; i++)
            {
                var p = new GameObject($"SpawnPoint_{i}");
                p.transform.SetParent(arena.transform, false);
                float a = i * Mathf.PI * 2f / points.Length;
                p.transform.localPosition = new Vector3(Mathf.Cos(a) * 11f, 1.2f, Mathf.Sin(a) * 11f);
                points[i] = p.transform;
            }
            spawner.spawnPoints = points;
            var center = new GameObject("ArenaCenter");
            center.transform.SetParent(arena.transform, false);
            spawner.arenaCenter = center.transform;

            // Memory Stability v1 testbed (ggd §56.3): living Limot drain the score,
            // kills restore it, and the world visibly desaturates as it falls.
            arena.AddComponent<Volume>();
            arena.AddComponent<Bayani.Story.StabilityLook>();
            var zone = arena.AddComponent<Bayani.Core.MemoryStabilityZone>();
            zone.zoneName = "Mactan (arena test)";

            // Phasing 2.2 — scannable relic: TALA panel + Diwa/stability/XP rewards (SO-driven)
            var relic = GetOrCreate($"{DataDir}/Artifact_ShadowCharm.asset", () =>
            {
                var a = ScriptableObject.CreateInstance<Bayani.Story.ArtifactData>();
                a.artifactId = "AR_0001";
                a.displayName = "Shadow Charm";
                a.description = "Carved bone charm, anito-guardian type. TALA: \"Belief record attributes these to village protectors. The corrosion here predates the Limot event — I am... uncertain. Recommend restoration of local memory before conclusion.\"";
                a.historicalStatus = "ethnographic";
                a.codexCategory = "SPIRITS";
                return a;
            });
            var relicGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            relicGO.name = "Scannable_Relic";
            relicGO.transform.SetParent(arena.transform, false);
            relicGO.transform.localPosition = new Vector3(3f, 0.9f, 3f);
            relicGO.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            var rc = relicGO.GetComponent<BoxCollider>();
            rc.isTrigger = true;
            rc.size = Vector3.one * 2.4f;
            relicGO.AddComponent<Bayani.Story.ArtifactScanner>().data = relic;

            // wave title cards need a ScreenFader host in this scene
            if (Object.FindFirstObjectByType<Bayani.Story.ScreenFader>() == null)
            {
                var fader = new GameObject("Story (ScreenFader)");
                fader.AddComponent<Bayani.Story.ScreenFader>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log("[BAYANI] Wave arena ready: 3 waves → 2 / 4 / 7 spawned, up to 10 alive in wave 3. Watch the FPS counter — and the MEMORY bar draining while Limot live!");
            EditorUtility.DisplayDialog("Wave arena ready",
                "Enemy prefabs + WaveData_Arena.asset created.\n\n" +
                "Press PLAY in SC_00_Prototype:\n• Waves spawn in a ring (title cards announce them)\n" +
                "• Wave 3 keeps up to 10 enemies alive → the 60 FPS gate check\n\n" +
                "Tip: delete the hand-placed Anino/Lingid/Bantay objects for a pure wave run.", "OK");
        }

        // ---------- helpers ----------
        private static SpawnEntry Entry(GameObject prefab, int count, float spacing) =>
            new SpawnEntry { enemyPrefab = prefab, count = count, spacing = spacing };

        private static GameObject GetOrCreatePrefab(string name, EnemyData data, Vector3 scale)
        {
            string path = $"{PrefabDir}/{name}.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;

            // same graybox recipe as Phase1SceneBuilder.SpawnEnemyIfMissing
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = name;
            go.transform.localScale = scale;
            Object.DestroyImmediate(go.GetComponent<CapsuleCollider>());
            go.AddComponent<CharacterController>().center = Vector3.zero;

            var hurt = go.AddComponent<Hurtbox>();
            hurt.team = Hitbox.Team.Enemy;

            var hbGO = new GameObject("Hitbox");
            hbGO.transform.SetParent(go.transform, false);
            hbGO.transform.localPosition = new Vector3(0f, 0.1f, 0.9f);
            hbGO.transform.localScale = new Vector3(1.1f, 1.6f, 1.8f);
            var hb = hbGO.AddComponent<Hitbox>();
            hb.gameObject.AddComponent<BoxCollider>().isTrigger = true;
            hb.team = Hitbox.Team.Enemy;
            hb.owner = go;
            hbGO.SetActive(false);

            var enemy = go.AddComponent<LimotEnemy>();
            enemy.data = data;
            enemy.hurtbox = hurt;
            enemy.hitbox = hb;                       // player self-wires in Awake at runtime

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        private static T GetOrCreate<T>(string path, System.Func<T> make) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;
            AssetDatabase.CreateAsset(make(), path);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        private static void EnsureFolder(string parent, string folder)
        {
            if (AssetDatabase.IsValidFolder($"{parent}/{folder}")) return;
            if (!AssetDatabase.IsValidFolder(parent))
                AssetDatabase.CreateFolder("Assets", parent.Substring("Assets/".Length));
            AssetDatabase.CreateFolder(parent, folder);
        }
    }
}
