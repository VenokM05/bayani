// BAYANI — Graybox HUD (phasing.md 1.8): HP/Stamina/Diwa bars + combat state.
// IMGUI on purpose: zero scene setup, disposable when the real UI Prefabs arrive.

using Bayani.Combat;
using UnityEngine;

namespace Bayani.UI
{
    public class CombatHUD : MonoBehaviour
    {
        private CombatResources _res;
        private PlayerCombat _combat;
        private string _flash = "";
        private float _flashUntil;

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

            GUI.Label(new Rect(x, 96f, 400f, 22f), _combat != null ? _combat.StateText : "");
            if (Time.unscaledTime < _flashUntil)
                GUI.Label(new Rect(x + w + 14f, 72f, 200f, 24f), $"<b>{_flash}</b>");

            GUI.Label(new Rect(20f, Screen.height - 30f, 700f, 24f),
                "LMB combo (L-L-L-H)  |  RMB dodge (i-frames)  |  Q Diwa burst  |  WASD/Shift/Space move");
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
