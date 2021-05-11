using System;
using System.Collections.Generic;
using System.Numerics;
using MathHelperRedux;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Helpers;
using ShipCombatCore.Simulation.Behaviours.Recording;
using ShipCombatCore.Simulation.Report.Curves;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public abstract class BaseRadarBehaviour
        : ProcessBehaviour, IRecorder
    {
#pragma warning disable 8618
        private Property<YololContext> _context;

        private Property<Vector3> _position;
        private Property<Quaternion> _orientation;

        private Property<Vector3> _target;
        private Property<Vector3> _direction;
        private Property<float> _angle;
        private Property<float> _range;

        private Vector3DirectionCurve _directionCurve;
        private Vector3PositionCurve _targetCurve;
        private FloatCurve _angleCurve;
        private FloatCurve _rangeCurve;
#pragma warning restore 8618

        public IEnumerable<ICurve> Curves
        {
            get
            {
                yield return _directionCurve;
                yield return _angleCurve;
                yield return _rangeCurve;
                yield return _targetCurve;
            }
        }

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _position = context.CreateProperty(PropertyNames.Position);
            _orientation = context.CreateProperty(PropertyNames.Orientation);

            _context = context.CreateProperty(PropertyNames.YololContext);

            _direction = context.CreateProperty(PropertyNames.RadarDirection);
            _directionCurve = new Vector3DirectionCurve(_direction);

            _angle = context.CreateProperty(PropertyNames.RadarAngle);
            _angleCurve = new FloatCurve(_angle);

            _range = context.CreateProperty(PropertyNames.RadarRange);
            _rangeCurve = new FloatCurve(_range);

            _target = context.CreateProperty(PropertyNames.RadarTarget);
            _targetCurve = new Vector3PositionCurve(_target);

            base.CreateProperties(context);
        }

        public void Record(uint ms)
        {
            _directionCurve.Extend(ms);
            _angleCurve.Extend(ms);
            _rangeCurve.Extend(ms);
            _targetCurve.Extend(ms);
        }

        protected abstract float Elevation(YololContext ctx);

        protected abstract float Bearing(YololContext ctx);

        protected abstract float BeamAngle(YololContext ctx);

        protected abstract float BeamRange(YololContext ctx);

        protected override void Update(float elapsedTime)
        {
            var ctx = _context.Value;
            if (ctx == null)
                return;

            var elevation = Elevation(ctx);
            var bearing = Bearing(ctx);
            var angle = BeamAngle(ctx);
            var range = BeamRange(ctx);

            // Update properties (and thus curves)
            _direction.Value = RadarDirection(elevation, bearing);
            _range.Value = range;
            _angle.Value = angle;

            // Find entities along the beam
            var entities = FindEntities(_direction.Value, (float)Math.Cos(angle * angle.ToRadians()), range);

            // Output results
            var count = ctx.Get(":radar_count");
            count.Value = (Number)entities.Count;

            var id = ctx.Get(":radar_out_id");
            var type = ctx.Get(":radar_out_type");
            var dist = ctx.Get(":radar_out_dist");
            var dot = ctx.Get(":radar_out_dot");
            
            var idx = (int)YololValue.Number(ctx.Get(":radar_idx").Value, -1, int.MaxValue);
            if (idx < 0 || idx >= entities.Count)
            {
                id.Value = "";
                dist.Value = 0;
                dot.Value = 0;
                type.Value = "";
                _target.Value = Vector3.Zero;
            }
            else
            {
                var e = entities[idx];
                id.Value = e.Detectable.ID;
                type.Value = e.Detectable.Type.ToString();
                dist.Value = (Number)e.Dist;
                dot.Value = (Number)e.Dot;
                _target.Value = e.Detectable.Position;
            }
        }

        private List<RadarReturn> FindEntities(Vector3 beamDirWorld, float dotMin, float rangeMax)
        {
            var l = new List<RadarReturn>();
            if (Owner.Scene == null)
                return l;

            foreach (var item in Owner.Scene.GetManager<RadarDetectable.Manager>().Detectable)
            {
                if (ReferenceEquals(item.Owner, Owner))
                    continue;

                var vec = item.Position - _position.Value;
                var dist = vec.Length();
                if (dist > rangeMax)
                    continue;
                vec /= dist;

                var dot = Vector3.Dot(beamDirWorld, vec);
                if (dot < dotMin)
                    continue;

                l.Add(new RadarReturn(item, dist, dot));
            }

            // Sort by distance, so index 0 is always the closest item
            l.Sort((a, b) => a.Dist.CompareTo(b.Dist));

            return l;
        }

        private readonly struct RadarReturn
        {
            public float Dist { get; }
            public float Dot { get; }
            public RadarDetectable Detectable { get; }

            public RadarReturn(RadarDetectable detectable, float dist, float dot)
            {
                Dist = dist;
                Dot = dot;
                Detectable = detectable;
            }
        }

        private Vector3 RadarDirection(float elevation, float bearing)
        {
            return Vector3.Normalize(Targeting.WorldDirection(
                elevation,
                new Vector3(0, 0, -1),
                bearing,
                new Vector3(1, 0, 0),
                _orientation.Value
            ));
        }

        private class Manager
            : Manager<BaseRadarBehaviour>
        {
        }
    }
}
