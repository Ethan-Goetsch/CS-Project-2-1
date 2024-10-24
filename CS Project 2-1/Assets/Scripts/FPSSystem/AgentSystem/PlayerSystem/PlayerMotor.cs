using FPSSystem.MovementSystem;
using Sirenix.OdinInspector;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace FPSSystem.AgentSystem.PlayerSystem
{
    public class PlayerMotor : Motor
    {
        [TitleGroup("Components")]
        [Required, SerializeField]
        private CharacterController characterController;

        [TitleGroup("Stats")]
        [SerializeField]
        private float movementSpeed = 5f;

        public override void HandleMovement(ActionBuffers actions)
        {
            var horizontalMovement = actions.ContinuousActions[0];
            var verticalMovement = actions.ContinuousActions[1];

            var horizontalRotation = actions.ContinuousActions[2];
            var verticalRotation = actions.ContinuousActions[3];

            Move(horizontalMovement, verticalMovement);
            Rotate(horizontalRotation, verticalRotation);
        }

        private void Move(float horizontal, float vertical)
        {
            var movement = transform.right * horizontal;
            movement += transform.forward * vertical;
            movement.Normalize();
            movement.y = 0f;

            characterController.Move(movement * (movementSpeed * Time.deltaTime));
        }

        private void Rotate(float horizontal, float vertical)
        {

        }
    }
}
