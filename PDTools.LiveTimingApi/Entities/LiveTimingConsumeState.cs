
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.LiveTimingApi.Entities;

public class LiveTimingConsumeState
{
    /// <summary>
    /// Tire/Battery/Fuel for each entry in the event.
    /// </summary>
    public LiveTimingConsumeStateEntry[] ConsumeState { get; set; }

    public class LiveTimingConsumeStateEntry
    {
        /// <summary>
        /// Battery remaining (1.0 to 0.0).
        /// </summary>
        public float Battery { get; set; }

        /// <summary>
        /// Fuel remaining (in %, 100.0 to 0.0).
        /// </summary>
        public float Fuel { get; set; }

        /// <summary>
        /// Tire Wear for each tire. (0.0 to 1.0 when used).
        /// </summary>
        public float[] TireWear { get; set; }
    }
}
