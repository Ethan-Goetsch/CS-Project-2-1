using System.Collections.Generic;
using FPSSystem.AgentSystem;
using FPSSystem.UISystem.HUD;
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

        [TitleGroup("Info")]
        [Required, SerializeField]
        private GameObject environmentCamera;

        [Required, SerializeField]
        private FPSEnvironmentHUD environmentHUD;

        private List<FPSAgent> agents;

        public bool IsFocused { get; private set; }

        public void Initialize()
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
            gameObject.SetActive(true);
        }

        public void Unfocus()
        {
            IsFocused = false;
            gameObject.SetActive(false);
        }

        private void OnAgentDamaged(OnDamaged evt)
        {
            var (primary, secondary) = GetAgentsFromEvents(evt.Agent);
            primary.AddReward(0.1f);
            secondary.AddReward(-0.1f);
        }

        private void OnAgentHealed(OnHealed evt)
        {
            var reward = 1f * (evt.Amount / evt.Agent.MaxHealth);
            var (primary, secondary) = GetAgentsFromEvents(evt.Agent);
            primary.AddReward(reward);
            secondary.AddReward(-reward);
        }

        private void OnAgentKilled(OnKilled evt)
        {
            var (primary, secondary) = GetAgentsFromEvents(evt.Agent);
            primary.AddReward(1f);
            secondary.AddReward(-1f);

            primary.EndEpisode();
            secondary.EndEpisode();
        }

        private (FPSAgent primary, FPSAgent secondary) GetAgentsFromEvents(FPSAgent agent) => agent == agent1 ? (agent1, agent2) : (agent2, agent1);
    }
}
