// BAYANI — Phase 1 combat arena builder (Editor tool).
// One click: Tools → BAYANI → Phase 1: Build Combat Arena
//  1) Creates ScriptableObject data assets in Assets/Data/Combat/ (if missing)
//     — L-L-L-H combo + Diwa skill + Anino enemy (all values editable in Inspector)
//  2) Upgrades SC_00_Prototype: Kai gets resources/combat/hitbox, two Anino graybox
//     enemies spawn, HUD object added. Re-runnable (skips existing pieces).

using Bayani.Combat;
using Bayani.Enemy;
using Bayani.Player;
using Bayani.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Bayani.EditorTools
{
    public static class Phase1SceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Prototype/SC_00_Prototype.unity";
        private const string DataDir = "Assets/Data/Combat";

        [MenuItem("Tools/BAYANI/Phase 1 - Build Combat Arena")]
        public static void Build()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (scene.path != ScenePath)
                scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            // ---------- 1) Data assets (exit gate: values live in SOs, not code) ----------
            var light1 = GetOrCreate($"{DataDir}/Attack_Light1.asset", MakeLight("Light 1", 0.16f, 0.10f, 0.22f, 8f));
            var light2 = GetOrCreate($"{DataDir}/Attack_Light2.asset", MakeLight("Light 2", 0.14f, 0.10f, 0.24f, 9f));
            var light3 = GetOrCreate($"{DataDir}/Attack_Light3.asset", MakeLight("Light 3", 0.16f, 0.12f, 0.30f, 11f));
            var heavy = GetOrCreate($"{DataDir}/Attack_Heavy.asset", MakeHeavy());
            var skill = GetOrCreate($"{DataDir}/Attack_DiwaBurst.asset", MakeSkill());
            var anino = GetOrCreate($"{DataDir}/Enemy_Anino.asset", ScriptableObject.CreateInstance<EnemyData>());

            // ---------- 2) Player rig ----------
            var kai = GameObject.Find("Kai");
            if (kai == null) { EditorUtility.DisplayDialog("Phase 1", "No 'Kai' in scene — run the Phase 0 builder first.", "OK"); return; }
            var combat = kai.GetComponent<PlayerCombat>();
            if (combat == null)
            {
                var resources = kai.AddComponent<CombatResources>();
                var hurtbox = kai.AddComponent<Hurtbox>();
                hurtbox.team = Hitbox.Team.Player;
                combat = kai.AddComponent<PlayerCombat>();
                combat.resources = resources;
                combat.hurtbox = hurtbox;

                var hbGO = new GameObject("Hitbox");
                hbGO.transform.SetParent(kai.transform, false);
                hbGO.transform.localPosition = new Vector3(0f, 0.1f, 0.9f);
                var hb = hbGO.AddComponent<Hitbox>();
                hb.gameObject.AddComponent<BoxCollider>().isTrigger = true;
                hb.team = Hitbox.Team.Player;
                hb.owner = kai;
                hbGO.SetActive(false);
                combat.hitbox = hb;
            }
            combat.combo = new[] { light1, light2, light3, heavy };
            combat.skill = skill;

            // ---------- 3) Enemies ----------
            SpawnEnemyIfMissing("Anino_1", new Vector3(4f, 1.2f, 3f), combat, anino);
            SpawnEnemyIfMissing("Anino_2", new Vector3(-4f, 1.2f, 5f), combat, anino);

            // ---------- 4) HUD ----------
            if (Object.FindFirstObjectByType<CombatHUD>() == null)
            {
                var hud = new GameObject("CombatHUD (Graybox)");
                hud.AddComponent<CombatHUD>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("[BAYANI] Phase 1 arena ready. PLAY → LMB×4 combo chain, RMB dodge (time it into an attack = PERFECT DODGE +8 Diwa), Q burst at 25+ Diwa.");
            EditorUtility.DisplayDialog("Phase 1 arena ready",
                "Combat data assets created in Assets/Data/Combat/.\n" +
                "Kai: combo + dodge + skill · 2 Anino enemies · HUD bars.\n\n" +
                "Press PLAY and answer the ONLY question that matters:\n" +
                "→ Does hitting a cube with L-L-L-H feel good?", "OK");
        }

        private static void SpawnEnemyIfMissing(string name, Vector3 pos, PlayerCombat player, EnemyData data)
        {
            if (GameObject.Find(name) != null) return;
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = name;
            go.transform.position = pos;
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
            enemy.hitbox = hb;
            enemy.player = player;
        }

        // ---------- asset factory helpers ----------
        private static T GetOrCreate<T>(string path, T candidate) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;
            if (!AssetDatabase.IsValidFolder(DataDir))
            {
                AssetDatabase.CreateFolder("Assets/Data", "Combat");
                AssetDatabase.Refresh();
            }
            AssetDatabase.CreateAsset(candidate, path);
            return candidate;
        }

        private static AttackData MakeLight(string n, float windup, float active, float recovery, float dmg)
        {
            var a = ScriptableObject.CreateInstance<AttackData>();
            a.attackName = n; a.windup = windup; a.active = active; a.recovery = recovery;
            a.damage = dmg; a.range = 1.6f; a.knockback = 2f;
            return a;
        }

        private static AttackData MakeHeavy()
        {
            var a = ScriptableObject.CreateInstance<AttackData>();
            a.attackName = "HEAVY"; a.isHeavy = true;
            a.windup = 0.30f; a.active = 0.14f; a.recovery = 0.45f;
            a.damage = 18f; a.staminaCost = 15f;      // ggd §56.2 heavy = −15 stamina
            a.knockback = 4f;                          // >=3 staggers telegraphs (phasing 1.7)
            a.range = 1.9f;
            return a;
        }

        private static AttackData MakeSkill()
        {
            var a = ScriptableObject.CreateInstance<AttackData>();
            a.attackName = "DIWA BURST"; a.isSkill = true;
            a.windup = 0.22f; a.active = 0.18f; a.recovery = 0.35f;
            a.damage = 25f; a.diwaCost = 25f;          // spends earned Diwa, big hit
            a.knockback = 4f; a.range = 2.2f;
            return a;
        }
    }
}
