using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Structures.MGameParameter
{
    /// <summary>
    /// GT6 Weather Data. Allows for more control & chance over weather scripting.
    /// </summary>
    public class WeatherData
    {
        /// <summary>
        /// % of the total progression which this steps starts. This will be a percentage of <see cref="RaceParameter.WeatherTotalSec"/>.
        /// </summary>
        public float TimeRate { get; set; }

        /// <summary>
        /// Lowest (worse) the weather condition is at this step.
        /// </summary>
        public float Low { get; set; }

        /// <summary>
        /// Highest (worse) the weather condition is at this step.
        /// </summary>
        public float High { get; set; }

        public override string ToString()
            => $"Step ({TimeRate}%) Low: {Low} - High: {High}";

    }
}
