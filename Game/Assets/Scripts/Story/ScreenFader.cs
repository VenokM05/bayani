// BAYANI — Graybox screen fader: fade to/from black, flash, and hold captions.
// IMGUI overlay (matches CombatHUD graybox approach — no UI package needed yet).
// Used by the prologue: SEQ 03 transit feel, SEQ 04 vision flash, SEQ 05 fall-through-time.

using System.Collections;
using UnityEngine;

namespace Bayani.Story
{
    public class ScreenFader : MonoBehaviour
    {
        public static ScreenFader Instance { get; private set; }

        private float _alpha;
        private string _caption;          // shown full-screen while black
        private string _bottomCaption;    // small caption that doesn't need black (title cards)
        private float _bottomTimer;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public static bool IsBlack => Instance != null && Instance._alpha >= 0.99f;

        // Fade out (optionally with a caption revealed in the dark), then fade back in.
        public static void BlackOut(string caption, float fadeOut = 1f, float hold = 1.5f, float fadeIn = 1f)
        {
            if (Instance != null) Instance.StartCoroutine(Instance.BlackOutRoutine(caption, fadeOut, hold, fadeIn));
        }

        // Quick white flash (SEQ 04 visions).
        public static void Flash(float duration = 0.35f)
        {
            if (Instance != null) Instance.StartCoroutine(Instance.FlashRoutine(duration));
        }

        // Small title card bottom-center that doesn't block view (e.g. "CEBU, 1521", "NO SERVICE").
        public static void TitleCard(string text, float hold = 2.5f)
        {
            if (Instance == null) return;
            Instance._bottomCaption = text;
            Instance._bottomTimer = hold;
        }

        private IEnumerator BlackOutRoutine(string caption, float fadeOut, float hold, float fadeIn)
        {
            _caption = caption;
            yield return FadeTo(1f, fadeOut);
            yield return new WaitForSeconds(hold);
            yield return FadeTo(0f, fadeIn);
            _caption = null;
        }

        private IEnumerator FlashRoutine(float duration)
        {
            yield return FadeTo(1f, duration * 0.3f);
            yield return new WaitForSeconds(duration * 0.25f);
            yield return FadeTo(0f, duration * 0.6f);
        }

        private IEnumerator FadeTo(float target, float time)
        {
            float start = _alpha;
            float t = 0f;
            while (t < time)
            {
                t += Time.deltaTime;
                _alpha = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t / time));
                yield return null;
            }
            _alpha = target;
        }

        private void Update()
        {
            if (_bottomTimer > 0f) _bottomTimer -= Time.deltaTime;
        }

        private void OnGUI()
        {
            if (_alpha > 0.001f)
            {
                GUI.color = new Color(0f, 0f, 0f, _alpha);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
                GUI.color = Color.white;
                if (_alpha > 0.6f && !string.IsNullOrEmpty(_caption))
                {
                    var style = new GUIStyle(GUI.skin.label)
                    { alignment = TextAnchor.MiddleCenter, fontSize = 26, fontStyle = FontStyle.Italic };
                    style.normal.textColor = new Color(1f, 1f, 1f, Mathf.InverseLerp(0.6f, 1f, _alpha));
                    GUI.Label(new Rect(0, Screen.height * 0.45f, Screen.width, 60f), _caption, style);
                }
            }

            if (_bottomTimer > 0f && !string.IsNullOrEmpty(_bottomCaption))
            {
                var style = new GUIStyle(GUI.skin.label)
                { alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold };
                float a = Mathf.Clamp01(_bottomTimer / 0.5f);
                style.normal.textColor = new Color(1f, 0.85f, 0.4f, a);
                GUI.Label(new Rect(0, Screen.height * 0.12f, Screen.width, 32f), _bottomCaption, style);
            }
        }
    }
}
