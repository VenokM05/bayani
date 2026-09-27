// BAYANI — Graybox HUD (phasing.md 1.8 + prd-progression.md §6):
// HP(stamina/diwa with numbers) · MEMORY · LVL/XP · overhead enemy health bars ·
// 5-slot skill bar with cooldowns · toast line.
// IMGUI on purpose: zero scene setup, disposable when the real UI Prefabs arrive.

using System.Collections.Generic;
using Bayani.Combat;
using Bayani.Core;
using UnityEngine;

namespace Bayani.UI
{
    public class CombatHUD : MonoBehaviour
    {
        private CombatResources _res;
        private PlayerCombat _combat;
        private PlayerProgression _prog;
        private PlayerSkills _skills;
        private string _flash = "";
        private float _flashUntil;
        private string _toast;
        private float _toastUntil;
        private float _fps;                          // smoothed fps — the Phase 1 gate box reads this
        private readonly List<IBattleHealth> _tmpBars = new List<IBattleHealth>();

        private void Update()
        {
            _fps = Mathf.Lerp(_fps, 1f / Time.unscaledDeltaTime, Time.unscaledDeltaTime * 4f);
            // retry lookups each frame until found: the HUD may Start() before a
            // scene-loaded/spawned player exists, and components arrive with the installer
            if (_res == null)
            {
                _res = FindFirstObjectByType<CombatResources>();
                if (_res != null) _res.OnResourceChanged += OnResourceChanged;
            }
            if (_combat == null) _combat = FindFirstObjectByType<PlayerCombat>();
            if (_prog == null) _prog = FindFirstObjectByType<PlayerProgression>();
            if (_skills == null) _skills = FindFirstObjectByType<PlayerSkills>();
            if (!string.IsNullOrEmpty(ProgressStore.Toast))
            {
                _toast = ProgressStore.Toast;
                _toastUntil = Time.unscaledTime + 3f;
                ProgressStore.Toast = null;
            }
        }

        private void Start()
        {
            _res = FindFirstObjectByType<CombatResources>();
            _combat = FindFirstObjectByType<PlayerCombat>();
            if (_res != null) _res.OnResourceChanged += OnResourceChanged;
        }

        private void OnDestroy()
        {
            if (_res != null) _res.OnResourceChanged -= OnResourceChanged;
        }

        private void OnResourceChanged(string kind, float delta)
        {
            if (delta > 0f)
            {
                _flash = $"+{delta:0} {kind.ToUpper()}";
                _flashUntil = Time.unscaledTime + 1.2f;
            }
        }

        private void OnGUI()
        {
            if (_res == null && _prog == null) return;   // nothing to show yet
            float w = 260f, x = 20f;

            // §6: HP shows current/max numbers, not just a fill
            if (_res != null)
            {
                Bar(x, 20f, w, _res.HP / _res.MaxHP, new Color(0.85f, 0.25f, 0.25f),
                    $"HP {_res.HP:0}/{_res.MaxHP:0}");
                Bar(x, 46f, w, _res.Stamina / _res.MaxStamina, new Color(0.3f, 0.8f, 0.4f),
                    $"STAMINA {_res.Stamina:0}/{_res.MaxStamina:0}");
                Bar(x, 72f, w, _res.Diwa / _res.MaxDiwa, new Color(0.35f, 0.55f, 0.95f),
                    $"DIWA {_res.Diwa:0}/{_res.MaxDiwa:0}");
            }

            // Memory Stability row — only when a zone exists (ggd §56.3; HUD starts at 80%)
            var zone = MemoryStabilityZone.Current;
            float stateY = 96f;
            if (zone != null)
            {
                Bar(x, 98f, w, zone.Value / 100f, new Color(0.62f, 0.42f, 0.85f), $"MEMORY {zone.Value:0}% · {zone.zoneName}");
                stateY = 124f;
            }

            // LVL/XP row — only once the progression layer is installed
            if (_prog != null)
            {
                int needed = _prog.XpNeeded;
                float xpPct = needed == int.MaxValue ? 1f : Mathf.Clamp01((float)_prog.Xp / needed);
                Bar(x, stateY, w, xpPct, new Color(0.92f, 0.72f, 0.25f),
                    needed == int.MaxValue ? $"LVL {_prog.Level} · MAX" : $"LVL {_prog.Level} · XP {_prog.Xp}/{needed}");
                stateY += 26f;
            }

            GUI.Label(new Rect(x, stateY, 400f, 22f), _combat != null ? _combat.StateText : "");
            if (Time.unscaledTime < _flashUntil)
                GUI.Label(new Rect(x + w + 14f, 72f, 200f, 24f), $"<b>{_flash}</b>");
            if (Time.unscaledTime < _toastUntil && !string.IsNullOrEmpty(_toast))
            {
                var ts = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
                ts.normal.textColor = new Color(0.95f, 0.85f, 0.45f);
                GUI.Label(new Rect(0, Screen.height * 0.22f, Screen.width, 28f), _toast, ts);
            }

            SkillBar();
            OverheadBars();

            GUI.Label(new Rect(20f, Screen.height - 30f, 900f, 24f),
                "ATTACK LMB · BLOCK RMB (timed = PARRY) · DODGE LCtrl · BURST Q · SKILLS 1–5 (swap armed/unarmed G) · INTERACT E/F");

            // FPS, top-right: green ≥55 · yellow ≥40 · red below (60 FPS gate check)
            var fs = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperRight, fontSize = 16, fontStyle = FontStyle.Bold };
            fs.normal.textColor = _fps >= 55f ? new Color(0.4f, 0.9f, 0.45f) : _fps >= 40f ? Color.yellow : Color.red;
            GUI.Label(new Rect(Screen.width - 160f, 14f, 145f, 22f), $"{_fps:F0} FPS", fs);
        }

