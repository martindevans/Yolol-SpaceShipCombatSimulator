﻿using System;
using System.Numerics;
using Myre.Entities;
using Ninject;
using ShipCombatCore.Simulation.Behaviours;
using ShipCombatCore.Simulation.Behaviours.Recording;

namespace ShipCombatCore.Simulation.Entities
{
    public class ShellEntity
        : EntityDescription
    {
        public ShellEntity(IKernel kernel)
            : base(kernel)
        {
            AddProperty(PropertyNames.EntityType, EntityType.Shell);
            AddBehaviour<TeamMember>();
            
            // Explode after timeout or when taking any damage
            AddBehaviour<LifetimeLimit>();
            AddBehaviour<DamageInstantlyKills>();
            AddBehaviour<ExplodeOnDeath>();
            AddBehaviour<ClockDevice>();

            // Physics
            AddBehaviour<Integrate>();

            // Collision
            AddBehaviour<SphereColliderSecondary>();
            AddProperty(PropertyNames.SphereRadius, 1);

            // Radar
            AddBehaviour<RadarDetectable>();

            // Recording
            AddBehaviour<RecorderMaster>();
            AddBehaviour<RecordTransformPosition>();
        }

        public Entity Create(float fuse, uint team, Vector3 position, Vector3 velocity)
        {
            var e = base.Create();

            e.GetProperty(PropertyNames.UniqueName)!.Value = Guid.NewGuid().ToString();

            e.GetProperty(PropertyNames.TeamOwner)!.Value = team;

            e.GetProperty(PropertyNames.Position)!.Value = position;
            e.GetProperty(PropertyNames.Velocity)!.Value = velocity;
            e.GetProperty(PropertyNames.Lifetime)!.Value = fuse;

            return e;
        }
    }
}