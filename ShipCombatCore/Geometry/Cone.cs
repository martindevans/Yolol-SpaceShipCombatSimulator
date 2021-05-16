using System;
using System.Numerics;

namespace ShipCombatCore.Geometry
{
    public readonly struct Cone
    {
        /// <summary>
        /// Direction along center line of cone
        /// </summary>
        public readonly Vector3 Direction;

        /// <summary>
        /// Angle from the center line to the edge
        /// </summary>
        public readonly float Angle;

        public Cone(Vector3 direction, float angle)
        {
            Direction = direction;
            Angle = angle;
        }

        public bool Contains(Cone other)
        {
            var angle = Math.Acos(Vector3.Dot(Direction, other.Direction));
            return angle + other.Angle < Angle;
        }
    }
}
