using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace FPSSystem.AmmoSystem
{   

    public class AmmoPickup : MonoBehaviour
    {
        [SerializeField]
        private float _ammoAmount = 10f;

        public event Action OnPickupCollected;
        
        private void OnTriggerEnter(Collider other){
            IAmmoAvailable ammoAvailable = other.GetComponent<IAmmoAvailable>();
            if (ammoAvailable != null)
            {
                ammoAvailable.TakeAmmo(_ammoAmount);
                Debug.Log("Ammo taken");
                Destroy(gameObject);
                OnPickupCollected?.Invoke();
            }
        }

        [Button]
        private void Consume(){
            Destroy(gameObject);
            OnPickupCollected?.Invoke();
        }

    }
}