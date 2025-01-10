using System.Collections.Generic;
using System.Linq;
using FPSSystem.AmmoSystem;
using FPSSystem.AnimationSystem;
using FPSSystem.DamageSystem;
using FPSSystem.HealthSystem;
using FPSSystem.MovementSystem;
using FPSSystem.TrainingSystem;
using FPSSystem.Utils;
using FPSSystem.WeaponSystem;
using R3;
using Sirenix.OdinInspector;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Events;

namespace FPSSystem.AgentSystem
{
    public class FPSAgent : Agent, IDamagable, IHealable, IAmmoAvailable
    {
        private readonly Subject<IAgentEvent> _onEvent = new();

        [TitleGroup("Components")]
        [Required, SerializeField]
        private Animator animator;

        [Required, SerializeField]
        private CharacterController characterController;

        [Required, SerializeField]
        private MovementController movementController;

        [Required, SerializeField]
        private Weapon weapon;

        [TitleGroup("Stats")]
        [SerializeField]
        private float maxHealth = 100;

        [FoldoutGroup("Events")]
        [SerializeField]
        private UnityEvent onDamaged, onHealed, onKilled, onWin, onLose;

        private float _health;
        private FPSEnvironmentController _fpsController;

        public Vector3 Position => transform.position;
        public float MaxHealth => maxHealth;

        [TitleGroup("Runtime")]
        [ShowInInspector, ReadOnly]
        public float Health
        {
            get => _health;
            private set
            {
                if (Health == value) return;
                var previous = Health;
                _health = value;
                _onEvent.OnNext(new IAgentEvent.OnHealthChanged(this, previous, value));
            }
        }

        [ShowInInspector, ReadOnly]
        public float Ammo => Weapon.CurrentAmmo;
        public MovementController MovementController => movementController;
        public Weapon Weapon => weapon;
        public List<Collider> Colliders { get; private set; }

        public Observable<T> OnEvent<T>() where T : IAgentEvent => _onEvent.OfType<IAgentEvent, T>();

        public override void Initialize()
        {
            _fpsController = GetComponentInParent<FPSEnvironmentController>();
            Colliders = GetComponentsInChildren<Collider>().ToList();
        }

        public override void OnEpisodeBegin()
        {
            Health = MaxHealth;

            animator.Rebind();
            animator.Update(0);

            movementController.Initialize(this, animator, characterController);
            Weapon.Initialize(this, animator);

            var position = _fpsController.GetStartingPosition(this);
            var rotation = _fpsController.GetStartingRotation(this);
            transform.SetPositionAndRotation(position, rotation);

            _onEvent.OnNext(new IAgentEvent.OnEpisodeBegin(this));
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(transform.position);
            sensor.AddObservation(transform.rotation);

            sensor.AddObservation(Weapon.CurrentAmmo.Normalize(0, Weapon.MaxAmmo));
            sensor.AddObservation(Health.Normalize(0, MaxHealth));

            sensor.AddObservation(Weapon.CanShoot);
            sensor.AddObservation(Weapon.CanReload);
            sensor.AddObservation(Weapon.ShootTimer.Normalize(0f, Weapon.Definition.RateOfFire));
            sensor.AddObservation(Weapon.CurrentReloads.Normalize(0, Weapon.MaxReloads));
        }

        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            actionMask.SetActionEnabled(0, 1, Weapon.CanShoot);
            actionMask.SetActionEnabled(0, 2, Weapon.CanReload);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            var horizontalMovement = actions.ContinuousActions[0];
            var verticalMovement = actions.ContinuousActions[1];

            var horizontalRotation = actions.ContinuousActions[2];
            var verticalRotation = actions.ContinuousActions[3];

            movementController.HandleMovement(new Vector2(horizontalMovement, verticalMovement));
            movementController.HandleRotation(new Vector2(horizontalRotation, verticalRotation));

            switch (actions.DiscreteActions[0])
            {
                case 1:
                    Shoot();
                    break;
                case 2:
                    Reload();
                    break;
            }
        }

        public override string ToString()
        {
            return gameObject.name;
        }

        public void TakeDamage(float damage)
        {
            var previousHealth = Health;
            var newHealth = Health - damage;
            newHealth = Mathf.Clamp(newHealth, 0, MaxHealth);
            Health = newHealth;

            animator.SetFloat(AnimationParameters.DamageCount, AnimationParameters.GetRandomDamage());
            animator.SetTrigger(AnimationParameters.Damage);

            onDamaged.Invoke();
            _onEvent.OnNext(new IAgentEvent.OnDamaged(this, previousHealth, Health));

            if (Health <= 0 && previousHealth > 0)
            {
                OnKilled();
            }
        }

        private void OnKilled()
        {
            onKilled.Invoke();
            _onEvent.OnNext(new IAgentEvent.OnKilled(this));
        }

        public void TakeHealing(float healing)
        {
            var previousHealth = Health;
            var newHealth = Health + healing;
            newHealth = Mathf.Clamp(newHealth, 0, MaxHealth);
            Health = newHealth;

            onHealed.Invoke();
            _onEvent.OnNext(new IAgentEvent.OnHealed(this, previousHealth, Health));
        }

        public void TakeAmmo(int ammo)
        {
            Weapon.TakeAmmo(ammo);
        }

        public void Shoot()
        {
            Weapon.Shoot();
        }

        public void Reload()
        {
            Weapon.Reload();
        }

        public void Win()
        {
            animator.SetTrigger(AnimationParameters.Win);
            onWin.Invoke();
        }

        public void Lose()
        {
            animator.SetTrigger(AnimationParameters.Lose);
            onLose.Invoke();
        }
    }
}
