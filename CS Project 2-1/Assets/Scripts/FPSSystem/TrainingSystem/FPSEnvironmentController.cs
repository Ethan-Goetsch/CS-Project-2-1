using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FPSSystem.AgentSystem;
using FPSSystem.AmmoSystem;
using FPSSystem.HealthSystem;
using FPSSystem.MovementSystem;
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

        [Required, SerializeField]
        private AmmoPickupGlobalController ammoPickUpController;

        [TitleGroup("Environment")]
        [Required, SerializeField]
        private MeshRenderer environmentRenderer;

        [Required, SerializeField]
        private Material winMaterial, loseMaterial;

        [TitleGroup("Info")]
        [Required, SerializeField]
        private GameObject environmentCamera;

        [Required, SerializeField]
        private FPSEnvironmentHUD environmentHUD;

        [TitleGroup("Angle")]
        [Required, SerializeField]
        [Range(0, 30)]
        [Tooltip("The angle in which the agent is rewarded for facing the other agent")]
        public float rewardAngle = 10;



        private TrainingManager _trainingManager;
        private List<FPSAgent> _agents;

        public bool IsFocused { get; private set; }
        private Reward CurrentReward => _trainingManager.CurrentReward;

        public void Initialize(TrainingManager trainingManager)
        {
            _trainingManager = trainingManager;
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

                agent.MovementController
                    .OnEvent<IMovementEvent.AgentMoveEvent>()
                    .Subscribe(OnAgentMove)
                    .AddTo(this);
                agent.MovementController
                    .OnEvent<IMovementEvent.AgentRotateEvent>()
                    .Subscribe(OnAgentRotate)
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
                    .OnEvent<IWeaponEvent.OnAmmoRestoredEvent>()
                    .Subscribe(evt => OnAgentAmmoRestored(agent, evt));
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
            ammoPickUpController.Initialize();
        }

        private IEnumerator EndEpisode(FPSAgent winner, FPSAgent loser)
        {
            winner.Win();
            loser.Lose();

            yield return new WaitForSeconds(2f);

            environmentRenderer.material = winner == agent1 ? winMaterial : loseMaterial;

            winner.EndEpisode();
            loser.EndEpisode();
        }

        private void OnAgentShoot(FPSAgent agent, IWeaponEvent.OnShootEvent evt)
        {
            agent.AddReward(CurrentReward.ShootReward);
        }

        private void OnAgentDamaged(IAgentEvent.OnDamaged evt)
        {
            var (damagedAgent, damageAgent) = GetAgentsFromEvents(evt.Agent);

            damagedAgent.AddReward(CurrentReward.DamagedReward);
            damageAgent.AddReward(CurrentReward.DamageReward);
        }

        private void OnAgentHealed(IAgentEvent.OnHealed evt)
        {
            // var rewardMultiplier = 1f * (evt.Amount / evt.Agent.MaxHealth);
            var (healedAgent, otherAgent) = GetAgentsFromEvents(evt.Agent);

            healedAgent.AddReward(CurrentReward.HealedReward);
        }

        private void OnAgentKilled(IAgentEvent.OnKilled evt)
        {
            var (killedAgent, killAgent) = GetAgentsFromEvents(evt.Agent);
            killedAgent.AddReward(CurrentReward.KilledReward);
            killAgent.AddReward(CurrentReward.KillReward);

            StartCoroutine(EndEpisode(killAgent, killedAgent));
        }

        private void OnAgentMove(IMovementEvent.AgentMoveEvent evt)
        {
            var (movedAgent, towardsAgent) = GetAgentsFromEvents(evt.Agent);

            if(Vector3.Angle(movedAgent.transform.forward, towardsAgent.transform.position - movedAgent.transform.position) < rewardAngle)
            {
                movedAgent.AddReward(CurrentReward.FacingReward);
            }
        }

        private void OnAgentRotate(IMovementEvent.AgentRotateEvent evt)
        {
            var (movedAgent, towardsAgent) = GetAgentsFromEvents(evt.Agent);

            if(Vector3.Angle(movedAgent.transform.forward, towardsAgent.transform.position - movedAgent.transform.position) < rewardAngle)
            {
                movedAgent.AddReward(CurrentReward.FacingReward);
            }
        }

        private void OnAgentReload(FPSAgent agent, IWeaponEvent.OnReloadEvent evt)
        {
            // var reward = evt.AmountReloaded >= evt.MaxAmmo / 2
            //     ? 1f * evt.AmountReloaded
            //     : -1f * ((evt.MaxAmmo / 2) - evt.AmountReloaded);
            // agent.AddReward(reward);

            agent.AddReward(evt.AmountReloaded > 1
                ? CurrentReward.GoodReloadReward
                : CurrentReward.BadReloadReward);
        }

        private void OnAgentAmmoRestored(FPSAgent agent, IWeaponEvent.OnAmmoRestoredEvent evt)
        {
            agent.AddReward(CurrentReward.AmmoRestoredReward);
        }

        private void OnAgentMissed(FPSAgent agent, IWeaponEvent.OnMissHitEvent evt)
        {
            agent.AddReward(CurrentReward.BulletMissReward);
        }

        private void OnAgentHitEnvironment(FPSAgent agent, IWeaponEvent.OnEnvironmentHitEvent evt)
        {
            agent.AddReward(CurrentReward.BulletMissReward);
        }

        private (FPSAgent primary, FPSAgent secondary) GetAgentsFromEvents(FPSAgent agent) => agent == agent1 ? (agent1, agent2) : (agent2, agent1);
    }
}
