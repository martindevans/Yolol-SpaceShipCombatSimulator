using System;
using MathHelperRedux;
using Myre.Entities;
using Newtonsoft.Json;

namespace ShipCombatCore.Simulation.Report.Curves
{
    public class FloatCurve
        : BaseCurve<float>
    {
        private readonly float _errorScale;

        public virtual int Rounding => 2;

        public FloatCurve(string name, float errorScale = 1)
            : base(name)
        {
            _errorScale = errorScale;
        }

        protected override float Estimate(in float start, in float end, float t)
        {
            return MathHelper.Lerp(start, end, t);
        }

        protected override float Error(in float expected, in float estimated)
        {
            return Math.Abs(expected - estimated) * _errorScale;
        }

        protected override void WriteKeyframeElements(JsonWriter writer, in float value)
        {
            writer.WritePropertyName("V");

            var r = Math.Round(value, Rounding);
            if (r == 0)
                r = 0;

            writer.WriteValue(r);
        }
    }

    public class FloatPropertyCurve
        : FloatCurve
    {
        private readonly Property<float> _property;

        public FloatPropertyCurve(Property<float> property, float errorScale = 1)
            : base(property.Name, errorScale)
        {
            _property = property;
        }

        public void Extend(uint ms)
        {
            Extend(ms, _property.Value);
        }
    }
}
