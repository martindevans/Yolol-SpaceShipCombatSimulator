using System.Collections.Generic;
using System.Numerics;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Simulation.Report.Curves;

namespace ShipCombatCore.Simulation.Behaviours.Recording
{
    public class RecordTransformPosition
        : Behaviour, IRecorder
    {
#pragma warning disable 8618
        private Property<Vector3> _position;

        private Vector3PositionCurve _positionCurve;
#pragma warning restore 8618

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _position = context.CreateProperty(PropertyNames.Position);

            _positionCurve = new Vector3PositionCurve(_position);

            base.CreateProperties(context);
        }

        public void Record(uint ms)
        {
            _positionCurve.Extend(ms);
        }

        public IEnumerable<ICurve> Curves
        {
            get
            {
                yield return _positionCurve;
            }
        }
    }
}
