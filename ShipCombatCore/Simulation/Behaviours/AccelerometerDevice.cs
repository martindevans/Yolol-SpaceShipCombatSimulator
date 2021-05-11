using System;
using System.Numerics;
using Myre.Entities;
using Myre.Entities.Behaviours;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class AccelerometerDevice
        : ProcessBehaviour
    {
#pragma warning disable 8618
        private Property<YololContext> _context;
        private Property<Vector3> _velocity;
#pragma warning restore 8618

        private YololVariable? _accelx;
        private YololVariable? _accely;
        private YololVariable? _accelz;
        private Vector3 _prevVel;

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);
            _velocity = context.CreateProperty(PropertyNames.Velocity);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            var acceleration = _velocity.Value - _prevVel;
            _prevVel = _velocity.Value;
            
            var ctx = _context.Value;   
            if (ctx == null)
                return;

            _accelx ??= ctx.Get(":accel_x");
            _accely ??= ctx.Get(":accel_y");
            _accelz ??= ctx.Get(":accel_z");

            _accelx.Value = (Number)acceleration.X;
            _accely.Value = (Number)acceleration.Y;
            _accelz.Value = (Number)acceleration.Z;
        }

        public class Manager
            : Manager<AccelerometerDevice>
        {
        }
    }
}
