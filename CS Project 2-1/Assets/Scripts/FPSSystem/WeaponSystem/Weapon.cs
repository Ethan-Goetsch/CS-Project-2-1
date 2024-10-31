using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FPSSystem.AgentSystem;
using FPSSystem.DamageSystem;
using FPSSystem.ProjectileSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace FPSSystem.WeaponSystem
{
    public class Weapon : MonoBehaviour
    {
        [TitleGroup("Components")]
        [Required, SerializeField]
        private Transform spawnPoint;

        [TitleGroup("Projectile")]
        [Required, SerializeField]
        private ProjectileDefinition projectileDefinition;

        [TitleGroup("Ammo")]
        [SerializeField]
        private int maxAmmo = 24;

        [SerializeField]
        private float reloadSpeed = 0.2f;

        [FoldoutGroup("Events")]
        [SerializeField]
        private UnityEvent onShoot, onHit, onMiss, onStartReload, onStopReload;

        private FPSAgent _owner;

        [TitleGroup("Runtime")]
        [ShowInInspector, ReadOnly]
        public int CurrentAmmo { get; private set; }

        [ShowInInspector, ReadOnly]
        public bool IsReloading { get; private set; }

        [ShowInInspector, ReadOnly]
        public bool CanShoot => !IsReloading && CurrentAmmo > 0;

        [ShowInInspector, ReadOnly]
        public bool CanReload => !IsReloading && CurrentAmmo < MaxAmmo;

        public int MaxAmmo => maxAmmo;
        public float ReloadSpeed => reloadSpeed;

        public void Initialize(FPSAgent owner)
        {
            _owner = owner;
            CurrentAmmo = maxAmmo;
        }

        public void Shoot()
        {
            onShoot.Invoke();
            CurrentAmmo--;

            var projectile = ProjectileManager.GetOrCreate(projectileDefinition);
            projectile.Enable(new ProjectileArgs
            {
                Owner = _owner,
                Weapon = this,
                Position = spawnPoint.position,
                Rotation = spawnPoint.rotation,
                OwnerColliders = _owner.Colliders.ToList(),
                OnDamagableHit = OnProjectileHit,
                OnEnvironmentHit = OnProjectileEnvironmentHit,
                OnExpire = OnProjectileExpire
            });
        }

        public void Reload()
        {
            StartCoroutine(ReloadAmmo());
        }

        private void OnProjectileHit(ProjectileController projectile, IDamagable damagable)
        {
            damagable.TakeDamage(projectile.Definition.Damage);
            projectile.Disable();
        }

        private void OnProjectileEnvironmentHit(ProjectileController projectile, GameObject collision)
        {
            projectile.Disable();
        }

        private void OnProjectileExpire(ProjectileController projectile)
        {
            projectile.Disable();
        }

        private IEnumerator ReloadAmmo()
        {
            onStartReload.Invoke();
            IsReloading = true;

            yield return new WaitForSeconds(ReloadSpeed);

            CurrentAmmo = MaxAmmo;

            IsReloading = false;
            onStopReload.Invoke();
        }
    }
}
