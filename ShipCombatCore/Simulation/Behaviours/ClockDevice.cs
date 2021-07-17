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

        private IVariable? _clock;
        private IVariable? _clockDt;

        public double Time { get; private set; }

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            Time += elapsedTime;

            var ctx = _context.Value;   
            if (ctx == null)
                return;

            _clock ??= ctx.Get(":clock");
            _clock.Value = (Number)Time;

            _clockDt ??= ctx.Get(":clock_dt");
            _clockDt.Value = (Number)elapsedTime;
        }

        public class Manager
            : Manager<ClockDevice>
        {
        }
    }
}