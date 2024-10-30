using System.Collections.Generic;
using FPSSystem.DamageSystem;
using UnityEngine;

namespace FPSSystem.WeaponSystem
{
    public class ProjectileController : MonoBehaviour
    {
        private ProjectileArgs _args;
        private List<IDamagable> _damagedTargets;

        private float _time;

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
        }

        public void Enable(ProjectileArgs args)
        {
            _args = args;
            _damagedTargets = new List<IDamagable>();

            _time = Definition.Damage;

            transform.SetPositionAndRotation(_args.Position, _args.Rotation);
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            _args = new ProjectileArgs();
            _damagedTargets = null;

            gameObject.SetActive(false);
        }
    }
}
