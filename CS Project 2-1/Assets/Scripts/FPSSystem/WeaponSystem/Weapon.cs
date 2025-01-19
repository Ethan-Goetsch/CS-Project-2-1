using System;
using System.Collections;
using System.Linq;
using FPSSystem.AgentSystem;
using FPSSystem.AnimationSystem;
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
        private int maxReloads = 2;

        [TitleGroup("Sound")]
        [SerializeField]
        private float shootRadius = 10f, reloadRadius = 5f;

        [FoldoutGroup("Events")]
        [SerializeField]
        private UnityEvent onShoot, onHit, onMiss, onAmmoRestored, onStartReload, onStopReload;

        private readonly Subject<IWeaponEvent> _onEvent = new();
        private FPSAgent _owner;
        private Animator _animator;

        [TitleGroup("Runtime")]
        [ShowInInspector, ReadOnly]
        public int CurrentAmmo { get; private set; }

        [ShowInInspector, ReadOnly]
        public int CurrentReloads { get; private set; }

        [ShowInInspector, ReadOnly]
        public float ShootTimer { get; private set; }


        [ShowInInspector, ReadOnly]
        public bool IsReloading { get; private set; }

        [ShowInInspector, ReadOnly]
        public bool CanShoot => !IsReloading && CurrentAmmo > 0 && ShootTimer <= 0;

        [ShowInInspector, ReadOnly]
        public bool CanReload => !IsReloading && CurrentAmmo < MaxAmmo && CurrentReloads > 0;

        public int MaxAmmo => maxAmmo;
        public int MaxReloads => maxReloads;

        public ProjectileDefinition Definition => projectileDefinition;

        public Observable<T> OnEvent<T>() where T : IWeaponEvent => _onEvent.OfType<IWeaponEvent, T>();

        private void Update()
        {
            if (ShootTimer > 0)
            {
                ShootTimer -= Time.deltaTime;
            }
        }

        public void Initialize(FPSAgent owner, Animator animator)
        {
            _owner = owner;
            _animator = animator;

            CurrentAmmo = MaxAmmo;
            CurrentReloads = MaxReloads;
            Debug.Log("Current reloads : " + CurrentReloads);
        }

        public void TakeAmmo(int ammo)
        {
            var previousAmmo = CurrentAmmo;
            var previousReloads = CurrentReloads;

            var newAmmo = CurrentAmmo + ammo;
            newAmmo = Math.Clamp(newAmmo, 0, MaxAmmo);
            CurrentAmmo = newAmmo;

            CurrentReloads = MaxReloads;

            onAmmoRestored.Invoke();
            _onEvent.OnNext(new IWeaponEvent.OnAmmoRestoredEvent(this, MaxAmmo, previousAmmo, newAmmo, MaxReloads, previousReloads, CurrentReloads));
        }

        public void Shoot()
        {
            CurrentAmmo--;
            ShootTimer = Definition.FireRate;

            SoundManager.PlaySound(new Sound
            {
                Origin = spawnPoint.position,
                Radius = shootRadius
            });

            var projectile = ProjectileManager.GetOrCreate(Definition);

            projectile.Enable(new ProjectileArgs
            {
                Owner = _owner,
                Weapon = this,
                SpawnPoint = spawnPoint,
                OwnerColliders = _owner.Colliders.ToList(),
                OnDamagableHit = OnProjectileHit,
                OnEnvironmentHit = OnProjectileEnvironmentHit,
                OnExpire = OnProjectileExpire
            });

            _animator.SetFloat(AnimationParameters.ShootCount, AnimationParameters.GetRandomShoot());
            _animator.SetTrigger(AnimationParameters.Shoot);

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
            CurrentReloads -= 1;

            onStartReload.Invoke();
            IsReloading = true;

            _animator.SetTrigger(AnimationParameters.Reload);

            SoundManager.PlaySound(new Sound
            {
                Origin = spawnPoint.position,
                Radius = reloadRadius
            });

            yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1 || _animator.IsInTransition(1));

            CurrentAmmo = MaxAmmo;
            SoundManager.PlaySound(new Sound
            {
                Origin = spawnPoint.position,
                Radius = reloadRadius
            });

            IsReloading = false;
            onStopReload.Invoke();
            _onEvent.OnNext(new IWeaponEvent.OnReloadEvent(this, MaxAmmo, previous, CurrentAmmo));
        }
    }
}
