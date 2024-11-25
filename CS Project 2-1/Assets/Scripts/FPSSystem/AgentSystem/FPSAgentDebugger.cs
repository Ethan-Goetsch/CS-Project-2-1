using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.AgentSystem
{
    [RequireComponent(typeof(FPSAgent))]
    public class FPSAgentDebugger : MonoBehaviour
    {
        private FPSAgent _agent;

        [ShowInInspector, ReadOnly]
        public float Health => _agent.Health;

        [ShowInInspector, ReadOnly]
        public float Ammo => _agent.Ammo;

        private void Awake()
        {
            _agent = GetComponent<FPSAgent>();
        }

        [Button]
        public void TakeDamage(float damage)
        {
            _agent.TakeDamage(damage);
        }

        [Button]
        public void TakeHealing(float healing)
        {
            _agent.TakeHealing(healing);
        }

        [Button]
        public void Shoot()
        {
            _agent.Shoot();
        }

        [Button]
        public void Reload()
        {
            _agent.Reload();
        }
    }
}
