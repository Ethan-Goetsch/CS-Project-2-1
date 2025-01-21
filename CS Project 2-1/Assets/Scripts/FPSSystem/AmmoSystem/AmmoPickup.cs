using FPSSystem.PickupSystem;
using FPSSystem.SoundSystem;
using UnityEngine;

namespace FPSSystem.AmmoSystem
{
    public class AmmoPickup : Pickup
    {
        [SerializeField]
        private int restoreAmmount = 10;

        protected override void OnPickup(Collider other)
        {
            if (!other.TryGetComponent<IAmmoAvailable>(out var ammoAvailable)) return;
            SoundManager.PlaySound(new Sound
            {
                Origin = transform.position,
                Radius = soundRadius,
            });
            ammoAvailable.TakeAmmo(restoreAmmount);
        }
    }
}
