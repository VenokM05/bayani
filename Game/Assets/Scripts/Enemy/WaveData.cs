// BAYANI — Wave arena data (phasing 1.10): waves of enemy prefab groups, SO-driven
// like everything else (exit gate: no hard-coded combat values).
// Built by Tools → BAYANI → Phase 1.10, tunable in the Inspector.

using System;
using UnityEngine;

namespace Bayani.Enemy
{
    [Serializable]
    public class SpawnEntry
    {
        public GameObject enemyPrefab;
        public int count = 2;
        public float spacing = 0.9f;          // seconds between spawns in this group
    }

    [Serializable]
    public class WaveDef
    {
        public SpawnEntry[] entries;
        public float startDelay = 3f;         // before wave 1 only (inter-wave uses WaveData.interWaveDelay)
    }

    [CreateAssetMenu(menuName = "BAYANI/Wave Data")]
    public class WaveData : ScriptableObject
    {
        public WaveDef[] waves;
        public float interWaveDelay = 4f;     // breathing room after a clear
        public float clearHold = 1.5f;        // pause between "all dead" and next wave
    }
}
