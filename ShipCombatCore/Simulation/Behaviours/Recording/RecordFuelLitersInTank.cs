using Myre;

namespace ShipCombatCore.Simulation.Behaviours.Recording
{
    public class RecordFuelLitersInTank
        : BaseRecordBoundedFloat16
    {
        protected override TypedName<float> PropertyName => PropertyNames.FuelLitersInTank;
    }
}
