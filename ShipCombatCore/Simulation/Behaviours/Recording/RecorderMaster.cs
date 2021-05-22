using System;
using System.Collections.Generic;
using Myre.Entities.Behaviours;
using ShipCombatCore.Simulation.Report.Curves;
using System.Linq;
using Myre.Collections;
using ShipCombatCore.Simulation.Entities;

namespace ShipCombatCore.Simulation.Behaviours.Recording
{
    public class RecorderMaster
        : Behaviour
    {
        public IEnumerable<ICurve> Curves => Owner.GetBehaviours<IRecorder>().SelectMany(a => a.Curves);

#pragma warning disable 8618
        public string ID { get; set; }
#pragma warning restore 8618

        public EntityType Type { get; set; }

        public override void Initialise(INamedDataProvider? initialisationData)
        {
            ID = Owner.GetProperty(PropertyNames.UniqueName)?.Value ?? throw new InvalidOperationException("Recorded entity does not have unique ID");
            Type = Owner.GetProperty(PropertyNames.EntityType)?.Value ?? throw new InvalidOperationException("Recorded entity does not have entity type");

            base.Initialise(initialisationData);
        }

        public class Manager
            : BehaviourManager<RecorderMaster>
        {
            private readonly HashSet<RecorderMaster> _allMasters = new();
            private readonly HashSet<Recording> _active = new();

            public IEnumerable<RecorderMaster> Recordings => _allMasters;

            public override void Add(RecorderMaster behaviour)
            {
                _allMasters.Add(behaviour);

                _active.Add(new Recording(
                    behaviour,
                    behaviour.Owner.GetBehaviours<IRecorder>()
                ));

                base.Add(behaviour);
            }

            public override bool Remove(RecorderMaster behaviour)
            {
                _active.RemoveWhere(a => a.Master.Equals(behaviour));

                return base.Remove(behaviour);
            }

            public void Record(uint timestamp)
            {
                foreach (var recording in _active)
                foreach (var recorder in recording.Recorders)
                    recorder.Record(timestamp);
            }

            private readonly struct Recording
            {
                public readonly RecorderMaster Master;
                public readonly IReadOnlyList<IRecorder> Recorders;

                public Recording(RecorderMaster master, IReadOnlyList<IRecorder> recorders)
                {
                    Master = master;
                    Recorders = recorders;
                }
            }
        }
    }
}
