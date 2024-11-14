using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.TrainingSystem
{
    public class FPSSpawnPoint : MonoBehaviour
    {
        [Required]
        public Transform Agent1Spawn;

        [Required]
        public Transform Agent2Spawn;
    }
}
