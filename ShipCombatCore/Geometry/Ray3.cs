using System;
using System.Numerics;

namespace ShipCombatCore.Geometry
{
    internal readonly struct HalfRay3
    {
        public readonly Vector3 Position;
        public readonly Vector3 Direction;

        public HalfRay3(Vector3 position, Vector3 direction)
        {
            Position = position;
            Direction = direction;
        }

        public Vector3 ClosestPoint(Vector3 point, out float distanceAlongLine)
        {
            distanceAlongLine = ClosestPointDistanceAlongLine(point);
            return Position + Direction * distanceAlongLine;
        }

        public float ClosestPointDistanceAlongLine(Vector3 point)
        {
            var direction = Direction;
            var lengthSq = direction.LengthSquared();

            return Math.Max(0, Vector3.Dot(point - Position, direction) / lengthSq);
        }
    }
}