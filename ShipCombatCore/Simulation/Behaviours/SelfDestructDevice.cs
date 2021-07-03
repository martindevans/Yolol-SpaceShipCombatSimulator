using Myre.Collections;
using Myre.Entities;
using Myre.Entities.Behaviours;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class SelfDestructDevice
        : ProcessBehaviour
    {
#pragma warning disable 8618
        private Property<YololContext> _context;
#pragma warning restore 8618

        private IVariable? _prime;
        private IVariable? _trigger;

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            var ctx = _context.Value;   
            if (ctx == null)
                return;

            _prime ??= ctx.Get(":self_destruct_prime");
            _trigger ??= ctx.Get(":self_destruct_trigger");

            if (_prime.Value.ToBool() && _trigger.Value.ToBool())
                Owner.Dispose(new NamedBoxCollection());
        }

        public class Manager
            : Manager<SelfDestructDevice>
        {
        }
    }
}