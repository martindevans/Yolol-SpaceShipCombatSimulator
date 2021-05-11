using System.Numerics;
using Myre.Entities;
using Myre.Entities.Behaviours;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class SpaceDrag
        : ProcessBehaviour
    {
#pragma warning disable 8618
        private Property<Vector3> _velocity;
        private Property<Vector3> _angularVelocity;
#pragma warning restore 8618

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _velocity = context.CreateProperty(PropertyNames.Velocity);
            _angularVelocity = context.CreateProperty(PropertyNames.AngularVelocity);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            _velocity.Value = Vector3.Lerp(_velocity.Value, Vector3.Zero, elapsedTime * 0.5f);
            _angularVelocity.Value = Vector3.Lerp(_angularVelocity.Value, Vector3.Zero, elapsedTime * 0.9f);
        }

        private class Manager
            : Manager<SpaceDrag>
        {
        }
    }
}
