using System;
using System.Numerics;
using Newtonsoft.Json;

namespace ShipCombatCore.Extensions
{
    public static class Vector2Extensions
    {
        public static void SerializeElements(this Vector2 vec, JsonWriter writer, int round = 3)
        {
            writer.WritePropertyName("A");
            writer.WriteValue(Math.Round(vec.X, round));
        
            writer.WritePropertyName("B");
            writer.WriteValue(Math.Round(vec.Y, round));
        }
    }
}
