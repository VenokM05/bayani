// BAYANI — Graybox dialogue runner (IMGUI, like CombatHUD). Plays a DialogueAsset,
// advances with E / Enter / gamepad A, locks player controls while active,
// and draws interact prompts for StoryTriggers. Phase 2+ can swap the render for
// real UI without touching StoryTrigger or the data assets.

using System;
using Bayani.Core;
using Bayani.Player;
using UnityEngine;

namespace Bayani.Story
{
    public class DialoguePlayer : MonoBehaviour
    {
        public static DialoguePlayer Instance { get; private set; }

        // Prompt plumbing for StoryTriggers (nearest trigger wins).
        public static string Prompt;

        private DialogueAsset _asset;
        private int _index;
        private Action _onFinished;
        private PlayerController _player;
        private int _advanceLockedUntil;    // ignore E on the same frame that opened the dialogue

        public static bool IsActive => Instance != null && Instance._asset != null;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Play(DialogueAsset asset, Action onFinished = null)
        {
            if (asset == null || asset.lines == null || asset.lines.Length == 0)
            {
                onFinished?.Invoke();
                return;
            }
            _asset = asset;
            _index = 0;
            _onFinished = onFinished;
            _advanceLockedUntil = Time.frameCount + 1;
            _player = FindFirstObjectByType<PlayerController>();
            if (_player != null) _player.ControlsEnabled = false;
        }

        private void Update()
        {
            if (_asset == null) return;
            if (Time.frameCount <= _advanceLockedUntil) return;   // swallow the opening press

            if (BayaniInput.InteractPressed)
            {
                _index++;
                if (_index >= _asset.lines.Length) Finish();
            }
        }

        private void Finish()
        {
            var done = _onFinished;
            _asset = null;
            _onFinished = null;
            if (_player != null) _player.ControlsEnabled = true;
            done?.Invoke();
        }

        private void OnGUI()
        {
            if (_asset != null)
            {
                var line = _asset.lines[_index];
                bool narration = string.IsNullOrEmpty(line.speaker) || line.speaker == "NARRATION";

                float w = Mathf.Min(Screen.width * 0.7f, 900f);
                float x = (Screen.width - w) * 0.5f;
                float h = narration ? 74f : 96f;
                float y = Screen.height - h - 48f;

                GUI.color = new Color(0.04f, 0.04f, 0.06f, 0.88f);
                GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
                GUI.color = new Color(0.92f, 0.72f, 0.25f, 0.9f);          // gold rule on top
                GUI.DrawTexture(new Rect(x, y, w, 2f), Texture2D.whiteTexture);
                GUI.color = Color.white;

                if (narration)
                {
                    var ns = new GUIStyle(GUI.skin.label)
                    { alignment = TextAnchor.MiddleCenter, fontSize = 19, fontStyle = FontStyle.Italic, wordWrap = true };
                    ns.normal.textColor = new Color(0.85f, 0.85f, 0.9f);
                    GUI.Label(new Rect(x + 20, y, w - 40, h), line.text, ns);
                }
                else
                {
                    var ss = new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold };
                    ss.normal.textColor = new Color(0.92f, 0.72f, 0.25f);
                    GUI.Label(new Rect(x + 20, y + 10, w - 40, 20), line.speaker.ToUpper(), ss);

                    var ts = new GUIStyle(GUI.skin.label) { fontSize = 18, wordWrap = true };
                    ts.normal.textColor = Color.white;
                    GUI.Label(new Rect(x + 20, y + 32, w - 40, h - 40), line.text, ts);
                }

                var hs = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperRight, fontSize = 12 };
                hs.normal.textColor = new Color(1f, 1f, 1f, 0.5f);
                GUI.Label(new Rect(x, y + h - 22, w - 12, 16), $"[E / A]  {_index + 1}/{_asset.lines.Length}", hs);
            }
            else if (!string.IsNullOrEmpty(Prompt))
            {
                var ps = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 18, fontStyle = FontStyle.Bold };
                ps.normal.textColor = new Color(1f, 0.9f, 0.6f);
                GUI.Label(new Rect(0, Screen.height * 0.62f, Screen.width, 28), Prompt, ps);
            }
        }
    }
}
