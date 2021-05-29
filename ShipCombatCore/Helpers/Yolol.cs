using System;
using Yolol.Execution;

namespace ShipCombatCore.Helpers
{
    public static class YololValue
    {
        public static float Number(Value value, float min = float.MinValue, float max = float.MaxValue)
        {
            var v = 0f;
            if (value.Type == Yolol.Execution.Type.Number)
                v = (float)value.Number;

            return Math.Clamp(v, min, max);
        }
    }
}
