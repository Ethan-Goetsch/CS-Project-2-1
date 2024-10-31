using System;
using System.Collections.Generic;
using FPSSystem.AgentSystem;
using FPSSystem.DamageSystem;
using FPSSystem.ProjectileSystem;
using UnityEngine;

namespace FPSSystem.WeaponSystem
{
    public struct ProjectileArgs
    {
        public FPSAgent Owner;
        public Weapon Weapon;

        public Vector3 Position;
        public Quaternion Rotation;

        public List<Collider> OwnerColliders;

        public Action<ProjectileController, IDamagable> OnDamagableHit;
        public Action<ProjectileController, GameObject> OnEnvironmentHit;
        public Action<ProjectileController> OnExpire;
    }
}
