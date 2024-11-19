using FPSSystem.DamageSystem;

namespace FPSSystem.WeaponSystem
{
    public interface IWeaponEvent
    {
        public record OnShootEvent(Weapon Weapon) : IWeaponEvent;
        public record OnReloadEvent(Weapon Weapon) : IWeaponEvent;

        public abstract record OnHitEvent(Weapon Weapon) : IWeaponEvent;

        public record OnDamagableHitEvent(Weapon Weapon, IDamagable Damagable) : IWeaponEvent;
        public record OnEnvironmentHitEvent(Weapon Weapon) : IWeaponEvent;
        public record OnMissHitEvent(Weapon Weapon) : IWeaponEvent;

        public Weapon Weapon { get; }
    }
}
