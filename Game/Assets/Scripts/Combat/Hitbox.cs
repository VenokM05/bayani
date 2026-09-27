// BAYANI — Damage source volume (phasing.md 1.5). A trigger collider ENABLED only
// during an attack's active window (windup → active → recovery). Graybox: the builder
// creates a scaled cube child in front of the owner; real weapons socket in later.
// Dedupe: each target can be hit once per activation window (no machine-gun swings).

using System.Collections.Generic;
using UnityEngine;

namespace Bayani.Combat
{
    public class Hitbox : MonoBehaviour
    {
        public enum Team { Player, Enemy }

        [Header("Wired by owner at spawn")]
        public float damage = 10f;
        public float knockback = 2f;
        public GameObject owner;
        public Team team;

        private readonly HashSet<Hurtbox> _hitThisWindow = new();
        private readonly HashSet<Hurtbox> _skipLogged = new();   // 10B diagnostic, once per window

        /// <summary>Fires when this swing connects — owner uses it for Diwa gain + hit-stop.</summary>
        public event System.Action<Hurtbox> OnLanded;

        /// <summary>Call when the attack's ACTIVE phase begins.</summary>
        public void ActivateWindow()
        {
            _hitThisWindow.Clear();
            _skipLogged.Clear();
            gameObject.SetActive(true);
        }

        /// <summary>Call when the ACTIVE phase ends.</summary>
        public void DeactivateWindow() => gameObject.SetActive(false);

        private void OnTriggerStay(Collider other)
        {
            var hurt = other.GetComponentInParent<Hurtbox>();
            if (hurt == null) return;
            // 10B diagnostic: contact exists but a filter drops it — name the filter
            if (hurt.IsInvulnerable || hurt.team == team) 
            {
                if (!_skipLogged.Contains(hurt))
                {
                    _skipLogged.Add(hurt);
                    Debug.Log($"[BAYANI][10B-probe] {name} overlapped {hurt.name} but dropped: " +
                              $"invuln={hurt.IsInvulnerable} hurtTeam={hurt.team} myTeam={team}");
                }
                return;
            }
            if (_hitThisWindow.Contains(hurt)) return;     // one hit per window per target

            _hitThisWindow.Add(hurt);
            hurt.TakeDamage(damage, transform.position, knockback, owner);
            OnLanded?.Invoke(hurt);
        }

        private void OnDisable() => _hitThisWindow.Clear();
    }
}
