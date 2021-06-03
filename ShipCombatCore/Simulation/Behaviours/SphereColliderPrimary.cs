using System.Collections.Generic;
using System.Numerics;
using HandyCollections.Geometry;
using JetBrains.Annotations;
using Myre.Collections;
using Myre.Entities;
using Myre.Entities.Behaviours;
using SwizzleMyVectors.Geometry;

namespace ShipCombatCore.Simulation.Behaviours
{
    /// <summary>
    /// A thing that can collide with secondary colliders
    /// </summary>
    [DefaultManager(typeof(Manager))]
    public class SphereColliderPrimary
        : ProcessBehaviour
    {
        private Property<float>? _radius;
        private Property<Vector3>? _position;

        private Manager? _primaryManager;
        private SphereColliderSecondary.Manager? _secondaryManager;

        public float Radius => _radius?.Value ?? 0;
        public Vector3 Position => _position?.Value ?? Vector3.Zero;

        public BoundingBox Bounds => new(new BoundingSphere(Position, Radius));

        public override void Initialise(INamedDataProvider? initialisationData)
        {
            base.Initialise(initialisationData);

            _primaryManager = Owner.Scene?.GetManager<Manager>();
            _secondaryManager = Owner.Scene?.GetManager<SphereColliderSecondary.Manager>();
        }

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _radius = context.CreateProperty(PropertyNames.SphereRadius);
            _position = context.CreateProperty(PropertyNames.Position);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            if (_primaryManager == null)
                return;

            var secondarys = _secondaryManager?.Items;
            if (secondarys == null)
                return;

            foreach (var secondary in secondarys)
            {
                foreach (var primary in _primaryManager.GetColliders(secondary.Bounds))
                {
                    var d = Vector3.DistanceSquared(primary.Position, secondary.Position);
                    var r = primary.Radius + secondary.Radius;

                    if (d <= r * r)
                    {
                        // If the impacting object has velocity bounce off
                        var vel = secondary.Owner.GetProperty(PropertyNames.Velocity);
                        if (vel != null)
                            vel.Value *= -0.15f;

                        secondary.Owner.Dispose(new NamedBoxCollection());

                        break;
                    }
                }
            }
        }

        [UsedImplicitly]
        private class Manager
            : Manager<SphereColliderPrimary>
        {
            private readonly Octree<SphereColliderPrimary> _octree;

            public Manager()
            {
                _octree = new Octree<SphereColliderPrimary>(new BoundingBox(new Vector3(-20000), new Vector3(30000)), 1, 8);
            }

            public override void Add(SphereColliderPrimary behaviour)
            {
                base.Add(behaviour);

                _octree.Insert(behaviour.Bounds, behaviour);
            }

            public IEnumerable<SphereColliderPrimary> GetColliders(BoundingBox bounds)
            {
                return _octree.Intersects(bounds);
            }

            public override bool Remove(SphereColliderPrimary behaviour)
            {
                var removed = base.Remove(behaviour);
                if (removed)
                    _octree.Remove(behaviour.Bounds, behaviour);

                return removed;
            }
        }
    }
}
