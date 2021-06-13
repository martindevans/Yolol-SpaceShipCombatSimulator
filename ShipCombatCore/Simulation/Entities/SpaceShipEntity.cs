using System.Collections.Generic;
using System.Numerics;
using Myre.Entities;
using Ninject;
using ShipCombatCore.Simulation.Behaviours;
using ShipCombatCore.Simulation.Behaviours.Recording;

namespace ShipCombatCore.Simulation.Entities
{
    public class SpaceShipEntity
        : EntityDescription
    {
        public SpaceShipEntity(IKernel kernel)
            : base(kernel)
        {
            AddProperty(PropertyNames.UniqueName);
            AddProperty(PropertyNames.TeamName);
            AddProperty(PropertyNames.EntityType, EntityType.SpaceBattleShip);
            AddBehaviour<TeamMember>();

            // Physics
            AddBehaviour<Integrate>();
            AddBehaviour<ExtraMass>();
            AddBehaviour<RadarDetectable>();

            // Collision
            AddBehaviour<SphereColliderSecondary>();
            AddProperty(PropertyNames.SphereRadius, 30);

            // Damage
            AddBehaviour<DamageRemovesFuel>();
            AddBehaviour<OutOfFuelDestroy>();
            AddBehaviour<CosmicRadiationDamage>();
            AddBehaviour<HulkOnDeath>();

            // Yolol Devices
            AddBehaviour<YololHost>();
            AddBehaviour<ClockDevice>();
            AddBehaviour<GyroscopeDevice>();
            AddBehaviour<PositioningDevice>();
            AddBehaviour<FuelTank>();
            AddBehaviour<Engine>();
            AddBehaviour<MomentumWheels>();
            AddBehaviour<Turrets>();
            AddBehaviour<MissileLauncher>();
            AddBehaviour<ActiveRadarScannerDevice>();
            AddBehaviour<Radio>();
            AddBehaviour<CaptainsLog>();
            AddBehaviour<MathHelperDevice>();

            // Recording
            AddBehaviour<RecorderMaster>();
            AddBehaviour<RecordTransformPosition>();
            AddBehaviour<RecordTransformOrientation>();
            AddBehaviour<RecordActualThrottle>();
            AddBehaviour<RecordFuelLitersInTank>();
            AddBehaviour<RecordRunningLight>();
            //AddBehaviour<RecordCosmicRadiation>();
            AddBehaviour<DebugGizmoRecorder>();
        }

        public Entity Create(string name, string teamName, uint team, Vector3 position, Vector3 velocity, Quaternion orientation, Vector3 angularVelocity, IReadOnlyList<Yolol.Grammar.AST.Program> programs)
        {
            var e = base.Create();

            e.GetProperty(PropertyNames.UniqueName)!.Value = name;
            e.GetProperty(PropertyNames.TeamName)!.Value = teamName;
            e.GetProperty(PropertyNames.TeamOwner)!.Value = team;

            e.GetProperty(PropertyNames.Position)!.Value = position;
            e.GetProperty(PropertyNames.Velocity)!.Value = velocity;
            e.GetProperty(PropertyNames.Orientation)!.Value = orientation;
            e.GetProperty(PropertyNames.AngularVelocity)!.Value = angularVelocity;

            e.GetProperty(PropertyNames.ExtraMass)!.Value = 3000f;

            e.GetProperty(PropertyNames.YololContext)!.Value = new YololContext(programs);

            e.GetProperty(PropertyNames.FuelLitersInTank)!.Value = 1500;
            e.GetProperty(PropertyNames.FuelConsumptionRate)!.Value = Constants.SpaceShipFuelConsumption;
            e.GetProperty(PropertyNames.MaxEngineForce)!.Value = Constants.SpaceShipThrust;

            e.GetProperty(PropertyNames.MissileLauncherAmmo)!.Value = 20;

            return e;
        }
    }
}