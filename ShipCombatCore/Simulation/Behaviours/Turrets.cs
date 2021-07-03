using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Helpers;
using ShipCombatCore.Simulation.Behaviours.Recording;
using ShipCombatCore.Simulation.Entities;
using ShipCombatCore.Simulation.Report.Curves;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class Turrets
        : ProcessBehaviour, IRecorder
    {
        private readonly ShellEntity _shellFactory;
        private const ushort TurretCount = 4;

        private const float MinElevation = Constants.TurretMinElevation;
        private const float MaxElevation = Constants.TurretMaxElevation;
        private const float ElevationSpeed = Constants.TurretElevationSpeed;
        private const float BearingSpeed = Constants.TurretBearingSpeed;

        private const float MinFuse = Constants.TurretMinFuse;
        private const float MaxFuse = Constants.TurretMaxFuse;

        private const float ShellSpeed = Constants.TurretShellSpeed;
        private const float CooldownTime = Constants.TurretRefireTime;

        private readonly List<Turret> _turrets = new();

        public IEnumerable<ICurve> Curves => _turrets.SelectMany(t => t.Curves);

        private Property<YololContext> _context;

#pragma warning disable 8618
        public Turrets(ShellEntity shellFactory)
#pragma warning restore 8618
        {
            _shellFactory = shellFactory;
        }

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);

            for (var i = 0; i < TurretCount; i++)
                _turrets.Add(new Turret(_shellFactory, i));

            foreach (var turret in _turrets)
                turret.CreateProperties(context);

            base.CreateProperties(context);
        }

        public void Record(uint ms)
        {
            foreach (var turret in _turrets)
                turret.Record(ms);
        }

        protected override void Update(float elapsedTime)
        {
            var ctx = _context.Value;
            if (ctx == null)
                return;

            foreach (var turret in _turrets)
                turret.Update(elapsedTime, ctx, Owner.Scene!);
        }

        private class Turret
            : IRecorder
        {
            private readonly ShellEntity _shellFactory;
            private readonly int _index;

            private BoundedFloat16PropertyCurve _bearingCurve;
            private BoundedFloat16PropertyCurve _elevationCurve;

            private Property<uint> _team;

            private Property<Vector3> _position;
            private Property<Vector3> _velocity;
            private Property<Quaternion> _orientation;

            private Property<float> _bearing;
            private Property<float> _elevation;

            private float _cooldownTime;

            private readonly string _elevationName;
            private readonly string _bearingName;
            private readonly string _actualElevationName;
            private readonly string _actualBearingName;
            private readonly string _gunReadyName;
            private readonly string _gunTriggerName;
            private readonly string _gunFuseName;

#pragma warning disable 8618
            public Turret(ShellEntity shellFactory, int index)
#pragma warning restore 8618
            {
                _shellFactory = shellFactory;
                _index = index;

                _elevationName = $":tgt_gun_elevation_{_index}";
                _bearingName = $":tgt_gun_bearing_{_index}";
                _actualElevationName = $":gun_elevation_{_index}";
                _actualBearingName = $":gun_bearing_{_index}";
                _gunReadyName = $":gun_ready_{_index}";
                _gunTriggerName = $":gun_trigger_{_index}";
                _gunFuseName = $":gun_fuse_{_index}";
            }

            public void CreateProperties(Entity.ConstructionContext context)
            {
                _team = context.CreateProperty(PropertyNames.TeamOwner);

                _position = context.CreateProperty(PropertyNames.Position);
                _velocity = context.CreateProperty(PropertyNames.Velocity);
                _orientation = context.CreateProperty(PropertyNames.Orientation);

                _bearing = context.CreateProperty(new Myre.TypedName<float>($"gun_turret_bearing_{_index}"));
                _elevation = context.CreateProperty(new Myre.TypedName<float>($"gun_turret_elevation_{_index}"));

                _bearingCurve = new BoundedFloat16PropertyCurve(_bearing);
                _elevationCurve = new BoundedFloat16PropertyCurve(_elevation);
            }

            public void Update(float elapsedTime, YololContext ctx, Scene scene)
            {
                // Change gun angles
                var tgtElevation = ctx.Get(_elevationName);
                _elevation.Value = MoveTo(_elevation.Value, YololValue.Number(tgtElevation.Value, MinElevation, MaxElevation), ElevationSpeed * elapsedTime);
                var tgtBearing = ctx.Get(_bearingName);
                _bearing.Value = MoveToAngle(_bearing.Value, YololValue.Number(tgtBearing.Value, 0, 360), BearingSpeed * elapsedTime);

                // Copy angles back to Yolol
                var actualElevation = ctx.Get(_actualElevationName);
                actualElevation.Value = (Number)_elevation.Value;
                var actualBearing = ctx.Get(_actualBearingName);
                actualBearing.Value = (Number)_bearing.Value;

                // Check if gun is ready
                var ready = ctx.Get(_gunReadyName);
                _cooldownTime -= elapsedTime;
                if (_cooldownTime <= 0)
                    ready.Value = (Number)true;

                // Fire gun
                var trigger = ctx.Get(_gunTriggerName);
                var t = trigger.Value;
                if (_cooldownTime <= 0 && t.Type == Yolol.Execution.Type.Number && t.Number > (Number)0) 
                {
                    trigger.Value = t - (Number)1;
                    _cooldownTime = CooldownTime;
                    ready.Value = (Number)false;

                    // Work out what the fuse setting is for this gun and then spawn a shell
                    var fuseVar = ctx.Get(_gunFuseName);
                    var fuse = Math.Clamp(fuseVar.Value.Type == Yolol.Execution.Type.Number ? (float)fuseVar.Value.Number : 0, MinFuse, MaxFuse);
                    scene.Add(_shellFactory.Create(fuse, _team.Value, _position.Value, _velocity.Value + GunDirection() * ShellSpeed));
                }
            }

            private static float MoveTo(float current, float target, float maxDelta)
            {
                // If close to target angle snap to final angle
                var delta = Math.Abs(current - target);
                if (delta < maxDelta)
                    return target;

                // Move to target point
                if (target < current)
                    return current - maxDelta;
                else
                    return current + maxDelta;
            }

            private static float MoveTowards(float current, float target, float maxDelta)
            {
                return Math.Abs(target - current) <= maxDelta
                    ? target
                    : current + Math.Sign(target - current) * maxDelta;
            }

            private static float MoveToAngle(float current, float target, float maxDelta)
            {
                var num = DeltaAngle(current, target);
                if (-maxDelta < num && num < maxDelta)
                    return target;
                target = current + num;
                return MoveTowards(current, target, maxDelta);
            }

            private static float DeltaAngle(float current, float target)
            {
                var num = Repeat(target - current, 360f);
                if ((double) num > 180.0)
                    num -= 360f;
                return num;
            }

            private static float Repeat(float t, float length)
            {
                return Math.Clamp(t - (float)Math.Floor(t / length) * length, 0.0f, length);
            }

            private Vector3 GunDirection()
            {
                return Targeting.WorldDirection(_elevation.Value, new Vector3(0, 0, -1), _bearing.Value, new Vector3(1, 0, 0), _orientation.Value);
            }

            public void Record(uint ms)
            {
                _bearingCurve.Extend(ms);
                _elevationCurve.Extend(ms);
            }

            public IEnumerable<ICurve> Curves
            {
                get
                {
                    yield return _bearingCurve;
                    yield return _elevationCurve;
                }
            }
        }

        public class Manager
            : Manager<Turrets>
        {
        }
    }
}
