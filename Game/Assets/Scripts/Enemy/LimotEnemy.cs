// BAYANI — Limot enemy brain (phasing.md 1.7): Chase → TELEGRAPH (>=0.6s, visible)
// → Attack → Recover (punish window). Variants (1.9) = different EnemyData assets.
// Graybox movement: direct transform steering, no NavMesh dependency yet (Phase 2 arena
// upgrades to NavMeshAgent when patrol routes arrive).

using Bayani.Combat;
using UnityEngine;

namespace Bayani.Enemy
{
    public class LimotEnemy : MonoBehaviour, IDamageable, IBattleHealth
    {
        [Header("Wired by Phase1 builder")]
        public EnemyData data;
        public Hurtbox hurtbox;
        public Hitbox hitbox;              // child trigger, disabled by default
        public PlayerCombat player;        // for kill rewards + chase target

        private enum State { Idle, Chase, Telegraph, Attack, Recover, Dead }
        private State _state = State.Idle;
        public bool IsDead => _state == State.Dead;   // wave spawner checks this for clear conditions
        private float _stateUntil;
        private float _hp;
        private Renderer _visual;
        private MaterialPropertyBlock _mpb;
        private CharacterController _cc;   // optional; falls back to transform move
        private bool _staggered;
        private Vector3 _baseScale = Vector3.one;
        private bool _countedAsNest;         // Memory Stability drain registration (ggd §56.3)

        private void OnEnable()
        {
            BattleHealthRegistry.Add(this);
            if (!_countedAsNest && Bayani.Core.MemoryStabilityZone.Current != null)
            {
                Bayani.Core.MemoryStabilityZone.Current.NestArrived();
                _countedAsNest = true;
            }
        }

        private void OnDestroy()
        {
            BattleHealthRegistry.Remove(this);
            if (_countedAsNest) Bayani.Core.MemoryStabilityZone.Current?.NestLeft();
        }

        // ---- IBattleHealth (HUD overhead bars, PRD §6) ----
        public float HealthFraction => _hp / Mathf.Max(1f, data.maxHP);
        public string BarName => data.enemyName;
        public Vector3 HeadPoint => transform.position + Vector3.up * (2f * transform.localScale.y);
        public bool BarVisible => !IsDead;
        public bool BarAlwaysVisible => false;   // regular enemy: small projected bar only when damaged

        private void Awake()
        {
            if (player == null) player = FindFirstObjectByType<PlayerCombat>();  // runtime-spawned prefabs self-wire
            _hp = data.maxHP;
            _baseScale = transform.localScale;
            _visual = GetComponentInChildren<Renderer>();
            _mpb = new MaterialPropertyBlock();
            _cc = GetComponent<CharacterController>();
            if (hitbox != null) hitbox.DeactivateWindow();
        }

        private void Update()
        {
            if (_state == State.Dead || data == null) return;
            var target = player != null ? player.transform : null;
            if (target == null) return;

            float dist = Vector3.Distance(transform.position, target.position);

            switch (_state)
            {
                case State.Idle:
                    if (dist <= data.aggroRadius) Enter(State.Chase);
                    break;

                case State.Chase:
                    if (data.behavior == EnemyData.Behavior.Ranged)
                    {
                        // Lingid: keep standoff distance, side-step would come later
                        if (dist > data.attackRange * 0.8f) MoveToward(target.position, data.moveSpeed);
                        else if (dist < data.attackRange * 0.5f) MoveToward(target.position, -data.moveSpeed * 0.7f);
                        FaceTarget(target);
                        if (dist <= data.attackRange * 1.15f) Enter(State.Telegraph);
                    }
                    else
                    {
                        MoveToward(target.position, data.moveSpeed);
                        FaceTarget(target);
                        if (dist <= data.attackRange * 0.9f) Enter(State.Telegraph);
                    }
                    break;

                case State.Telegraph:
                    // Stand still, glow + grow: the "hit me now" advertisement (>=0.6s per phasing gate)
                    PulseVisual(1f + 0.15f * Mathf.PingPong(Time.time * 4f, 1f), TelegraphColor);
                    FaceTarget(target);
                    if (Time.time >= _stateUntil)
                    {
                        if (data.behavior == EnemyData.Behavior.Ranged)
                        {
                            FireProjectile(target);
                            Enter(State.Recover);
                        }
                        else
                        {
                            if (hitbox != null)
                            {
                                hitbox.damage = data.attackDamage;
                                hitbox.knockback = 2f;
                                hitbox.transform.rotation = transform.rotation;
                                hitbox.ActivateWindow();
                            }
                            Enter(State.Attack);
                        }
                    }
                    break;

                case State.Attack:
                    if (Time.time >= _stateUntil)
                    {
                        hitbox?.DeactivateWindow();
                        Enter(State.Recover);
                    }
                    break;

                case State.Recover:
                    // The punish window — if the player interrupts, stagger resets less of it
                    PulseVisual(1f, new Color(0.55f, 0.4f, 0.6f));
                    if (Time.time >= _stateUntil) Enter(State.Chase);
                    break;
            }
        }

