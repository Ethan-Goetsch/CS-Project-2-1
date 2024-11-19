using FPSSystem.AgentSystem;
using FPSSystem.TrainingSystem;
using FPSSystem.UISystem.Component;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace FPSSystem.UISystem.HUD
{
    public class FPSAgentHUD : MonoBehaviour
    {
        public struct Args
        {
            public FPSEnvironmentController Controller;
            public FPSAgent Agent;
        }

        [Required, SerializeField]
        private HealthBar healthBar;

        [Required, SerializeField]
        private TextMeshProUGUI infoLabel;

        private Args _args;

        public void Initialize(Args args)
        {
            _args = args;

            args.Agent
                .OnEntityEvent<OnHealthChanged>()
                .Subscribe(evt => healthBar.SetValue(evt.New))
                .AddTo(this);
            args.Agent
                .OnEntityEvent<IAgentEvent>()
                .Subscribe(evt => UpdateInfoLabel())
                .AddTo(this);

            healthBar.Initialize(args.Agent.MaxHealth, args.Agent.Health);
            UpdateInfoLabel();
        }

        private void UpdateInfoLabel()
        {
            infoLabel.text = $"Health: {_args.Agent.Health} \nAmmo: {_args.Agent.Ammo}";
        }
    }
}
