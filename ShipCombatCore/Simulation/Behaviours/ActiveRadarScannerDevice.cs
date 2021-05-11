using ShipCombatCore.Helpers;

namespace ShipCombatCore.Simulation.Behaviours
{
    public class ActiveRadarScannerDevice
        : BaseRadarBehaviour
    {
        public const float MinRange = 20;
        public const float MaxRange = 20000;

        public const float MinAngle = 1;
        public const float MaxAngle = 80;

        protected override float Elevation(YololContext ctx)
        {
            return YololValue.Number(ctx.Get(":radar_elevation").Value, 0, 360);
        }

        protected override float Bearing(YololContext ctx)
        {
            return YololValue.Number(ctx.Get(":radar_bearing").Value, 0, 360);
        }

        protected override float BeamAngle(YololContext ctx)
        {
            return YololValue.Number(ctx.Get(":radar_beam_angle").Value, MinAngle, MaxAngle);
        }

        protected override float BeamRange(YololContext ctx)
        {
            return YololValue.Number(ctx.Get(":radar_beam_range").Value, MinRange, MaxRange);
        }
    }
}
