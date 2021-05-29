using Myre.Entities;
using Newtonsoft.Json;

namespace ShipCombatCore.Simulation.Report.Curves
{
    public class BoolPropertyCurve
        : BasePropertyCurve<bool>
    {
        public BoolPropertyCurve(Property<bool> property)
            : base(property)
        {
        }

        protected override bool Estimate(in bool start, in bool end, float t)
        {
            return start;
        }

        protected override float Error(in bool expected, in bool estimated)
        {
            return expected == estimated ? 0 : 1;
        }

        protected override void WriteKeyframeElements(JsonWriter writer, in bool value)
        {
            writer.WritePropertyName("V");
            writer.WriteValue(value ? 1 : 0);
        }
    }
}
