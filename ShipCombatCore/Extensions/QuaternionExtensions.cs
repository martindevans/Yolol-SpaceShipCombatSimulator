using System;
using System.Numerics;
using Newtonsoft.Json;

namespace ShipCombatCore.Extensions
{
    public static class QuaternionExtensions
    {
        public static void SerializeElements(this Quaternion quat, JsonWriter writer, int rounding = 4)
        {
            writer.WritePropertyName("W");
            writer.WriteValue(Math.Round(quat.W, rounding));

            writer.WritePropertyName("X");
            writer.WriteValue(Math.Round(quat.X, rounding));

            writer.WritePropertyName("Y");
            writer.WriteValue(Math.Round(quat.Y, rounding));

            writer.WritePropertyName("Z");
            writer.WriteValue(Math.Round(quat.Z, rounding));
        }
    }
}
