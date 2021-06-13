using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using CommandLine;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ShipCombatCore.Model;
using ShipCombatCore.Name;
using ShipCombatCore.Simulation;
using ShipCombatCore.Simulation.Report;

namespace SpaceShipCombatSimulator
{
    public class Program
    {
        [UsedImplicitly] private class Options
        {
            [Option('a', "fleeta", HelpText = "Folder containing fleet A", Required = true)]
#pragma warning disable 8618
            public string PathA { get; [UsedImplicitly] set; }

            [Option('b', "fleetb", HelpText = "Folder containing fleet B", Required = true)]
            public string PathB { get; [UsedImplicitly] set; }

            [Option('o', "output", HelpText = "File to write replay to", Required = false, Default = null)]
            public string? OutputPath { get; [UsedImplicitly] set; }
#pragma warning restore 8618
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Names:");
            var r = new Random();
            for (var i = 0; i < 15; i++)
                Console.WriteLine(ShipName.Generate(r));

            Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
        }

        private static void Run(Options options)
        {
            var a = Fleet.TryLoad(options.PathA);
            if (a == null)
            {
                Console.WriteLine("**Failed to load fleet A**");
                return;
            }

            var b = Fleet.TryLoad(options.PathB);
            if (b == null)
            {
                Console.WriteLine("**Failed to load fleet B**");
                return;
            }

            var sim = new Simulation(a, Path.GetFileNameWithoutExtension(options.PathA), b, Path.GetFileNameWithoutExtension(options.PathB));
            Console.WriteLine("Created Scene");

            Report? report;
            using (var loga = File.CreateText("CaptainsLog_A.txt"))
            using (var logb = File.CreateText("CaptainsLog_B.txt"))
            {
                sim.AddLog(0, loga);
                sim.AddLog(1, logb);
                report = sim.Run();
            }

            Console.WriteLine(report);

            var output = options.OutputPath ?? "output.json.deflate";

            using (var file = File.Create(output))
            using (var zip = new DeflateStream(file, CompressionLevel.Optimal))
            using (var stream = new StreamWriter(zip))
            using (var writer = new JsonTextWriter(stream) { Formatting = Formatting.None })
            {
                report.Serialize(writer);
                writer.Flush();
                Console.WriteLine($"File Size (Compressed): {ToFileSize(file.Position)}");
            }

            #if DEBUG
            using (var file = File.Create("output.json"))
            using (var stream = new StreamWriter(file))
            using (var writer = new JsonTextWriter(stream) { Formatting = Formatting.Indented })
            {
                report.Serialize(writer);
                writer.Flush();
                Console.WriteLine($"File Size (Uncompressed): {ToFileSize(file.Position)}");
            }
            #endif
        }

        private static string ToFileSize(long bytes)
        {
            var sizes = new[] { "B", "KB", "MB", "GB", "TB" };
            var order = 0;
            while (bytes >= 1024 && order < sizes.Length - 1) {
                order++;
                bytes /= 1024;
            }
            return $"{bytes:0.##}{sizes[order]}";
        }
    }
}
