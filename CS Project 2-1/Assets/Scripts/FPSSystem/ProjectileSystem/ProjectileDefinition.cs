using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.ProjectileSystem
{
    [CreateAssetMenu]
    public class ProjectileDefinition : SerializedScriptableObject
    {
        [TitleGroup("Projectile")]
        [Required]
        public GameObject Prefab;

        [TitleGroup("Effects")]
        [Required]
        public GameObject MuzzleFX, HitFX;

        [TitleGroup("Settings")]
        public float Lifetime = 10f;
        public float FireRate = 0.2f;
        public float Speed = 10f;
        public float Damage = 10f;

        public float RateOfFire => 1 / FireRate;
    }
}
