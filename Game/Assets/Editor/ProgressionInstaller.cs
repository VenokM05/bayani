// BAYANI — Progression installer (docs/prd-progression.md).
// One click: Tools → BAYANI → Install Progression + Skills
// 1) Authors the data layer (Inspector-tunable, nothing hard-coded):
//    XpTable_Slice (§56.4 budget: L1→L10 = 5,000 XP) + 8 SkillData SOs —
//    armed: Solo Baston · Espada Y Daga · Dos Manos · Baraw
//    unarmed: Suntok · Dumog · Buno · Sipa   (slot 5 reserved)
// 2) Patches EVERY scene containing a "Kai" with PlayerCombat:
//    PlayerProgression + PlayerSkills on Kai, DeathManager on the story host,
//    a Checkpoint at spawn. Re-runnable; run again after rebuilding scenes.
// 3) Sets quest XP on story dialogue SOs (§56.4: quests 250–800) and repairs
//    the Anito's XP to the boss band if its asset predates the fix.
// 4) Authors the skeleton systems' data + components (docs/prd-progression.md §9):
//    auto-heal + stat menu on the story host, an InventoryHost per scene,
//    starter ItemData SOs and a QuestData template.

using Bayani.Combat;
using Bayani.Enemy;
using Bayani.Player;
using Bayani.Story;
using Bayani.Story.QuestGuide;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Bayani.EditorTools
{
    public static class ProgressionInstaller
    {
        private static readonly string SkillDir = "Assets/Data/Skills";
        private static readonly string ProgDir = "Assets/Data/Progression";
        private static readonly string ItemDir = "Assets/Data/Items";
        private static readonly string QuestDir = "Assets/Data/Quests";

        [MenuItem("Tools/BAYANI/Install Progression + Skills")]
        public static void Install()
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Progression", "Exit Play mode first.", "OK");
                return;
            }

            // ---------- 1) data layer ----------
            var table = GetOrCreate($"{ProgDir}/XpTable_Slice.asset", () =>
            {
                var t = ScriptableObject.CreateInstance<XpTable>();
                t.name = "XpTable_Slice";
                return t;   // defaults ARE the §56.4 budget (sum = 5,000)
            });

            var solo = Skill("Skill_SoloBaston", "Solo Baston", SkillCategory.Armed, 1,
                SkillCostType.Stamina, 15f, 4f, 12f, 2.2f, "SkillArmed1", new Color(1f, 0.85f, 0.4f));
            var espada = Skill("Skill_EspadaYDaga", "Espada Y Daga", SkillCategory.Armed, 3,
                SkillCostType.Stamina, 25f, 8f, 20f, 2.4f, "SkillArmed2", new Color(1f, 0.6f, 0.2f));
            var dosManos = Skill("Skill_DosManos", "Dos Manos", SkillCategory.Armed, 5,
                SkillCostType.Diwa, 25f, 12f, 32f, 2.6f, "SkillArmed3", new Color(0.9f, 0.25f, 0.2f));
            var baraw = Skill("Skill_Baraw", "Baraw", SkillCategory.Armed, 7,
                SkillCostType.Stamina, 10f, 3f, 8f, 1.8f, "SkillArmed4", new Color(0.6f, 0.9f, 1f));
            baraw.knockback = 1.5f; baraw.hitDelay = 0.1f; baraw.lungeSpeed = 5f;
            espada.hitDelay = 0.22f; espada.hitDuration = 0.16f;
            dosManos.knockback = 5f; dosManos.hitDelay = 0.3f;      // staggers even the Anito's casts
            solo.knockback = 2f;

            var suntok = Skill("Skill_Suntok", "Suntok", SkillCategory.Unarmed, 1,
                SkillCostType.Stamina, 10f, 3f, 8f, 1.8f, "SkillUnarmed1", new Color(1f, 0.95f, 0.75f));
            suntok.hitDelay = 0.12f; suntok.lungeSpeed = 5f;
            var dumog = Skill("Skill_Dumog", "Dumog", SkillCategory.Unarmed, 2,
                SkillCostType.Stamina, 20f, 7f, 16f, 1.9f, "SkillUnarmed2", new Color(0.8f, 0.6f, 0.9f));
            dumog.knockback = 4f;
            var buno = Skill("Skill_Buno", "Buno", SkillCategory.Unarmed, 4,
                SkillCostType.Diwa, 20f, 10f, 24f, 2f, "SkillUnarmed3", new Color(0.4f, 0.8f, 0.5f));
            buno.knockback = 6f; buno.hitDelay = 0.28f;             // the throw — slam hard
            var sipa = Skill("Skill_Sipa", "Sipa", SkillCategory.Unarmed, 6,
                SkillCostType.Stamina, 15f, 5f, 12f, 2f, "SkillUnarmed4", new Color(1f, 0.75f, 0.5f));
            sipa.hitDelay = 0.15f;

            var loadout = new[] { solo, espada, dosManos, baraw, suntok, dumog, buno, sipa };

            // quest XP on story dialogue (§56.4 quest band 250–800)
            var questXp = new System.Collections.Generic.Dictionary<string, int>
            {
                { "DLG_SEQ01_Waking", 250 }, { "DLG_SEQ02_Argument", 250 }, { "DLG_SEQ03_Transit", 250 },
                { "DLG_SEQ04_Activation", 400 }, { "DLG_SEQ05_Fall", 300 },
                { "DLG_SEQ06_Arrival", 250 }, { "DLG_SEQ07_PeopleOfCebu", 300 }, { "DLG_SEQ08_TheName", 350 },
                { "DLG_SEQ09_Mactan", 300 }, { "DLG_SEQ09B_EdgeAwakens", 400 }, { "DLG_SEQ10_NightBefore", 350 },
                { "DLG_SEQ10B_Intro", 250 },
            };
            foreach (var kv in questXp)
            {
                var d = AssetDatabase.LoadAssetAtPath<DialogueAsset>($"Assets/Data/Story/{kv.Key}.asset");
                if (d != null && d.xpReward == 0) { d.xpReward = kv.Value; EditorUtility.SetDirty(d); }
            }
            // data repair: Anito boss XP to the §56.4 floor if its asset predates the band
            var anitoData = AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/Data/Combat/EnemyData_CorruptedAnito.asset");
            if (anitoData != null && anitoData.xpOnKill < 500f)
            { anitoData.xpOnKill = 500f; EditorUtility.SetDirty(anitoData); }

            // ---------- 1b) skeleton-systems data (PRD §9) ----------
            var stone = GetOrCreate($"{ItemDir}/Item_RiverStone.asset", () =>
            {
                var i = ScriptableObject.CreateInstance<ItemData>();
                i.name = "Item_RiverStone";
                return i;
            });
            stone.itemName = "River Stone"; stone.stackable = true; stone.maxStack = 5;
            stone.description = "Smooth from the Guadalupe current. A habit from home — the river has no memory, so it does not forget you either.";
            EditorUtility.SetDirty(stone);

            var bell = GetOrCreate($"{ItemDir}/Item_ShrineBell.asset", () =>
            {
                var i = ScriptableObject.CreateInstance<ItemData>();
                i.name = "Item_ShrineBell";
                return i;
            });
            bell.itemName = "Shrine Bell"; bell.stackable = false; bell.maxStack = 1;
            bell.hpBonus = 10f;
            bell.description = "Tugs once for every name the battle kept. The first thing the cave gave back.";
            EditorUtility.SetDirty(bell);

            var quest = GetOrCreate($"{QuestDir}/Quest_Template.asset", () =>
            {
                var q = ScriptableObject.CreateInstance<QuestData>();
                q.name = "Quest_Template";
                return q;
            });
            quest.questId = "template_not_wired";   // rename + point a StoryTrigger.questId at it to activate
            quest.title = "Sample Quest";
            quest.objectiveText = "Copy this asset: one SO = one tracked objective with rewards.";
            quest.completionCondition = QuestCondition.DialogueFinished;
            quest.rewardXp = 300; quest.rewardDiwa = 15;
            EditorUtility.SetDirty(quest);

            // visible quest-guide demo: the cave's first words open an objective
            var caveIntro = AssetDatabase.LoadAssetAtPath<DialogueAsset>("Assets/Data/Story/DLG_SEQ10B_Intro.asset");
            if (caveIntro != null && string.IsNullOrEmpty(caveIntro.objectiveText))
            {
                caveIntro.objectiveText = "Find what is eating the nest under the shore";
                caveIntro.autoTrack = true;
                EditorUtility.SetDirty(caveIntro);
            }

            // ---------- 2) patch every Kai scene ----------
            string scenesRoot = System.IO.Path.Combine(Application.dataPath, "Scenes");
            int patched = 0;
            foreach (var file in System.IO.Directory.GetFiles(scenesRoot, "*.unity", System.IO.SearchOption.AllDirectories))
            {
                string assetPath = "Assets" + file.Substring(Application.dataPath.Length).Replace('\\', '/');
                var scene = EditorSceneManager.OpenScene(assetPath, OpenSceneMode.Single);

                GameObject kai = null;
                foreach (var go in scene.GetRootGameObjects())
                    if (go.name == "Kai") { kai = go; break; }
                var combat = kai != null ? kai.GetComponent<PlayerCombat>() : null;
                if (combat == null) continue;           // not a playable scene — leave it alone

                var res = kai.GetComponent<CombatResources>();

                var prog = kai.GetComponent<PlayerProgression>();
                if (prog == null) prog = kai.AddComponent<PlayerProgression>();
                prog.table = table; prog.resources = res; prog.combat = combat;

                var skills = kai.GetComponent<PlayerSkills>();
                if (skills == null) skills = kai.AddComponent<PlayerSkills>();
                skills.combat = combat; skills.loadout = loadout;

                // DeathManager rides on the story host (has DialoguePlayer), else on Kai
                GameObject host = null;
                foreach (var go in scene.GetRootGameObjects())
                {
                    if (go.GetComponent<DialoguePlayer>() != null) { host = go; break; }
                }
                var deathTarget = host != null ? host : kai;
                var death = deathTarget.GetComponent<DeathManager>();
                if (death == null) death = deathTarget.AddComponent<DeathManager>();

                // auto-heal + stat menu ride the same story host (PRD §9)
                var heal = deathTarget.GetComponent<PlayerAutoHeal>();
                if (heal == null) heal = deathTarget.AddComponent<PlayerAutoHeal>();
                heal.resources = res;

                var menu = deathTarget.GetComponent<StatUpgradeMenu>();
                if (menu == null) menu = deathTarget.AddComponent<StatUpgradeMenu>();
                menu.resources = res; menu.progression = prog;

                // inventory host — the GO only carries the singleton; state lives in statics
                GameObject invGo = null;
                foreach (var go in scene.GetRootGameObjects())
                    if (go.name == "InventoryHost") { invGo = go; break; }
                if (invGo == null)
                {
                    invGo = new GameObject("InventoryHost");
                    invGo.transform.position = Vector3.zero;
                }
                if (invGo.GetComponent<Inventory>() == null) invGo.AddComponent<Inventory>();

                // spawn checkpoint at Kai's planted position (one per scene, idempotent)
                GameObject cpGo = null;
                foreach (var go in scene.GetRootGameObjects())
                    if (go.name.StartsWith("Checkpoint_Spawn")) { cpGo = go; break; }
                if (cpGo == null)
                {
                    cpGo = new GameObject("Checkpoint_Spawn");
                    cpGo.transform.position = kai.transform.position;
                }
                if (cpGo.GetComponent<Checkpoint>() == null) cpGo.AddComponent<Checkpoint>();

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene, assetPath);
                patched++;
                Debug.Log($"[BAYANI] Progression installed → {assetPath}");
            }

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Progression + Skills",
                $"Data authored + {patched} scene(s) patched.\n\n" +
                "1–5 skill slots · G swaps armed/unarmed · Q stays Diwa Burst · I opens STATS\n" +
                "Level curve: L1→L10 = 5,000 XP (§56.4) · auto-heal + stat menu + inventory + quest guide installed\n" +
                "Death → YOU DIED → checkpoint respawn, full HP/stamina, XP kept.\n\n" +
                "Re-run this after rebuilding any scene.", "OK");
        }

        // ---------- helpers ----------
        private static SkillData Skill(string file, string name, SkillCategory cat, int unlock,
            SkillCostType costType, float cost, float cd, float dmg, float range, string param, Color vfx)
        {
            var s = GetOrCreate($"{SkillDir}/{file}.asset", () =>
            {
                var a = ScriptableObject.CreateInstance<SkillData>();
                a.name = name;
                return a;
            });
            s.skillName = name; s.category = cat; s.unlockLevel = unlock;
            s.costType = costType; s.cost = cost; s.cooldown = cd;
            s.damage = dmg; s.range = range;
            s.animatorParam = param;                      // fires once Kai has an Animator (Mixamo pass)
            s.castVfxColor = vfx;
            EditorUtility.SetDirty(s);
            return s;
        }

        private static T GetOrCreate<T>(string path, System.Func<T> make) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;
            EnsureFolder(System.IO.Path.GetDirectoryName(path).Replace('\\', '/'));
            var asset = make();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = path.Substring(0, path.LastIndexOf('/'));
            string folder = path.Substring(path.LastIndexOf('/') + 1);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, folder);
        }
    }
}
