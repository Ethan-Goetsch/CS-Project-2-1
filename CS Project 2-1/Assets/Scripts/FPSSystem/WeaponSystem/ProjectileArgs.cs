using System;
using FPSSystem.AgentSystem;
using FPSSystem.DamageSystem;
using UnityEngine;

namespace FPSSystem.WeaponSystem
{
    public struct ProjectileArgs
    {
        public FPSAgent Owner;
        public Weapon Weapon;

        public Vector3 Position;
        public Quaternion Rotation;

        public Action<ProjectileController, IDamagable> OnDamagableHit;
        public Action<ProjectileController, GameObject> OnEnvironmentHit;
        public Action<ProjectileController> OnExpire;
    }
}
