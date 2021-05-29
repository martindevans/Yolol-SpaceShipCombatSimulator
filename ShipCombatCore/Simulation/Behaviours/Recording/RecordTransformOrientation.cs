using System.Collections.Generic;
using System.Numerics;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Simulation.Report.Curves;

namespace ShipCombatCore.Simulation.Behaviours.Recording
{
    public class RecordTransformOrientation
        : Behaviour, IRecorder
    {
#pragma warning disable 8618
        private Property<Quaternion> _orientation;

        private QuaternionPropertyCurve _orientationCurve;
#pragma warning restore 8618

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _orientation = context.CreateProperty(PropertyNames.Orientation);

            _orientationCurve = new QuaternionPropertyCurve(_orientation);

            base.CreateProperties(context);
        }

        public void Record(uint ms)
        {
            _orientationCurve.Extend(ms);
        }

        public IEnumerable<ICurve> Curves
        {
            get
            {
                yield return _orientationCurve;
            }
        }
    }
}
