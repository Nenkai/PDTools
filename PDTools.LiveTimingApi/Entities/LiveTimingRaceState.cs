
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.LiveTimingApi.Entities
{
    public class LiveTimingRaceState
    {
        /// <summary>
        /// Time differences between each entry, ordered by standing.
        /// </summary>
        public int[] Diff { get; set; }

        /// <summary>
        /// Hour of the day.
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// Minute of the day.
        /// </summary>
        public int Minute { get; set; }

        public int NbEnabledRanking { get; set; }

        /// <summary>
        /// Second of the day.
        /// </summary>
        public int Second { get; set; }

        /// <summary>
        /// Ordered entry indices for the current standings.
        /// </summary>
        public int[] Standings { get; set; }

        /// <summary>
        /// Time progress.
        /// </summary>
        public int Time { get; set; }

        public int TopLaps { get; set; }
    }
}
