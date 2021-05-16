using System;
using Myre.Entities;
using ShipCombatCore.Simulation.Behaviours;
using ShipCombatCore.Simulation.Behaviours.Recording;

namespace ShipCombatCore.Simulation.Entities
{
    public class VictoryMarkerEntity
        : EntityDescription
    {
        public VictoryMarkerEntity()
        {
            AddProperty(PropertyNames.UniqueName, Guid.NewGuid().ToString());
            AddProperty(PropertyNames.EntityType, EntityType.VictoryMarker);

            AddBehaviour<VictoryMarker>();

            AddBehaviour<RecorderMaster>();
            AddBehaviour<RecordTransformPosition>();
        }

        public Entity Create(uint winner)
        {
            var e = base.Create();

            e.GetProperty(PropertyNames.UniqueName)!.Value = $"Team {winner} Wins!";

            return e;
        }
    }
}