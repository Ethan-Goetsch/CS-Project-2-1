using R3;
using UnityEngine;

namespace FPSSystem.PickupSystem
{
    public abstract class Pickup : MonoBehaviour
    {
        [SerializeField]
        private float activeDuration = 10f;

        protected readonly Subject<IPickupEvent> _onEvent = new();

        private float _activeTimer;

        public Observable<T> OnEvent<T>() where T : IPickupEvent => _onEvent.OfType<IPickupEvent, T>();

        private void Update()
        {
            if (_activeTimer > 0)
            {
                _activeTimer -= Time.deltaTime;
            }
            else
            {
                _onEvent.OnNext(new IPickupEvent.OnPickupDespawned(this));
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            OnPickup(other);
        }

        public virtual void Enable(Transform spawnPoint)
        {
            _activeTimer = activeDuration;

            transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            gameObject.SetActive(true);
        }

        public virtual void Disable()
        {
            gameObject.SetActive(false);
        }

        protected abstract void OnPickup(Collider other);
    }
}
