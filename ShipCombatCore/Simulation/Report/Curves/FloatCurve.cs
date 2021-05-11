using System;
using MathHelperRedux;
using Myre.Entities;
using Newtonsoft.Json;

namespace ShipCombatCore.Simulation.Report.Curves
{
    public class FloatCurve
        : BaseCurve<float>
    {
        public virtual int Rounding => 2;

        public FloatCurve(Property<float> property)
            : base(property)
        {
        }

        protected override float Estimate(in float start, in float end, float t)
        {
            return MathHelper.Lerp(start, end, t);
        }

        protected override float Error(in float expected, in float estimated)
        {
            return Math.Abs(expected - estimated);
        }

        protected override void WriteKeyframeElements(JsonWriter writer, in float value)
        {
            writer.WritePropertyName("V");
            writer.WriteValue(Math.Round(value, Rounding));
        }
    }
}
