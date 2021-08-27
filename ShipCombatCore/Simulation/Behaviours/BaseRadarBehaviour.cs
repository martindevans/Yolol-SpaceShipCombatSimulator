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
using ShipCombatCore.Simulation.Entities;
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

        private Vector3DirectionPropertyCurve _directionCurve;
        private Vector3PositionPropertyCompoundCurve _targetCurve;
        private BoundedFloat16PropertyCurve _angleCurve;
        private FloatPropertyCurve _rangeCurve;
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

        private List<RadarReturn> _lastScanData = new();

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _position = context.CreateProperty(PropertyNames.Position);
            _orientation = context.CreateProperty(PropertyNames.Orientation);

            _context = context.CreateProperty(PropertyNames.YololContext);

            _direction = context.CreateProperty(PropertyNames.RadarDirection);
            _directionCurve = new Vector3DirectionPropertyCurve(_direction);
            _direction.Value = RadarDirection(0, 0);

            _angle = context.CreateProperty(PropertyNames.RadarAngle);
            _angleCurve = new BoundedFloat16PropertyCurve(_angle);

            _range = context.CreateProperty(PropertyNames.RadarRange);
            _rangeCurve = new FloatPropertyCurve(_range);

            _target = context.CreateProperty(PropertyNames.RadarTarget);
            _targetCurve = new Vector3PositionPropertyCompoundCurve(_target);

            base.CreateProperties(context);
        }

        protected override void Initialised()
        {
            base.Initialised();

            var ctx = _context.Value;
            if (ctx != null)
                Initialised(ctx);
        }

        protected virtual void Initialised(YololContext ctx)
        {
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

            var trigger = ctx.Get(":radar_trigger");
            if (trigger.Value.ToBool())
            {
                trigger.Value = (Number)0;

                var elevation = Elevation(ctx);
                var bearing = Bearing(ctx);
                var angle = BeamAngle(ctx);
                var range = BeamRange(ctx);

                // Update properties (and thus curves)
                _direction.Value = RadarDirection(elevation, bearing);
                _range.Value = range;
                _angle.Value = angle;

                ctx.Get(":radar_dir_x").Value = (Number)_direction.Value.X;
                ctx.Get(":radar_dir_y").Value = (Number)_direction.Value.Y;
                ctx.Get(":radar_dir_z").Value = (Number)_direction.Value.Z;

                // Find entities along the beam
                _lastScanData = FindEntities(_direction.Value, angle.ToRadians());

                // Apply the radar filter blacklist
                var filterVal = ctx.Get(":radar_filter").Value;
                if (filterVal.Type == Yolol.Execution.Type.String)
                {
                    var filterString = filterVal.ToString();
                    for (var i = _lastScanData.Count - 1; i >= 0; i--)
                    {
                        var item = _lastScanData[i];
                        if (filterString.Contains(item.Detectable.Type.ToEnumString()) || filterString.Contains(item.Detectable.ID))
                            _lastScanData.RemoveAt(i);
                    }
                }
            }

            // Output results
            var count = ctx.Get(":radar_count");
            count.Value = (Number)_lastScanData.Count;

            var id = ctx.Get(":radar_out_id");
            var type = ctx.Get(":radar_out_type");
            var dist = ctx.Get(":radar_out_dist");
            
            var idx = (int)YololValue.Number(ctx.Get(":radar_idx").Value, -1, int.MaxValue);
            if (idx < 0 || idx >= _lastScanData.Count)
            {
                id.Value = "";
                dist.Value = (Number)0;
                type.Value = "";
                _target.Value = Vector3.Zero;
            }
            else
            {
                var e = _lastScanData[idx];
                id.Value = e.Detectable.ID;
                type.Value = e.Detectable.Type.ToEnumString();
                dist.Value = (Number)e.Dist;
                _target.Value = e.Detectable.Position;
            }
        }

        private List<RadarReturn> FindEntities(Vector3 beamDirWorld, float beamAngle)
        {
            var l = new List<RadarReturn>();
            if (Owner.Scene == null)
                return l;

            var tanHalfAngle = MathF.Tan(beamAngle / 2);
            var beam = new HalfRay3(_position.Value, beamDirWorld);

            var candidates = new List<RadarCandidate>();
            foreach (var item in Owner.Scene.GetManager<RadarDetectable.Manager>().Detectable)
            {
                // Don't detect self
                if (ReferenceEquals(item.Owner, Owner))
                    continue;

                // Skip items which are too close
                var vectorToItem = item.Position - _position.Value;
                var distanceToItem = vectorToItem.Length();
                if (distanceToItem < 1)
                    continue;

                // Skip items which are too far from the beam
                var pointOnBeam = beam.ClosestPoint(item.Position, out var distanceAlongBeam);
                var distFromBeam = Vector3.Distance(pointOnBeam, item.Position);

                // Calculate beam width at the given distance
                var beamRadius = tanHalfAngle * distanceAlongBeam;

                // Check if the sphere of the item intersects the beam
                if (distFromBeam - item.Radius - beamRadius > 0)
                    continue;

                // Store candidate for later filtering
                candidates.Add(new RadarCandidate(
                    vectorToItem / distanceToItem,
                    MathF.Asin(item.Radius / distanceToItem),
                    distanceToItem - item.Radius,
                    item
                ));
            }

            // Sort by distance before removing circles obscured by closer things
            candidates.Sort((a, b) => a.Distance.CompareTo(b.Distance));

            // Remove circles which are totally obscured by earlier circles
            for (var i = 0; i < candidates.Count; i++)
            {
                var c = candidates[i];
                for (var j = candidates.Count - 1; j > i; j--)
                    if (Obscures(c, candidates[j]))
                        candidates.RemoveAt(j);
            }

            // Convert into radar return objects
            l.AddRange(candidates.Select(a => new RadarReturn(a.Item, a.Distance)));

            return l;
        }

        private static bool Obscures(RadarCandidate close, RadarCandidate far)
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
