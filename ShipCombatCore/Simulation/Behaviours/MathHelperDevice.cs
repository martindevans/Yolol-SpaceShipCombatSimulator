using System.Collections.Generic;
using System.Numerics;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Helpers;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class MathHelperDevice
        : ProcessBehaviour
    {
#pragma warning disable 8618
        private Property<YololContext> _context;

        private Property<Quaternion> _orientation;
#pragma warning restore 8618

        private YololVariable? _mode;
        private YololVariable[]? _params;

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);

            _orientation = context.CreateProperty(PropertyNames.Orientation);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            var ctx = _context.Value;   
            if (ctx == null)
                return;

            _mode ??= ctx.Get(":mathhelper_mode");
            if (_params == null)
            {
                _params = new[] {
                    ctx.Get(":mathhelper_a"),
                    ctx.Get(":mathhelper_b"),
                    ctx.Get(":mathhelper_c"),
                    ctx.Get(":mathhelper_d"),
                    ctx.Get(":mathhelper_e"),
                    ctx.Get(":mathhelper_f"),
                    ctx.Get(":mathhelper_g"),
                    ctx.Get(":mathhelper_h"),
                    ctx.Get(":mathhelper_i"),
                    ctx.Get(":mathhelper_j"),
                };
            }

            if (_mode.Value.Type != Type.String || _mode.Value == "")
                return;

            if (_mode.Value.Equals("reset"))
                Reset(_params);
            else if (_mode.Value.Equals("add"))
                Add(_params);
            else if (_mode.Value.Equals("world_dir"))
                WorldDir(_params);
            else if (_mode.Value.Equals("mulq"))
                MulQuat(_params);
            else if (_mode.Value.Equals("mulqv"))
                MulQuatVec(_params);

            _mode.Value = "";
        }

        private static void MulQuat(IReadOnlyList<YololVariable> parameters)
        {
            var q1 = new Quaternion(
                YololValue.Number(parameters[0].Value),
                YololValue.Number(parameters[1].Value),
                YololValue.Number(parameters[2].Value),
                YololValue.Number(parameters[3].Value)
            );
            var q2 = new Quaternion(
                YololValue.Number(parameters[4].Value),
                YololValue.Number(parameters[5].Value),
                YololValue.Number(parameters[6].Value),
                YololValue.Number(parameters[7].Value)
            );

            var m = q1 * q2;

            parameters[0].Value = (Number)m.W;
            parameters[1].Value = (Number)m.X;
            parameters[2].Value = (Number)m.Y;
            parameters[3].Value = (Number)m.Z;
        }

        private static void MulQuatVec(IReadOnlyList<YololVariable> parameters)
        {
            var q1 = LoadQuaternion(parameters, 0, 1, 2, 3);
            var v1 = LoadVector3(parameters, 4, 5, 6);

            var m = Vector3.Transform(v1, q1);

            parameters[4].Value = (Number)m.X;
            parameters[5].Value = (Number)m.Y;
            parameters[6].Value = (Number)m.Z;
        }

        private static void Reset(IReadOnlyList<YololVariable> parameters)
        {
            foreach (var variable in parameters)
                variable.Value = 0;
        }

        private static void Add(IReadOnlyList<YololVariable> parameters)
        {
            var acc = 0f;

            foreach (var variable in parameters)
                acc += YololValue.Number(variable.Value);

            parameters[0].Value = (Number)acc;
        }

        private void WorldDir(IReadOnlyList<YololVariable> parameters)
        {
            var bearing = YololValue.Number(parameters[0].Value, 0, 360);
            var elevation = YololValue.Number(parameters[1].Value, 0, 360);

            var bearingAxis = LoadVector3(parameters, 2, 3, 4);
            var elevationAxis = LoadVector3(parameters, 5, 6, 7);

            var dir = Targeting.WorldDirection(
                elevation,
                Vector3.Normalize(elevationAxis),
                bearing,
                Vector3.Normalize(bearingAxis),
                _orientation.Value
            );

            parameters[0].Value = (Number)dir.X;
            parameters[1].Value = (Number)dir.Y;
            parameters[2].Value = (Number)dir.Z;
        }

        private static Quaternion LoadQuaternion(IReadOnlyList<YololVariable> parameters, int w, int x, int y, int z)
        {
            return new Quaternion(
                YololValue.Number(parameters[w].Value),
                YololValue.Number(parameters[x].Value),
                YololValue.Number(parameters[y].Value),
                YololValue.Number(parameters[z].Value)
            );
        }

        private static Vector3 LoadVector3(IReadOnlyList<YololVariable> parameters, int x, int y, int z)
        {
            return new Vector3(
                YololValue.Number(parameters[x].Value),
                YololValue.Number(parameters[y].Value),
                YololValue.Number(parameters[z].Value)
            );
        }

        public class Manager
            : Manager<MathHelperDevice>
        {
        }
    }
}
