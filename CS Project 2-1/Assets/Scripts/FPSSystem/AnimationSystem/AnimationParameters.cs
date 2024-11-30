using UnityEngine;

namespace FPSSystem.AnimationSystem
{
    public static class AnimationParameters
    {
        public static readonly int HorizontalDirection = Animator.StringToHash(nameof(HorizontalDirection));
        public static readonly int VerticalDirection = Animator.StringToHash(nameof(VerticalDirection));

        public static readonly int Speed = Animator.StringToHash(nameof(Speed));
        public static readonly int ShootCount = Animator.StringToHash(nameof(ShootCount));
        public static readonly int DamageCount = Animator.StringToHash(nameof(DamageCount));

        public static readonly int IsMoving = Animator.StringToHash(nameof(IsMoving));
        public static readonly int Lose = Animator.StringToHash(nameof(Lose));
        public static readonly int Win = Animator.StringToHash(nameof(Win));
        public static readonly int Shoot = Animator.StringToHash(nameof(Shoot));
        public static readonly int Reload = Animator.StringToHash(nameof(Reload));
        public static readonly int Damage = Animator.StringToHash(nameof(Damage));

        public static int GetRandomShoot() => Random.Range(0, 3);
        public static int GetRandomDamage() => Random.Range(0, 4);
    }
}
