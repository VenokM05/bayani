// BAYANI — Phase 0 minimal third-person follow camera (graybox prototype).
// Orbits behind the player using the mouse pitch/yaw the PlayerController produces.
// SETUP: attach to the Main Camera; drag the "Kai" capsule into `target`.
// Cinemachine-based camera with collision resolution replaces this in Phase 1 (phasing.md 1.2).

using UnityEngine;

namespace Bayani.Core
{
    public class ThirdPersonFollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float distance = 4f;
        [SerializeField] private float heightOffset = 1.4f;
        [SerializeField] private float smoothTime = 0.12f;

        private Vector3 _velocity;

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
        }
    }
}