        private Color TelegraphColor => data.behavior switch
        {
            EnemyData.Behavior.Ranged => new Color(1f, 0.55f, 0.1f),   // orange = incoming shot
            EnemyData.Behavior.Tank => new Color(0.8f, 0.1f, 0.45f),   // magenta = heavy, sidestep!
            _ => new Color(1f, 0.25f, 0.1f),                            // red = melee
        };

        private void FireProjectile(Transform target)
        {
            var p = new GameObject("LingidShot");
            Vector3 origin = transform.position + Vector3.up * 0.7f;
            Vector3 dir = (target.position + Vector3.up * 0.6f - origin).normalized;
            p.transform.position = origin + dir * 0.6f;
            var shot = p.AddComponent<Projectile>();
            shot.speed = data.projectileSpeed;
            shot.damage = data.attackDamage;
            shot.team = Hitbox.Team.Enemy;
            shot.owner = gameObject;
            shot.Init(dir);
        }

        private void Enter(State s)
        {
            _state = s;
            float dur = s switch
            {
                State.Telegraph => data.telegraphTime,
                State.Attack => data.attackActiveTime,
                State.Recover => _staggered ? data.recoverTime * 0.5f : data.recoverTime,
                _ => 0f
            };
            _stateUntil = Time.time + dur;
            _staggered = false;
            if (s != State.Telegraph) PulseVisual(1f, Color.white);
        }

        private void MoveToward(Vector3 pos, float speed)
        {
            Vector3 dir = (pos - transform.position); dir.y = 0f;
            if (dir.sqrMagnitude < 0.01f) return;
            dir.Normalize();
            if (_cc != null) _cc.Move(dir * speed * Time.deltaTime);
            else transform.position += dir * speed * Time.deltaTime;
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
            if (_state == State.Dead) return;
            _hp -= damage;

            // Strong-enough hits interrupt telegraphs — poise per variant (Bantay resists everything but parry)
            if (knockback >= data.staggerKnockback && (_state == State.Telegraph || _state == State.Attack))
            {
                hitbox?.DeactivateWindow();
                _staggered = true;
                Enter(State.Recover);
            }

            if (_hp <= 0f) Die();
        }

        /// <summary>A successful player PARRY always staggers, even tanks — long open window.</summary>
        public void NotifyParried()
        {
            if (_state == State.Dead) return;
            hitbox?.DeactivateWindow();
            _staggered = false;                        // full, extra-long recovery: the parry reward
            _state = State.Recover;
            _stateUntil = Time.time + data.recoverTime * 1.8f;
            PulseVisual(1f, new Color(0.55f, 0.4f, 0.6f));
            Debug.Log($"[BAYANI] {data.enemyName} parried — punish window open");
        }

        private void Die()
        {
            _state = State.Dead;
            hitbox?.DeactivateWindow();
            PulseVisual(1f, new Color(0.25f, 0.2f, 0.3f));
            // Item drop (PRD §9) — opt-in per EnemyData; quest kill-counter hook
            if (data.dropItem != null && Random.value <= data.dropChance)
                Bayani.Player.Inventory.Add(data.dropItem);
            Bayani.Story.QuestGuide.QuestJournal.OnEnemyKilled(data);
            player?.OnKillConfirmed(data);
            // Memory Stability: kill = +stability, and this nest stops draining (ggd §56.3)
            if (_countedAsNest)
            {
                Bayani.Core.MemoryStabilityZone.Current?.NestLeft();
                _countedAsNest = false;
            }
            Bayani.Core.MemoryStabilityZone.Current?.AddStability(data.stabilityOnKill, $"{data.enemyName} defeated");
            Debug.Log($"[BAYANI] {data.enemyName} destroyed (+{data.diwaOnKill} Diwa, +{data.xpOnKill} XP in Phase 2)");
            Destroy(GetComponent<Hurtbox>());
            // Sink into the ground like dissolving memory, then remove
            Destroy(gameObject, 2f);
        }
    }
}
