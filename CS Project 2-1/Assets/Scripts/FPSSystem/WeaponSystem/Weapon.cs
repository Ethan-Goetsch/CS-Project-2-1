using System;
using System.Collections;
using System.Linq;
using FPSSystem.AgentSystem;
using FPSSystem.DamageSystem;
using FPSSystem.ProjectileSystem;
using FPSSystem.SoundSystem;
using R3;
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
        private float reloadDuration = 2f;

        [TitleGroup("Sound")]
        [SerializeField]
        private float shootRadius = 10f, reloadRadius = 5f;

        [FoldoutGroup("Events")]
        [SerializeField]
        private UnityEvent onShoot, onHit, onMiss, onStartReload, onStopReload;

        private readonly Subject<IWeaponEvent> _onEvent = new();
        private FPSAgent _owner;

        [TitleGroup("Runtime")]
        [ShowInInspector, ReadOnly]
        public int CurrentAmmo { get; private set; }

        [ShowInInspector, ReadOnly]
        public bool IsReloading { get; private set; }

        [ShowInInspector, ReadOnly]
        public float FireTimer { get; private set; }

        [ShowInInspector, ReadOnly]
        public bool CanShoot => !IsReloading && CurrentAmmo > 0 && FireTimer <= 0;

        [ShowInInspector, ReadOnly]
        public bool CanReload => !IsReloading && CurrentAmmo < MaxAmmo;

        public int MaxAmmo => maxAmmo;
        public float ReloadDuration => reloadDuration;

        public ProjectileDefinition Definition => projectileDefinition;

        public Observable<T> OnEvent<T>() where T : IWeaponEvent => _onEvent.OfType<IWeaponEvent, T>();

        public void Initialize(FPSAgent owner)
        {
            _owner = owner;
            CurrentAmmo = maxAmmo;
        }

        private void Update()
        {
            if (FireTimer > 0)
            {
                FireTimer -= Time.deltaTime;
            }
        }

        public void Shoot()
        {
            CurrentAmmo--;
            FireTimer = Definition.RateOfFire;
            PlaySound(shootRadius);

            var projectile = ProjectileManager.GetOrCreate(Definition);

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

            onShoot.Invoke();
            _onEvent.OnNext(new IWeaponEvent.OnShootEvent(this));
        }

        public void Reload()
        {
            StartCoroutine(ReloadAmmo());
        }

        private void OnProjectileHit(ProjectileController projectile, IDamagable damagable)
        {
            damagable.TakeDamage(projectile.Definition.Damage);
            projectile.Disable();
            _onEvent.OnNext(new IWeaponEvent.OnDamagableHitEvent(this, damagable));
        }

        private void OnProjectileEnvironmentHit(ProjectileController projectile, GameObject collision)
        {
            _onEvent.OnNext(new IWeaponEvent.OnEnvironmentHitEvent(this));
            projectile.Disable();
        }

        private void OnProjectileExpire(ProjectileController projectile)
        {
            _onEvent.OnNext(new IWeaponEvent.OnMissHitEvent(this));
            projectile.Disable();
        }

        private IEnumerator ReloadAmmo()
        {
            var previous = CurrentAmmo;

            onStartReload.Invoke();
            IsReloading = true;
            PlaySound(reloadRadius);

            yield return new WaitForSeconds(ReloadDuration);

            CurrentAmmo = MaxAmmo;

            PlaySound(reloadRadius);
            IsReloading = false;
            onStopReload.Invoke();

            _onEvent.OnNext(new IWeaponEvent.OnReloadEvent(this, MaxAmmo, previous, CurrentAmmo));
        }

        private void PlaySound(float radius)
        {
            SoundManager.PlaySound(new Sound
            {
                Origin = _owner.transform.position,
                Radius = radius
            });
        }
    }
}
