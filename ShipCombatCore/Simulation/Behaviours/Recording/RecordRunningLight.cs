using System.Collections.Generic;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Simulation.Report.Curves;

namespace ShipCombatCore.Simulation.Behaviours.Recording
{
    public class RecordRunningLight
        : Behaviour, IRecorder
    {
#pragma warning disable 8618
        private Property<YololContext> _context;

        private Property<bool> _lightState;
        private BoolPropertyCurve _lightCurve;
#pragma warning restore 8618

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);

            _lightState = context.CreateProperty(PropertyNames.RunningLightState);
            _lightCurve = new BoolPropertyCurve(_lightState);

            base.CreateProperties(context);
        }

        public void Record(uint ms)
        {
            var state = _context.Value?.Get(":light").Value.ToBool() ?? false;
            _lightState.Value = state;
            _lightCurve.Extend(ms);
        }

        public IEnumerable<ICurve> Curves
        {
            get
            {
                yield return _lightCurve;
            }
        }
    }
}