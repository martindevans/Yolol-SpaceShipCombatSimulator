using System;
using System.Numerics;
using Myre.Entities;
using Ninject;
using ShipCombatCore.Simulation.Behaviours;
using ShipCombatCore.Simulation.Behaviours.Recording;

namespace ShipCombatCore.Simulation.Entities
{
    public class AsteroidEntity
        : EntityDescription
    {
        public AsteroidEntity(IKernel kernel)
            : base(kernel)
        {
            AddProperty(PropertyNames.UniqueName);
            AddProperty(PropertyNames.EntityType, EntityType.Asteroid);

            // Physics
            AddBehaviour<RadarDetectable>();
            AddProperty(PropertyNames.Position);
            AddProperty(PropertyNames.Orientation);

            // Collision
            AddBehaviour<SphereColliderPrimary>();

            // Recording
            AddBehaviour<RecorderMaster>();
            AddBehaviour<RecordTransformPosition>();
            AddBehaviour<RecordSphereRadius>();
        }

        public Entity Create(Vector3 position, Quaternion orientation, float radius)
        {
            var e = base.Create();

            e.GetProperty(PropertyNames.UniqueName)!.Value = Guid.NewGuid().ToString();

            e.GetProperty(PropertyNames.Position)!.Value = position;
            e.GetProperty(PropertyNames.Orientation)!.Value = orientation;

            e.GetProperty(PropertyNames.SphereRadius)!.Value = radius;

            return e;
        }
    }
}