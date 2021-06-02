using System.Numerics;
using Myre.Entities;
using Newtonsoft.Json;
using ShipCombatCore.Extensions;

namespace ShipCombatCore.Simulation.Report.Curves
{
    //public class Vector3PositionPropertyElementCurve
    //    : ICurve
    //{
    //    private readonly Property<Vector3> _property;

    //    private readonly BoundedFloat32Curve _x;
    //    private readonly BoundedFloat32Curve _y;
    //    private readonly BoundedFloat32Curve _z;

    //    public Vector3PositionPropertyCurve(Property<Vector3> property)
    //    {
    //        _property = property;

    //        _x = new BoundedFloat32Curve($"{property.Name}.x", 1);
    //        _y = new BoundedFloat32Curve($"{property.Name}.y", 1);
    //        _z = new BoundedFloat32Curve($"{property.Name}.z", 1);
    //    }

    //    public void Extend(uint ms)
    //    {
    //        var v = _property.Value;
    //        _x.Extend(ms, v.X);
    //        _y.Extend(ms, v.Y);
    //        _z.Extend(ms, v.Z);
    //    }

    //    public void Serialize(JsonWriter writer)
    //    {
    //        _x.Serialize(writer);
    //        _y.Serialize(writer);
    //        _z.Serialize(writer);
    //    }
    //}

    public class Vector3PositionPropertyCompoundCurve
        : BasePropertyCurve<Vector3>
    {
        public virtual int Rounding => 1;

        public Vector3PositionPropertyCompoundCurve(Property<Vector3> property)
            : base(property)
        {
        }

        protected override Vector3 Estimate(in Vector3 start, in Vector3 end, float t)
        {
            return start * (1 - t) + end * t;
        }

        protected override float Error(in Vector3 expected, in Vector3 estimated)
        {
            return Vector3.Distance(expected, estimated);
        }

        protected override void WriteKeyframeElements(JsonWriter writer, in Vector3 value)
        {
            value.SerializeElements(writer, Rounding);
        }
    }
}
