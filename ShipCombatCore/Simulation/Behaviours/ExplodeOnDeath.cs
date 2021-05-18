using System.Numerics;
using Myre.Collections;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Simulation.Entities;

namespace ShipCombatCore.Simulation.Behaviours
{
    public class ExplodeOnDeath
        : Behaviour
    {
#pragma warning disable 8618
        private Property<Vector3> _position;
#pragma warning restore 8618

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _position = context.CreateProperty(PropertyNames.Position);
        }

        public override void Shutdown(INamedDataProvider? shutdownData)
        {
            base.Shutdown(shutdownData);

            Owner.Scene?.Add(new ExplosionEntity(Owner.Scene.Kernel).Create(_position.Value));
        }
    }
}
