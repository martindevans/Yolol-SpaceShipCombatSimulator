using System;
using System.Numerics;
using Myre.Entities;
using Ninject;
using ShipCombatCore.Simulation.Behaviours;
using ShipCombatCore.Simulation.Behaviours.Recording;

namespace ShipCombatCore.Simulation.Entities
{
    public class ExplosionEntity
        : EntityDescription
    {
        public ExplosionEntity(IKernel kernel)
            : base(kernel)
        {
            AddProperty(PropertyNames.UniqueName);
            AddProperty(PropertyNames.EntityType, EntityType.Explosion);

            AddBehaviour<LifetimeLimit>();

            // Physics
            AddProperty(PropertyNames.Position);

            // Damage
            AddBehaviour<DamageSphere>();

            // Recording
            AddBehaviour<RecorderMaster>();
            AddBehaviour<RecordTransformPosition>();
        }

        public Entity Create(Vector3 position)
        {
            var e = base.Create();

            e.GetProperty(PropertyNames.UniqueName)!.Value = Guid.NewGuid().ToString();

            e.GetProperty(PropertyNames.Position)!.Value = position;

            return e;
        }
    }
}