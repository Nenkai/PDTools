using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDTools.Structures.MGameParameter
{
    public class BoostTable
    {
        // Names were taken from GT7 tables

        /// <summary>
        /// The distance it takes for the boost to reach its maximum rate, 200 will require a very long distance to reach -8 for example
        /// </summary>
        public byte RearDistance2 { get; set; }

        /// <summary>
        /// The 2nd step up from initial rate, this kicks in depending on the Start value when you're at a certain distance behind or ahead of the AI
        /// </summary>
        public sbyte RearRate2 { get; set; }

        /// <summary>
        /// When to start applying the Maximum Rate when behind/ahead of the AI
        /// </summary>
        public byte RearDistance1 { get; set; }

        /// <summary>
        /// The 1st step of the boost, kicks in when you pass the AI or behind them
        /// </summary>
        public sbyte RearRate1 { get; set; }

        public byte FrontDistance2 { get; set; }
        public sbyte FrontRate2 { get; set; }
        public byte FrontDistance1 { get; set; }
        public sbyte FrontRate1 { get; set; }

        /// <summary>
        /// Also known as reference rank
        /// </summary>
        public byte TargetPosition { get; set; }

        /// <summary>
        /// Race progress in % at which this table will activate
        /// </summary>
        public byte RaceProgress { get; set; }

        public bool IsDefault()
        {
            if (RearDistance2 == 0 && RearRate2 == 0 && RearDistance1 == 0 && RearRate1 == 0
              && FrontDistance2 == 0 && FrontRate2 == 0 && FrontDistance1 == 0 && FrontRate1 == 0
              && TargetPosition == 0 && RaceProgress == 0)
                return true;

            return false;
        }

        public void CopyTo(BoostTable other)
        {
            other.RearDistance2 = RearDistance2;
            other.RearRate2 = RearRate2;
            other.RearDistance1 = RearDistance1;
            other.RearRate1 = RearRate1;
            other.FrontDistance2 = FrontDistance2;
            other.FrontRate2 = FrontRate2;
            other.FrontDistance1 = FrontDistance1;
            other.FrontRate1 = FrontRate1;
            other.TargetPosition = TargetPosition;
            other.RaceProgress = RaceProgress;
        }
    }
}
