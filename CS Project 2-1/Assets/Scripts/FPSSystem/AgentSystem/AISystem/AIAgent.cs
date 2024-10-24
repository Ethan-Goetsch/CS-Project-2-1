using Sirenix.OdinInspector;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace FPSSystem.AgentSystem.AISystem
{
    [RequireComponent(typeof(AIMotor))]
    public class AIAgent : FPSAgent
    {
        [TitleGroup("Components")]
        private AIMotor motor;

        public override void Initialize()
        {
            base.Initialize();
            motor = GetComponent<AIMotor>();
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            base.OnActionReceived(actions);
            motor.HandleMovement(actions);
        }
    }
}
