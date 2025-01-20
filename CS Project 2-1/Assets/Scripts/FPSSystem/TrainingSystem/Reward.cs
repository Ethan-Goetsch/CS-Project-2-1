using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FPSSystem.TrainingSystem
{
    [Serializable]
    public class Reward
    {
        [TitleGroup("Health")]
        [Range(-1f, 1f)]
        [Tooltip("Reward given to an agent for being damaged")]
        public float DamagedReward = 0f;

        [Range(-1f, 1f)]
        [Tooltip("Reward given to an agent for damaging another agent")]
        public float DamageReward = 0.5f;

        [Range(-1f, 1f)]
        [Tooltip("Reward given to an agent for picking up a Health Pickup")]
        public float HealedReward = 0.5f;

        [TitleGroup("Shooting")]
        [Range(-1f, 1f)]
        [Tooltip("Reward given to an agent for shooting")]
        public float ShootReward = 0.1f;

        [Range(-1f, 1f)]
        [Tooltip("Reward given to an agent for missing a bullet. Either hitting the environment or not hitting anything at all")]
        public float BulletMissReward = -0.05f;

        [TitleGroup("Reloading")]
        [Range(-1f, 1f)]
        [Tooltip("Reward given to an agent for reloading more than 1 bullet")]
        public float GoodReloadReward = 0.1f;

        [Range(-1f, 1f)]
        [Tooltip("Reward given to an agent for reloading only 1 bullet")]
        public float BadReloadReward = -0.1f;

        [Range(-1f, 1f)]
        [Tooltip("Reward given to an agent for picking up an Ammo Pickup")]
        public float AmmoRestoredReward = 0.1f;

        [TitleGroup("Movement")]
        [Range(-1f, 1f)]
        [Tooltip("Reward given to an agent for facing the opponent")]
        public float FacingReward = 0.1f;

        public readonly float KillReward = 1f;
        public readonly float KilledReward = -1f;
    }
}
