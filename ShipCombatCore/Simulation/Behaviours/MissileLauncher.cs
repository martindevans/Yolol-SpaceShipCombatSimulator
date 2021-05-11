using System;
using System.Numerics;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Simulation.Entities;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class MissileLauncher
        : ProcessBehaviour
    {
        private const ushort CooldownTime = 4;
        private float _cooldownTime = CooldownTime;

#pragma warning disable 8618
        private Property<YololContext> _context;

        private Property<Vector3> _position;
        private Property<Vector3> _velocity;
        private Property<Quaternion> _orientation;
        private Property<Vector3> _angularVelocity;

        private Property<uint> _ammo;
#pragma warning restore 8618

        private YololVariable? _trigger;
        private YololVariable? _code;
        private YololVariable? _ready;
        private YololVariable? _ammoVar;

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);

            _position = context.CreateProperty(PropertyNames.Position);
            _velocity = context.CreateProperty(PropertyNames.Velocity);
            _orientation = context.CreateProperty(PropertyNames.Orientation);
            _angularVelocity = context.CreateProperty(PropertyNames.AngularVelocity);

            _ammo = context.CreateProperty(PropertyNames.MissileLauncherAmmo);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            _ammoVar ??= _context.Value!.Get(":missile_ammo");
            _ammoVar.Value = _ammo.Value;

            _cooldownTime -= elapsedTime;
            if (_cooldownTime > 0)
                return;

            _trigger ??= _context.Value!.Get(":missile_trigger");
            if (_trigger.Value <= 0)
                return;

            _code ??= _context.Value!.Get(":missile_code");
            var code = _code.Value.ToString();

            _ready ??= _context.Value!.Get(":missile_ready");
            if (_cooldownTime <= 0)
                _ready.Value = (Number)true;

            if (_ammo.Value == 0)
                return;

            var result = Parser.ParseProgram(code);
            var program = result.IsOk ? result.Ok : new Program(new Line[0]);
            Owner.Scene?.Add(new MissileEntity().Create(Guid.NewGuid().ToString(), _position.Value, _velocity.Value, _orientation.Value, _angularVelocity.Value, program));
            _trigger.Value--;
            _ammo.Value--;
            _ammoVar.Value = _ammo.Value;

            _cooldownTime = CooldownTime;
            _ready.Value = (Number)false;
        }

        private class Manager
            : Manager<MissileLauncher>
        {
        }
    }
}
