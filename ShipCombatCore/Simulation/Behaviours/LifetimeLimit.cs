using Myre.Collections;
using Myre.Entities;
using Myre.Entities.Behaviours;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class LifetimeLimit
        : ProcessBehaviour
    {
#pragma warning disable 8618
        private Property<float> _life;
#pragma warning restore 8618

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _life = context.CreateProperty(PropertyNames.Lifetime);
        }

        public override void Initialise(INamedDataProvider? initialisationData)
        {
            _life.Value = initialisationData?.GetValue(PropertyNames.Lifetime) ?? _life.Value;
        }

        protected override void Update(float elapsedTime)
        {
            _life.Value -= elapsedTime;

            if (_life.Value < 0)
                Owner.Dispose(new NamedBoxCollection());
        }

        public class Manager
            : Manager<LifetimeLimit>
        {
        }
    }
}
