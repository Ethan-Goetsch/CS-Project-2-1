using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.ProjectileSystem
{
    [CreateAssetMenu]
    public class ProjectileDefinition : SerializedScriptableObject
    {
        public GameObject Prefab;
        public float Lifetime = 10f;
        public float FireRate = 0.2f;
        public float Speed = 10f;
        public float Damage = 10f;

        public float RateOfFire => 1 / FireRate;
    }
}
