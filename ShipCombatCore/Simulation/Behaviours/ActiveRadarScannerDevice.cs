using ShipCombatCore.Helpers;

namespace ShipCombatCore.Simulation.Behaviours
{
    public class ActiveRadarScannerDevice
        : BaseRadarBehaviour
    {
        public const float MinRange = Constants.RadarMinRange;
        public const float MaxRange = Constants.RadarMaxRange;

        public const float MinAngle = Constants.RadarMinAngle;
        public const float MaxAngle = Constants.RadarMaxAngle;

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
