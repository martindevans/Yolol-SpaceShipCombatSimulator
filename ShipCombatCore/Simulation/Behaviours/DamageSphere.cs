using System;
using System.Numerics;
using Myre.Collections;
using Myre.Entities;
using Myre.Entities.Behaviours;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class DamageSphere
        : ProcessBehaviour
    {
        public const float DistanceScale = 0.05f;

        private bool _applied;

#pragma warning disable 8618
        private Property<float> _damage;
        private Property<float> _range;
        private Property<Vector3> _position;
#pragma warning restore 8618

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _damage = context.CreateProperty(PropertyNames.DamageSphereAmount);
            _range = context.CreateProperty(PropertyNames.DamageSphereRange);
            _position = context.CreateProperty(PropertyNames.Position);

            base.CreateProperties(context);
        }

        public override void Initialise(INamedDataProvider? initialisationData)
        {
            _damage.Value = initialisationData?.GetValue(PropertyNames.DamageSphereAmount) ?? 50000;
            _range.Value = initialisationData?.GetValue(PropertyNames.DamageSphereRange) ?? (float)Math.Sqrt(_damage.Value);

            base.Initialise(initialisationData);
        }

        protected override void Update(float elapsedTime)
        {
            if (_applied)
                return;
            _applied = true;

            var entities = Owner.Scene?.Entities ?? Array.Empty<Entity>();
            foreach (var entity in entities)
            {
                if (ReferenceEquals(entity, Owner))
                    continue;

                var damages = entity.GetBehaviours<IDamageReceiver>();
                if (damages.Count == 0)
                    continue;

                var pos = entity.GetProperty(PropertyNames.Position);
                if (pos == null)
                    continue;

                var distSqr = Vector3.DistanceSquared(pos.Value, _position.Value) * DistanceScale;
                if (distSqr > _range.Value * _range.Value)
                    continue;

                foreach (var item in damages)
                    item.Damage(_damage.Value / distSqr, DamageType.Explosion);
            }
        }

        private class Manager
            : Manager<DamageSphere>
        {
        }
    }

    public interface IDamageReceiver
    {
        void Damage(float damage, DamageType type);
    }

    public enum DamageType
    {
        Explosion,
        CosmicRadiation,
        ExtremeCosmicRadiation
    }
}
