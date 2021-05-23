using System;
using Myre.Entities;
using Ninject;
using ShipCombatCore.Simulation.Behaviours;
using ShipCombatCore.Simulation.Behaviours.Recording;

namespace ShipCombatCore.Simulation.Entities
{
    public class VictoryMarkerEntity
        : EntityDescription
    {
        public VictoryMarkerEntity(IKernel kernel)
            : base(kernel)
        {
            AddProperty(PropertyNames.UniqueName);
            AddProperty(PropertyNames.EntityType, EntityType.VictoryMarker);

            AddBehaviour<VictoryMarker>();

            AddBehaviour<RecorderMaster>();
            AddBehaviour<RecordTransformPosition>();
        }

        public Entity Create(string label)
        {
            var e = base.Create();

            e.GetProperty(PropertyNames.UniqueName)!.Value = $"Team '{label}' Wins!";

            return e;
        }
    }
}