using Myre.Collections;
using Myre.Entities.Behaviours;

namespace ShipCombatCore.Simulation.Behaviours
{
    public class DamageInstantlyKills
        : Behaviour, IDamageReceiver
    {
        public void Damage(float damage)
        {
            if (damage >= 10)
                Owner.Dispose(new NamedBoxCollection());
        }
    }
}
