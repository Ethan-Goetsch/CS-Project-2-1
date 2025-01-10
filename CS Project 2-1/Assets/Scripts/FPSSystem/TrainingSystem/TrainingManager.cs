using System.Collections.Generic;
using System.Linq;
using FPSSystem.UISystem.HUD;
using Sirenix.OdinInspector;
using UnityEngine;
using FPSSystem.UISystem;
using HUD;

namespace FPSSystem.TrainingSystem
{
    public class TrainingManager : MonoBehaviour
    {
        //[SerializeField]
        private Reward currentReward;

        [Required, SerializeField]
        private RewardHandler rewardHandler;

        [Required, SerializeField]
        private FPSTrainingHUD hud;

        [Required, SerializeField]
        private RewardsHUD rewardsHUD;

        private List<FPSEnvironmentController> _controllers;

        public Reward CurrentReward(){
            
            return rewardHandler.CurrentReward;
        }

        private void Awake()
        {
            _controllers = GetComponentsInChildren<FPSEnvironmentController>(true).ToList();
            _controllers.ForEach(c => c.Initialize(this));

            hud.Initialize(new FPSTrainingHUD.Args
            {
                Manager = this,
                EnvironmentCount = _controllers.Count
            });

            rewardsHUD.Initialize();

            currentReward = rewardHandler.CurrentReward;
        }

        private void Start()
        {
            if (_controllers.Count > 0)
            {
                SetTrainingEnvironment(0);
            }
        }

        [Button]
        public void SetTrainingEnvironment(int index)
        {
            for (var i = 0; i < _controllers.Count; i++)
            {
                if (i == index)
                {
                    _controllers[i].Focus();
                }
                else
                {
                    _controllers[i].Unfocus();
                }
            }
        }
    }

}
