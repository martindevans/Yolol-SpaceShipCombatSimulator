﻿using System;
using System.Numerics;
using Myre.Entities;
using Ninject;
using ShipCombatCore.Simulation.Behaviours;
using ShipCombatCore.Simulation.Behaviours.Recording;
using Yolol.Grammar.AST;

namespace ShipCombatCore.Simulation.Entities
{
    public class MissileEntity
        : EntityDescription
    {
        public MissileEntity(IKernel kernel)
            : base(kernel)
        {
            AddProperty(PropertyNames.UniqueName);
            AddProperty(PropertyNames.EntityType, EntityType.Missile);
            AddBehaviour<TeamMember>();

            // Physics
            AddBehaviour<Integrate>();
            AddBehaviour<ExtraMass>();
            AddBehaviour<RadarDetectable>();

            // Collision
            AddBehaviour<SphereColliderSecondary>();
            AddProperty(PropertyNames.SphereRadius, 1);

            // Explode after taking any significant damage
            AddBehaviour<CosmicRadiationDamage>();
            AddBehaviour<DamageInstantlyKills>();
            AddBehaviour<ExplodeOnDeath>();
            AddBehaviour<SelfDestructDevice>();

            // Yolol Devices
            AddBehaviour<YololHost>();
            AddBehaviour<ClockDevice>();
            AddBehaviour<GyroscopeDevice>();
            AddBehaviour<PositioningDevice>();
            AddBehaviour<FuelTank>();
            AddBehaviour<Engine>();
            AddBehaviour<MomentumWheels>();
            AddBehaviour<FixedActiveRadarScannerDevice>();
            AddBehaviour<Radio>();

            // Recording
            AddBehaviour<RecorderMaster>();
            AddBehaviour<RecordTransformPosition>();
            AddBehaviour<RecordTransformOrientation>();
            AddBehaviour<RecordActualThrottle>();
        }

        public Entity Create(uint team, Vector3 position, Vector3 velocity, Quaternion orientation, Vector3 angularVelocity, Program program)
        {
            var e = base.Create();

            e.GetProperty(PropertyNames.UniqueName)!.Value = Guid.NewGuid().ToString();
            e.GetProperty(PropertyNames.TeamOwner)!.Value = team;

            e.GetProperty(PropertyNames.Position)!.Value = position;
            e.GetProperty(PropertyNames.Velocity)!.Value = velocity;
            e.GetProperty(PropertyNames.ExtraMass)!.Value = 100f;

            e.GetProperty(PropertyNames.Orientation)!.Value = orientation;
            e.GetProperty(PropertyNames.AngularVelocity)!.Value = angularVelocity;

            e.GetProperty(PropertyNames.YololContext)!.Value = new YololContext(new[] { program });

            e.GetProperty(PropertyNames.FuelLitersInTank)!.Value = 15;
            e.GetProperty(PropertyNames.FuelConsumptionRate)!.Value = Constants.MissileFuelConsumption;
            e.GetProperty(PropertyNames.MaxEngineForce)!.Value = Constants.MissileThrust;

            return e;
        }
    }
}