
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Structures.PS3.MGameParameter
{
    public class WeatherData
    {
        public float TimeRate { get; set; }
        public float Low { get; set; }
        public float High { get; set; }

        public override string ToString()
            => $"Step ({TimeRate}%) Low: {Low} - High: {High}";

    }
}
