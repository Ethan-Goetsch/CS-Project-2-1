using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FPSSystem.WeaponSystem
{
    public class ProjectileManager
    {
        private static Dictionary<ProjectileDefinition, List<ProjectileController>> _projectileDictionary;
        private static Transform _projectileParent;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _projectileDictionary = new Dictionary<ProjectileDefinition, List<ProjectileController>>();
            _projectileParent = new GameObject(nameof(ProjectileManager)).transform;
        }

        public static ProjectileController GetOrCreate(ProjectileDefinition definition)
        {
            if (_projectileDictionary.TryGetValue(definition, out var projectiles)
                && projectiles.Any(p => !p.gameObject.activeInHierarchy))
            {
                return projectiles.FirstOrDefault(p => !p.gameObject.activeInHierarchy) ?? CreateProjectile(definition);
            }
            else
            {
                return CreateProjectile(definition);
            }
        }

        private static ProjectileController CreateProjectile(ProjectileDefinition definition)
        {
            var projectile = Object.Instantiate(definition.Prefab, _projectileParent).GetComponent<ProjectileController>();
            projectile.Initialize(definition);

            if (_projectileDictionary.TryGetValue(definition, out var projectiles))
            {
                projectiles.Add(projectile);
            }
            else
            {
                _projectileDictionary.Add(definition, new List<ProjectileController>
                {
                    projectile
                });
            }

            return projectile;
        }
    }
}
