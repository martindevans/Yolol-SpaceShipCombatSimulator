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

        public string ID { get; set; }
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
            private readonly HashSet<RecorderMaster> _recordings = new();

            public IEnumerable<RecorderMaster> Recordings => _recordings;

            public override void Add(RecorderMaster behaviour)
            {
                _recordings.Add(behaviour);

                base.Add(behaviour);
            }

            public void Record(uint timestamp)
            {
                foreach (var item in Behaviours)
                {
                    var recorders = item.Owner.GetBehaviours<IRecorder>();
                    foreach (var recorder in recorders)
                        recorder.Record(timestamp);
                }
            }
        }
    }
}
