using FPSSystem.DamageSystem;
using FPSSystem.HealthSystem;
using FPSSystem.MovementSystem;
using FPSSystem.WeaponSystem;
using R3;
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

        public Observable<T> OnEntityEvent<T>() where T : IAgentEvent => _onEvent.OfType<IAgentEvent, T>();

        public override void Initialize()
        {
            _fpsController = GetComponentInParent<IFPSController>();
        }

        public override void OnEpisodeBegin()
        {
            var position = _fpsController.GetStartingPosition(this);
            var rotation = _fpsController.GetStartingRotation(this);
            transform.SetPositionAndRotation(position, rotation);

            Health = MaxHealth;
            movementController.Initialize();
            weapon.Initialize(this);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(transform.localPosition);
            sensor.AddObservation(transform.localRotation);
        }

        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            actionMask.SetActionEnabled(3, 1, weapon.CanShoot);
            actionMask.SetActionEnabled(4, 1, weapon.CanReload);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            var horizontalMovement = GetDirectionFromAction(actions.DiscreteActions[0]);
            var verticalMovement = GetDirectionFromAction(actions.DiscreteActions[1]);

            var horizontalRotation = GetDirectionFromAction(actions.DiscreteActions[2]);

            var shouldAttack = actions.DiscreteActions[3] == 1;
            var shouldSpecialAttack = actions.DiscreteActions[4] == 1;

            movementController.HandleMovement(new Vector2(horizontalMovement, verticalMovement));
            movementController.HandleRotation(horizontalRotation);

            if (shouldAttack)
            {
                weapon.Shoot();
            }

            if (shouldSpecialAttack)
            {
                weapon.Reload();
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
