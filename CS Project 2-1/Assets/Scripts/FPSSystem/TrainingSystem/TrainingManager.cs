using System.Collections.Generic;
using System.Linq;
using FPSSystem.UISystem.HUD;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.TrainingSystem
{
    public class TrainingManager : MonoBehaviour
    {
        [Required, SerializeField]
        private FPSTrainingHUD hud;

        private List<FPSEnvironmentController> _controllers;

        private void Awake()
        {
            _controllers = GetComponentsInChildren<FPSEnvironmentController>(true).ToList();
            _controllers.ForEach(c => c.Initialize());
            hud.Initialize(new FPSTrainingHUD.Args
            {
                Manager = this,
                EnvironmentCount = _controllers.Count
            });
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
