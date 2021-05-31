using System.Collections.Generic;
using Myre;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Simulation.Report.Curves;

namespace ShipCombatCore.Simulation.Behaviours.Recording
{
    public abstract class BaseRecordBoundedFloat16
        : Behaviour, IRecorder
    {
#pragma warning disable 8618
        private Property<float> _property;
        private BoundedFloat16PropertyCurve _curve;
#pragma warning restore 8618

        protected abstract TypedName<float> PropertyName { get; }

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _property = context.CreateProperty(PropertyName);
            _curve = new BoundedFloat16PropertyCurve(_property);

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
