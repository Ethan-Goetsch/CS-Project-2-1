using FPSSystem.SoundSystem;
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

        [TitleGroup("Sound")]
        [SerializeField]
        private float soundRadius = 5f;

        private CharacterController _characterController;

        public void Initialize(CharacterController characterController)
        {
            _characterController = characterController;
        }

        public void HandleMovement(Vector2 direction)
        {
            if (direction == Vector2.zero) return;

            var movement = new Vector3(direction.x, 0, direction.y);
            movement.Normalize();
            movement.y = 0;
            movement *= speed;

            var sound = new Sound
            {
                Origin = _characterController.transform.position,
                Radius = soundRadius
            };

            SoundManager.PlaySound(sound);
            _characterController.Move(movement * Time.deltaTime);
        }

        public void HandleRotation(int horizontalDirection)
        {
            if (horizontalDirection == 0) return;
            var horizontalRotation = transform.up * horizontalDirection;
            transform.Rotate(horizontalRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
