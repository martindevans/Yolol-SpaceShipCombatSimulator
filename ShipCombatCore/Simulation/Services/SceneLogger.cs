using System;
using System.Collections.Generic;
using System.IO;
using Myre.Entities.Services;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Services
{
    public class SceneLogger
        : Service
    {
        private readonly Dictionary<uint, StreamWriter> _streams = new();
        private double _time;

        public override void Update(float elapsedTime)
        {
            base.Update(elapsedTime);

            _time += elapsedTime;
        }

        public void Set(uint team, StreamWriter output)
        {
            _streams.Add(team, output);
        }

        public void Log(uint team, string id, YString message)
        {
            if (!_streams.TryGetValue(team, out var stream))
                return;

            var timeMs = (int)TimeSpan.FromSeconds(_time).TotalMilliseconds;

            stream.WriteLine($"[{timeMs}ms] [{id}] {message}");
        }
    }
}
