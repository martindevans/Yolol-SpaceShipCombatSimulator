using System;
using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;
using Myre;
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
#pragma warning disable 8618
        private Property<float> _radius;
        private Property<Vector3> _position;

        private Manager _primaryManager;
        private SphereColliderSecondary.Manager _secondaryManager;
#pragma warning restore 8618

        public float Radius => _radius.Value;
        public Vector3 Position => _position.Value;

        private readonly HashSet<SphereColliderPrimary> _queryResults = new();

        public BoundingBox Bounds { get; private set; }

        public override void Initialise(INamedDataProvider? initialisationData)
        {
            base.Initialise(initialisationData);

            _primaryManager = Owner.Scene!.GetManager<Manager>();
            _secondaryManager = Owner.Scene!.GetManager<SphereColliderSecondary.Manager>();
        }

        protected void InitialiseBounds()
        {
            Bounds = new(new BoundingSphere(Position, Radius));
        }

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _radius = context.CreateProperty(PropertyNames.SphereRadius);
            _position = context.CreateProperty(PropertyNames.Position);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            foreach (var secondary in _secondaryManager.Items)
            {
                _queryResults.Clear();
                _primaryManager.GetColliders(secondary.Bounds, _queryResults);

                foreach (var primary in _queryResults)
                {
                    var d = Vector3.Distance(primary.Position, secondary.Position);
                    var r = primary.Radius + secondary.Radius;

                    if (d <= r)
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
            private const float BucketSize = 275;
            private readonly Dictionary<Int3, List<SphereColliderPrimary>> _buckets = new();

            private static Int3 ToBucket(Vector3 position)
            {
                return new Int3(
                    (int)MathF.Floor(position.X / BucketSize),
                    (int)MathF.Floor(position.Y / BucketSize),
                    (int)MathF.Floor(position.Z / BucketSize)
                );
            }

            public override void Add(SphereColliderPrimary behaviour)
            {
                base.Add(behaviour);

                behaviour.InitialiseBounds();
                var min = ToBucket(behaviour.Bounds.Min);
                var max = ToBucket(behaviour.Bounds.Max);

                for (var i = min.X; i <= max.X; i++)
                {
                    for (var j = min.Y; i <= max.Y; i++)
                    {
                        for (var k = min.Z; i <= max.Z; i++)
                        {
                            if (!_buckets.TryGetValue(new Int3(i, j, k), out var list))
                            {
                                list = new List<SphereColliderPrimary>();
                                _buckets[new Int3(i, j, k)] = list;
                            }

                            list.Add(behaviour);
                        }
                    }
                }
            }

            public void GetColliders(BoundingBox bounds, ISet<SphereColliderPrimary> results)
            {
                var min = ToBucket(bounds.Min);
                var max = ToBucket(bounds.Max);

                for (var i = min.X; i <= max.X; i++)
                {
                    for (var j = min.Y; i <= max.Y; i++)
                    {
                        for (var k = min.Z; i <= max.Z; i++)
                        {
                            if (_buckets.TryGetValue(new Int3(i, j, k), out var list))
                            {
                                results.UnionWith(list);
                            }
                        }
                    }
                }
            }

            public override bool Remove(SphereColliderPrimary behaviour)
            {
                var min = ToBucket(behaviour.Bounds.Min);
                var max = ToBucket(behaviour.Bounds.Max);

                for (var i = min.X; i <= max.X; i++)
                {
                    for (var j = min.Y; i <= max.Y; i++)
                    {
                        for (var k = min.Z; i <= max.Z; i++)
                        {
                            if (_buckets.TryGetValue(new Int3(i, j, k), out var list))
                                list.Add(behaviour);
                        }
                    }
                }

                return true;
            }
        }
    }
}
