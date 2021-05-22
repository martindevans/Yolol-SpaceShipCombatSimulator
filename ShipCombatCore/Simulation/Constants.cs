using ShipCombatCore.Simulation.Behaviours;
using System.Reflection;
using Yolol.Execution;

namespace ShipCombatCore.Simulation
{
    public class Constants
    {
        public static readonly float SpaceShipThrust = 5000;
        public static readonly float SpaceShipFuelConsumption = 3;

        public static readonly float MissileThrust = 5000;
        public static readonly float MissileFuelConsumption = 1;

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
