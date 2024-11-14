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

        public void Initialize(Args args)
        {
            healthBar.Initialize(args.Agent.MaxHealth, args.Agent.Health);
            args.Agent
                .OnEntityEvent<OnHealthChanged>()
                .Subscribe(evt => healthBar.SetValue(evt.New))
                .AddTo(this);
        }
    }
}
