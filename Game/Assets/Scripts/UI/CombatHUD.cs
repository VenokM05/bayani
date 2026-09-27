// BAYANI — Graybox HUD (phasing.md 1.8): HP/Stamina/Diwa bars + combat state.
// IMGUI on purpose: zero scene setup, disposable when the real UI Prefabs arrive.

using Bayani.Combat;
using Bayani.Core;
using UnityEngine;

namespace Bayani.UI
{
    public class CombatHUD : MonoBehaviour
    {
        private CombatResources _res;
        private PlayerCombat _combat;
        private string _flash = "";
        private float _flashUntil;
        private float _fps;                          // smoothed fps — the Phase 1 gate box reads this

        private void Update()
        {
            _fps = Mathf.Lerp(_fps, 1f / Time.unscaledDeltaTime, Time.unscaledDeltaTime * 4f);
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
            if (_res == null) return;
            float w = 260f, x = 20f;

            Bar(x, 20f, w, _res.HP / _res.MaxHP, new Color(0.85f, 0.25f, 0.25f), "HP");
            Bar(x, 46f, w, _res.Stamina / _res.MaxStamina, new Color(0.3f, 0.8f, 0.4f), "STAMINA");
            Bar(x, 72f, w, _res.Diwa / _res.MaxDiwa, new Color(0.35f, 0.55f, 0.95f), "DIWA");

            // Memory Stability row — only when a zone exists (ggd §56.3; HUD starts at 80%)
            var zone = MemoryStabilityZone.Current;
            float stateY = 96f;
            if (zone != null)
            {
                Bar(x, 98f, w, zone.Value / 100f, new Color(0.62f, 0.42f, 0.85f), $"MEMORY {zone.Value:0}% · {zone.zoneName}");
                stateY = 124f;
            }

            GUI.Label(new Rect(x, stateY, 400f, 22f), _combat != null ? _combat.StateText : "");
            if (Time.unscaledTime < _flashUntil)
                GUI.Label(new Rect(x + w + 14f, 72f, 200f, 24f), $"<b>{_flash}</b>");

            GUI.Label(new Rect(20f, Screen.height - 30f, 900f, 24f),
                "ATTACK LMB/LT · HOLD BLOCK RMB/X (perfect-timed = PARRY +12) · DODGE LCtrl/RT · SKILL Q/Y · MOVE WASD/stick");

            // FPS, top-right: green ≥55 · yellow ≥40 · red below (60 FPS gate check)
            var fs = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperRight, fontSize = 16, fontStyle = FontStyle.Bold };
            fs.normal.textColor = _fps >= 55f ? new Color(0.4f, 0.9f, 0.45f) : _fps >= 40f ? Color.yellow : Color.red;
            GUI.Label(new Rect(Screen.width - 160f, 14f, 145f, 22f), $"{_fps:F0} FPS", fs);
        }

        private void Bar(float x, float y, float w, float pct, Color c, string label)
        {
            pct = Mathf.Clamp01(pct);
            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.DrawTexture(new Rect(x, y, w, 16f), Texture2D.whiteTexture);
            GUI.color = c;
            GUI.DrawTexture(new Rect(x + 1f, y + 1f, (w - 2f) * pct, 14f), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(x + 6f, y - 2f, w, 18f), label);
        }
    }
}
