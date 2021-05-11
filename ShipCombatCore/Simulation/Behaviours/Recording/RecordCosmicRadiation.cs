using Myre;

namespace ShipCombatCore.Simulation.Behaviours.Recording
{
    public class RecordCosmicRadiation
        : BaseRecordFloat
    {
        protected override TypedName<float> PropertyName => PropertyNames.CosmicRadiation;
    }
}
