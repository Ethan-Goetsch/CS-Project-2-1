using FPSSystem.AgentSystem;
using FPSSystem.AnimationSystem;
using FPSSystem.SoundSystem;
using R3;
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

        private FPSAgent _agent;
        private Animator _animator;
        private CharacterController _characterController;

        [TitleGroup("Debug")]
        [ShowInInspector, ReadOnly]
        public bool IsGrounded { get; private set; }

        private readonly Subject<IMovementEvent> _subject = new();

        public Observable<T> OnEvent<T>() where T : IMovementEvent => _subject.OfType<IMovementEvent, T>();

        public void Initialize(FPSAgent agent, Animator animator, CharacterController characterController)
        {
            _agent = agent;
            _animator = animator;
            _characterController = characterController;
        }

        public void HandleMovement(Vector2 direction)
        {
            HandleGravity();

            if (direction == Vector2.zero)
            {
                _animator.SetFloat(AnimationParameters.HorizontalDirection, 0);
                _animator.SetFloat(AnimationParameters.VerticalDirection, 0);
                _animator.SetBool(AnimationParameters.IsMoving, false);
                return;
            }

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

            _animator.SetFloat(AnimationParameters.HorizontalDirection, movement.x);
            _animator.SetFloat(AnimationParameters.VerticalDirection, movement.y);
            _animator.SetBool(AnimationParameters.IsMoving, true);

            movement *= Time.deltaTime;

            _characterController.Move(movement);
            _subject.OnNext(new IMovementEvent.AgentMoveEvent(_agent, movement));
        }

        public void HandleRotation(Vector2 direction)
        {
            if (direction == Vector2.zero) return;
            var horizontalRotation = transform.up * direction.x;
            transform.Rotate(horizontalRotation, Time.deltaTime * rotationSpeed);
            _subject.OnNext(new IMovementEvent.AgentRotateEvent(_agent, horizontalRotation));
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
