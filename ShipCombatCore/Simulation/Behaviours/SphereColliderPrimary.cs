using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;
using Myre.Collections;
using Myre.Entities;
using Myre.Entities.Behaviours;

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
            var primarys = _primaryManager?.Behaviours;
            if (primarys == null)
                return;

            var secondarys = _secondaryManager?.Items;
            if (secondarys == null)
                return;

            foreach (var primary in primarys)
            {
                var primaryPos = primary.Position;
                var primaryRad = primary.Radius;

                foreach (var secondary in secondarys)
                {
                    var d = Vector3.DistanceSquared(primaryPos, secondary.Position);
                    var r = primaryRad + secondary.Radius;

                    if (d <= r * r)
                    {
                        // If the impacting object has velocity bounce off
                        var vel = secondary.Owner.GetProperty(PropertyNames.Velocity);
                        if (vel != null)
                            vel.Value *= -0.15f;

                        secondary.Owner.Dispose(new NamedBoxCollection());
                    }
                }
            }
        }

        [UsedImplicitly]
        private class Manager
            : Manager<SphereColliderPrimary>
        {
        }
    }
}
