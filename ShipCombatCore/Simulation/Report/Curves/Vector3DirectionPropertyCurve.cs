using System;
using System.Numerics;
using Myre.Entities;
using Newtonsoft.Json;

namespace ShipCombatCore.Simulation.Report.Curves
{
    public class Vector3DirectionPropertyCurve
        : ICurve
    {
        private readonly Property<Vector3> _property;

        private readonly BoundedFloat16Curve _x;
        private readonly BoundedFloat16Curve _y;
        private readonly BoundedFloat16Curve _z;

        public Vector3DirectionPropertyCurve(Property<Vector3> property)
        {
            _property = property;

            _x = new BoundedFloat16Curve($"{property.Name}.x", 110);
            _y = new BoundedFloat16Curve($"{property.Name}.y", 110);
            _z = new BoundedFloat16Curve($"{property.Name}.z", 110);
        }

        public void Extend(uint ms)
        {
            if (_property.Value == Vector3.Zero)
                throw new InvalidOperationException("Zero length unit vector");

            var n = Vector3.Normalize(_property.Value);
            _x.Extend(ms, n.X);
            _y.Extend(ms, n.Y);
            _z.Extend(ms, n.Z);
        }

        public void Serialize(JsonWriter writer)
        {
            _x.Serialize(writer);
            _y.Serialize(writer);
            _z.Serialize(writer);
        }
    }
}
