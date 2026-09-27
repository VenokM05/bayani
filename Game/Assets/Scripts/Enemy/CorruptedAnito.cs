// BAYANI — Corrupted Anito mini-boss (SEQ 10B, content/enemies/corrupted-anito.md).
// Three phases, all telegraph-legible (1A verdict: readability over raw speed):
//   1 Veiled   — drifting shrine-spirit, melee sweeps; teaches spacing
//   2 Reaching — Shadow-call summons 2 Anino from the wall-shadows; teaches
//                wave-priority (heavy-finisher staggers the call; kill adds first
//                or the arena floods)
//   3 Wounded  — at 50% HP it drives itself into the seam-wall. WIN STATE IS
//                EXPULSION: Diwa-burst prompt (spend Q), never a killing blow.
//                "Driven back, not deleted" (storyline SEQ 10B / chapter rule).
// Memory Stability (ggd §56.3): aggro collapses the village zone to 0% — the
// door event the SEQ 10 caption promised; expulsion restores it to 100% and the
// player WATCHES the world regain color. The narrative machine's first gear.
// Stats live in the EnemyData SO — no hard-coded combat values.

using System.Collections;
using Bayani.Combat;
using Bayani.Story;
using UnityEngine;

namespace Bayani.Enemy
{
    public class CorruptedAnito : MonoBehaviour, IDamageable, IBattleHealth
    {
        [Header("Wired by Phase 2 cave builder")]
        public EnemyData data;
        public Hurtbox hurtbox;
        public Hitbox hitbox;
        public PlayerCombat player;
        public GameObject aninoPrefab;              // phase-2 summons
        public Transform[] shadowPoints;            // wall-shadow spawn spots
        public Transform seamWall;                  // phase-3 retreat target
        public DialogueAsset victoryDialogue;       // wakes the village

        [Header("Tuning (SO-owned stats stay in data)")]
        public float reachingAtPercent = 0.75f;     // phase 2 threshold
        public float woundedAtPercent = 0.50f;      // phase 3 + expel prompt
        public float shadowCallCooldown = 9f;
        public float expelPromptWindow = 6f;        // generous — teaches, not taxes

        private enum Phase { Veiled, Reaching, Wounded }
        private enum State { Dormant, Chase, Telegraph, Sweep, Recover, ShadowCall, GoToWall, Prompt, Expelled }

        private Phase _phase = Phase.Veiled;
        private State _state = State.Dormant;
        private float _hp;
        private float _stateUntil;
        private float _nextShadowCall;
        private bool _dead;
        private Renderer _visual;
        private MaterialPropertyBlock _mpb;
        private Vector3 _baseScale;
        private bool _countedAsNest;

        private void Awake()
        {
            if (player == null) player = FindFirstObjectByType<PlayerCombat>();
            _hp = data != null ? data.maxHP : 120f;
            _visual = GetComponentInChildren<Renderer>();
            _mpb = new MaterialPropertyBlock();
            _baseScale = transform.localScale;
            if (hitbox != null) hitbox.DeactivateWindow();
        }

        private void OnEnable()
        {
            BattleHealthRegistry.Add(this);
        }

        private void OnDestroy()
        {
            BattleHealthRegistry.Remove(this);
            if (_countedAsNest) Bayani.Core.MemoryStabilityZone.Current?.NestLeft();
        }

        // ---- IBattleHealth: the boss gets the big overhead bar (PRD §6) ----
        public float HealthFraction => _hp / Mathf.Max(1f, data.maxHP);
        public string BarName => data.enemyName;
        public Vector3 HeadPoint => transform.position + Vector3.up * (2f * transform.localScale.y);
        public bool BarVisible => !_dead && _state != State.Dormant;
        public bool BarAlwaysVisible => true;   // dedicated boss bar, visible from the pull

