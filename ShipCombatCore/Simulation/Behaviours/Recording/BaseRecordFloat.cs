using System.Collections.Generic;
using Myre;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Simulation.Report.Curves;

namespace ShipCombatCore.Simulation.Behaviours.Recording
{
    public abstract class BaseRecordFloat
        : Behaviour, IRecorder
    {
#pragma warning disable 8618
        private Property<float> _property;
        private FloatPropertyCurve _curve;
#pragma warning restore 8618

        protected abstract TypedName<float> PropertyName { get; }

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _property = context.CreateProperty(PropertyName);
            _curve = new FloatPropertyCurve(_property);

            base.CreateProperties(context);
        }

        public void Record(uint ms)
        {
            _curve.Extend(ms);
        }

        public IEnumerable<ICurve> Curves
        {
            get
            {
                yield return _curve;
            }
        }
    }
}
