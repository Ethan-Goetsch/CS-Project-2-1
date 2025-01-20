using FPSSystem.PickupSystem;
using FPSSystem.SoundSystem;
using UnityEngine;

namespace FPSSystem.HealthSystem
{
    public class HealthPickup : Pickup
    {
        [SerializeField]
        private float healAmount = 10f;

        protected override void OnPickup(Collider other)
        {
            if (!other.TryGetComponent<IHealable>(out var healable)) return;
            SoundManager.PlaySound(new Sound
            {
                Origin = transform.position,
                Radius = soundRadius,
            });
            healable.TakeHealing(healAmount);
        }
    }
}
