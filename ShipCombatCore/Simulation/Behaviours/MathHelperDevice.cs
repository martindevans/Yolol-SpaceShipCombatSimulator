using System;
using System.Collections.Generic;
using System.Numerics;
using MathHelperRedux;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Helpers;
using Yolol.Execution;
using Type = Yolol.Execution.Type;

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

        private IVariable? _mode;
        private IVariable[]? _params;

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
                    ctx.Get(":mathhelper_k"),
                    ctx.Get(":mathhelper_l"),
                    ctx.Get(":mathhelper_m"),
                    ctx.Get(":mathhelper_n"),
                    ctx.Get(":mathhelper_o"),
                    ctx.Get(":mathhelper_p"),
                    ctx.Get(":mathhelper_q"),
                    ctx.Get(":mathhelper_r"),
                    ctx.Get(":mathhelper_s"),
                    ctx.Get(":mathhelper_t"),
                    ctx.Get(":mathhelper_u"),
                    ctx.Get(":mathhelper_v"),
                    ctx.Get(":mathhelper_w"),
                    ctx.Get(":mathhelper_x"),
                    ctx.Get(":mathhelper_y"),
                    ctx.Get(":mathhelper_z"),
                };
            }

            if (_mode.Value.Type != Type.String || _mode.Value == "")
                return;

            Action<IReadOnlyList<IVariable>> func = _mode.Value.ToString() switch
            {
                "reset" => Reset,
                "shuffle" => Shuffle,
                "add" => Add,
                "world_dir" => WorldDir,
                
                "mulqv" => MulQuatVec,

                "mulqq" or "mulq" => MulQuatQuat,
                "qaxisangle" => QuatAxisAngle,
                "qypr" => QuatYPR,
                "qinv" => QuatInv,
                "dotqq" => DotQuatQuat,

                "dotvv" => DotVecVec,
                "crossvv" => CrossVecVec,

                _ => _ => { },
            };
            func(_params);

            _mode.Value = "";
        }

        private static void Shuffle(IReadOnlyList<IVariable> parameters)
        {
            var shuffle = parameters[25];
            if (shuffle.Value.Type == Type.Number)
                return;

            // Copy all values into temps, so by default nothing changes
            var temp = new Value[parameters.Count];
            for (var i = 0; i < parameters.Count; i++)
                temp[i] = parameters[i].Value;

            // Apply all commands one by one
            var index = 0;
            foreach (var character in shuffle.Value.ToString())
            {
                if (char.IsLetter(character))
                {
                    // It's a letter - copy that register into this one
                    var idx = char.ToLowerInvariant(character) - 97;
                    temp[index] = parameters[idx].Value;
                }
                else if (char.IsDigit(character))
                {
                    // It's a digit - put that number into this register
                    var val = character - 48;
                    temp[index] = (Number)val;
                }

                index++;
            }

            // Copy all temps back into values
            for (var i = 0; i < parameters.Count; i++)
                parameters[i].Value = temp[i];
        }

        private static void DotVecVec(IReadOnlyList<IVariable> parameters)
        {
            var v1 = LoadVector3(parameters, 0, 1, 2);
            var v2 = LoadVector3(parameters, 4, 5, 6);

            parameters[3].Value = (Number)Vector3.Dot(v1, v2);
        }

        private static void CrossVecVec(IReadOnlyList<IVariable> parameters)
        {
            var v1 = LoadVector3(parameters, 0, 1, 2);
            var v2 = LoadVector3(parameters, 4, 5, 6);

            var r = Vector3.Cross(v1, v2);

            parameters[7].Value = (Number)r.X;
            parameters[8].Value = (Number)r.Y;
            parameters[9].Value = (Number)r.Z;
        }

        private static void DotQuatQuat(IReadOnlyList<IVariable> parameters)
        {
            var q1 = LoadQuaternion(parameters, 0, 1, 2, 3);
            var q2 = LoadQuaternion(parameters, 4, 5, 6, 7);

            parameters[8].Value = (Number)Quaternion.Dot(q1, q2);
        }

        private static void QuatInv(IReadOnlyList<IVariable> parameters)
        {
            var q = LoadQuaternion(parameters, 0, 1, 2, 3);
            var qinv = Quaternion.Inverse(q);

            parameters[0].Value = (Number)qinv.W;
            parameters[1].Value = (Number)qinv.X;
            parameters[2].Value = (Number)qinv.Y;
            parameters[3].Value = (Number)qinv.Z;
        }

        private static void QuatAxisAngle(IReadOnlyList<IVariable> parameters)
        {
            var v = LoadVector3(parameters, 0, 1, 2);
            var a = LoadSingle(parameters, 3);

            var q = Quaternion.CreateFromAxisAngle(v, a.ToRadians());

            parameters[0].Value = (Number)q.W;
            parameters[1].Value = (Number)q.X;
            parameters[2].Value = (Number)q.Y;
            parameters[3].Value = (Number)q.Z;
        }

        private static void QuatYPR(IReadOnlyList<IVariable> parameters)
        {
            var v = LoadVector3(parameters, 0, 1, 2);

            var q = Quaternion.CreateFromYawPitchRoll(
                v.X.ToRadians(),
                v.Y.ToRadians(),
                v.Z.ToRadians()
            );

            parameters[0].Value = (Number)q.W;
            parameters[1].Value = (Number)q.X;
            parameters[2].Value = (Number)q.Y;
            parameters[3].Value = (Number)q.Z;
        }

        private static void MulQuatQuat(IReadOnlyList<IVariable> parameters)
        {
            var q1 = LoadQuaternion(parameters, 0, 1, 2, 3);
            var q2 =LoadQuaternion(parameters, 4, 5, 6, 7);

            var m = q1 * q2;

            parameters[0].Value = (Number)m.W;
            parameters[1].Value = (Number)m.X;
            parameters[2].Value = (Number)m.Y;
            parameters[3].Value = (Number)m.Z;
        }

        private static void MulQuatVec(IReadOnlyList<IVariable> parameters)
        {
            var q1 = LoadQuaternion(parameters, 0, 1, 2, 3);
            var v1 = LoadVector3(parameters, 4, 5, 6);

            var m = Vector3.Transform(v1, q1);

            parameters[4].Value = (Number)m.X;
            parameters[5].Value = (Number)m.Y;
            parameters[6].Value = (Number)m.Z;
        }

        private static void Reset(IReadOnlyList<IVariable> parameters)
        {
            foreach (var variable in parameters)
                variable.Value = (Number)0;
        }

        private static void Add(IReadOnlyList<IVariable> parameters)
        {
            var acc = 0f;

            foreach (var variable in parameters)
                acc += YololValue.Number(variable.Value);

            parameters[0].Value = (Number)acc;
        }

        private void WorldDir(IReadOnlyList<IVariable> parameters)
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

        private static Quaternion LoadQuaternion(IReadOnlyList<IVariable> parameters, int w, int x, int y, int z)
        {
            return new Quaternion(
                YololValue.Number(parameters[x].Value),
                YololValue.Number(parameters[y].Value),
                YololValue.Number(parameters[z].Value),
                YololValue.Number(parameters[w].Value)
            );
        }

        private static Vector3 LoadVector3(IReadOnlyList<IVariable> parameters, int x, int y, int z)
        {
            return new Vector3(
                YololValue.Number(parameters[x].Value),
                YololValue.Number(parameters[y].Value),
                YololValue.Number(parameters[z].Value)
            );
        }

        private static float LoadSingle(IReadOnlyList<IVariable> parameters, int v)
        {
            return YololValue.Number(parameters[v].Value);

        }

        public class Manager
            : Manager<MathHelperDevice>
        {
        }
    }
}
