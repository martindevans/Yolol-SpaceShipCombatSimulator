using System.Numerics;
using Myre.Entities;
using Myre.Entities.Behaviours;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class PositioningDevice
        : ProcessBehaviour
    {
#pragma warning disable 8618
        private Property<YololContext> _context;
        private Property<Vector3> _velocity;
        private Property<Vector3> _position;
#pragma warning restore 8618

        private YololVariable? _accelx;
        private YololVariable? _accely;
        private YololVariable? _accelz;
        private Vector3 _prevVel;

        private YololVariable? _velx;
        private YololVariable? _vely;
        private YololVariable? _velz;

        private YololVariable? _posx;
        private YololVariable? _posy;
        private YololVariable? _posz;

        private Vector3? _initialPosition;

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);
            _velocity = context.CreateProperty(PropertyNames.Velocity);
            _position = context.CreateProperty(PropertyNames.Position);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            if (!_initialPosition.HasValue)
                _initialPosition = _position.Value;

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

            _velx ??= ctx.Get(":vel_x");
            _vely ??= ctx.Get(":vel_y");
            _velz ??= ctx.Get(":vel_z");

            _velx.Value = (Number)_velocity.Value.X;
            _vely.Value = (Number)_velocity.Value.Y;
            _velz.Value = (Number)_velocity.Value.Z;

            _posx ??= ctx.Get(":pos_x");
            _posy ??= ctx.Get(":pos_y");
            _posz ??= ctx.Get(":pos_z");

            var offsetPos = _position.Value - _initialPosition.Value;
            _posx.Value = (Number)offsetPos.X;
            _posy.Value = (Number)offsetPos.Y;
            _posz.Value = (Number)offsetPos.Z;
        }

        public class Manager
            : Manager<PositioningDevice>
        {
        }
    }
}
