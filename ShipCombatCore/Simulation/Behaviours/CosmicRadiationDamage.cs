using System;
using System.Numerics;
using Myre.Entities;
using Myre.Entities.Behaviours;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class CosmicRadiationDamage
        : ProcessBehaviour
    {
        private const float MaxDistance = 6000;
        private const float MaxRadiationDamage = 1;
        private const float BoostRadiationDamage = 5;

#pragma warning disable 8618
        private Property<YololContext> _context;
        private Property<Vector3> _position;
        private Property<float> _cosmicRadiation;
#pragma warning restore 8618

        private YololVariable? _rad;

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);
            _position = context.CreateProperty(PropertyNames.Position);
            _cosmicRadiation = context.CreateProperty(PropertyNames.CosmicRadiation);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            // Get Yolol context
            var ctx = _context.Value;   
            if (ctx == null)
                return;
            _rad ??= ctx.Get(":cosmic_radiation");

            // Calculate radiation based on how far from the origin the ship is
            // If ship is past danger limit give a big boost to damage
            var distance = _position.Value.Length();
            var rad = Math.Clamp(distance / MaxDistance, 0, 1);
            if (distance > MaxDistance)
                rad += BoostRadiationDamage;

            // Inform Yolol of value
            _cosmicRadiation.Value = rad;
            _rad.Value = (Number)(rad * 100);

            // Apply damage
            var damages = Owner.GetBehaviours<IDamageReceiver>();
            foreach (var item in damages)
                item.Damage(rad * MaxRadiationDamage * elapsedTime);
        }

        public class Manager
            : Manager<CosmicRadiationDamage>
        {
        }
    }
}