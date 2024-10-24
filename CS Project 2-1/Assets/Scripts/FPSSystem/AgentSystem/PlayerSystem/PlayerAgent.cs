using Sirenix.OdinInspector;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace FPSSystem.AgentSystem.PlayerSystem
{
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerAgent : FPSAgent
    {
        [TitleGroup("Components")]
        private PlayerMotor motor;

        public override void Initialize()
        {
            base.Initialize();
            motor = GetComponent<PlayerMotor>();
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuousActions = actionsOut.ContinuousActions;
            continuousActions[0] = Input.GetAxisRaw("Horizontal");
            continuousActions[1] = Input.GetAxisRaw("Vertical");

            continuousActions[2] = Input.GetAxisRaw("MouseX");
            continuousActions[3] = Input.GetAxisRaw("MouseY");
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            base.OnActionReceived(actions);
            motor.HandleMovement(actions);
        }
    }
}
