using System.Linq;
using Myre.Entities;
using Myre.Entities.Behaviours;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class VictoryMarker
        : Behaviour
    {
#pragma warning disable 8618
        private Property<string> _name;
#pragma warning restore 8618

        public string Name => _name!.Value!;

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _name = context.CreateProperty(PropertyNames.UniqueName);

            base.CreateProperties(context);
        }

        public class Manager
            : BehaviourManager<VictoryMarker>
        {
            public bool GameOver => Behaviours.Count > 0;

            public string? Winner => Behaviours.SingleOrDefault()?.Name;
        }
    }
}
