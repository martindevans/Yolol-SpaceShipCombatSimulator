using System.Numerics;
using Myre.Entities;
using Newtonsoft.Json;
using ShipCombatCore.Extensions;

namespace ShipCombatCore.Simulation.Report.Curves
{
    public class Vector3DirectionCurve
        : BaseCurve<Vector3>
    {
        public Vector3DirectionCurve(Property<Vector3> property)
            : base(property)
        {
        }

        protected override Vector3 Estimate(in Vector3 start, in Vector3 end, float t)
        {
            return Vector3.Normalize(start * (1 - t) + end * t);
        }

        protected override float Error(in Vector3 expected, in Vector3 estimated)
        {
            return (1 - Vector3.Dot(expected, estimated)) * 50;
        }

        protected override void WriteKeyframeElements(JsonWriter writer, in Vector3 value)
        {
            value.SerializeElements(writer);
        }
    }
}
