using JetBrains.Annotations;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FPSSystem.UISystem.Component
{
    [RequireComponent(typeof(Button))]
    public class ButtonComponent : MonoBehaviour
    {
        private Button _button;

        [SerializeField]
        private TextMeshProUGUI label;

        public Observable<Unit> OnClicked => _button.onClick.AsObservable();

        public void Initialize([CanBeNull] string text = null)
        {
            _button = GetComponent<Button>();

            if (label != null && text != null)
            {
                label.text = text;
            }
        }
    }
}
