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
#pragma warning disable 8618
        private Property<float> _radius;
        private Property<Vector3> _position;
#pragma warning restore 8618

        public float Radius => _radius.Value;
        public Vector3 Position => _position.Value;

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
