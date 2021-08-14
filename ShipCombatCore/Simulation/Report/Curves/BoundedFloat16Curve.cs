using System;
using System.Collections.Generic;
using System.Linq;
using Myre.Entities;
using Newtonsoft.Json;

namespace ShipCombatCore.Simulation.Report.Curves
{
    public class BoundedFloat16Curve
        : FloatCurve
    {
        public BoundedFloat16Curve(string name, float errorScale)
            : base(name, errorScale)
        {
        }

        private static string ToBase64<T>(IEnumerable<T> values)
            where T : struct
        {
            var data = values.ToArray();
            var byteCount = Buffer.ByteLength(data);
            byte[] bytes = new byte[byteCount];
            Buffer.BlockCopy(data, 0, bytes, 0, bytes.Length);
            
            return Convert.ToBase64String(bytes);
        }

        public override void Serialize(JsonWriter writer)
        {
            KeyFrameReduction();

            var max = Keyframes.Select(a => a.Value).Append(float.MinValue).Max();
            var min = Keyframes.Select(a => a.Value).Append(float.MaxValue).Min();
            if (Math.Abs(max - min) < float.Epsilon)
                max += 1;
            var range = max - min;

            writer.WriteStartObject();
            {
                writer.WritePropertyName("Name");
                writer.WriteValue(Name);

                writer.WritePropertyName("Min");
                writer.WriteValue(min);

                writer.WritePropertyName("Max");
                writer.WriteValue(max);

                writer.WritePropertyName("Type");
                writer.WriteValue(nameof(Single) + "_r16");

                writer.WritePropertyName("KeysData");
                writer.WriteValue(ToBase64(Keyframes.Select(a => (uint)a.Time.TotalMilliseconds)));

                writer.WritePropertyName("ValueData");
                writer.WriteValue(ToBase64(Keyframes.Select(a => (ushort)((a.Value - min) / range * ushort.MaxValue))));
            }
            writer.WriteEndObject();
        }
    }

    public class BoundedFloat16PropertyCurve
        : BoundedFloat16Curve
    {
        private readonly Property<float> _property;

        public BoundedFloat16PropertyCurve(Property<float> property, float errorScale = 1)
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
