using System.Numerics;
using Myre.Entities;
using Newtonsoft.Json;
using ShipCombatCore.Extensions;

namespace ShipCombatCore.Simulation.Report.Curves
{
    public class Vector3PositionCurve
        : BaseCurve<Vector3>
    {
        public virtual int Rounding => 2;

        public Vector3PositionCurve(Property<Vector3> property)
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
