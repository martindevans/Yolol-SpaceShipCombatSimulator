using System;
using System.Collections.Generic;
using System.Numerics;
using Myre.Collections;
using Myre.Entities;
using Myre.Entities.Behaviours;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class Integrate
        : ProcessBehaviour
    {
#pragma warning disable 8618
        
        private Property<Vector3> _force;
        private Property<Vector3> _torque;

        private Property<Quaternion> _orientation;
        private Property<Vector3> _angularVelocity;

        private Property<Vector3> _position;
        private Property<Vector3> _velocity;

        private IReadOnlyList<IMassProvider> _massProviders;
        private Property<float> _mass;
#pragma warning restore 8618

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _mass = context.CreateProperty(PropertyNames.Mass);
            _force = context.CreateProperty(PropertyNames.Force);
            _torque = context.CreateProperty(PropertyNames.Torque);
            _position = context.CreateProperty(PropertyNames.Position);
            _velocity = context.CreateProperty(PropertyNames.Velocity);

            _orientation = context.CreateProperty(PropertyNames.Orientation);
            _angularVelocity = context.CreateProperty(PropertyNames.AngularVelocity);

            base.CreateProperties(context);
        }

        public override void Initialise(INamedDataProvider? initialisationData)
        {
            _massProviders = Owner.GetBehaviours<IMassProvider>();

            base.Initialise(initialisationData);
        }

        protected override void Update(float elapsedTime)
        {
            _mass.Value = 1;
            foreach (var item in _massProviders)
                _mass.Value += Math.Max(0, item.Mass);

            _velocity.Value += elapsedTime * _force.Value / _mass.Value;
            _force.Value = Vector3.Zero;

            _angularVelocity.Value += elapsedTime * _torque.Value / _mass.Value;
            _torque.Value = Vector3.Zero;

            _position.Value += _velocity.Value * elapsedTime;
            _orientation.Value = Quaternion.Normalize(_orientation.Value * EulerXYZ(_angularVelocity.Value * elapsedTime));
        }

        public static Quaternion EulerXYZ(Vector3 xyz)
        {
            xyz *= 0.5f;

            var s = new Vector3(MathF.Sin(xyz.X), MathF.Sin(xyz.Y), MathF.Sin(xyz.Z));
            var c = new Vector3(MathF.Cos(xyz.X), MathF.Cos(xyz.Y), MathF.Cos(xyz.Z));

            return new Quaternion(
                s.X * c.Y * c.Z - s.Y * s.Z * c.X,
                s.Y * c.X * c.Z + s.X * s.Z * c.Y,
                s.Z * c.X * c.Y - s.X * s.Y * c.Z,
                c.X * c.Y * c.Z + s.Y * s.Z * s.X
            );
        }

        public class Manager
            : Manager<Integrate>
        {
        }
    }
}
