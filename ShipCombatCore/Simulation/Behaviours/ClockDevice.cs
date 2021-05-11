using Myre.Entities;
using Myre.Entities.Behaviours;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class ClockDevice
        : ProcessBehaviour
    {
#pragma warning disable 8618
        private Property<YololContext> _context;
#pragma warning restore 8618

        private double _elapsed;
        private YololVariable? _clock;

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

            _clock = ctx.Get(":clock");

            _elapsed += elapsedTime;
            _clock.Value = (Number)_elapsed;
        }

        public class Manager
            : Manager<ClockDevice>
        {
        }
    }
}