﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ShipCombatCore.Simulation.Behaviours.Recording;

namespace ShipCombatCore.Simulation.Report
{
    public class Report
    {
        private readonly IEnumerable<RecorderMaster> _recordings;

        public TimeSpan RealtimeDuration { get; }
        public uint? Winner { get; }

        public Report(TimeSpan realtimeDuration, IEnumerable<RecorderMaster> recordings, uint? winner)
        {
            _recordings = recordings;
            Winner = winner;
            RealtimeDuration = realtimeDuration;
        }

        public override string ToString()
        {
            return $"Real Time Elapsed: {RealtimeDuration.TotalMilliseconds}ms\n" +
                   $"Recorded Entities ({_recordings.Count()}):\n" + 
                   $"Winner: {Winner?.ToString() ?? "Draw"}\n" +
                   string.Join("\n", _recordings.Select(r => $" - {r.ID} ({r.Type})"));
        }

        public void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();
            {
                writer.WritePropertyName("Winner");
                writer.WriteValue(Winner);

                writer.WritePropertyName("Entities");
                writer.WriteStartArray();
                {
                    foreach (var recorder in _recordings)
                    {
                        writer.WriteStartObject();
                        {
                            writer.WritePropertyName("ID");
                            writer.WriteValue(recorder.ID);

                            writer.WritePropertyName("Type");
                            writer.WriteValue(recorder.Type.ToString());

                            writer.WritePropertyName("Curves");
                            writer.WriteStartArray();
                            {
                                foreach (var curve in recorder.Curves)
                                {
                                    writer.WriteStartObject();
                                    curve.Serialize(writer);
                                    writer.WriteEndObject();
                                }
                            }
                            writer.WriteEndArray();
                        }
                        writer.WriteEndObject();
                    }
                    
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
    }
}
