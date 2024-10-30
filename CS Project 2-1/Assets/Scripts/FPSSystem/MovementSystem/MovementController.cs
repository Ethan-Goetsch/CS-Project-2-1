using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.MovementSystem
{
    public class MovementController : MonoBehaviour
    {
        [TitleGroup("Components")]
        [Required, SerializeField]
        private CharacterController character;

        [TitleGroup("Settings")]
        [Required, SerializeField]
        private float speed = 5f;

        [SerializeField]
        private float rotationSpeed = 200f;

        public void Initialize()
        {

        }

        public void HandleMovement(Vector2 direction)
        {
            var movement = new Vector3(direction.x, 0, direction.y);
            movement.Normalize();
            movement.y = 0;
            movement *= speed;

            character.Move(movement * Time.deltaTime);
        }

        public void HandleRotation(int horizontalDirection)
        {
            var horizontalRotation = transform.up * horizontalDirection;
            transform.Rotate(horizontalRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
