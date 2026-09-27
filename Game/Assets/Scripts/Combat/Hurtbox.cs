// BAYANI — Damage receiver. Attach to anything that can be hit (player + enemies).
// Player gets an invulnerability window (dodge i-frames). Exposes events the
// player's combat brain uses for PERFECT DODGE detection and hit-flash for VFX.

using System;
using UnityEngine;

namespace Bayani.Combat
{
    [RequireComponent(typeof(Collider))]
    public class Hurtbox : MonoBehaviour
    {
        public Hitbox.Team team;                       // mirrored from owner
        public bool IsInvulnerable { get; private set; }

        public event Action<float, Vector3> OnTookDamage;   // (amount, fromPos) → hit-flash/knockback
        public event Action OnBlockedByIFrame;              // perfect-dodge candidate

        /// <summary>PlayerCombat hooks this: return true = hit fully deflected (block/parry), no damage applied.</summary>
        public delegate bool DeflectHandler(float damage, Vector3 fromPos, GameObject attacker);
        public DeflectHandler OnDeflect;

        private CombatResources _resources;             // player only (null on enemies)
        private IDamageable _damageable;                // enemies implement this

        private void Awake()
        {
            _resources = GetComponentInParent<CombatResources>();
            _damageable = GetComponentInParent<IDamageable>();
        }

        public void SetInvulnerable(bool value) => IsInvulnerable = value;

        public void TakeDamage(float damage, Vector3 fromPos, float knockback) =>
            TakeDamage(damage, fromPos, knockback, null);

        public void TakeDamage(float damage, Vector3 fromPos, float knockback, GameObject attacker)
        {
            if (IsInvulnerable)
            {
                OnBlockedByIFrame?.Invoke();
                return;
            }

            if (OnDeflect != null && OnDeflect(damage, fromPos, attacker))
                return;   // blocked or parried — handler applied costs/rewards itself

            if (_damageable != null) _damageable.ApplyDamage(damage, fromPos, knockback);
            else if (_resources != null) _resources.TakeHit(damage);
            else Debug.Log($"[BAYANI] {name} takes {damage} (no HP pool wired)");

            OnTookDamage?.Invoke(damage, fromPos);

            // Light knockback on the receiver (graybox: nudge, no physics dependency)
            var rb = GetComponentInParent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
                rb.AddExplosionForce(knockback * 4f, fromPos, 3f);
        }
    }
}
