// BAYANI — Wave arena spawner (phasing 1.10): runs a WaveData SO — spawn groups in a
// ring around the player's arena, next wave starts only when the previous is fully dead.
// Also the exit-gate stress rig: the built wave table tops out at 10 alive enemies
// so "60 FPS with 10 enemies" is directly verifiable.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bayani.Combat;
using UnityEngine;

namespace Bayani.Enemy
{
    public class WaveSpawner : MonoBehaviour
    {
        public WaveData data;
        public Transform[] spawnPoints;       // cycled; ring around the arena
        [Tooltip("Optional center for face-toward spawn rotation; defaults to player position.")]
        public Transform arenaCenter;

        private readonly List<LimotEnemy> _alive = new List<LimotEnemy>();
        private int _spawnCursor;

        public int AliveCount => _alive.Count(e => e != null && !e.IsDead);

        private void Start()
        {
            if (data == null || data.waves == null || data.waves.Length == 0) return;
            if (FindFirstObjectByType<PlayerCombat>() == null)
                Debug.LogWarning("[BAYANI] WaveSpawner: no PlayerCombat found — wire Kai up first (Phase 1 builder).", this);
            StartCoroutine(RunWaves());
        }

        private IEnumerator RunWaves()
        {
            for (int w = 0; w < data.waves.Length; w++)
            {
                var wave = data.waves[w];
                yield return new WaitForSeconds(w == 0 ? wave.startDelay : data.interWaveDelay);
                Bayani.Story.ScreenFader.TitleCard($"WAVE {w + 1} / {data.waves.Length}", 2.5f);

                if (wave.entries != null)
                    foreach (var entry in wave.entries)
                    {
                        if (entry == null || entry.enemyPrefab == null) continue;
                        for (int i = 0; i < entry.count; i++)
                        {
                            Spawn(entry.enemyPrefab);
                            yield return new WaitForSeconds(entry.spacing);
                        }
                    }

                // Clear condition: everyone down. Prune dead refs so the list can't leak.
                yield return new WaitWhile(() => AliveCount > 0);
                _alive.RemoveAll(e => e == null || e.IsDead);
                yield return new WaitForSeconds(data.clearHold);
            }
            Bayani.Story.ScreenFader.TitleCard("ARENA CLEARED — check the FPS counter!", 4f);
        }

        private void Spawn(GameObject prefab)
        {
            if (spawnPoints == null || spawnPoints.Length == 0) return;
            var point = spawnPoints[_spawnCursor % spawnPoints.Length];
            _spawnCursor++;

            var center = arenaCenter != null ? arenaCenter.position
                       : (FindFirstObjectByType<PlayerCombat>()?.transform.position ?? point.position);
            var rot = Quaternion.LookRotation(center - point.position).eulerAngles;
            rot.x = rot.z = 0f;

            var go = Instantiate(prefab, point.position + Vector3.up * 0.1f, Quaternion.Euler(rot));
            var enemy = go.GetComponent<LimotEnemy>();
            if (enemy != null) _alive.Add(enemy);
        }
    }
}
