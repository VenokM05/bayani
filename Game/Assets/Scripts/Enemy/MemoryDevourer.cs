// BAYANI — SEQ 11 Memory Devourer (content/enemies/memory-devourer.md).
// Chapter-1 finale, fought in the SEAMS of the Battle of Mactan.
// HP model: NOT a bar — three rift cycles, each sealed with a Diwa burst (blades
// pass through; only meaning bites). Then The Reaching: the last burst at the
// rift heart. It is EXPULLED, never killed (Beat Rule 4) — it flees deeper into
// the bloodline (Chapter 2's hook).
// Its presence is the chapter's macro drain: -12 on arrival at the battle shore,
// -2/min while any seam stays open (ggd §56.3). The no-fight rule: delaying is
// allowed, skipping is not — the drain does the teaching.
// Graybox note: the spec's Unnaming/Seam-spit attack table is represented by
// maw adds + the stability drain; bespoke attack animations are Phase 3 polish.

using System.Collections;
using System.Collections.Generic;
using Bayani.Combat;
using Bayani.Core;
using Bayani.Story;
using UnityEngine;

namespace Bayani.Enemy
{
    public class MemoryDevourer : MonoBehaviour, IDamageable
    {
        [Header("Wired by Phase 2 battle builder")]
        public PlayerCombat player;
        public GameObject aninoPrefab;              // the seam maws
        public Transform[] riftPoints;              // 3 seam pockets
        public DialogueAsset reachingDialogue;      // "Not yet!" — plays as The Reaching begins
        public DialogueAsset victoryDialogue;

        [Header("Tuning (data-driven per standing rule)")]
        public float diwaSealCost = 25f;            // "needs >= 25 Diwa — the fight grants it across cycles"
        public int addsPerRift = 3;
        public float engageRadius = 3.5f;
        public float arrivalDrain = 12f;            // -12 on first sight of the battle
        public float seamDrainPerHalfMin = 2f;      // -2/min while seams open
        public float xpForExpulsion = 1200f;        // §56.4 boss ceiling — final boss of the chapter

        private enum State { Watching, RiftOpen, Reaching, Expelled }
        private State _state = State.Watching;
        private int _sealed;
        private int _activeRift = -1;
        private bool _arrived;
        private float _nextDrain;
        private float _lastThrough;
        private readonly List<GameObject> _adds = new List<GameObject>();
        private bool[] _riftSealed;
        private Renderer _visual;
        private MaterialPropertyBlock _mpb;
        private Vector3 _baseScale;

        private void Awake()
        {
            _visual = GetComponent<Renderer>();
            _mpb = new MaterialPropertyBlock();
            _baseScale = transform.localScale;
            _riftSealed = new bool[riftPoints != null ? riftPoints.Length : 0];
        }

        private void Update()
        {
            if (_state == State.Expelled || player == null) return;

            // the chapter-1 macro drain — arrival hit once, then steady bleed
            var zone = MemoryStabilityZone.Current;
            if (!_arrived && Vector3.Distance(transform.position, player.transform.position) < 18f)
            {
                _arrived = true;
                zone?.AddStability(-arrivalDrain, "the Devourer sits at the battle's seams");
                ScreenFader.TitleCard("SOMETHING IS EATING THE BATTLE — ONLY KAI CAN SEE IT", 3.5f);
            }
            if (Time.time >= _nextDrain)
            {
                _nextDrain = Time.time + 30f;
                if (_state == State.Watching || _state == State.RiftOpen)
                    zone?.AddStability(-seamDrainPerHalfMin, "the seams stay open");
            }

            // shimmer of absence (graybox stand-in for the stolen-faces mantle)
            if (_visual != null)
            {
                _visual.GetPropertyBlock(_mpb);
                _mpb.SetColor("_BaseColor", Color.Lerp(new Color(0.08f, 0.05f, 0.12f),
                    new Color(0.22f, 0.12f, 0.3f), 0.5f + 0.5f * Mathf.Sin(Time.time * 1.7f)));
                _visual.SetPropertyBlock(_mpb);
            }

            switch (_state)
            {
                case State.Watching:
                    // Phase 1 of the spec: the player must CHOOSE to step into a seam.
                    for (int i = 0; i < riftPoints.Length; i++)
                        if (_riftSealed[i] || riftPoints[i] == null) continue;
                        else if (Vector3.Distance(player.transform.position, riftPoints[i].position) <= engageRadius)
                        { OpenRift(i); break; }
                    break;

                case State.RiftOpen:
                    Face(player.transform);
                    if (BayaniInput.SkillPressed)
                    {
                        if (player.resources != null && player.resources.CanAffordDiwa(diwaSealCost))
                        {
                            player.resources.SpendDiwa(diwaSealCost);
                            SealRift();
                        }
                        else ProgressStore.Toast = "The seam will not bite — need DIWA ≥ 25 (parry, don't farm)";
                    }
                    break;

                case State.Reaching:
                    // the long cinematic: battle audio thins; the last burst ends the chapter
                    Face(player.transform);
                    DialoguePlayer.Prompt = "THE RIFT HEART — SPEND DIWA [Q]";
                    if (BayaniInput.SkillPressed && player.resources != null
                        && player.resources.CanAffordDiwa(diwaSealCost))
                    {
                        DialoguePlayer.Prompt = null;
                        player.resources.SpendDiwa(diwaSealCost);
                        Expel();
                    }
                    break;
            }
        }

