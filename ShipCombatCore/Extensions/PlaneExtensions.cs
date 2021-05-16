using System.Numerics;

namespace ShipCombatCore.Extensions
{
    public static class PlaneExtensions
    {
        public static Vector2 Project(this Plane plane, Vector3 point)
        {
            var n = plane.Normal;

            // Determine an arbitrary "x axis" for the 2d coordinate space on the plane
            var x1 = Vector3.Dot(plane.Normal, Vector3.UnitY);
            var x2 = Vector3.Dot(plane.Normal, Vector3.UnitZ);
            var x = Vector3.Normalize(Vector3.Cross(plane.Normal, x1 < x2 ? Vector3.UnitY : Vector3.UnitZ));

            // https://stackoverflow.com/a/9605748/108234
            var xp = Vector3.Dot(point, x);
            var yp = Vector3.Dot(point, Vector3.Cross(n, x));

            return new Vector2(xp, yp);
        }
    }
}