        // ---- §6: 5-slot skill bar with cooldown indicators ----
        private void SkillBar()
        {
            if (_skills == null) return;
            const float slot = 84f, hgt = 46f;
            float bx = Screen.width * 0.5f - slot * 2.5f, by = Screen.height - 88f;

            var key = new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold };
            key.normal.textColor = Color.white;
            var nm = new GUIStyle(GUI.skin.label) { fontSize = 11 };
            nm.normal.textColor = new Color(1f, 1f, 1f, 0.8f);

            for (int i = 0; i < PlayerSkills.SlotCount; i++)
            {
                Rect r = new Rect(bx + i * slot, by, slot - 6f, hgt);
                bool has = _skills.SlotArmed(i), locked = _skills.SlotLocked(i);
                float cd = _skills.SlotCooldown(i), cdMax = Mathf.Max(0.01f, _skills.SlotCooldownMax(i));

                GUI.color = !has ? new Color(0.12f, 0.12f, 0.15f, 0.7f)
                    : locked ? new Color(0.25f, 0.18f, 0.1f, 0.8f)
                    : cd > 0f ? new Color(0.2f, 0.2f, 0.25f, 0.8f)
                    : new Color(0.5f, 0.4f, 0.15f, 0.85f);              // ready = glow frame
                GUI.DrawTexture(r, Texture2D.whiteTexture);
                if (has && cd > 0f)   // dark veil drains top-down = the cooldown timer
                {
                    GUI.color = new Color(0f, 0f, 0f, 0.68f);
                    GUI.DrawTexture(new Rect(r.x, r.y, r.width, r.height * Mathf.Clamp01(cd / cdMax)), Texture2D.whiteTexture);
                }
                GUI.color = Color.white;

                GUI.Label(new Rect(r.x + 5f, r.y + 1f, 30f, 18f), $"{i + 1}", key);
                string label = !has ? (i == PlayerSkills.SlotCount - 1 ? "RESERVED" : "—")
                    : locked ? $"Lvl {_skills.SlotUnlockLevel(i)}" : _skills.SlotName(i);
                GUI.Label(new Rect(r.x + 3f, r.y + 26f, r.width - 4f, 16f), label, nm);
                if (has && cd > 0.15f)
                    GUI.Label(new Rect(r.x + r.width - 30f, r.y + 2f, 28f, 16f), $"{cd:0.0}", key);
            }
            var mode = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleCenter };
            mode.normal.textColor = new Color(1f, 1f, 1f, 0.55f);
            GUI.Label(new Rect(bx, by - 18f, slot * 5f, 16f),
                _skills.Armed ? "ARMED — weapon skills (G: stow)" : "UNARMED — bare hands (G: ready weapon)", mode);
        }

        // ---- §6: overhead enemy health bars (screen-projected) ----
        private void OverheadBars()
        {
            var cam = Camera.main;
            if (cam == null || BattleHealthRegistry.Active.Count == 0) return;

            _tmpBars.Clear();
            foreach (var h in BattleHealthRegistry.Active)
                if (h != null && h.BarVisible && h.HealthFraction < 1f) _tmpBars.Add(h);   // only damaged — clean reads cleaner
            if (_tmpBars.Count == 0) return;

            var name = new GUIStyle(GUI.skin.label) { fontSize = 11, alignment = TextAnchor.MiddleCenter };
            name.normal.textColor = new Color(1f, 0.85f, 0.85f, 0.9f);

            var playerTf = _combat != null ? _combat.transform : null;
            foreach (var h in _tmpBars)
            {
                Vector3 head = h.HeadPoint;
                if (playerTf != null && (head - playerTf.position).sqrMagnitude > 42f * 42f) continue;
                Vector3 s = cam.WorldToScreenPoint(head);
                if (s.z <= 0f) continue;                                     // behind camera
                float bw = 80f, bh = 6f;
                var r = new Rect(s.x - bw * 0.5f, Screen.height - s.y - 14f, bw, bh);
                GUI.color = new Color(0f, 0f, 0f, 0.6f);
                GUI.DrawTexture(new Rect(r.x - 1f, r.y - 1f, r.width + 2f, r.height + 2f), Texture2D.whiteTexture);
                GUI.color = new Color(0.85f, 0.22f, 0.2f);
                GUI.DrawTexture(new Rect(r.x, r.y, r.width * Mathf.Clamp01(h.HealthFraction), r.height), Texture2D.whiteTexture);
                GUI.color = Color.white;
                GUI.Label(new Rect(r.x - 40f, r.y - 16f, bw + 80f, 14f), h.BarName, name);
            }
        }

        private void Bar(float x, float y, float w, float pct, Color c, string label)
        {
            pct = Mathf.Clamp01(pct);
            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.DrawTexture(new Rect(x, y, w, 16f), Texture2D.whiteTexture);
            GUI.color = c;
            GUI.DrawTexture(new Rect(x + 1f, y + 1f, (w - 2f) * pct, 14f), Texture2D.whiteTexture);
            GUI.color = Color.white;
            var ls = new GUIStyle(GUI.skin.label) { fontSize = 11 };
            ls.normal.textColor = Color.white;
            GUI.Label(new Rect(x + 6f, y - 2f, w, 18f), label, ls);
        }
    }
}