        // ---- flow ----

        private void OpenRift(int i)
        {
            _state = State.RiftOpen;
            _activeRift = i;
            _riftSealed[i] = false;   // explicit: this one is the live one
            ScreenFader.Flash();
            ProgressStore.Toast = $"SEAM {_sealed + 1} OF {riftPoints.Length} — clear the maws, then [Q] DIWA to seal";
            for (int n = 0; n < addsPerRift && aninoPrefab != null && riftPoints[i] != null; n++)
                _adds.Add(Instantiate(aninoPrefab,
                    riftPoints[i].position + Random.insideUnitSphere * 2.5f + Vector3.up * 0.1f,
                    Quaternion.identity));
        }

        private void SealRift()
        {
            if (_activeRift >= 0 && _activeRift < _riftSealed.Length) _riftSealed[_activeRift] = true;
            _sealed++;
            ScreenFader.TitleCard($"SEAM SEALED — {_sealed}/{riftPoints.Length}", 2.2f);
            // the maws thin out once their seam closes (graybox: they remain killable regardless)
            if (_sealed >= riftPoints.Length) EnterReaching();
            else _state = State.Watching;
        }

        private void EnterReaching()
        {
            _state = State.Reaching;
            _activeRift = -1;
            if (reachingDialogue != null) DialoguePlayer.Instance.Play(reachingDialogue);
            if (aninoPrefab != null)      // the last wave — "it reaches for the event itself"
                for (int n = 0; n < 2; n++)
                    _adds.Add(Instantiate(aninoPrefab, transform.position + new Vector3(n * 3f - 1.5f, 0.2f, -3f),
                        Quaternion.identity));
        }

        private void Expel()
        {
            _state = State.Expelled;
            ProgressStore.Toast = null;

            // no kill rewards — there IS no kill (post-"kill" Diwa: none, deliberate).
            // The chapter's XP lands as the expulsion reward instead (§56.4 boss ceiling).
            PlayerProgression.Instance?.AwardXp(xpForExpulsion, "SEQ 11 — the battle is remembered");

            var zone = MemoryStabilityZone.Current;
            zone?.AddStability(100f - zone.Value, "the seams close — the battle keeps its shape");
            ScreenFader.TitleCard("DRIVEN OUT — NOT KILLED", 4f);
            StartCoroutine(FleeAndFinish());
        }

        private IEnumerator FleeAndFinish()
        {
            // every ambient sound rushes back into the hole at once (audio brief) —
            // graybox: it sinks out of the seams, dragging its stolen faces with it
            DialoguePlayer.Prompt = null;
            float t = 0f;
            Vector3 start = transform.position;
            Vector3 end = start + Vector3.down * 4f;
            while (t < 1f)
            {
                t += Time.deltaTime / 2.2f;
                transform.position = Vector3.Lerp(start, end, t);
                transform.localScale = _baseScale * Mathf.Lerp(1f, 0.02f, t);
                yield return null;
            }
            if (victoryDialogue != null) DialoguePlayer.Instance.Play(victoryDialogue);
            Destroy(gameObject);
        }

        // ---- IDamageable: blades pass through ----

        public void ApplyDamage(float damage, Vector3 fromPos, float knockback)
        {
            // resistance: everything-physical. The line IS the teaching — rate-limited.
            if (Time.time - _lastThrough > 4f)
            {
                _lastThrough = Time.time;
                ProgressStore.Toast = "THE BLADE PASSES THROUGH — only Diwa bites";
            }
        }

        private void Face(Transform t)
        {
            Vector3 d = t.position - transform.position; d.y = 0f;
            if (d.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(d), 6f * Time.deltaTime);
        }
    }
}
