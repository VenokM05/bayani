// BAYANI — Procedural walk-bob for the Kai model while the real animation rig
// (Mixamo auto-rig pass) is pending. Fakes life: step bounce, subtle sway,
// slight forward lean at speed, settles to idle breathing when standing.
// Attach to the "Visual" child; reads speed from PlayerController.

using UnityEngine;

namespace Bayani.Player
{
    public class KaiWalkBob : MonoBehaviour
    {
        [Header("Tuning (graybox — Mixamo anims replace this)")]
        public float bobHeight = 0.05f;       // vertical bounce amplitude
        public float bobFrequency = 9f;       // radians/s at full speed
        public float swayAngle = 3.5f;        // sideways roll degrees
        public float leanAngle = 6f;          // forward pitch at run speed
        public float idleBreathSpeed = 1.6f;
        public float smoothing = 10f;

        private PlayerController _pc;
        private Vector3 _basePos;
        private Quaternion _baseRot;
        private float _phase;
        private float _energy;                // 0..1 blend idle↔walk

        private void Awake()
        {
            _pc = GetComponentInParent<PlayerController>();
            _basePos = transform.localPosition;
            _baseRot = transform.localRotation;
        }

        private void Update()
        {
            float speed = _pc != null ? _pc.PlanarVelocity.magnitude : 0f;
            float targetEnergy = Mathf.Clamp01(speed / 4.5f);
            _energy = Mathf.MoveTowards(_energy, targetEnergy, smoothing * Time.deltaTime);
            _phase += Time.deltaTime * Mathf.Lerp(idleBreathSpeed, bobFrequency, _energy);

            float bob = Mathf.Sin(_phase) * bobHeight * _energy
                      + Mathf.Sin(Time.time * idleBreathSpeed) * 0.008f * (1f - _energy);   // idle breath
            float sway = Mathf.Cos(_phase * 0.5f) * swayAngle * _energy;
            float lean = -leanAngle * _energy * Mathf.Clamp01(speed / 5f);

            transform.localPosition = _basePos + Vector3.up * bob;
            transform.localRotation = _baseRot
                * Quaternion.Euler(lean, 0f, sway);
        }
    }
}
