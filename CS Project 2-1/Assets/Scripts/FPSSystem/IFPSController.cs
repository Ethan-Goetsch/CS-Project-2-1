using FPSSystem.AgentSystem;
using UnityEngine;

namespace FPSSystem
{
    public interface IFPSController
    {
        public Vector3 GetStartingPosition(FPSAgent agent);

        public Quaternion GetStartingRotation(FPSAgent agent);
    }
}
