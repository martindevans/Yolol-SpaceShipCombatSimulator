using Myre.Entities;
using Myre.Entities.Behaviours;

namespace ShipCombatCore.Simulation.Behaviours
{
    public class ExtraMass
        : Behaviour, IMassProvider
    {
#pragma warning disable 8618
        private Property<float> _mass;
#pragma warning restore 8618

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _mass = context.CreateProperty(PropertyNames.ExtraMass);

            base.CreateProperties(context);
        }

        public float Mass => _mass.Value;
    }
}
