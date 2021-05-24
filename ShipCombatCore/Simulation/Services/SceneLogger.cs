using System.Collections.Generic;
using System.IO;
using Myre.Entities;
using Myre.Entities.Services;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Services
{
    public class SceneLogger
        : Service
    {
        private readonly Dictionary<uint, StreamWriter> _streams = new();
        private TimeService? _time;

        public override void Initialise(Scene scene)
        {
            base.Initialise(scene);

            _time = scene.GetService<TimeService>();
        }

        public void Set(uint team, StreamWriter output)
        {
            _streams.Add(team, output);
        }

        public void Log(uint team, string id, YString message)
        {
            if (!_streams.TryGetValue(team, out var stream))
                return;

            stream.WriteLine($"[{_time?.Tick ?? 0}] [{id}] {message}");
        }
    }
}
