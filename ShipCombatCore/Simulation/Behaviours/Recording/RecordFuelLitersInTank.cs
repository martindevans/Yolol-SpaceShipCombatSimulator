using Myre;

namespace ShipCombatCore.Simulation.Behaviours.Recording
{
    public class RecordFuelLitersInTank
        : BaseRecordFloat
    {
        protected override TypedName<float> PropertyName => PropertyNames.FuelLitersInTank;
    }
}
