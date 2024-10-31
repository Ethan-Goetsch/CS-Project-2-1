using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.ProjectileSystem
{
    [CreateAssetMenu]
    public class ProjectileDefinition : SerializedScriptableObject
    {
        public GameObject Prefab;
        public float Lifetime;
        public float Speed;
        public float Damage;
    }
}
