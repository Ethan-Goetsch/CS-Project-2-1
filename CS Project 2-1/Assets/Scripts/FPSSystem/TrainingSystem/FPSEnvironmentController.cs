using System.Collections.Generic;
using FPSSystem.AgentSystem;
using FPSSystem.UISystem.HUD;
using FPSSystem.WeaponSystem;
using R3;
using R3.Triggers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.TrainingSystem
{
    public class FPSEnvironmentController : MonoBehaviour
    {
        [TitleGroup("Agents")]
        [Required, SerializeField]
        private FPSAgent agent1, agent2;

        [TitleGroup("Bounds")]
        [Required, SerializeField]
        private Collider bounds;

        [TitleGroup("Transforms")]
        [Required, SerializeField]
        private Transform agent1Start, agent2Start;

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
                    .OnEvent<IWeaponEvent.OnMissHitEvent>()
                    .Subscribe(evt => OnAgentMissed(agent, evt))
                    .AddTo(this);
            }

            environmentHUD.Initialize(new FPSEnvironmentHUD.Args
            {
                Controller = this,
                Agent1 = agent1,
                Agent2 = agent2,
            });

            bounds.OnTriggerExitAsObservable()
                .Subscribe(OnExitBound)
                .AddTo(this);
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
            gameObject.SetActive(true);
        }

        public void Unfocus()
        {
            IsFocused = false;
            gameObject.SetActive(false);
        }

        private void OnAgentDamaged(IAgentEvent.OnDamaged evt)
        {
            var (primary, secondary) = GetAgentsFromEvents(evt.Agent);
            primary.AddReward(0.5f);
            secondary.AddReward(-0.1f);
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
            primary.AddReward(1f);
            secondary.AddReward(-1f);

            primary.EndEpisode();
            secondary.EndEpisode();
        }

        private void OnAgentMissed(FPSAgent agent, IWeaponEvent.OnMissHitEvent evt)
        {
            agent.AddReward(-0.1f);
        }

        private void OnExitBound(Collider evt)
        {
            if (!evt.gameObject.TryGetComponent<FPSAgent>(out var agent)) return;
            var (primary, secondary) = GetAgentsFromEvents(agent);
            primary.AddReward(-1f);

            primary.EndEpisode();
            secondary.EndEpisode();
        }

        private (FPSAgent primary, FPSAgent secondary) GetAgentsFromEvents(FPSAgent agent) => agent == agent1 ? (agent1, agent2) : (agent2, agent1);
    }
}
