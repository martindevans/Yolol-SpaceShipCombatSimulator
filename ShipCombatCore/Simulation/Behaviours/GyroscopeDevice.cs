using System;
using System.Numerics;
using Myre.Entities;
using Myre.Entities.Behaviours;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class GyroscopeDevice
        : ProcessBehaviour
    {
#pragma warning disable 8618
        private Property<YololContext> _context;
        private Property<Quaternion> _orientation;
        private Property<Vector3> _angularVelocity;
#pragma warning restore 8618

        private YololVariable? _gyrow;
        private YololVariable? _gyrox;
        private YololVariable? _gyroy;
        private YololVariable? _gyroz;

        private YololVariable? _angvelx;
        private YololVariable? _angvely;
        private YololVariable? _angvelz;

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);
            _orientation = context.CreateProperty(PropertyNames.Orientation);
            _angularVelocity = context.CreateProperty(PropertyNames.AngularVelocity);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            var ctx = _context.Value;
            if (ctx == null)
                return;

            _gyrow ??= ctx.Get(":gyro_w");
            _gyrox ??= ctx.Get(":gyro_x");
            _gyroy ??= ctx.Get(":gyro_y");
            _gyroz ??= ctx.Get(":gyro_z");

            _gyrow.Value = (Number)_orientation.Value.W;
            _gyrox.Value = (Number)_orientation.Value.X;
            _gyroy.Value = (Number)_orientation.Value.Y;
            _gyroz.Value = (Number)_orientation.Value.Z;

            _angvelx ??= ctx.Get(":angular_vel_x");
            _angvely ??= ctx.Get(":angular_vel_y");
            _angvelz ??= ctx.Get(":angular_vel_z");

            _angvelx.Value = (Number)(_angularVelocity.Value.X * 10000);
            _angvely.Value = (Number)(_angularVelocity.Value.Y * 10000);
            _angvelz.Value = (Number)(_angularVelocity.Value.Z * 10000);
        }

        public class Manager
            : Manager<GyroscopeDevice>
        {
        }
    }
}
