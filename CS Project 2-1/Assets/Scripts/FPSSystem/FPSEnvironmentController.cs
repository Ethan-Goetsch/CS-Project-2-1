using System.Collections.Generic;
using FPSSystem.AgentSystem;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem
{
    public class FPSEnvironmentController : MonoBehaviour, IFPSController
    {
        [TitleGroup("Agents")]
        [Required, SerializeField]
        private FPSAgent agent1, agent2;

        [TitleGroup("Transforms")]
        [Required, SerializeField]
        private Transform agent1Start, agent2Start;

        private List<FPSAgent> agents;

        private void Awake()
        {
            agents = new List<FPSAgent>
            {
                agent1,
                agent2
            };

            foreach (var agent in agents)
            {
                agent.OnEntityEvent<OnDamaged>()
                    .Subscribe(OnAgentDamaged)
                    .AddTo(this);
                agent.OnEntityEvent<OnHealed>()
                    .Subscribe(OnAgentHealed)
                    .AddTo(this);
                agent.OnEntityEvent<OnKilled>()
                    .Subscribe(OnAgentKilled)
                    .AddTo(this);
            }
        }

        public Vector3 GetStartingPosition(FPSAgent agent)
        {
            return agent == agent1 ? agent1Start.position : agent2Start.position;
        }

        public Quaternion GetStartingRotation(FPSAgent agent)
        {
            return agent == agent1 ? agent1Start.rotation : agent2Start.rotation;
        }

        private void OnAgentDamaged(OnDamaged evt)
        {
            var (primary, secondary) = GetAgentsFromEvents(evt.Agent);
            primary.AddReward(1f);
            secondary.AddReward(-1f);
        }

        private void OnAgentHealed(OnHealed evt)
        {
            var reward = 1f * (evt.Agent.MaxHealth / evt.Amount);
            var (primary, secondary) = GetAgentsFromEvents(evt.Agent);
            primary.AddReward(reward);
        }

        private void OnAgentKilled(OnKilled evt)
        {
            var (primary, secondary) = GetAgentsFromEvents(evt.Agent);
            primary.AddReward(10f);
            secondary.AddReward(-10f);

            primary.EndEpisode();
            secondary.EndEpisode();
        }

        private (FPSAgent primary, FPSAgent secondary) GetAgentsFromEvents(FPSAgent agent) => agent == agent1 ? (agent1, agent2) : (agent2, agent1);
    }
}
