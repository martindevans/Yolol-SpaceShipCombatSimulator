using System;
using System.Numerics;

namespace ShipCombatCore.Helpers
{
    public static class Targeting
    {
        public static Vector3 WorldDirection(float elevation, Vector3 elevationAxis, float bearing, Vector3 bearingAxis, Quaternion shipOrientation)
        {
            var fwd = new Vector3(0, 1, 0);
            var eq = Quaternion.CreateFromAxisAngle(elevationAxis, elevation / 180f * MathF.PI);
            var bq = Quaternion.CreateFromAxisAngle(bearingAxis, bearing / 180f * MathF.PI);

            return Vector3.Transform(fwd, shipOrientation * bq * eq);
        }

        public static Vector3 WorldDirection(float elevation, float bearing, Quaternion shipOrientation)
        {
            return WorldDirection(elevation, new Vector3(0, 0, -1), bearing, new Vector3(1, 0, 0), shipOrientation);
        }
    }
}
