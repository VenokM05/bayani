// BAYANI — Anything with its own health pool (enemies now, bosses later).
using UnityEngine;

namespace Bayani.Combat
{
    public interface IDamageable
    {
        void ApplyDamage(float damage, Vector3 fromPos, float knockback);
    }
}
