using System.Collections.Generic;
using ShipCombatCore.Simulation.Report.Curves;

namespace ShipCombatCore.Simulation.Behaviours.Recording
{
    public interface IRecorder
    {
        /// <summary>
        /// Record values to curves
        /// </summary>
        void Record(uint ms);

        /// <summary>
        /// Get all the curves this recorder has created
        /// </summary>
        IEnumerable<ICurve> Curves { get; }
    }
}
