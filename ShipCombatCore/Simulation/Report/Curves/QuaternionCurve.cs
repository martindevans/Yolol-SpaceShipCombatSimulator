using System.Numerics;
using Myre.Entities;
using Newtonsoft.Json;
using ShipCombatCore.Extensions;

namespace ShipCombatCore.Simulation.Report.Curves
{
    public class QuaternionCurve
        : BaseCurve<Quaternion>
    {
        public QuaternionCurve(Property<Quaternion> property)
            : base(property)
        {
        }

        protected override Quaternion Estimate(in Quaternion start, in Quaternion end, float t)
        {
            return Quaternion.Normalize(Quaternion.Lerp(start, end, t));
        }

        protected override float Error(in Quaternion expected, in Quaternion estimated)
        {
            var difference = expected - estimated;
            return difference.Length() * 200;
        }

        protected override void WriteKeyframeElements(JsonWriter writer, in Quaternion value)
        {
            value.SerializeElements(writer);
        }
    }
}