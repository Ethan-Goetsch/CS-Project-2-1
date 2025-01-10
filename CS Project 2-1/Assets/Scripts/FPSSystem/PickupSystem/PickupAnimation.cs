using DG.Tweening;
using Shapes;
using UnityEngine;

namespace FPSSystem.PickupSystem
{
    [RequireComponent(typeof(Disc))]
    public class PickupAnimation : MonoBehaviour
    {
        [SerializeField]
        private float duration = 1f;

        [SerializeField]
        private Ease ease = Ease.OutBounce;

        private Disc _disc;

        private void Awake()
        {
            _disc = GetComponent<Disc>();
        }

        private void Start()
        {
            DOVirtual.Float(_disc.Radius, 0f, duration, value => _disc.Radius = value)
                .SetEase(ease)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}
