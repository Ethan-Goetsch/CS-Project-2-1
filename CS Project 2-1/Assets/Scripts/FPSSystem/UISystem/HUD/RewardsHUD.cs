using UnityEngine;
using Sirenix.OdinInspector;
using FPSSystem.TrainingSystem;
using System.Collections;
using FPSSystem.UISystem.Component;
using System;
using Cysharp.Threading.Tasks;
using R3;

namespace HUD
{
    public class RewardsHUD : MonoBehaviour
    {

        [Required, SerializeField]
        private RewardHandler rewardHandler;


        [Required, SerializeField]
        private RectTransform buttonPanel;

        [Required, SerializeField]
        private GameObject environmentButtonPrefab;

        public void Initialize()
        {

            ArrayList paths = rewardHandler.GetAvailable();
            string finalPath = rewardHandler.FinalRewardPath();
            Debug.Log(finalPath);

            foreach (string path in paths)
            {
                Debug.Log($"Registering button for {path}");
                var button = Instantiate(environmentButtonPrefab, buttonPanel).GetComponent<ButtonComponent>();
                button.Initialize(path.Replace(finalPath, "").Replace(".json", ""));
                button.OnClicked.Subscribe(evt => rewardHandler.ReadRewardSetup(path)).AddTo(this);
            }

        }


    }
}