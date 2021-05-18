using System;
using System.Numerics;
using Myre.Collections;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Simulation.Entities;

namespace ShipCombatCore.Simulation.Behaviours
{
    public class HulkOnDeath
        : Behaviour
    {
#pragma warning disable 8618
        private Property<string> _name;
        private Property<Vector3> _position;
        private Property<Vector3> _velocity;
        private Property<Quaternion> _orientation;
        private Property<Vector3> _angularVelocity;
#pragma warning restore 8618

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _name = context.CreateProperty(PropertyNames.UniqueName);
            _position = context.CreateProperty(PropertyNames.Position);
            _velocity = context.CreateProperty(PropertyNames.Velocity);
            _orientation = context.CreateProperty(PropertyNames.Orientation);
            _angularVelocity = context.CreateProperty(PropertyNames.AngularVelocity);

            base.CreateProperties(context);
        }

        public override void Shutdown(INamedDataProvider? shutdownData)
        {
            base.Shutdown(shutdownData);

            // Give the hulk some random rotation
            var rand = new Random(_name.Value?.GetHashCode() ?? 17);
            var randomAngular = new Vector3((float)rand.NextDouble() - 0.5f, (float)rand.NextDouble() - 0.5f, (float)rand.NextDouble() - 0.5f) * 2f;

            // Create hulk entity
            Owner.Scene?.Add(new SpaceHulkEntity(Owner.Scene.Kernel).Create($"{_name.Value} (HULK)", _position.Value, _velocity.Value, _orientation.Value, _angularVelocity.Value + randomAngular));
        }
    }
}
