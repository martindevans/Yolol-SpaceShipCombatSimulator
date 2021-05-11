using System;
using System.Numerics;
using Myre.Entities;
using ShipCombatCore.Simulation.Behaviours;
using ShipCombatCore.Simulation.Behaviours.Recording;

namespace ShipCombatCore.Simulation.Entities
{
    public class ShellEntity
        : EntityDescription
    {
        public ShellEntity()
        {
            AddProperty(PropertyNames.UniqueName, Guid.NewGuid().ToString());
            AddProperty(PropertyNames.EntityType, EntityType.Shell);
            
            // Explode after timeout or when taking any damage
            AddBehaviour<LifetimeLimit>();
            AddBehaviour<DamageInstantlyKills>();
            AddBehaviour<ExplodeOnDeath>();

            // Physics
            AddBehaviour<Integrate>();

            // Collision
            AddBehaviour<SphereColliderSecondary>();
            AddProperty(PropertyNames.SphereColliderRadius, 1);

            // Radar
            AddBehaviour<RadarDetectable>();

            // Recording
            AddBehaviour<RecorderMaster>();
            AddBehaviour<RecordTransformPosition>();
        }

        public Entity Create(float fuse, Vector3 position, Vector3 velocity)
        {
            var e = base.Create();

            e.GetProperty(PropertyNames.Position)!.Value = position;
            e.GetProperty(PropertyNames.Velocity)!.Value = velocity;
            e.GetProperty(PropertyNames.Lifetime)!.Value = fuse;

            return e;
        }
    }
}