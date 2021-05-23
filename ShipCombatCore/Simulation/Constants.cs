using ShipCombatCore.Simulation.Behaviours;
using System.Reflection;
using Yolol.Execution;

namespace ShipCombatCore.Simulation
{
    public class Constants
    {
        public const float SpaceShipThrust = 5000;
        public const float SpaceShipFuelConsumption = 3;

        public const float MissileThrust = 5000;
        public const float MissileFuelConsumption = 1;

        public const float TurretMinElevation = 0;
        public const float TurretMaxElevation = 80;
        public const float TurretElevationSpeed = 90;
        public const float TurretBearingSpeed = 70;
        public const float TurretMinFuse = 2;
        public const float TurretMaxFuse = 30;
        public const float TurretShellSpeed = 300;
        public const float TurretRefireTime = 4;

        public const float RadarMinRange = 20;
        public const float RadarMaxRange = 20000;

        public const float RadarMinAngle = 1;
        public const float RadarMaxAngle = 80;

        public const float MissileRefireTime = 3;

        public static void SetConstants(YololContext ctx)
        {
            var fields = typeof(Constants).GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                var variable = ctx.MaybeGet($":const_{field.Name}");
                if (variable == null)
                    continue;

                var value = field.GetValue(null);

                if (field.FieldType == typeof(float))
                    variable.Value = (Number)(float)value;

                if (field.FieldType == typeof(string))
                    variable.Value = (string)value;
            }
        }
    }
}
