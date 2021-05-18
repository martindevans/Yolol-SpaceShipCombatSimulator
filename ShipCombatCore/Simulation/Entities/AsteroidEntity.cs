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
            AddProperty(PropertyNames.UniqueName, Guid.NewGuid().ToString());
            AddProperty(PropertyNames.EntityType, EntityType.Asteroid);

            // Physics
            AddBehaviour<Integrate>();
            AddBehaviour<ExtraMass>();
            AddBehaviour<RadarDetectable>();

            // Collision
            AddBehaviour<SphereColliderPrimary>();

            // Recording
            AddBehaviour<RecorderMaster>();
            AddBehaviour<RecordTransformPosition>();
        }

        public Entity Create(Vector3 position, Vector3 velocity, Quaternion orientation, Vector3 angularVelocity, float radius)
        {
            var e = base.Create();

            e.GetProperty(PropertyNames.Position)!.Value = position;
            e.GetProperty(PropertyNames.Velocity)!.Value = velocity;
            e.GetProperty(PropertyNames.Orientation)!.Value = orientation;
            e.GetProperty(PropertyNames.AngularVelocity)!.Value = angularVelocity;

            e.GetProperty(PropertyNames.ExtraMass)!.Value = 300000f;

            e.GetProperty(PropertyNames.SphereRadius)!.Value = radius;

            return e;
        }
    }
}