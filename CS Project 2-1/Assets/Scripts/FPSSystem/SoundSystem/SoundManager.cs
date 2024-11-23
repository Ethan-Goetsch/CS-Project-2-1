using UnityEngine;

namespace FPSSystem.SoundSystem
{
    public class SoundManager
    {
        private static Collider[] _results;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _results = new Collider[10];
        }

        public static void PlaySound(Sound sound)
        {
            var size = Physics.OverlapSphereNonAlloc(sound.Origin, sound.Radius, _results, LayerMask.GetMask(nameof(Sound)));
            for (var i = 0; i < size; i++)
            {
                if (!_results[i].TryGetComponent<ISoundListener>(out var listener)) continue;
                listener.HearSound(sound);
            }
        }
    }
}
