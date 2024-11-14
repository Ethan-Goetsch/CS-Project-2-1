using FPSSystem.AgentSystem;
using FPSSystem.TrainingSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.UISystem.HUD
{
    public class FPSEnvironmentHUD : MonoBehaviour
    {
        public struct Args
        {
            public FPSEnvironmentController Controller;
            public FPSAgent Agent1, Agent2;
        }

        [Required, SerializeField]
        private FPSAgentHUD agent1HUD, agent2HUD;

        public void Initialize(Args args)
        {
            agent1HUD.Initialize(new FPSAgentHUD.Args
            {
                Controller = args.Controller,
                Agent = args.Agent1,
            });

            agent2HUD.Initialize(new FPSAgentHUD.Args
            {
                Controller = args.Controller,
                Agent = args.Agent2,
            });
        }
    }
}
