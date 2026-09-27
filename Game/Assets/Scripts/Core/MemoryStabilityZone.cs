// BAYANI — Memory Stability zone score v1 (phasing 2.4, ggd §56.3).
// A per-zone 0-100 score of how intact the reconstructed memory is.
// NEVER causes game-over (spec). This graybox implements §56.3's numbers:
//   start 100-20 = 80 · −1/min per living Limot (v1 simplification of "corrupted nest")
//   +5 per Limot kill (data.diwaOnKill sibling: stabilityOnKill) · quests/scans/shrines come in 2.2/3.x.

using System;
using UnityEngine;

namespace Bayani.Core
{
    public class MemoryStabilityZone : MonoBehaviour
    {
        public static MemoryStabilityZone Current { get; private set; }

        [Header("ggd §56.3")]
        public string zoneName = "Zone";
        [Range(0f, 100f)] public float startValue = 80f;      // "100% − 20% in every zone"
        public float drainPerLivingLimotPerMin = 1f;

        public float Value { get; private set; }
        public event Action<float> OnChanged;

        private int _nestCount;                                // living Limot registered by LimotEnemy itself

        private void OnEnable()
        {
            Current = this;
            Value = startValue;
            OnChanged?.Invoke(Value);
        }

        private void OnDisable()
        {
            if (Current == this) Current = null;
        }

        public void NestArrived() => _nestCount++;
        public void NestLeft() => _nestCount = Mathf.Max(0, _nestCount - 1);

        public void AddStability(float delta, string reason = "")
        {
            var v = Value + delta;
            Value = Mathf.Clamp(v, 0f, 100f);
            OnChanged?.Invoke(Value);
            if (!string.IsNullOrEmpty(reason))
                Debug.Log($"[BAYANI] Stability {delta:+#;-#;0} ({reason}) → {Value:0}% in {zoneName}");
        }

        private void UpdateDrain()
        {
            if (_nestCount <= 0) return;
            AddStability(-_nestCount * drainPerLivingLimotPerMin / 60f * Time.deltaTime);
        }

        private void LateUpdate() => UpdateDrain();
    }
}
