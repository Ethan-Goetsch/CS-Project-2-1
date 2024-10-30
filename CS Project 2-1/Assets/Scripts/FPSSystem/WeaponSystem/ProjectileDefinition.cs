using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.WeaponSystem
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
