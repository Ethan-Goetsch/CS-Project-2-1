
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSSystem.HealthSystem
{

    public class HealthPickupGlobalController : MonoBehaviour
    {

        [SerializeField]
        private GameObject _healthPickupPrefab;

        [SerializeField]
        private List<Transform> _spawnPoints;

        [SerializeField]
        private float _respawnInterval = 10f;

        private Dictionary<Transform, GameObject> _activePickups = new();

        private void Start()
        {

            foreach (var spawnPoint in _spawnPoints)
            {
                SpawnPickup(spawnPoint);
            }
        }

        private void SpawnPickup(Transform spawnPoint)
        {
            if (_activePickups.ContainsKey(spawnPoint) && _activePickups[spawnPoint] != null)
                return;

            var pickup = Instantiate(_healthPickupPrefab, spawnPoint.position, spawnPoint.rotation);
            _activePickups[spawnPoint] = pickup;

            
            pickup.GetComponent<HealthPickup>().OnPickupCollected += () =>
            {
                StartCoroutine(RespawnPickup(spawnPoint));
            };
        }

        private IEnumerator RespawnPickup(Transform spawnPoint)
        {
            yield return new WaitForSeconds(_respawnInterval);
            SpawnPickup(spawnPoint);
        }

    }

}