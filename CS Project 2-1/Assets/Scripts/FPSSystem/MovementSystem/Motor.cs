using Unity.MLAgents.Actuators;
using UnityEngine;

namespace FPSSystem.MovementSystem
{
    public abstract class Motor : MonoBehaviour
    {
        public abstract void HandleMovement(ActionBuffers actions);
    }
}
