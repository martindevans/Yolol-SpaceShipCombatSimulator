using System;
using System.Numerics;
using Newtonsoft.Json;

namespace ShipCombatCore.Extensions
{
    public static class Vector3Extensions
    {
        public static void SerializeElements(this Vector3 vec, JsonWriter writer, int round = 3)
        {
            writer.WritePropertyName("X");
            writer.WriteValue(Math.Round(vec.X, round));
        
            writer.WritePropertyName("Y");
            writer.WriteValue(Math.Round(vec.Y, round));
        
            writer.WritePropertyName("Z");
            writer.WriteValue(Math.Round(vec.Z, round));
        }
    }
}
