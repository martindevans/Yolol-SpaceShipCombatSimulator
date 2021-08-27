using ShipCombatCore.Helpers;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Behaviours
{
    public class FixedActiveRadarScannerDevice
        : BaseRadarBehaviour
    {
        public const float MinRange = Constants.MissileRadarMinRange;
        public const float MaxRange = Constants.MissileRadarMaxRange;

        public const float MinAngle = Constants.MissileRadarMinAngle;
        public const float MaxAngle = Constants.MissileRadarMaxAngle;

        protected override float Elevation(YololContext ctx)
        {
            return 0;
        }

        protected override float Bearing(YololContext ctx)
        {
            return 270;
        }

        protected override void Initialised(YololContext ctx)
        {
            base.Initialised(ctx);

            ctx.Get(":radar_beam_angle").Value = (Number)15;
            ctx.Get(":radar_beam_range").Value = (Number)5000;
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
