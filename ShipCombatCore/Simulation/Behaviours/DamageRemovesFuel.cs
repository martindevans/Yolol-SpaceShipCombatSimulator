using System;
using Myre.Entities;
using Myre.Entities.Behaviours;

namespace ShipCombatCore.Simulation.Behaviours
{
    public class DamageRemovesFuel
        : Behaviour, IDamageReceiver
    {
#pragma warning disable 8618
        private Property<float> _fuel;
#pragma warning restore 8618

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _fuel = context.CreateProperty(PropertyNames.FuelLitersInTank);

            base.CreateProperties(context);
        }

        public void Damage(float damage)
        {
            _fuel.Value = Math.Max(0, _fuel.Value - damage);
        }
    }
}
