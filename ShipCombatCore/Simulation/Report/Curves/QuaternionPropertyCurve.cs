using System.Numerics;
using Myre.Entities;
using Newtonsoft.Json;

namespace ShipCombatCore.Simulation.Report.Curves
{
    public class QuaternionPropertyCurve
        : ICurve
    {
        private readonly Property<Quaternion> _property;

        private readonly BoundedFloat16Curve _w;
        private readonly BoundedFloat16Curve _x;
        private readonly BoundedFloat16Curve _y;
        private readonly BoundedFloat16Curve _z;

        public QuaternionPropertyCurve(Property<Quaternion> property)
        {
            _property = property;

            _w = new BoundedFloat16Curve($"{property.Name}.w", 135);
            _x = new BoundedFloat16Curve($"{property.Name}.x", 135);
            _y = new BoundedFloat16Curve($"{property.Name}.y", 135);
            _z = new BoundedFloat16Curve($"{property.Name}.z", 135);
        }

        public void Extend(uint ms)
        {
            var q = _property.Value;
            _w.Extend(ms, q.W);
            _x.Extend(ms, q.X);
            _y.Extend(ms, q.Y);
            _z.Extend(ms, q.Z);
        }

        public void Serialize(JsonWriter writer)
        {
            _w.Serialize(writer);
            _x.Serialize(writer);
            _y.Serialize(writer);
            _z.Serialize(writer);
        }
    }
}