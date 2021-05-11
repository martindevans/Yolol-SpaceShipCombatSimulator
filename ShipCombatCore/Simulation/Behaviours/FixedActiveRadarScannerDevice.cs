namespace ShipCombatCore.Simulation.Behaviours
{
    public class FixedActiveRadarScannerDevice
        : BaseRadarBehaviour
    {
        protected override float Elevation(YololContext ctx)
        {
            return 0;
        }

        protected override float Bearing(YololContext ctx)
        {
            return 270;
        }

        protected override float BeamAngle(YololContext ctx)
        {
            return 15;
        }

        protected override float BeamRange(YololContext ctx)
        {
            return 5000;
        }
    }
}
