// BAYANI — SEQ 07B "The First Shadow" (storyline.md: playable combat tutorial with a
// scripted elder rescue — "no death possible, the lesson is pressure without punishment").
// Trigger zone flow: title card + flash → intro dialogue → spawn one Anino while the
// player's hurtbox is INVULNERABLE → after rescueAfter seconds (or early if it dies —
// it can't hurt you either way, but the beat assumes you learn) the elder finishes it
// → naming dialogue ("Limot"). Data-driven: lines in Assets/Data/Story/, enemy = prefab.

using System.Collections;
using Bayani.Combat;
using Bayani.Enemy;
using UnityEngine;

namespace Bayani.Story
{
    public class TutorialAmbush : MonoBehaviour
    {
        [Header("Dialogue (SO assets, storyline-verbatim)")]
        public DialogueAsset intro;
        public DialogueAsset rescue;

        [Header("Staging")]
        public GameObject enemyPrefab;          // Anino.prefab (Phase 1.10)
        public Transform enemySpawnPoint;
        public Transform elderPos;              // the elder's finishing blow comes from here
        public float rescueAfter = 15f;
        public string titleCard = "SEQ 07B — THE FIRST SHADOW";

        private bool _started;

        private void OnTriggerEnter(Collider other)
        {
            if (_started || !other.CompareTag("Player")) return;
            _started = true;
            StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            // Let whatever the player just walked out of (SEQ 07 dialogue etc.) finish.
            while (DialoguePlayer.IsActive || ScreenFader.IsBlack) yield return null;

            if (!string.IsNullOrEmpty(titleCard)) ScreenFader.TitleCard(titleCard, 3f);
            ScreenFader.Flash();                       // the firelight flickers wrong

            bool done = intro == null;
            if (intro != null) DialoguePlayer.Instance.Play(intro, () => done = true);
            yield return new WaitUntil(() => done);

            var pc = FindFirstObjectByType<PlayerCombat>();
            if (pc == null) yield break;

            if (enemyPrefab != null)
            {
                Vector3 pos = enemySpawnPoint != null ? enemySpawnPoint.position
                                                      : transform.position + new Vector3(3f, 1.2f, 3f);
                var go = Instantiate(enemyPrefab, pos, Quaternion.LookRotation(pos - pc.transform.position, Vector3.up));
                var lim = go.GetComponent<LimotEnemy>();

                pc.hurtbox?.SetInvulnerable(true);     // no death possible — beat spec
                float until = Time.time + rescueAfter;
                yield return new WaitUntil(() => (lim != null && lim.IsDead) || Time.time >= until);

                if (lim != null && !lim.IsDead)        // the scripted rescue
                {
                    Vector3 from = elderPos != null ? elderPos.position : pos + Vector3.left * 2f;
                    lim.ApplyDamage(9999f, from, 5f);
                }
                yield return new WaitForSeconds(1.2f); // let the death smoke read
            }

            pc.hurtbox?.SetInvulnerable(false);

            if (rescue != null) DialoguePlayer.Instance.Play(rescue);
        }
    }
}
