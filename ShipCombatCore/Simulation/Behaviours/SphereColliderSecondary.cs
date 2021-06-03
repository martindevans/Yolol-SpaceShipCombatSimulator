using System.Collections.Generic;
using System.Numerics;
using Myre.Entities;
using Myre.Entities.Behaviours;
using SwizzleMyVectors.Geometry;

namespace ShipCombatCore.Simulation.Behaviours
{
    /// <summary>
    /// A thing that can collide with primary colliders
    /// </summary>
    [DefaultManager(typeof(Manager))]
    public class SphereColliderSecondary
        : Behaviour
    {
        private Property<float>? _radius;
        private Property<Vector3>? _position;

        public float Radius => _radius?.Value ?? 0;
        public Vector3 Position => _position?.Value ?? Vector3.Zero;

        public BoundingBox Bounds => new(new BoundingSphere(Position, Radius));

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _radius = context.CreateProperty(PropertyNames.SphereRadius);
            _position = context.CreateProperty(PropertyNames.Position);

            base.CreateProperties(context);
        }

        public class Manager
            : BehaviourManager<SphereColliderSecondary>
        {
            public IReadOnlyList<SphereColliderSecondary> Items => Behaviours;
        }
    }
}
