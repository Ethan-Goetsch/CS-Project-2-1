using System.Collections.Generic;
using FPSSystem.DamageSystem;
using FPSSystem.WeaponSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.ProjectileSystem
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class ProjectileController : MonoBehaviour
    {
        [TitleGroup("Components")]
        [Required, SerializeField]
        private new Rigidbody rigidbody;

        [Required, SerializeField]
        private new Collider collider;

        private float _time;

        private ProjectileArgs _args;
        private List<IDamagable> _damagedTargets;

        private Vector3 _hitStartScale;
        private Vector3 _muzzleStartScale;
        private GameObject _muzzleFX;

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
            }
        }

        public void Initialize(ProjectileDefinition definition)
        {
            Definition = definition;
            gameObject.SetActive(false);

            _muzzleFX = Instantiate(definition.MuzzleFX, transform);
            _muzzleStartScale = _muzzleFX.transform.localScale;
        }

        public void Enable(ProjectileArgs args)
        {
            _args = args;
            _damagedTargets = new List<IDamagable>();

            _time = Definition.Lifetime;
            _args.OwnerColliders.ForEach(c => Physics.IgnoreCollision(collider, c, true));

            _muzzleFX.transform.SetParent(_args.SpawnPoint, false);
            _muzzleFX.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _muzzleFX.transform.localScale = _muzzleStartScale;

            transform.position = _args.SpawnPoint.position;
            transform.forward = _args.SpawnPoint.forward;

            _muzzleFX.SetActive(true);
            gameObject.SetActive(true);

            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.AddForce(transform.forward * Definition.Speed, ForceMode.Impulse);
        }

        public void Disable()
        {
            _muzzleFX.transform.SetParent(transform);

            _muzzleFX.SetActive(false);
            gameObject.SetActive(false);

            _args.OwnerColliders.ForEach(c => Physics.IgnoreCollision(collider, c, false));

            _args = new ProjectileArgs();
            _damagedTargets = null;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!gameObject.activeInHierarchy) return;

            OnHit(other);

            var damagable = other.gameObject.GetComponentInParent<IDamagable>();
            if (damagable != null && !_damagedTargets.Contains(damagable))
            {
                OnDamagableHit(damagable);
            }
            else
            {
                OnEnvironmentHit(other);
            }
        }

        private void OnHit(Collision other)
        {
            var hitFX = ProjectileHitFXManager.GetOrCreate(Definition);
            hitFX.transform.position = other.contacts[0].point;
            hitFX.transform.forward = -1 * transform.forward;
            hitFX.gameObject.SetActive(true);

            _hitStartScale = _hitStartScale == Vector3.zero ? hitFX.transform.localScale : _hitStartScale;
            hitFX.transform.localScale = _hitStartScale;
        }

        private void OnDamagableHit(IDamagable damagable)
        {
            _damagedTargets.Add(damagable);
            _args.OnDamagableHit?.Invoke(this, damagable);
        }

        private void OnEnvironmentHit(Collision other)
        {
            _args.OnEnvironmentHit?.Invoke(this, other.gameObject);
        }
    }
}
