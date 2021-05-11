using System;
using System.Collections.Generic;
using System.Numerics;
using MathHelperRedux;
using Myre.Entities;
using ShipCombatCore.Model;
using ShipCombatCore.Name;
using ShipCombatCore.Simulation.Behaviours.Recording;
using ShipCombatCore.Simulation.Entities;

namespace ShipCombatCore.Simulation
{
    public class Simulation
    {
        public static readonly ushort MillisecondsPerTick = 10;
        public static readonly ulong SimulationDuration = (ulong)(TimeSpan.FromMinutes(25).TotalMilliseconds / MillisecondsPerTick);

        private readonly Scene _scene;

        public Simulation(Fleet red, Fleet blue)
        {
            _scene = BuildScene(red, blue);
        }

        private static void BuildFleet(Scene scene, Fleet fleet, Vector3 middle, Quaternion forward, Random rand, HashSet<string> names)
        {
            var shipEntity = new SpaceShipEntity();

            foreach (var ship in fleet.Ships)
            {
                // Generate a unique name
                string name;
                do {
                    name = ShipName.Generate(rand);
                } while (names.Contains(name));
                names.Add(name);

                // Choose an offset
                var offset = new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()) * 2 - Vector3.One;
                offset *= 250;

                // Spawn ship entity
                var e = shipEntity.Create(name, middle + offset, Vector3.Zero, forward, new Vector3(0, 0, 0), ship.Programs);
                scene.Add(e);

                // Load up data
                var ctx = e.GetProperty(PropertyNames.YololContext)?.Value;
                if (ctx != null)
                {
                    foreach (var (k, v) in fleet.Data)
                    {
                        var p = ctx.MaybeGet(k);
                        if (p != null)
                            p.Value = v;
                    }
                }
            }
        }

        private static Scene BuildScene(Fleet red, Fleet blue)
        {
            var scene = new Scene();
            scene.GetService<Myre.Entities.Services.ProcessService>();

            AsteroidEntity asteroidEntity = new();
            scene.Add(asteroidEntity.Create(Vector3.Zero, Vector3.Zero, Quaternion.Identity, Vector3.Zero, 300));

            var r = new Random();
            var names = new HashSet<string>();
            BuildFleet(scene, red, new Vector3(0, 0, 3000), Quaternion.Identity, r, names);
            BuildFleet(scene, blue, new Vector3(0, 0, -3000), Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), r, names);

            return scene;
        }

        public Report.Report Run()
        {
            var start = DateTime.UtcNow;

            var recorder = _scene.GetManager<RecorderMaster.Manager>();

            uint ts = 0;
            for (ulong i = 0; i < SimulationDuration; i++)
            {
                _scene.Update((float)TimeSpan.FromMilliseconds(MillisecondsPerTick).TotalSeconds);
                ts += MillisecondsPerTick;
                recorder.Record(ts);
            }

            return new Report.Report(
                DateTime.UtcNow - start,
                recorder.Recordings
            );
        }
    }
}
