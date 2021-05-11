using System.Collections.Generic;
using Yolol.Grammar.AST;

namespace ShipCombatCore.Model
{
    public class Ship
    {
        public IReadOnlyList<Program> Programs { get; }

        public Ship(IReadOnlyList<Program> programs)
        {
            Programs = programs;
        }
    }
}
