using System.Numerics;
using Myre.Collections;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Helpers;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class MomentumWheels
        : ProcessBehaviour
    {
#pragma warning disable 8618
        private Property<YololContext> _context;

        private Property<float> _maxWheelTorque;

        private Property<Vector3> _torque;
#pragma warning restore 8618

        private IVariable? _torqueX;
        private IVariable? _torqueY;
        private IVariable? _torqueZ;

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);

            _maxWheelTorque = context.CreateProperty(PropertyNames.MaxWheelTorque);

            _torque = context.CreateProperty(PropertyNames.Torque);

            base.CreateProperties(context);
        }

        public override void Initialise(INamedDataProvider? initialisationData)
        {
            _maxWheelTorque.Value = initialisationData?.GetValue(PropertyNames.MaxWheelTorque) ?? 0.2f;

            base.Initialise(initialisationData);
        }

        protected override void Update(float elapsedTime)
        {
            var ctx = _context.Value;
            if (ctx == null)
                return;

            _torqueX ??= ctx.Get(":torque_x");
            _torqueY ??= ctx.Get(":torque_y");
            _torqueZ ??= ctx.Get(":torque_z");

            _torque.Value += new Vector3(
                GetNumber(_torqueX) * _maxWheelTorque.Value,
                GetNumber(_torqueY) * _maxWheelTorque.Value,
                GetNumber(_torqueZ) * _maxWheelTorque.Value
            );
        }

        private static float GetNumber(IVariable? v)
        {
            return YololValue.Number(v?.Value ?? (Number)0, -1, 1);
        }

        public class Manager
            : Manager<MomentumWheels>
        {
        }
    }
}
