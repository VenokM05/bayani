// BAYANI — Memory Stability visual gradient (ggd §56.3): lower stability = heavier
// desaturation + darkening. Graybox implementation via a runtime URP Volume
// (ColorAdjustments); the real gradient later adds fog/glitch per location files.

using Bayani.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Bayani.Story
{
    [RequireComponent(typeof(Volume))]
    public class StabilityLook : MonoBehaviour
    {
        [Tooltip("Saturation at 0% stability (1 = fully saturated like today's graybox).")]
        [Range(0f, 1f)] public float minSaturation = 0.15f;
        [Tooltip("Exposure drop at 0% stability, in stops.")]
        public float minExposure = -0.8f;
        public float smoothing = 3f;

        private ColorAdjustments _adj;
        private MemoryStabilityZone _zone;
        private float _t = 1f;

        private void Awake()
        {
            var volume = GetComponent<Volume>();
            volume.isGlobal = true;
            volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
            _adj = volume.profile.Add<ColorAdjustments>(false);
        }

        private void OnEnable()
        {
            _zone = MemoryStabilityZone.Current;
            if (_zone != null) _zone.OnChanged += OnZoneChanged;
        }

        private void OnDisable()
        {
            if (_zone != null) _zone.OnChanged -= OnZoneChanged;
        }

        private void OnZoneChanged(float value) => _t = Mathf.Clamp01(value / 100f);

        private void Update()
        {
            if (_zone == null) { _zone = MemoryStabilityZone.Current; if (_zone != null) _zone.OnChanged += OnZoneChanged; }
            if (_zone == null || _adj == null) return;

            float target = Mathf.Clamp01(_zone.Value / 100f);
            _t = Mathf.MoveTowards(_t, target, smoothing * Time.deltaTime);
            _adj.saturation.value = Mathf.Lerp(minSaturation, 1f, _t);
            _adj.postExposure.value = Mathf.Lerp(minExposure, 0f, _t);
        }
    }
}