        private void Update()
        {
            if (_dead || data == null || player == null) return;

            if (_state == State.Dormant)
            {
                // Dormant until Kai steps in — then the village memory collapses (door event).
                if (Vector3.Distance(transform.position, player.transform.position) <= data.aggroRadius)
                    Activate();
                return;
            }

            var target = player.transform;
            float dist = Vector3.Distance(transform.position, target.position);

            switch (_state)
            {
                case State.Chase:
                    MoveToward(target.position, data.moveSpeed);
                    FaceTarget(target);
                    if (dist <= data.attackRange * 0.9f) EnterTelegraph();
                    else if (_phase == Phase.Reaching && Time.time >= _nextShadowCall)
                        Enter(State.ShadowCall);
                    break;

                case State.Telegraph:
                    FaceTarget(target);
                    PulseVisual(1f + 0.15f * Mathf.PingPong(Time.time * 4f, 1f), Color.red);
                    if (Time.time >= _stateUntil)
                    {
                        if (hitbox != null)
                        {
                            hitbox.damage = data.attackDamage;   // shrine sweep, 14 HP per spec
                            hitbox.knockback = 2.5f;
                            hitbox.transform.rotation = transform.rotation;
                            hitbox.ActivateWindow();
                        }
                        Enter(State.Sweep);
                    }
                    break;

                case State.Sweep:
                    if (Time.time >= _stateUntil)
                    {
                        hitbox?.DeactivateWindow();
                        Enter(State.Recover);
                    }
                    break;

                case State.Recover:
                    PulseVisual(1f, new Color(0.75f, 0.55f, 0.9f));   // punish window
                    if (Time.time >= _stateUntil) Enter(State.Chase);
                    break;

                case State.ShadowCall:
                    // both arms raised, walls darken — 1.4s, interruptible with heavy
                    FaceTarget(target);
                    PulseVisual(1f + 0.25f * Mathf.PingPong(Time.time * 3f, 1f), new Color(1f, 0.55f, 0.1f));
                    if (Time.time >= _stateUntil)
                    {
                        SummonAnino();
                        _nextShadowCall = Time.time + shadowCallCooldown;
                        Enter(State.Chase);
                    }
                    break;

                case State.GoToWall:
                    // Wounded: drag itself toward the seam-wall; invulnerable on the way.
                    if (seamWall == null) { Enter(State.Prompt); break; }
                    MoveToward(seamWall.position, data.moveSpeed * 1.6f);
                    FaceTarget(seamWall);
                    PulseVisual(1f, new Color(0.6f, 0.35f, 0.8f));
                    if (Vector3.Distance(transform.position, seamWall.position) < 1.2f)
                        Enter(State.Prompt);
                    break;

                case State.Prompt:
                    // The expel window: spend Diwa on Q. Missing it isn't punishment —
                    // the fight resumes and the prompt comes again (parries earn Diwa back).
                    DialoguePlayer.Prompt = "SPEND DIWA — [Q]";
                    if (player.resources != null && player.skill != null
                        && Bayani.Core.BayaniInput.SkillPressed
                        && player.resources.CanAffordDiwa(player.skill.diwaCost))
                    {
                        DialoguePlayer.Prompt = null;
                        Expel();
                    }
                    else if (Time.time >= _stateUntil)
                    {
                        DialoguePlayer.Prompt = null;
                        hurtbox?.SetInvulnerable(false);
                        _phase = Phase.Reaching;        // fight resumes; it will turn for the wall again
                        Enter(State.Chase);
                    }
                    break;
            }
        }

        // ---- flow helpers ----

        private void Activate()
        {
            _state = State.Chase;
            _nextShadowCall = Time.time + 4f;
            if (Bayani.Core.MemoryStabilityZone.Current != null && !_countedAsNest)
            {
                var zone = Bayani.Core.MemoryStabilityZone.Current;
                zone.NestArrived();                       // its nest drains while it feeds (§56.3)
                _countedAsNest = true;
                zone.AddStability(-zone.Value, "the anito is feeding");   // collapse to 0% — the door event
            }
            Bayani.Story.ScreenFader.Flash();
        }

        private void Enter(State s)
        {
            _state = s;
            float dur = s switch
            {
                State.Telegraph => data.telegraphTime,
                State.Sweep => data.attackActiveTime,
                State.Recover => data.recoverTime,
                State.ShadowCall => 1.4f,
                State.Prompt => expelPromptWindow,
                _ => 0f
            };
            _stateUntil = Time.time + dur;
            if (s != State.Telegraph && s != State.ShadowCall) PulseVisual(1f, Color.white);
        }

        private void EnterTelegraph() => Enter(State.Telegraph);

        private void SummonAnino()
        {
            if (aninoPrefab == null || shadowPoints == null || shadowPoints.Length == 0) return;
            for (int i = 0; i < 2 && i < shadowPoints.Length; i++)
                if (shadowPoints[i] != null)
                    Instantiate(aninoPrefab, shadowPoints[i].position + Vector3.up * 0.1f,
                        shadowPoints[i].rotation);
            Debug.Log("[BAYANI] Anito Shadow-call — the wall-shadows answer (kill adds or the arena floods)");
        }

