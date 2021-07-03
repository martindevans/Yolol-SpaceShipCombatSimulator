using Myre.Entities;
using Myre.Entities.Behaviours;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class FuelTank
        : ProcessBehaviour, IMassProvider
    {
        public const float FuelDensity = 1;

#pragma warning disable 8618
        private Property<YololContext> _context;
        private Property<float> _fuelLitersInTank;
#pragma warning restore 8618

        private IVariable? _fuel;
        
        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);
            _fuelLitersInTank = context.CreateProperty(PropertyNames.FuelLitersInTank);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            var ctx = _context.Value;
            if (ctx == null)
                return;

            _fuel ??= ctx.Get(":fuel");
            _fuel.Value = (Number)_fuelLitersInTank.Value;
        }

        public class Manager
            : Manager<FuelTank>
        {
        }

        public float Mass => _fuelLitersInTank.Value * FuelDensity;
    }
}
