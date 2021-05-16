using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ShipCombatCore.Model
{
    public class Fleet
    {
        public IReadOnlyList<Ship> Ships { get; }

        public IReadOnlyList<(string, string)> Data { get; }

        public Fleet(IReadOnlyList<Ship> ships, IReadOnlyList<(string, string)> data)
        {
            Ships = ships;
            Data = data;
        }

        public static Fleet? TryLoad(string path)
        {
            return TryLoadZip(path)
                ?? TryLoadDirectory(path);
        }

        private static Fleet? TryLoadZip(string path)
        {
            if (!File.Exists(path) || Path.GetExtension(path) != ".zip")
                return null;

            return TryLoadZip(ZipFile.OpenRead(path));
        }

        public static Fleet? TryLoadZip(ZipArchive archive)
        {
            var tmp = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            try
            {
                archive.ExtractToDirectory(tmp.FullName);
                var subdir = Directory.GetDirectories(tmp.FullName).FirstOrDefault();
                if (subdir == null)
                    return null;

                return TryLoadDirectory(subdir);
            }
            finally
            {
                tmp.Delete(true);
            }
        }

        private static Fleet? TryLoadDirectory(string path)
        {
            if (!Directory.Exists(path))
                return null;

            var ships = new List<Ship>();
            var data = new List<(string, string)>();
            foreach (var subdirectory in Directory.GetDirectories(path))
            {
                var dirname = Path.GetFileName(subdirectory);
                if (dirname.Equals("data", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var file in Directory.GetFiles(subdirectory, "*.txt"))
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        var content = File.ReadAllText(file);
                        data.Add(($":{name}", content));
                    }
                }
                else
                {
                    var programs = new List<Yolol.Grammar.AST.Program>();
                    foreach (var file in Directory.GetFiles(subdirectory, "*.yolol"))
                    {
                        var result = Yolol.Grammar.Parser.ParseProgram(File.ReadAllText(file));
                        if (!result.IsOk)
                            Console.WriteLine(result.Err);
                        else
                            programs.Add(result.Ok);
                    }
                    ships.Add(new Ship(programs));
                }
            }

            return new Fleet(ships, data);
        }
    }
}
