
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Structures.PS3.MGameParameter
{
    public class BoostTable
    {
        /// <summary>
        /// The distance it takes for the boost to reach its maximum rate, 200 will require a very long distance to reach -8 for example
        /// </summary>
        public byte FrontLimit { get; set; }

        /// <summary>
        /// The 2nd step up from initial rate, this kicks in depending on the Start value when you're at a certain distance behind or ahead of the AI
        /// </summary>
        public sbyte FrontMaximumRate { get; set; }

        /// <summary>
        /// When to start applying the Maximum Rate when behind/ahead of the AI
        /// </summary>
        public byte FrontStart { get; set; }

        /// <summary>
        /// The 1st step of the boost, kicks in when you pass the AI or behind them
        /// </summary>
        public sbyte FrontInitialRate { get; set; }

        public byte RearLimit { get; set; }
        public sbyte RearMaximumRate { get; set; }
        public byte RearStart { get; set; }
        public sbyte RearInitialRate { get; set; }

        public byte ReferenceRank { get; set; }
        public byte Unk { get; set; }

        public bool IsDefault()
        {
            if (FrontLimit == 0 && FrontMaximumRate == 0 && FrontStart == 0 && FrontInitialRate == 0
              && RearLimit == 0 && RearMaximumRate == 0 && RearStart == 0 && RearInitialRate == 0
              && ReferenceRank == 0 && Unk == 0)
                return true;

            return false;
        }
    }
}
