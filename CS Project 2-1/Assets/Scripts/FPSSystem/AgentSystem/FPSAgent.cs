using System.Collections.Generic;
using System.Linq;
using FPSSystem.AgentSystem.SensorSystem;
using FPSSystem.DamageSystem;
using FPSSystem.HealthSystem;
using FPSSystem.MovementSystem;
using FPSSystem.WeaponSystem;
using R3;
using R3.Triggers;
using Sirenix.OdinInspector;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using UnityEngine;

namespace FPSSystem.AgentSystem
{
    public class FPSAgent : Agent, IDamagable, IHealable
    {
        private readonly Subject<IAgentEvent> _onEvent = new();

        [TitleGroup("Components")]
        [Required, SerializeField]
        private CharacterController characterController;

        [Required, SerializeField]
        private MovementController movementController;

        [Required, SerializeField]
        private Weapon weapon;

        private IFPSController _fpsController;

        [TitleGroup("Stats")]
        [SerializeField]
        private float maxHealth = 100;

        public Vector3 Position => transform.position;
        public float MaxHealth => maxHealth;

        [TitleGroup("Runtime")]
        [ShowInInspector, ReadOnly]
        public float Health { get; private set; }

        public List<Collider> Colliders { get; private set; }

        public Observable<T> OnEntityEvent<T>() where T : IAgentEvent => _onEvent.OfType<IAgentEvent, T>();

        [Required, SerializeField]
        private FpsVisionSensor _visionSensor;

        public override void Initialize()
        {

            _fpsController = GetComponentInParent<IFPSController>();
            Colliders = GetComponentsInChildren<Collider>().ToList();

            // add vision sensor
            _visionSensor = gameObject.AddComponent<FpsVisionSensor>();


        }

        public override void OnEpisodeBegin()
        {
            var position = _fpsController.GetStartingPosition(this);
            var rotation = _fpsController.GetStartingRotation(this);
            transform.SetPositionAndRotation(position, rotation);

            Health = MaxHealth;
            movementController.Initialize(characterController);
            weapon.Initialize(this);
        }

        public override void CollectObservations(VectorSensor sensor)
        {   
            // Observe position
            sensor.AddObservation(transform.localPosition);

            // Observe rotation 
            sensor.AddObservation(transform.localRotation);

            // Observe raycasts for vision. Collects a float array from a flattened list of float arrays, where each
            // array has hot-encoded values for hit object tags, 0/1 if ray hit something at all, and normalized ray distance to hit object (1.0f if missed)
            sensor.AddObservation(_visionSensor.CollectObservation());

            // We may want to add a second, separate vision sensor so the agent can see in a more wide area vertically, which is more human-like
        }

        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            actionMask.SetActionEnabled(3, 1, weapon.CanShoot);
            actionMask.SetActionEnabled(3, 2, weapon.CanReload);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            var horizontalMovement = GetDirectionFromAction(actions.DiscreteActions[0]);
            var verticalMovement = GetDirectionFromAction(actions.DiscreteActions[1]);

            var horizontalRotation = GetDirectionFromAction(actions.DiscreteActions[2]);

            movementController.HandleMovement(new Vector2(horizontalMovement, verticalMovement));
            movementController.HandleRotation(horizontalRotation);

            switch (actions.DiscreteActions[3])
            {
                case 1:
                    weapon.Shoot();
                    break;
                case 2:
                    weapon.Reload();
                    break;
            }
        }

        public override string ToString()
        {
            return gameObject.name;
        }

        public void TakeDamage(float damage)
        {
            var previous = Health;
            Health -= Mathf.Clamp(damage, 0, MaxHealth);
            _onEvent.OnNext(new OnDamaged(this, previous, Health));

            if (Health == 0)
            {
                _onEvent.OnNext(new OnKilled(this));
            }
        }

        public void TakeHealing(float healing)
        {
            var previous = Health;
            Health += Mathf.Clamp(healing, 0, MaxHealth);
            _onEvent.OnNext(new OnHealed(this, previous, Health));
        }

        private int GetDirectionFromAction(int action)
        {
            return action switch
            {
                0 => 0,
                1 => 1,
                2 => -1,
                _ => 0
            };
        }
    }
}
