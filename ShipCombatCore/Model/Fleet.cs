using System.Collections.Generic;

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
    }
}
