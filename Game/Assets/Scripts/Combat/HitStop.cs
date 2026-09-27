// BAYANI — Hit-stop (phasing.md 1.6): the single cheapest "this hit feels real" trick.
// Freeze the world for ~60ms on a landed hit. Uses unscaled time so it never stalls.

using System.Collections;
using UnityEngine;

namespace Bayani.Combat
{
    public static class HitStop
    {
        public const float DefaultDuration = 0.06f;
        public const float TimeScaleDuring = 0.02f;

        public static void Play(MonoBehaviour host, float duration = DefaultDuration)
        {
            if (host == null) return;
            host.StartCoroutine(StopRoutine(duration));
        }

        private static IEnumerator StopRoutine(float duration)
        {
            Time.timeScale = TimeScaleDuring;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1f;
        }
    }
}
