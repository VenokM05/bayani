// BAYANI — Graybox projectile (ranged Lingid shot). Self-assembles a trigger sphere
// + kinematic rigidbody at runtime; damages the first opposing Hurtbox it touches.

using Bayani.Combat;
using UnityEngine;

namespace Bayani.Enemy
{
    public class Projectile : MonoBehaviour
    {
        public float speed = 12f;
        public float damage = 10f;
        public float knockback = 2f;
        public float lifetime = 3f;
        public Hitbox.Team team;
        public GameObject owner;

        private Vector3 _dir;
        private float _bornAt;

        public void Init(Vector3 direction)
        {
            _dir = direction.normalized;
            transform.rotation = Quaternion.LookRotation(_dir);
        }

        private void Awake()
        {
            _bornAt = Time.time;
            var col = gameObject.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.25f;
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;                       // required for trigger events

            var visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Object.Destroy(visual.GetComponent<Collider>());
            visual.transform.SetParent(transform, false);
            visual.transform.localScale = Vector3.one * 0.35f;
        }

        private void Update()
        {
            transform.position += _dir * speed * Time.deltaTime;
            if (Time.time - _bornAt >= lifetime) Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            var hurt = other.GetComponentInParent<Hurtbox>();
            if (hurt == null || hurt.IsInvulnerable) return;
            if (hurt.team == team) return;

            hurt.TakeDamage(damage, transform.position, knockback, gameObject);
            Destroy(gameObject);
        }
    }
}
