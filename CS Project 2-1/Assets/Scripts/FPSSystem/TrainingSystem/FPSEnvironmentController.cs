using System.Collections.Generic;
using FPSSystem.AgentSystem;
using FPSSystem.HealthSystem;
using FPSSystem.UISystem.HUD;
using FPSSystem.WeaponSystem;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.TrainingSystem
{
    public class FPSEnvironmentController : MonoBehaviour
    {
        [TitleGroup("Agents")]
        [Required, SerializeField]
        private FPSAgent agent1, agent2;

        [TitleGroup("Transforms")]
        [Required, SerializeField]
        private Transform agent1Start, agent2Start;

        [TitleGroup("Pickups")]
        [Required, SerializeField]
        private HealthPickupGlobalController healthPickUpController;

        [TitleGroup("Info")]
        [Required, SerializeField]
        private GameObject environmentCamera;

        [Required, SerializeField]
        private FPSEnvironmentHUD environmentHUD;

        private List<FPSAgent> _agents;

        public bool IsFocused { get; private set; }

        public void Initialize()
        {
            _agents = new List<FPSAgent>
            {
                agent1,
                agent2
            };

            agent1.OnEvent<IAgentEvent.OnEpisodeBegin>()
                .Subscribe(evt => BeginEpisode())
                .AddTo(this);

            foreach (var agent in _agents)
            {
                agent.OnEvent<IAgentEvent.OnDamaged>()
                    .Subscribe(OnAgentDamaged)
                    .AddTo(this);
                agent.OnEvent<IAgentEvent.OnHealed>()
                    .Subscribe(OnAgentHealed)
                    .AddTo(this);
                agent.OnEvent<IAgentEvent.OnKilled>()
                    .Subscribe(OnAgentKilled)
                    .AddTo(this);
                agent.Weapon
                    .OnEvent<IWeaponEvent.OnShootEvent>()
                    .Subscribe(evt => OnAgentShoot(agent, evt))
                    .AddTo(this);
                agent.Weapon
                    .OnEvent<IWeaponEvent.OnReloadEvent>()
                    .Subscribe(evt => OnAgentReload(agent, evt))
                    .AddTo(this);
                agent.Weapon
                    .OnEvent<IWeaponEvent.OnMissHitEvent>()
                    .Subscribe(evt => OnAgentMissed(agent, evt))
                    .AddTo(this);
                agent.Weapon
                    .OnEvent<IWeaponEvent.OnEnvironmentHitEvent>()
                    .Subscribe(evt => OnAgentHitEnvironment(agent, evt))
                    .AddTo(this);
            }

            environmentHUD.Initialize(new FPSEnvironmentHUD.Args
            {
                Controller = this,
                Agent1 = agent1,
                Agent2 = agent2,
            });
        }

        public Vector3 GetStartingPosition(FPSAgent agent)
        {
            return agent == agent1 ? agent1Start.position : agent2Start.position;
        }

        public Quaternion GetStartingRotation(FPSAgent agent)
        {
            return agent == agent1 ? agent1Start.rotation : agent2Start.rotation;
        }

        public void Focus()
        {
            IsFocused = true;
            environmentCamera.SetActive(true);
            environmentHUD.gameObject.SetActive(true);
        }

        public void Unfocus()
        {
            IsFocused = false;
            environmentCamera.SetActive(false);
            environmentHUD.gameObject.SetActive(false);
        }

        private void BeginEpisode()
        {
            healthPickUpController.Initialize();
        }

        private void EndEpisode()
        {
            agent1.EndEpisode();
            agent2.EndEpisode();
        }

        private void OnAgentShoot(FPSAgent agent, IWeaponEvent.OnShootEvent evt)
        {
            agent.AddReward(0.1f);
        }

        private void OnAgentDamaged(IAgentEvent.OnDamaged evt)
        {
            var (primary, secondary) = GetAgentsFromEvents(evt.Agent);
            secondary.AddReward(0.5f);
        }

        private void OnAgentHealed(IAgentEvent.OnHealed evt)
        {
            var reward = 1f * (evt.Amount / evt.Agent.MaxHealth);
            var (primary, secondary) = GetAgentsFromEvents(evt.Agent);
            primary.AddReward(reward);
        }

        private void OnAgentKilled(IAgentEvent.OnKilled evt)
        {
            var (primary, secondary) = GetAgentsFromEvents(evt.Agent);
            secondary.AddReward(1f);
            primary.AddReward(-1f);

            EndEpisode();
        }

        private void OnAgentReload(FPSAgent agent, IWeaponEvent.OnReloadEvent evt)
        {
            // var reward = evt.AmountReloaded >= evt.MaxAmmo / 2
            //     ? 1f * evt.AmountReloaded
            //     : -1f * ((evt.MaxAmmo / 2) - evt.AmountReloaded);
            // agent.AddReward(reward);

            if (evt.AmountReloaded > 1)
            {
                agent.AddReward(0.1f);
            }
            else
            {
                agent.AddReward(-0.1f);
            }
        }

        private void OnAgentMissed(FPSAgent agent, IWeaponEvent.OnMissHitEvent evt)
        {
            agent.AddReward(-0.05f);
        }

        private void OnAgentHitEnvironment(FPSAgent agent, IWeaponEvent.OnEnvironmentHitEvent evt)
        {
            agent.AddReward(-0.05f);
        }

        private (FPSAgent primary, FPSAgent secondary) GetAgentsFromEvents(FPSAgent agent) => agent == agent1 ? (agent1, agent2) : (agent2, agent1);
    }
}
