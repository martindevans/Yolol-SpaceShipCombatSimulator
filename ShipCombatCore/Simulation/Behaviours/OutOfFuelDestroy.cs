using System;
using System.Numerics;
using Myre.Collections;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Simulation.Entities;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class OutOfFuelDestroy
        : ProcessBehaviour
    {
#pragma warning disable 8618
        private Property<float> _fuel;
#pragma warning restore 8618

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _fuel = context.CreateProperty(PropertyNames.FuelLitersInTank);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            if (_fuel.Value > 0)
                return;

            // Dispose this entity, it's been replaced
            Owner.Dispose(new NamedBoxCollection());
        }

        private class Manager
            : Manager<OutOfFuelDestroy>
        {
        }
    }
}
