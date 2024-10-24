using FPSSystem.MovementSystem;
using Sirenix.OdinInspector;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace FPSSystem.AgentSystem.AISystem
{
    public class AIMotor : Motor
    {
        [TitleGroup("Components")]
        [Required, SerializeField]
        private CharacterController characterController;

        [TitleGroup("Stats")]
        [SerializeField]
        private float movementSpeed = 5f;

        [SerializeField]
        private float rotationSpeed = 200f;

        public override void HandleMovement(ActionBuffers actions)
        {
            var horizontalMovement = actions.ContinuousActions[0];
            var verticalMovement = actions.ContinuousActions[1];

            var rotation = actions.DiscreteActions[0];

            Move(horizontalMovement, verticalMovement);
            Rotate(rotation);
        }

        private void Move(float horizontal, float vertical)
        {
            var movement = transform.right * horizontal;
            movement += transform.forward * vertical;
            movement.Normalize();
            movement.y = 0f;

            characterController.Move(movement * (movementSpeed * Time.deltaTime));
        }

        private void Rotate(int direction)
        {
            var rotation = direction switch
            {
                0 => transform.up * 1f,
                1 => transform.up * -1f,
                _ => Vector3.zero
            };

            transform.Rotate(rotation, Time.deltaTime * rotationSpeed);
        }
    }
}
