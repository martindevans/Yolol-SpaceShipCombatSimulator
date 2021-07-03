using Myre.Collections;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Simulation.Services;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class CaptainsLog
        : ProcessBehaviour
    {
#pragma warning disable 8618
        private Property<YololContext> _context;
        private Property<uint> _team;
        private Property<string> _id;
#pragma warning restore 8618

        private SceneLogger? _logger;
        private IVariable? _log;

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);
            _team = context.CreateProperty(PropertyNames.TeamOwner);
            _id = context.CreateProperty(PropertyNames.UniqueName);
        }

        public override void Initialise(INamedDataProvider? initialisationData)
        {
            base.Initialise(initialisationData);

            _logger = Owner.Scene?.GetService<SceneLogger>();
        }

        protected override void Update(float elapsedTime)
        {
            if (_logger == null)
                return;

            var ctx = _context.Value;
            if (ctx == null)
                return;

            _log ??= ctx.Get(":log");

            var v = _log.Value;
            if (v.Type != Type.String)
                return;

            _logger.Log(_team.Value, _id.Value ?? "?", v.String);
            _log.Value = (Number)0;
        }

        public class Manager
            : Manager<CaptainsLog>
        {
        }
    }
}
