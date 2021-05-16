using System.Numerics;

namespace ShipCombatCore.Geometry
{
    public readonly struct Circle
    {
        public Vector2 Position { get; }
        public float Radius { get; }

        public Circle(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public bool Contains(Circle other)
        {
            var d = Vector2.Distance(Position, other.Position);
            return Radius > (d + other.Radius);
        }
    }
}
