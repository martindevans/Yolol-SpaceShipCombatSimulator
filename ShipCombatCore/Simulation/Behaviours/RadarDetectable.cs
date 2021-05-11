using System.Collections.Generic;
using System.Numerics;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Simulation.Entities;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class RadarDetectable
        : Behaviour
    {
#pragma warning disable 8618
        private Property<Vector3> _position;
        private Property<string> _uniqueId;
        private Property<EntityType> _type;
#pragma warning restore 8618

        public Vector3 Position => _position.Value;
        public string ID => _uniqueId.Value!;
        public EntityType Type => _type.Value;

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _position = context.CreateProperty(PropertyNames.Position);
            _uniqueId = context.CreateProperty(PropertyNames.UniqueName);
            _type = context.CreateProperty(PropertyNames.EntityType);

            base.CreateProperties(context);
        }

        public class Manager
            : BehaviourManager<RadarDetectable>
        {
            public IEnumerable<RadarDetectable> Detectable => Behaviours;
        }
    }
}
