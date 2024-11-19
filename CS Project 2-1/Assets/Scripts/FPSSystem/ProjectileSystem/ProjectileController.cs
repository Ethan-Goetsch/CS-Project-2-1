using System.Collections.Generic;
using FPSSystem.DamageSystem;
using FPSSystem.WeaponSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.ProjectileSystem
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class ProjectileController : MonoBehaviour
    {
        [Title("Components")]
        [Required, SerializeField]
        private new Rigidbody rigidbody;

        [Required, SerializeField]
        private new Collider collider;

        private float _time;

        private ProjectileArgs _args;
        private List<IDamagable> _damagedTargets;

        public ProjectileDefinition Definition { get; private set; }

        private void Update()
        {
            if (_time > 0)
            {
                _time -= Time.deltaTime;
            }
            else
            {
                _args.OnExpire?.Invoke(this);
                return;
            }

            transform.position += transform.forward * (Definition.Speed * Time.deltaTime);
        }

        public void Initialize(ProjectileDefinition definition)
        {
            Definition = definition;
            gameObject.SetActive(false);
        }

        public void Enable(ProjectileArgs args)
        {
            _args = args;
            _damagedTargets = new List<IDamagable>();

            _time = Definition.Damage;

            _args.OwnerColliders.ForEach(c => Physics.IgnoreCollision(collider, c, true));

            transform.SetPositionAndRotation(_args.Position, _args.Rotation);
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);

            _args.OwnerColliders.ForEach(c => Physics.IgnoreCollision(collider, c, false));

            _args = new ProjectileArgs();
            _damagedTargets = null;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!gameObject.activeInHierarchy) return;

            var damagable = other.gameObject.GetComponentInParent<IDamagable>();
            if (damagable != null && !_damagedTargets.Contains(damagable))
            {
                _damagedTargets.Add(damagable);
                _args.OnDamagableHit?.Invoke(this, damagable);
            }
            else
            {
                _args.OnEnvironmentHit?.Invoke(this, other.gameObject);
            }
        }
    }
}
