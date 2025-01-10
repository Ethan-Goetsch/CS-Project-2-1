using FPSSystem.TrainingSystem;
using FPSSystem.UISystem.Component;
using HUD;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.UISystem.HUD
{
    public class FPSTrainingHUD : MonoBehaviour
    {
        public struct Args
        {
            public TrainingManager Manager;
            public int EnvironmentCount;
        }

        [Required, SerializeField]
        private RectTransform buttonPanel;

        [Required, SerializeField]
        private GameObject environmentButtonPrefab;

        private Args _args;

        public void Initialize(Args args)
        {
            _args = args;

            for (var i = 0; i < _args.EnvironmentCount; i++)
            {
                var environmentInstance = i;
                var button = Instantiate(environmentButtonPrefab, buttonPanel).GetComponent<ButtonComponent>();
                button.Initialize($"Env {i + 1}");
                button.OnClicked
                    .Subscribe(evt => args.Manager.SetTrainingEnvironment(environmentInstance))
                    .AddTo(this);
            }
            
        }
    }
}
