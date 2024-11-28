using FPSSystem.DamageSystem;

namespace FPSSystem.WeaponSystem
{
    public interface IWeaponEvent
    {
        public record OnShootEvent(Weapon Weapon) : IWeaponEvent;

        public record OnReloadEvent(Weapon Weapon, int MaxAmmo, int Previous, int New) : IWeaponEvent
        {
            public int AmountReloaded => New - Previous;
        }

        public record OnAmmoRestoredEvent(Weapon Weapon, int MaxAmmo, int Previous, int New) : IWeaponEvent
        {
            public int AmountReloaded => New - Previous;
        }

        public abstract record OnHitEvent(Weapon Weapon) : IWeaponEvent;

        public record OnDamagableHitEvent(Weapon Weapon, IDamagable Damagable) : IWeaponEvent;
        public record OnEnvironmentHitEvent(Weapon Weapon) : IWeaponEvent;
        public record OnMissHitEvent(Weapon Weapon) : IWeaponEvent;

        public Weapon Weapon { get; }
    }
}
