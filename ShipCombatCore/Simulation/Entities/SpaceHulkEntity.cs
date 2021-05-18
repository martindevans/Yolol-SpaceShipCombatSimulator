using System.Numerics;
using Myre.Entities;
using Ninject;
using ShipCombatCore.Simulation.Behaviours;
using ShipCombatCore.Simulation.Behaviours.Recording;

namespace ShipCombatCore.Simulation.Entities
{
    public class SpaceHulkEntity
        : EntityDescription
    {
        public SpaceHulkEntity(IKernel kernel)
            : base(kernel)
        {
            AddProperty(PropertyNames.UniqueName);
            AddProperty(PropertyNames.EntityType, EntityType.SpaceHulk);

            // Physics
            AddBehaviour<Integrate>();
            AddBehaviour<SpaceDrag>();
            AddBehaviour<RadarDetectable>();
            AddProperty(PropertyNames.SphereRadius, 50);

            // Recording
            AddBehaviour<RecorderMaster>();
            AddBehaviour<RecordTransformPosition>();
            AddBehaviour<RecordTransformOrientation>();
        }

        public Entity Create(string name, Vector3 position, Vector3 velocity, Quaternion orientation, Vector3 angularVelocity)
        {
            var e = base.Create();

            e.GetProperty(PropertyNames.UniqueName)!.Value = name;

            e.GetProperty(PropertyNames.Position)!.Value = position;
            e.GetProperty(PropertyNames.Velocity)!.Value = velocity;

            e.GetProperty(PropertyNames.Orientation)!.Value = orientation;
            e.GetProperty(PropertyNames.AngularVelocity)!.Value = angularVelocity;

            return e;
        }
    }
}