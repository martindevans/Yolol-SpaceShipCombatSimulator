using System.Collections.Generic;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Simulation.Entities;

namespace ShipCombatCore.Simulation.Behaviours
{
    public class TeamMember
        : Behaviour
    {
#pragma warning disable 8618
        private Property<uint> _team;
        private Property<EntityType> _type;
#pragma warning restore 8618

        public uint Team => _team.Value;
        public EntityType Type => _type.Value;

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _team = context.CreateProperty(PropertyNames.TeamOwner);
            _type = context.CreateProperty(PropertyNames.EntityType);

            base.CreateProperties(context);
        }

        public class Manager
            : BehaviourManager<TeamMember>
        {
            public new IReadOnlyList<TeamMember> Behaviours => base.Behaviours;
        }
    }
}
