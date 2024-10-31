using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.MovementSystem
{
    public class MovementController : MonoBehaviour
    {
        [TitleGroup("Settings")]
        [Required, SerializeField]
        private float speed = 5f;

        [SerializeField]
        private float rotationSpeed = 200f;

        private CharacterController _characterController;

        public void Initialize(CharacterController characterController)
        {
            _characterController = characterController;
        }

        public void HandleMovement(Vector2 direction)
        {
            var movement = new Vector3(direction.x, 0, direction.y);
            movement.Normalize();
            movement.y = 0;
            movement *= speed;

            _characterController.Move(movement * Time.deltaTime);
        }

        public void HandleRotation(int horizontalDirection)
        {
            var horizontalRotation = transform.up * horizontalDirection;
            transform.Rotate(horizontalRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
