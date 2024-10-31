using System.Linq;
using UnityEngine;

namespace FPSSystem.SoundSystem
{
    public class SoundManager
    {
        public static void PlaySound(Sound sound)
        {
            var collisions = Physics.OverlapSphere(sound.Origin, sound.Radius);
            var soundListeners = collisions
                .Select(c => c.GetComponent<ISoundListener>())
                .Where(s => s != null)
                .ToList();
            soundListeners.ForEach(s => s.HearSound(sound));
        }
    }
}
