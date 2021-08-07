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
        public const float TurretShellSpeed = 400;
        public const float TurretRefireTime = 4;
        public const float TurretBearingAxisX = 1;
        public const float TurretBearingAxisY = 0;
        public const float TurretBearingAxisZ = 0;
        public const float TurretElevationAxisX = 0;
        public const float TurretElevationAxisY = 0;
        public const float TurretElevationAxisZ = -1;

        public const float RadarMinRange = 20;
        public const float RadarMaxRange = 20000;

        public const float RadarMinAngle = 1;
        public const float RadarMaxAngle = 90;

        public const float MissileRefireTime = 3;

        public const float ShipLocalForwardX = 0;
        public const float ShipLocalForwardY = 0;
        public const float ShipLocalForwardZ = -1;

        public const float ShipBaseMass = 3000;
        public const float MissileBaseMass = 100;

        public const float MissileWheelTorque = 20f;
        public const float ShipWheelTorque = 20f;

        public const float ShipRadius = 30;
        public const float MissileRadius = 2;

        public static void SetConstants(YololContext ctx)
        {
            var fields = typeof(Constants).GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                var variable = ctx.Get($":const_{field.Name}");

                var value = field.GetValue(null);

                if (field.FieldType == typeof(float))
                    variable.Value = (Number)(float)value;

                if (field.FieldType == typeof(string))
                    variable.Value = (string)value;
            }
        }
    }
}
