using Myre.Collections;
using Myre.Entities.Behaviours;

namespace ShipCombatCore.Simulation.Behaviours
{
    public class DamageInstantlyKills
        : Behaviour, IDamageReceiver
    {
        public void Damage(float damage, DamageType type)
        {
            if (damage >= 10 || type == DamageType.ExtremeCosmicRadiation)
                Owner.Dispose(new NamedBoxCollection());
        }
    }
}
