using System;

namespace ShipCombatCore.Simulation.Entities
{
    public enum EntityType
    {
        SpaceBattleShip,
        SpaceHulk,

        Missile,
        Shell,

        Explosion,

        Asteroid,

        VictoryMarker
    }

    public static class EntityTypeExtensions
    {
        private static readonly string[] Names = Enum.GetNames(typeof(EntityType));

        public static string ToEnumString(this EntityType e)
        {
            return Names[(int)e];
        }
    }
}
