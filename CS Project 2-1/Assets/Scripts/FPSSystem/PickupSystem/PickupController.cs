using System.Collections;
using System.Collections.Generic;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FPSSystem.PickupSystem
{
    public abstract class PickupController<T> : MonoBehaviour where T : Pickup
    {
        [SerializeField]
        protected T pickup;

        [SerializeField]
        protected float respawnInterval = 10f;

        [SerializeField]
        protected List<Transform> spawnPoints;

        public void Initialize()
        {
            SpawnPickupAtRandomPoint(pickup);
        }

        private void Start()
        {
            pickup.OnEvent<IPickupEvent.OnPickupCollected>()
                .Subscribe(_ => OnPickUpCollected(pickup))
                .AddTo(this);
            pickup.OnEvent<IPickupEvent.OnPickupDespawned>()
                .Subscribe(_ => OnPickUpDespawned(pickup))
                .AddTo(this);
        }

        private void OnPickUpCollected(T p)
        {
            p.Disable();
            StartCoroutine(RespawnPickup(p));
        }

        private void OnPickUpDespawned(T p)
        {
            SpawnPickupAtRandomPoint(p);
        }

        private IEnumerator RespawnPickup(T p)
        {
            yield return new WaitForSeconds(respawnInterval);
            SpawnPickupAtRandomPoint(p);
        }

        private void SpawnPickupAtRandomPoint(T p)
        {
            var spawnIndex = Random.Range(0, spawnPoints.Count);
            p.Enable(spawnPoints[spawnIndex]);
        }
    }
}
