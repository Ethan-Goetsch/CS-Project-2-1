using FPSSystem.DamageSystem;
using FPSSystem.HealthSystem;
using R3;
using Sirenix.OdinInspector;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace FPSSystem.AgentSystem
{
    public abstract class FPSAgent : Agent, IDamagable, IHealable
    {
        protected readonly Subject<IAgentEvent> OnEvent = new();

        [TitleGroup("Controllers")]
        protected IFPSController FPSController;

        [TitleGroup("Stats")]
        [SerializeField]
        private float maxHealth = 100;

        public Vector3 Position => transform.position;
        public float MaxHealth => maxHealth;

        [TitleGroup("Runtime")]
        [ShowInInspector, ReadOnly]
        public float Health { get; private set; }

        public Observable<T> OnEntityEvent<T>() where T : IAgentEvent => OnEvent.OfType<IAgentEvent, T>();

        public override void Initialize()
        {
            FPSController = GetComponentInParent<IFPSController>();
        }

        public override void OnEpisodeBegin()
        {
            var position = FPSController.GetStartingPosition(this);
            var rotation = FPSController.GetStartingRotation(this);
            transform.SetPositionAndRotation(position, rotation);

            Health = MaxHealth;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(transform.localPosition);
            sensor.AddObservation(transform.localRotation);
        }

        public override string ToString()
        {
            return gameObject.name;
        }

        public void TakeDamage(float damage)
        {
            var previous = Health;
            Health -= Mathf.Clamp(damage, 0, MaxHealth);
            OnEvent.OnNext(new OnDamaged(this, previous, Health));

            if (Health == 0)
            {
                OnEvent.OnNext(new OnKilled(this));
            }
        }

        public void TakeHealing(float healing)
        {
            var previous = Health;
            Health += Mathf.Clamp(healing, 0, MaxHealth);
            OnEvent.OnNext(new OnHealed(this, previous, Health));
        }
    }
}
