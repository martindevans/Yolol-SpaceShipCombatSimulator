using Myre;

namespace ShipCombatCore.Simulation.Behaviours.Recording
{
    public class RecordActualThrottle
        : BaseRecordFloat
    {
        protected override TypedName<float> PropertyName => PropertyNames.ActualThrottle;
    }
}
