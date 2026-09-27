// BAYANI — Respawn checkpoint (docs/prd-progression.md §2). Trigger volume: the
// player's last-visited position+rotation is stored in ProgressStore (static →
// survives death AND scene load). The installer places one at each scene spawn
// and at story-safe anchors (shrine, fire circle). Re-touching just moves it.

using Bayani.Combat;
using UnityEngine;

namespace Bayani.Story
{
    public class Checkpoint : MonoBehaviour
    {
        [Tooltip("Facing baked into the respawn (yaw only).")]
        public Vector3 respawnEuler = Vector3.zero;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            ProgressStore.SetCheckpoint(gameObject.scene.name,
                transform.position, Quaternion.Euler(0f, respawnEuler.y, 0f));
            Debug.Log($"[BAYANI] Checkpoint: {name} ({gameObject.scene.name})");
        }
    }
}
