using System.Collections.Generic;
using System.Numerics;
using Myre.Entities;
using ShipCombatCore.Simulation.Behaviours;
using ShipCombatCore.Simulation.Behaviours.Recording;

namespace ShipCombatCore.Simulation.Entities
{
    public class SpaceShipEntity
        : EntityDescription
    {
        public SpaceShipEntity()
        {
            AddProperty(PropertyNames.UniqueName);
            AddProperty(PropertyNames.EntityType, EntityType.SpaceBattleShip);
            AddBehaviour<TeamMember>();

            // Physics
            AddBehaviour<Integrate>();
            AddBehaviour<ExtraMass>();
            AddBehaviour<RadarDetectable>();

            // Collision
            AddBehaviour<SphereColliderSecondary>();
            AddProperty(PropertyNames.SphereRadius, 50);

            // Damage
            AddBehaviour<DamageRemovesFuel>();
            AddBehaviour<OutOfFuelDestroy>();
            AddBehaviour<CosmicRadiationDamage>();
            AddBehaviour<HulkOnDeath>();

            // Yolol Devices
            AddBehaviour<YololHost>();
            AddBehaviour<ClockDevice>();
            AddBehaviour<GyroscopeDevice>();
            AddBehaviour<AccelerometerDevice>();
            AddBehaviour<FuelTank>();
            AddBehaviour<Engine>();
            AddBehaviour<MomentumWheels>();
            AddBehaviour<Turrets>();
            AddBehaviour<MissileLauncher>();
            AddBehaviour<ActiveRadarScannerDevice>();
            AddBehaviour<Radio>();

            // Recording
            AddBehaviour<RecorderMaster>();
            AddBehaviour<RecordTransformPosition>();
            AddBehaviour<RecordTransformOrientation>();
            AddBehaviour<RecordActualThrottle>();
            AddBehaviour<RecordFuelLitersInTank>();
            AddBehaviour<RecordRunningLight>();
            AddBehaviour<RecordCosmicRadiation>();
        }

        public Entity Create(string name, uint team, Vector3 position, Vector3 velocity, Quaternion orientation, Vector3 angularVelocity, IReadOnlyList<Yolol.Grammar.AST.Program> programs)
        {
            var e = base.Create();

            e.GetProperty(PropertyNames.UniqueName)!.Value = name;
            e.GetProperty(PropertyNames.TeamOwner)!.Value = team;

            e.GetProperty(PropertyNames.Position)!.Value = position;
            e.GetProperty(PropertyNames.Velocity)!.Value = velocity;
            e.GetProperty(PropertyNames.Orientation)!.Value = orientation;
            e.GetProperty(PropertyNames.AngularVelocity)!.Value = angularVelocity;

            e.GetProperty(PropertyNames.ExtraMass)!.Value = 3000f;

            e.GetProperty(PropertyNames.YololContext)!.Value = new YololContext(programs);

            e.GetProperty(PropertyNames.FuelLitersInTank)!.Value = 1500;
            e.GetProperty(PropertyNames.FuelConsumptionRate)!.Value = 3;
            e.GetProperty(PropertyNames.MaxEngineForce)!.Value = 5000;

            e.GetProperty(PropertyNames.MissileLauncherAmmo)!.Value = 20;

            return e;
        }
    }
}