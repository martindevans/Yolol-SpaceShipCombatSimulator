using System.Collections.Generic;
using System.Numerics;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Simulation.Behaviours.Recording;
using ShipCombatCore.Simulation.Report.Curves;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class DebugGizmoRecorder
        : ProcessBehaviour, IRecorder
    {
#pragma warning disable 8618
        private Property<YololContext> _context;
        private Property<Vector3> _spherePos;
        private Property<float> _sphereRad;
        private Property<Vector3> _sphereColor;

        private Vector3PositionPropertyCurve _spherePositionCurve;
        private FloatPropertyCurve _sphereRadCurve;
        private Vector3PositionPropertyCurve _sphereColorCurve;
#pragma warning restore 8618

        private YololVariable? _spherePosXVar;
        private YololVariable? _spherePosYVar;
        private YololVariable? _spherePosZVar;
        private YololVariable? _sphereRadVar;
        private YololVariable? _sphereRadColRVar;
        private YololVariable? _sphereRadColGVar;
        private YololVariable? _sphereRadColBVar;

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);

            _spherePos = context.CreateProperty(PropertyNames.DebugSpherePosition);
            _sphereRad = context.CreateProperty(PropertyNames.DebugSphereRadius);
            _sphereColor = context.CreateProperty(PropertyNames.DebugSphereColor);

            _spherePositionCurve = new Vector3PositionPropertyCurve(_spherePos);
            _sphereRadCurve = new FloatPropertyCurve(_sphereRad);
            _sphereColorCurve = new Vector3PositionPropertyCurve(_sphereColor);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            var ctx = _context.Value;   
            if (ctx == null)
                return;

            _spherePosXVar ??= ctx.Get(":debug_sphere_pos_x");
            _spherePosYVar ??= ctx.Get(":debug_sphere_pos_y");
            _spherePosZVar ??= ctx.Get(":debug_sphere_pos_z");
            _sphereRadVar ??= ctx.Get(":debug_sphere_rad");
            _sphereRadColRVar ??= ctx.Get(":debug_sphere_r");
            _sphereRadColGVar ??= ctx.Get(":debug_sphere_g");
            _sphereRadColBVar ??= ctx.Get(":debug_sphere_b");

            _spherePos.Value = new Vector3(
                Helpers.YololValue.Number(_spherePosXVar.Value, -100000, 100000),
                Helpers.YololValue.Number(_spherePosYVar.Value, -100000, 100000),
                Helpers.YololValue.Number(_spherePosZVar.Value, -100000, 100000)
            );
            _sphereRad.Value = Helpers.YololValue.Number(_sphereRadVar.Value, 0, 10000);
            _sphereColor.Value = new Vector3(
                Helpers.YololValue.Number(_sphereRadColRVar.Value, 0, 1),
                Helpers.YololValue.Number(_sphereRadColGVar.Value, 0, 1),
                Helpers.YololValue.Number(_sphereRadColBVar.Value, 0, 1)
            );
        }

        public void Record(uint ms)
        {
            _spherePositionCurve.Extend(ms);
            _sphereRadCurve.Extend(ms);
            _sphereColorCurve.Extend(ms);
        }

        public IEnumerable<ICurve> Curves
        {
            get
            {
                yield return _spherePositionCurve;
                yield return _sphereRadCurve;
                yield return _sphereColorCurve;
            }
        }

        public class Manager
            : Manager<DebugGizmoRecorder>
        {
        }
    }
}
