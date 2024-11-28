using R3;
using UnityEngine;
using Sirenix.OdinInspector;

namespace FPSSystem.HealthSystem
{
    public class HealthPickup : MonoBehaviour
    {
        private readonly Subject<Unit> _onPickupCollected = new();
        private readonly Subject<Unit> _onPickupDespawn = new();

        [SerializeField]
        private float healingAmount = 10f;

        [SerializeField]
        private float activeDuration = 10f;

        private float _activeTimer;

        public Observable<Unit> OnPickUpCollected => _onPickupCollected;
        public Observable<Unit> OnPickUpDespawn => _onPickupDespawn;

        private void Update()
        {
            if (_activeTimer > 0)
            {
                _activeTimer -= Time.deltaTime;
            }
            else
            {
                _onPickupDespawn.OnNext(Unit.Default);
            }
        }

        public void Enable(Transform spawnPoint)
        {
            _activeTimer = activeDuration;

            transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            var healable = other.GetComponent<IHealable>();
            if (healable == null) return;

            healable.TakeHealing(healingAmount);
            Consume();
        }

        [Button]
        private void Consume()
        {
            _onPickupCollected.OnNext(Unit.Default);
        }
    }
}
