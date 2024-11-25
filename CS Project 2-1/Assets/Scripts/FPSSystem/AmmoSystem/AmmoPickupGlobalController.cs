
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSSystem.AmmoSystem
{

    public class AmmoPickupGlobalController : MonoBehaviour
    {

        [SerializeField]
        private GameObject _ammoPickupPrefab;

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

            var pickup = Instantiate(_ammoPickupPrefab, spawnPoint.position, spawnPoint.rotation);
            _activePickups[spawnPoint] = pickup;

            
            pickup.GetComponent<AmmoPickup>().OnPickupCollected += () =>
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