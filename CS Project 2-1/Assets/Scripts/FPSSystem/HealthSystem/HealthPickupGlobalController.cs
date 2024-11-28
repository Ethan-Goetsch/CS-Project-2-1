using System.Collections;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace FPSSystem.HealthSystem
{
    public class HealthPickupGlobalController : MonoBehaviour
    {
        [SerializeField]
        private HealthPickup healthPickup;

        [SerializeField]
        private float respawnInterval = 10f;

        [SerializeField]
        private List<Transform> spawnPoints;

        public void Initialize()
        {
            SpawnPickupAtRandomPoint(healthPickup);
        }

        private void Start()
        {
            healthPickup.OnPickUpCollected
                .Subscribe(_ => OnPickUpCollected(healthPickup))
                .AddTo(this);
            healthPickup.OnPickUpDespawn
                .Subscribe(_ => OnPickUpDespawned(healthPickup))
                .AddTo(this);
        }

        private void OnPickUpCollected(HealthPickup pickup)
        {
            pickup.Disable();
            StartCoroutine(RespawnPickup(pickup));
        }

        private void OnPickUpDespawned(HealthPickup pickup)
        {
            SpawnPickupAtRandomPoint(pickup);
        }

        private IEnumerator RespawnPickup(HealthPickup pickup)
        {
            yield return new WaitForSeconds(respawnInterval);
            SpawnPickupAtRandomPoint(pickup);
        }

        private void SpawnPickupAtRandomPoint(HealthPickup pickup)
        {
            var spawnIndex = Random.Range(0, spawnPoints.Count);
            pickup.Enable(spawnPoints[spawnIndex]);
        }
    }
}
