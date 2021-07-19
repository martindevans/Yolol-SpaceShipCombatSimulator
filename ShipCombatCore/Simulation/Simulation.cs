using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using MathHelperRedux;
using Myre.Entities;
using ShipCombatCore.Model;
using ShipCombatCore.Name;
using ShipCombatCore.Simulation.Behaviours;
using ShipCombatCore.Simulation.Behaviours.Recording;
using ShipCombatCore.Simulation.Entities;
using ShipCombatCore.Simulation.Services;

namespace ShipCombatCore.Simulation
{
    public class Simulation
    {
        private readonly Fleet _team0;
        private readonly string _name0;
        private readonly Fleet _team1;
        private readonly string _name1;

        public static readonly ushort MillisecondsPerTick = 10;
        public static readonly ulong SimulationDuration = (ulong)(TimeSpan.FromMinutes(20).TotalMilliseconds / MillisecondsPerTick);

        private readonly Scene _scene;

        public Simulation(Fleet team0, string name0, Fleet team1, string name1)
        {
            _team0 = team0;
            _name0 = name0;
            _team1 = team1;
            _name1 = name1;

            _scene = BuildScene();
        }

        public void AddLog(uint team, StreamWriter output)
        {
            _scene.GetService<SceneLogger>().Set(team, output);
        }

        private static void BuildFleet(Scene scene, Fleet fleet, string teamName, uint team, Vector3 middle, Quaternion forward, Random rand, ISet<string> names)
        {
            var shipEntity = new SpaceShipEntity(scene.Kernel);

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
                offset *= 350;

                // Spawn ship entity
                var e = shipEntity.Create(name, teamName, team, middle + offset, Vector3.Zero, forward, new Vector3(0, 0, 0), ship.Programs);
                scene.Add(e);

                // Load up data
                var ctx = e.GetProperty(PropertyNames.YololContext)?.Value;
                if (ctx != null)
                    foreach (var (k, v) in fleet.Data)
                        ctx.Get(k).Value = v;
            }
        }

        private Scene BuildScene()
        {
            var scene = new Scene(new Ninject.StandardKernel());
            scene.GetService<Myre.Entities.Services.ProcessService>();

            // Place central asteroid
            AsteroidEntity asteroidEntity = new(scene.Kernel);
            scene.Add(asteroidEntity.Create(Vector3.Zero, Quaternion.Identity, 300));

            var r = new Random();

            // Place asteroids around the center in a sphere
            for (var i = 0; i < 25; i++)
            {
                var dist = (float)(4500 * r.NextDouble());
                if (dist > 350)
                {
                    var pos = Myre.Extensions.RandomExtensions.RandomNormalVector(r) * dist * new Vector3(1, 0.5f, 1);
                    var rot = Quaternion.Normalize(new Quaternion(
                        (float)r.NextDouble(),
                        (float)r.NextDouble(),
                        (float)r.NextDouble(),
                        (float)r.NextDouble()
                    ));
                    var size = (float)r.NextDouble() * 120 + 30;
                    scene.Add(asteroidEntity.Create(pos, rot, size));
                }
            }

            var names = new HashSet<string>();
            BuildFleet(scene, _team0, _name0, 0, new Vector3(0, 0, 5000), Quaternion.Identity, r, names);
            BuildFleet(scene, _team1, _name1, 1, new Vector3(0, 0, -5000), Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi), r, names);

            return scene;
        }

        public Report.Report Run()
        {
            var start = DateTime.UtcNow;

            var recorder = _scene.GetManager<RecorderMaster.Manager>();
            var teams = _scene.GetManager<TeamMember.Manager>();

            var dt = (float)TimeSpan.FromMilliseconds(MillisecondsPerTick).TotalSeconds;
            uint? winner = null;
            uint ts = 0;
            for (ulong i = 0; i < SimulationDuration; i++)
            {
                Tick();

                var ships = teams.Behaviours.Where(a => a.Type == EntityType.SpaceBattleShip).GroupBy(a => a.Team).ToList();
                if (ships.Count == 1)
                {
                    winner = ships[0].Key;
                    var name = winner == 0 ? _name0 : _name1;
                    _scene.Add(new VictoryMarkerEntity(_scene.Kernel).Create(name));

                    // Run 5 more seconds of sim
                    float extraTime = 10;
                    while (extraTime > 0)
                    {
                        extraTime -= dt;
                        Tick();
                    }

                    break;
                }
            }

            return new Report.Report(
                _scene,
                DateTime.UtcNow - start,
                recorder.Recordings,
                winner
            );

            void Tick()
            {
                _scene.Update(dt);
                ts += MillisecondsPerTick;
                recorder.Record(ts);
            }
        }

        
    }
}
