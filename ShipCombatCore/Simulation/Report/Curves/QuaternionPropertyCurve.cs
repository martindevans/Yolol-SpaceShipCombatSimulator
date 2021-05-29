using System.Numerics;
using Myre.Entities;
using Newtonsoft.Json;

namespace ShipCombatCore.Simulation.Report.Curves
{
    public class QuaternionPropertyCurve
        : ICurve
    {
        private readonly Property<Quaternion> _property;

        private readonly FloatCurve _w;
        private readonly FloatCurve _x;
        private readonly FloatCurve _y;
        private readonly FloatCurve _z;

        public QuaternionPropertyCurve(Property<Quaternion> property)
        {
            _property = property;

            _w = new FloatCurve($"{property.Name}.w", 45);
            _x = new FloatCurve($"{property.Name}.x", 45);
            _y = new FloatCurve($"{property.Name}.y", 45);
            _z = new FloatCurve($"{property.Name}.z", 45);
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