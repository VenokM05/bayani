// BAYANI — Artifact scan interaction (phasing 2.2): approach → [E/F] → TALA scan panel
// (name, historical_status badge, read-out) → grants Diwa / Stability / XP from the
// ArtifactData SO. One scan per artifact, ever. Graybox IMGUI like the dialogue box;
// the real scan VFX + codex write lands with the 2.3 codex UI.

using Bayani.Combat;
using Bayani.Player;
using UnityEngine;

namespace Bayani.Story
{
    public class ArtifactScanner : MonoBehaviour
    {
        public ArtifactData data;
        public string prompt = "[E/F] Scan artifact";

        [Tooltip("Item granted to the inventory on scan (PRD §9 — null = none).")]
        public Bayani.Player.ItemData grantItem;

        private bool _scanned;
        private bool _panelOpen;
        private bool _inside;
        private float _panelCloseAt;         // ignore the open-key leaking into close
        private PlayerController _player;

        private void Update()
        {
            if (_panelOpen)
            {
                if (Time.time >= _panelCloseAt && Bayani.Core.BayaniInput.InteractPressed)
                    ClosePanel();
                return;
            }
            if (_scanned || data == null || !_inside) return;

            DialoguePlayer.Prompt = prompt;
            if (Bayani.Core.BayaniInput.InteractPressed && !DialoguePlayer.IsActive)
                Scan();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player")) _inside = true;
        }
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _inside = false;
            if (DialoguePlayer.Prompt == prompt) DialoguePlayer.Prompt = null;
        }

        private void Scan()
        {
            _scanned = true;
            DialoguePlayer.Prompt = null;

            // Rewards — all values from the SO (data-driven gate rule)
            var res = FindFirstObjectByType<CombatResources>();
            if (res != null && data.diwaReward > 0f) res.AddDiwa(data.diwaReward);
            Core.MemoryStabilityZone.Current?.AddStability(data.stabilityReward, $"scan: {data.displayName}");
            // XP ledger now exists (PlayerProgression) — artifact award = §56.4's 50
            if (data.xpReward > 0f)
                FindFirstObjectByType<Bayani.Combat.PlayerProgression>()?.AwardXp(data.xpReward, data.displayName);
            // inventory + quest hooks (PRD §9)
            if (grantItem != null) Bayani.Player.Inventory.Add(grantItem);
            Bayani.Story.QuestGuide.QuestJournal.OnArtifactScanned(data.codexCategory);
            Debug.Log($"[BAYANI] ARTIFACT SCAN '{data.displayName}' [{data.historicalStatus}] → +{data.diwaReward:0} Diwa, +{data.stabilityReward:0} stability, +{data.xpReward:0} XP — codex {data.codexCategory} entry queued");

            _player = FindFirstObjectByType<PlayerController>();
            if (_player != null) _player.ControlsEnabled = false;
            _panelOpen = true;
            _panelCloseAt = Time.time + 0.25f;
        }

        private void ClosePanel()
        {
            _panelOpen = false;
            if (_player != null) _player.ControlsEnabled = true;
        }

        private static Color BadgeColor(string status) => status switch
        {
            "historical"    => new Color(0.35f, 0.75f, 0.4f),
            "archaeological"=> new Color(0.4f, 0.68f, 0.9f),
            "ethnographic"  => new Color(0.9f, 0.7f, 0.3f),
            "folklore"      => new Color(0.75f, 0.45f, 0.85f),
            _               => new Color(0.6f, 0.6f, 0.6f),   // fictional
        };

        private void OnGUI()
        {
            if (!_panelOpen) return;

            float w = Mathf.Min(Screen.width * 0.55f, 640f);
            float h = 240f;
            float x = (Screen.width - w) * 0.5f, y = (Screen.height - h) * 0.5f;

            GUI.color = new Color(0.03f, 0.05f, 0.08f, 0.94f);
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
            GUI.color = new Color(0.35f, 0.75f, 0.95f, 0.9f);              // TALA cyan frame
            GUI.DrawTexture(new Rect(x, y, w, 2f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(x, y + h - 2f, w, 2f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(x, y, 2f, h), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(x + w - 2f, y, 2f, h), Texture2D.whiteTexture);
            GUI.color = Color.white;

            var head = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold };
            head.normal.textColor = new Color(0.35f, 0.75f, 0.95f);
            GUI.Label(new Rect(x + 16, y + 10, w - 32, 18), "TALA // ARTIFACT SCAN", head);

            var name = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold };
            GUI.Label(new Rect(x + 16, y + 32, w - 32, 26), data.displayName, name);

            // historical_status badge (§51 — the trust grammar of the codex)
            var badge = new GUIStyle(GUI.skin.box) { fontSize = 12, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            var bc = BadgeColor(data.historicalStatus);
            badge.normal.textColor = Color.black;
            var prev = GUI.backgroundColor;
            GUI.backgroundColor = bc;
            GUI.Box(new Rect(x + 16, y + 62, data.historicalStatus.Length * 8f + 26f, 20f), data.historicalStatus.ToUpper());
            GUI.backgroundColor = prev;

            var desc = new GUIStyle(GUI.skin.label) { fontSize = 15, wordWrap = true };
            desc.normal.textColor = new Color(0.9f, 0.9f, 0.94f);
            GUI.Label(new Rect(x + 16, y + 92, w - 32, h - 130), data.description, desc);

            var rw = new GUIStyle(GUI.skin.label) { fontSize = 13 };
            rw.normal.textColor = new Color(0.92f, 0.72f, 0.25f);
            GUI.Label(new Rect(x + 16, y + h - 30, w - 32, 18),
                $"+{data.diwaReward:0} DIWA   ·   +{data.stabilityReward:0} MEMORY   ·   +{data.xpReward:0} XP", rw);

            var hint = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperRight, fontSize = 12 };
            hint.normal.textColor = new Color(1f, 1f, 1f, 0.5f);
            GUI.Label(new Rect(x, y - 22f, w - 4, 16), "[E / A] close", hint);
        }
    }
}
