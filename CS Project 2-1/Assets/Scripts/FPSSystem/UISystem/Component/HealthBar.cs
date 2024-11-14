using UnityEngine;
using UnityEngine.UI;

namespace FPSSystem.UISystem.Component
{
    [RequireComponent(typeof(Slider))]
    public class HealthBar : MonoBehaviour
    {
        private Slider _slider;

        public void Initialize(float initialMaxValue, float initialValue)
        {
            _slider = GetComponent<Slider>();

            SetMaxValue(initialMaxValue);
            SetValue(initialValue);
        }

        public void SetValue(float value)
        {
            _slider.value = value;
        }

        public void SetMaxValue(float value)
        {
            _slider.maxValue = value;
        }
    }
}