        private void CheckPhaseFloor()
        {
            if (_phase != Phase.Wounded && _hp <= data.maxHP * woundedAtPercent)
            {
                _phase = Phase.Wounded;
                hurtbox?.SetInvulnerable(true);   // cannot be killed — only expelled
                DialoguePlayer.Prompt = null;
                Bayani.Story.ScreenFader.TitleCard("THE ANITO turns for the seam-wall — SPEND DIWA [Q]", 3.5f);
                Enter(State.GoToWall);
            }
            else if (_phase == Phase.Veiled && _hp <= data.maxHP * reachingAtPercent)
            {
                _phase = Phase.Reaching;
                _nextShadowCall = Time.time + 2f;
                Bayani.Story.ScreenFader.TitleCard("THE ANITO reaches into the wall-shadows", 2.5f);
            }
        }

        private void Expel()
        {
            DialoguePlayer.Prompt = null;
            _dead = true;
            _state = State.Expelled;
            hitbox?.DeactivateWindow();

            player?.OnKillConfirmed(data);        // rewards via the SO (Diwa +6 per spec)
            var zone = Bayani.Core.MemoryStabilityZone.Current;
            if (zone != null)
            {
                if (_countedAsNest) { zone.NestLeft(); _countedAsNest = false; }
                zone.AddStability(100f - zone.Value, "the anito is driven back — the village wakes");
            }

            Bayani.Story.ScreenFader.TitleCard("DRIVEN BACK — NOT DELETED", 4f);
            StartCoroutine(SinkAndFinish());
        }

        private IEnumerator SinkAndFinish()
        {
            // one clean bell-strike from off-screen (audio brief) → sink into the wall
            float t = 0f;
            Vector3 start = transform.position;
            Vector3 end = seamWall != null ? seamWall.position + Vector3.back * 1.5f : transform.position + Vector3.back * 1.5f;
            while (t < 1f)
            {
                t += Time.deltaTime / 1.6f;
                transform.position = Vector3.Lerp(start, end, t);
                transform.localScale = _baseScale * Mathf.Lerp(1f, 0.05f, t);
                yield return null;
            }
            if (victoryDialogue != null) DialoguePlayer.Instance.Play(victoryDialogue);
            Destroy(gameObject);
        }

        // ---- movement/visual (same graybox vocabulary as LimotEnemy) ----

        private void MoveToward(Vector3 pos, float speed)
        {
            Vector3 dir = pos - transform.position; dir.y = 0f;
            if (dir.sqrMagnitude < 0.01f) return;
            transform.position += dir.normalized * speed * Time.deltaTime;
        }

        private void FaceTarget(Transform t)
        {
            Vector3 d = t.position - transform.position; d.y = 0f;
            if (d.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(d), 10f * Time.deltaTime);
        }

        private void PulseVisual(float scale, Color tint)
        {
            transform.localScale = _baseScale * scale;
            if (_visual == null) return;
            _visual.GetPropertyBlock(_mpb);
            _mpb.SetColor("_BaseColor", tint);
            _visual.SetPropertyBlock(_mpb);
        }

        // ---- IDamageable ----

        public void ApplyDamage(float damage, Vector3 fromPos, float knockback)
        {
            if (_dead || _state == State.Dormant) return;

            if (_state == State.ShadowCall && knockback >= data.staggerKnockback)
            {
                // heavy finisher interrupts the call — the teaches-wave-priority answer
                Enter(State.Recover);
                Debug.Log("[BAYANI] Shadow-call interrupted!");
                return;                                     // no HP on interrupt — the reward IS the cancel
            }

            _hp -= damage;
            if (knockback >= data.staggerKnockback && (_state == State.Telegraph || _state == State.Sweep))
            {
                hitbox?.DeactivateWindow();
                Enter(State.Recover);
            }
            CheckPhaseFloor();
        }

        /// <summary>Parry always lands its punish — even on minibosses (Phase 1 rule).</summary>
        public void NotifyParried()
        {
            if (_dead || _state == State.Dormant) return;
            hitbox?.DeactivateWindow();
            _state = State.Recover;
            _stateUntil = Time.time + data.recoverTime * 1.8f;
            PulseVisual(1f, new Color(0.55f, 0.4f, 0.6f));
        }
    }
}
