// BAYANI — Death & respawn (docs/prd-progression.md §2).
// HP hits 0 → lock controls, "YOU DIED" overlay ~2.5s (real time) → respawn at the
// last visited Checkpoint: same scene = teleport, different scene = LoadScene then
// place. Full HP + stamina restored; XP, level, skills, checkpoint all live in
// ProgressStore (static) so they survive the reload untouched. Diwa (earned meter)
// is also kept — the rhythm restarts, the story doesn't regress. 2s mercy
// invulnerability on arrival. No XP loss: BAYANI punishes with time, not progress.

using Bayani.Combat;
using Bayani.Story;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bayani.Player
{
    public class DeathManager : MonoBehaviour
    {
        public float deathHold = 2.5f;              // "You Died" screen time (real seconds)
        public float mercyInvuln = 2f;              // seconds of safety after respawn

        private CombatResources _res;
        private PlayerCombat _combat;
        private PlayerSkills _skills;
        private PlayerController _controller;
        private Hurtbox _hurtbox;
        private float _diedAt = -1f;
        private bool _respawning;

        private void Start()
        {
            _res = FindFirstObjectByType<CombatResources>();
            var kai = GameObject.FindGameObjectWithTag("Player");
            if (kai != null)
            {
                _combat = kai.GetComponent<PlayerCombat>();
                _skills = kai.GetComponent<PlayerSkills>();
                _controller = kai.GetComponent<PlayerController>();
                _hurtbox = kai.GetComponent<Hurtbox>();
            }
            if (_res != null) _res.OnDeath += OnDeath;

            // respawn that crossed a scene boundary: place Kai at the checkpoint
            if (ProgressStore.PendingPlacement)
            {
                ProgressStore.PendingPlacement = false;
                PlaceAtCheckpoint();
                _res?.Refill();
            }
        }

        private void OnDestroy()
        {
            if (_res != null) _res.OnDeath -= OnDeath;
        }

        private void OnDeath()
        {
            if (_diedAt >= 0f || _respawning) return;
            _diedAt = Time.unscaledTime;
            ProgressStore.Toast = null;
            Debug.Log("[BAYANI] Kai is down — respawn at checkpoint (progress kept).");
            if (_controller != null) _controller.ControlsEnabled = false;
            if (_combat != null) _combat.enabled = false;
            if (_skills != null) _skills.enabled = false;
        }

        private void Update()
        {
            if (_diedAt < 0f) return;
            if (Time.unscaledTime - _diedAt < deathHold) return;

            _diedAt = -1f;
            _respawning = true;
            var kai = GameObject.FindGameObjectWithTag("Player");

            // same scene → teleport; else load the checkpoint's scene (ProgressStore rides through)
            if (kai != null && !string.IsNullOrEmpty(ProgressStore.CheckpointScene)
                && ProgressStore.CheckpointScene != gameObject.scene.name)
            {
                if (SceneInBuild(ProgressStore.CheckpointScene))
                {
                    ProgressStore.PendingPlacement = true;      // new scene's DeathManager.Start places Kai
                    SceneManager.LoadScene(ProgressStore.CheckpointScene);
                    return;
                }
                // graybox workflow: later scenes may not be in Build Settings yet —
                // revive in place instead of teleporting to another scene's coordinates
                ProgressStore.CheckpointScene = "";
            }

            PlaceAtCheckpoint();
            FinishRespawn();
        }

        private static bool SceneInBuild(string name)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                if (System.IO.Path.GetFileNameWithoutExtension(
                        SceneUtility.GetScenePathByBuildIndex(i)) == name)
                    return true;
            return false;
        }

        private void PlaceAtCheckpoint()
        {
            var kai = GameObject.FindGameObjectWithTag("Player");
            if (kai == null || string.IsNullOrEmpty(ProgressStore.CheckpointScene)) return;
            if (ProgressStore.CheckpointScene != gameObject.scene.name) return;   // cross-scene handled in Start
            kai.transform.SetPositionAndRotation(ProgressStore.CheckpointPosition, ProgressStore.CheckpointRotation);
        }

        private void FinishRespawn()
        {
            _respawning = false;
            _res?.Refill();                                   // full HP + stamina (§2); Diwa kept
            if (_combat != null) _combat.enabled = true;
            if (_skills != null) _skills.enabled = true;
            if (_controller != null) _controller.ControlsEnabled = true;
            if (_hurtbox != null && mercyInvuln > 0f)
                StartCoroutine(Mercy(_hurtbox));
            ProgressStore.Toast = "RESPAWNED — XP and skills intact";
            ScreenFader.Flash();
        }

        private System.Collections.IEnumerator Mercy(Hurtbox hb)
        {
            hb.SetInvulnerable(true);
            yield return new WaitForSeconds(mercyInvuln);
            if (hb != null) hb.SetInvulnerable(false);
        }

        private void OnGUI()
        {
            if (_diedAt < 0f) return;
            GUI.color = new Color(0f, 0f, 0f, Mathf.Clamp01((Time.unscaledTime - _diedAt) / 0.6f) * 0.72f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            var style = new GUIStyle(GUI.skin.label)
            { fontSize = 64, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            style.normal.textColor = new Color(0.75f, 0.15f, 0.12f);
            GUI.Label(new Rect(0, Screen.height * 0.36f, Screen.width, 90f), "YOU DIED", style);

            var sub = new GUIStyle(GUI.skin.label) { fontSize = 18, alignment = TextAnchor.MiddleCenter };
            sub.normal.textColor = new Color(1f, 1f, 1f, 0.75f);
            GUI.Label(new Rect(0, Screen.height * 0.36f + 92f, Screen.width, 26f),
                "returning to the last checkpoint… memory, skills and XP are kept", sub);
        }
    }
}
