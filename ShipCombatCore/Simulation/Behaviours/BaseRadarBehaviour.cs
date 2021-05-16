using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathHelperRedux;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Geometry;
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
            var entities = FindEntities(_direction.Value, angle.ToRadians());

            // Output results
            var count = ctx.Get(":radar_count");
            count.Value = (Number)entities.Count;

            var id = ctx.Get(":radar_out_id");
            var type = ctx.Get(":radar_out_type");
            var dist = ctx.Get(":radar_out_dist");
            
            var idx = (int)YololValue.Number(ctx.Get(":radar_idx").Value, -1, int.MaxValue);
            if (idx < 0 || idx >= entities.Count)
            {
                id.Value = "";
                dist.Value = 0;
                type.Value = "";
                _target.Value = Vector3.Zero;
            }
            else
            {
                var e = entities[idx];
                id.Value = e.Detectable.ID;
                type.Value = e.Detectable.Type.ToString();
                dist.Value = (Number)e.Dist;
                _target.Value = e.Detectable.Position;
            }
        }

        private List<RadarReturn> FindEntities(Vector3 beamDirWorld, float beamAngle)
        {
            var l = new List<RadarReturn>();
            if (Owner.Scene == null)
                return l;

            var sinAngle = (float)Math.Sin(beamAngle);
            var beam = new HalfRay3(_position.Value, beamDirWorld);

            var circles = new List<RadarCandidate>();
            foreach (var item in Owner.Scene.GetManager<RadarDetectable.Manager>().Detectable)
            {
                // Don't detect self
                if (ReferenceEquals(item.Owner, Owner))
                    continue;

                // Skip items which are too close
                if (Vector3.DistanceSquared(item.Position, _position.Value) < 1)
                    continue;

                // Skip items which are too far from the beam
                var pointOnBeam = beam.ClosestPoint(item.Position, out var distanceAlongBeam);
                var distFromBeam = Vector3.Distance(pointOnBeam, item.Position);

                // Calculate beam width at the given distance
                var beamWidth = sinAngle * distanceAlongBeam;

                // Check if the circle of the item intersects the beam
                if (distFromBeam - item.Radius - beamWidth > 0)
                    continue;

                // Project point onto plane and store projected circle in list
                // Scale down radius of circle by distance (i.e. width of the beam)
                circles.Add(new RadarCandidate(
                    Vector3.Normalize(item.Position - _position.Value),
                    MathF.Asin(item.Radius / Vector3.Distance(item.Position, _position.Value)),
                    distanceAlongBeam,
                    item
                ));
            }

            // Sort by distance before removing circles obscured by closer things
            circles.Sort((a, b) => a.Distance.CompareTo(b.Distance));

            // Remove circles which are totally obscured by earlier circles
            for (var i = 0; i < circles.Count; i++)
            {
                var c = circles[i];
                for (var j = circles.Count - 1; j > i; j--)
                    if (Obscures(c, circles[j]))
                        circles.RemoveAt(j);
            }

            // Convert into radar return objects
            l.AddRange(circles.Select(a => new RadarReturn(a.Item, a.Distance)));

            return l;
        }

        private bool Obscures(RadarCandidate close, RadarCandidate far)
        {
            var coneClose = new Cone(close.Direction, close.Angle);
            var coneFar = new Cone(far.Direction, far.Angle);

            return coneClose.Contains(coneFar);
        }

        private readonly struct RadarCandidate
        {
            public readonly Vector3 Direction;
            public readonly float Angle;
            public readonly float Distance;
            public readonly RadarDetectable Item;

            public RadarCandidate(Vector3 direction, float angle, float distance, RadarDetectable item)
            {
                Direction = direction;
                Angle = angle;
                Distance = distance;
                Item = item;
            }
        }

        private readonly struct RadarReturn
        {
            public float Dist { get; }
            public RadarDetectable Detectable { get; }

            public RadarReturn(RadarDetectable detectable, float dist)
            {
                Dist = dist;
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
