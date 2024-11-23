using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace FPSSystem.HealthSystem
{   

    public class HealthPickup : MonoBehaviour
    {
        [SerializeField]
        private float _healingAmount = 10f;

        public event Action OnPickupCollected;
        
        private void OnTriggerEnter(Collider other){
            IHealable healable = other.GetComponent<IHealable>();
            if (healable != null)
            {
                healable.TakeHealing(_healingAmount);
                Debug.Log("Healing taken");
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