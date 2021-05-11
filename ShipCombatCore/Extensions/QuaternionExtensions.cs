using System;
using System.Numerics;
using Newtonsoft.Json;

namespace ShipCombatCore.Extensions
{
    public static class QuaternionExtensions
    {
        public static void SerializeElements(this Quaternion quat, JsonWriter writer)
        {
            writer.WritePropertyName("W");
            writer.WriteValue(Math.Round(quat.W, 5));

            writer.WritePropertyName("X");
            writer.WriteValue(Math.Round(quat.X, 5));
   
            writer.WritePropertyName("Y");
            writer.WriteValue(Math.Round(quat.Y, 5));
        
            writer.WritePropertyName("Z");
            writer.WriteValue(Math.Round(quat.Z, 5));
        }
    }
}
