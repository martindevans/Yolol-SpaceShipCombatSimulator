using Myre;

namespace ShipCombatCore.Simulation.Behaviours.Recording
{
    public class RecordSphereRadius
        : BaseRecordFloat
    {
        protected override TypedName<float> PropertyName => PropertyNames.SphereColliderRadius;
    }
}
