// BAYANI — Phase 0 minimal third-person follow camera (graybox prototype).
// Orbits behind the player using the mouse pitch/yaw the PlayerController produces.
// SETUP: attach to the Main Camera; drag the "Kai" capsule into `target`.
// Cinemachine-based camera with collision resolution replaces this in Phase 1 (phasing.md 1.2).

using UnityEngine;

namespace Bayani.Core
{
    public class ThirdPersonFollowCamera : MonoBehaviour
    {
        [SerializeField] public Transform target;   // public: wired directly by Phase0SceneBuilder
        [SerializeField] private float distance = 4f;
        [SerializeField] private float heightOffset = 1.4f;
        [SerializeField] private float smoothTime = 0.12f;

        private Vector3 _velocity;

        // --- Impulse shake (phasing 1B): heavy hits, parries, player damage ---
        private float _shakeAmp;
        private float _shakeUntil;

        /// <summary>Adds a shake impulse; overlapping impulses take the stronger amplitude.</summary>
        public void AddShake(float amplitude, float duration)
        {
            _shakeAmp = Mathf.Max(_shakeAmp, amplitude);
            _shakeUntil = Mathf.Max(_shakeUntil, Time.time + duration);
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // Derive yaw/pitch from the target's current rotation so both scripts stay in sync.
            float yaw = target.eulerAngles.y;
            float pitch = -Mathf.Clamp(target.eulerAngles.x * 0.5f, -15f, 35f);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 anchor = target.position + Vector3.up * heightOffset;
            Vector3 desired = anchor - rotation * Vector3.forward * distance;

            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);
            transform.LookAt(anchor, Vector3.up);

            // Shake: decaying random offset applied after positioning
            if (Time.time < _shakeUntil)
            {
                float decay = (_shakeUntil - Time.time) / Mathf.Max(0.01f, _shakeUntil - Time.time + 0.15f);
                Vector3 off = Random.insideUnitSphere * _shakeAmp * Mathf.Clamp01(decay);
                off.y *= 0.5f;                                   // less vertical nausea
                transform.position += off;
            }
            else
            {
                _shakeAmp = 0f;
            }
        }
    }
}
