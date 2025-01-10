using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FPSSystem.ProjectileSystem
{
    public class ProjectileHitFXManager
    {
        private static Dictionary<ProjectileDefinition, List<GameObject>> _hitDictionary;
        private static Transform _hitParent;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _hitDictionary = new Dictionary<ProjectileDefinition, List<GameObject>>();
            _hitParent = new GameObject(nameof(ProjectileHitFXManager)).transform;
        }

        public static GameObject GetOrCreate(ProjectileDefinition definition)
        {
            if (_hitDictionary.TryGetValue(definition, out var effects)
                && effects.Any(p => !p.gameObject.activeInHierarchy))
            {
                return effects.FirstOrDefault(p => !p.gameObject.activeInHierarchy) ?? CreateHit(definition);
            }
            else
            {
                return CreateHit(definition);
            }
        }

        private static GameObject CreateHit(ProjectileDefinition definition)
        {
            var effect = Object.Instantiate(definition.HitFX, _hitParent);
            if (_hitDictionary.TryGetValue(definition, out var effects))
            {
                effects.Add(effect);
            }
            else
            {
                _hitDictionary.Add(definition, new List<GameObject>
                {
                    effect
                });
            }

            effect.gameObject.name += $" {_hitDictionary[definition].Count}";
            return effect;
        }
    }
}
