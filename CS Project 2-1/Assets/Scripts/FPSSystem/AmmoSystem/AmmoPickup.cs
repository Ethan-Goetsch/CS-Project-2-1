using FPSSystem.PickupSystem;
using UnityEngine;

namespace FPSSystem.AmmoSystem
{
    public class AmmoPickup : Pickup
    {
        [SerializeField]
        private int restoreAmmount = 10;

        protected override void OnPickup(Collider other)
        {
            if (other.TryGetComponent<IAmmoAvailable>(out var ammoAvailable))
            {
                ammoAvailable.TakeAmmo(restoreAmmount);
            }
        }
    }
}
