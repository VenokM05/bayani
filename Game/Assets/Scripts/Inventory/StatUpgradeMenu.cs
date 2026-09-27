// BAYANI — Stat upgrade overlay (docs/prd-progression.md §9). Press I (or Tab /
// gamepad Start) to open: three stats, linear skill-point costs — buying level
// N costs N points (level 1 = 1 point, the "one skill point per level" start).
// Purchases are banked in ProgressStore (stat LEVELS + bonus TOTALS) so they
// survive death and scene loads; PlayerProgression re-applies the totals on
// Awake. Locked until Lvl 2 — the first skill point is the tutorial for this.
// Graybox IMGUI like everything else in Phase 2.

using Bayani.Combat;
using UnityEngine;

namespace Bayani.Player
{
    public class StatUpgradeMenu : MonoBehaviour
    {
        public CombatResources resources;     // installer wires
        public PlayerProgression progression; // stamina recompute routes through it

        [Header("Per-level deltas (Inspector)")]
        public float hpPerLevel = 10f;
        public float staminaRegenPerLevel = 2f;
        public float diwaPerLevel = 10f;
        public int maxStatLevel = 5;

        [Header("Cost: buying level N costs N * costPerLevel points (linear)")]
        public int costPerLevel = 1;

        public bool IsOpen { get; private set; }

        private void Update()
        {
            if (resources == null) resources = FindFirstObjectByType<CombatResources>();

            if (Bayani.Core.BayaniInput.MenuPressed)
            {
                if (IsOpen) { IsOpen = false; return; }
                // the menu's whole currency is skill points — no points, no menu
                if (ProgressStore.Level < 2 || ProgressStore.SkillPoints <= 0)
                {
                    ProgressStore.Toast = "STATS UNLOCK AT LVL 2 — earn your first skill point";
                    return;
                }
                IsOpen = true;
            }
            if (IsOpen && (UnityEngine.InputSystem.Keyboard.current?.escapeKey.wasPressedThisFrame ?? false))
                IsOpen = false;
        }

        private void TryUpgrade(int stat)
        {
            int level = stat switch
            {
                0 => ProgressStore.StatLevelMaxHP,
                1 => ProgressStore.StatLevelStamina,
                _ => ProgressStore.StatLevelDiwa,
            };
            if (level >= maxStatLevel)
            {
                ProgressStore.Toast = $"MAX — {StatName(stat)} is capped at level {maxStatLevel}";
                return;
            }
            int cost = (level + 1) * Mathf.Max(1, costPerLevel);
            if (ProgressStore.SkillPoints < cost)
            {
                ProgressStore.Toast = "No skill points remaining";
                return;
            }
            ProgressStore.SkillPoints -= cost;
            switch (stat)
            {
                case 0:
                    ProgressStore.StatLevelMaxHP++;
                    ProgressStore.StatBonusHP += hpPerLevel;
                    resources?.GrowMaxHP(hpPerLevel);
                    break;
                case 1:
                    ProgressStore.StatLevelStamina++;
                    ProgressStore.StatBonusStaminaRegen += staminaRegenPerLevel;
                    progression?.RefreshStaminaRegen();
                    break;
                default:
                    ProgressStore.StatLevelDiwa++;
                    ProgressStore.StatBonusDiwa += diwaPerLevel;
                    resources?.GrowMaxDiwa(diwaPerLevel);
                    break;
            }
            ProgressStore.Toast = $"{StatName(stat).ToUpper()} → LEVEL {level + 1}  ·  -{cost} SP";
        }

        private static string StatName(int i) => i switch { 0 => "Max HP", 1 => "Stamina Regen", _ => "Diwa Capacity" };
        private int LevelOf(int i) => i switch
        {
            0 => ProgressStore.StatLevelMaxHP,
            1 => ProgressStore.StatLevelStamina,
            _ => ProgressStore.StatLevelDiwa,
        };
        private string ValueOf(int i) => i switch
        {
            0 => resources != null ? $"{resources.MaxHP:0}" : "—",
            1 => resources != null ? $"{resources.StaminaRegenPerSec:0}/s" : "—",
            _ => resources != null ? $"{resources.MaxDiwa:0}" : "—",
        };

        private void OnGUI()
        {
            if (!IsOpen) return;

            float w = Mathf.Min(Screen.width * 0.5f, 520f), h = 268f;
            float x = (Screen.width - w) * 0.5f, y = (Screen.height - h) * 0.5f;

            GUI.color = new Color(0.03f, 0.05f, 0.08f, 0.95f);
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
            GUI.color = new Color(0.92f, 0.72f, 0.25f, 0.9f);
            GUI.DrawTexture(new Rect(x, y, w, 2f), Texture2D.whiteTexture);
            GUI.color = Color.white;

            var title = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold };
            GUI.Label(new Rect(x + 16, y + 10, w - 32, 24), "BAYANI — STATS", title);
            var sp = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperRight, fontSize = 14, fontStyle = FontStyle.Bold };
            sp.normal.textColor = new Color(0.92f, 0.72f, 0.25f);
            GUI.Label(new Rect(x, y + 12, w - 16, 20), $"{ProgressStore.SkillPoints} SKILL POINTS", sp);

            var row = new GUIStyle(GUI.skin.label) { fontSize = 15 };
            row.normal.textColor = new Color(0.9f, 0.9f, 0.94f);

            for (int i = 0; i < 3; i++)
            {
                float ry = y + 48 + i * 56;
                int lvl = LevelOf(i);
                int cost = (lvl + 1) * Mathf.Max(1, costPerLevel);
                GUI.Label(new Rect(x + 16, ry, w - 200, 20),
                    $"{StatName(i)}   now {ValueOf(i)}   ·   Lv {lvl}/{maxStatLevel}", row);
                bool affordable = ProgressStore.SkillPoints >= cost && lvl < maxStatLevel;
                GUI.enabled = affordable;
                if (GUI.Button(new Rect(x + w - 168, ry - 4, 152, 28),
                    lvl >= maxStatLevel ? "MAXED" : $"Upgrade — {cost} SP"))
                    TryUpgrade(i);
                GUI.enabled = true;
            }

            var hint = new GUIStyle(GUI.skin.label) { fontSize = 12 };
            hint.normal.textColor = new Color(1f, 1f, 1f, 0.5f);
            GUI.Label(new Rect(x + 16, y + h - 28, w - 32, 18),
                "Costs scale linearly — level N costs N points · [I / Esc] close", hint);
        }
    }
}
