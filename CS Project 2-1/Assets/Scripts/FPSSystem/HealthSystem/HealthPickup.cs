using FPSSystem.PickupSystem;
using UnityEngine;

namespace FPSSystem.HealthSystem
{
    public class HealthPickup : Pickup
    {
        [SerializeField]
        private float healAmount = 10f;

        protected override void OnPickup(Collider other)
        {
            if (other.TryGetComponent<IHealable>(out var healable))
            {
                healable.TakeHealing(healAmount);
            }
        }
    }
}
