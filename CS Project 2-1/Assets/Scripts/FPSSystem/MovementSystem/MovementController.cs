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

        private Vector3 _gravityForce;
        private CharacterController _characterController;

        [TitleGroup("Debug")]
        [ShowInInspector, ReadOnly]
        public bool IsGrounded { get; private set; }

        public void Initialize(CharacterController characterController)
        {
            _characterController = characterController;
        }

        public void HandleMovement(Vector2 direction)
        {
            HandleGravity();

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

        public void HandleRotation(Vector2 direction)
        {
            if (direction == Vector2.zero) return;
            var horizontalRotation = transform.up * direction.x;
            transform.Rotate(horizontalRotation, Time.deltaTime * rotationSpeed);
        }

        private void HandleGravity()
        {
            if (IsGrounded)
            {
                _gravityForce = Vector3.down;
            }
            else
            {
                _gravityForce += Physics.gravity * Time.deltaTime;
            }

            _characterController.Move(_gravityForce * Time.deltaTime);
        }
    }
}
