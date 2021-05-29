using System.Numerics;
using MathHelperRedux;
using Myre.Entities;
using Myre.Entities.Behaviours;
using Type = Yolol.Execution.Type;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class Engine
        : ProcessBehaviour
    {
        private static readonly Vector3 Forward = new Vector3(Constants.ShipLocalForwardX, Constants.ShipLocalForwardY, Constants.ShipLocalForwardZ);

#pragma warning disable 8618
        private Property<YololContext> _context;

        private Property<float> _fuelLitersInTank;
        private Property<float> _fuelConsumptionRate;
        private Property<float> _maxEngineForce;

        private Property<float> _actualEngineThrottle;

        private Property<Vector3> _force;
        private Property<Quaternion> _orientation;
#pragma warning restore 8618

        private YololVariable? _throttle;
        
        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);

            _fuelLitersInTank = context.CreateProperty(PropertyNames.FuelLitersInTank);
            _fuelConsumptionRate = context.CreateProperty(PropertyNames.FuelConsumptionRate);
            _maxEngineForce = context.CreateProperty(PropertyNames.MaxEngineForce);

            _actualEngineThrottle = context.CreateProperty(PropertyNames.ActualThrottle);

            _force = context.CreateProperty(PropertyNames.Force);
            _orientation = context.CreateProperty(PropertyNames.Orientation);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            var t = CalculateThrottle();
            _actualEngineThrottle.Value = t;

            if (t <= 0)
                return;

            // Update fuel tank
            var fuel = _fuelConsumptionRate.Value * t * elapsedTime;
            if (_fuelLitersInTank.Value < fuel)
            {
                _actualEngineThrottle.Value = 0;
                _fuelLitersInTank.Value = 0;
                return;
            }

            _fuelLitersInTank.Value -= fuel;

            // Apply force
            var fwd = Vector3.Transform(Forward, _orientation.Value);
            _force.Value += fwd * _maxEngineForce.Value * t;
            
        }

        private float CalculateThrottle()
        {
            var ctx = _context.Value;
            if (ctx == null)
                return 0;

            _throttle ??= ctx.Get(":throttle");
            var throttle = _throttle.Value;

            if (throttle.Type != Type.Number)
                return 0;

            if (throttle <= 0)
                return 0;

            return MathHelper.Clamp((float)throttle.Number, 0, 1);
        }

        public class Manager
            : Manager<Engine>
        {
        }
    }
}
