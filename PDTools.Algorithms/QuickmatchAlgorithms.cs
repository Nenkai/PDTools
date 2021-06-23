using System;

namespace PDTools.Algorithms
{
    public class QuickMatchAlgorithms
    {
        // From race_online_event.ad:getReward

        // game:qmatch.prize_entries_num_table
        public static int[] sEntriesNumTable = new int[] { 100, 100, 100, 100, 100, 100, 105, 110, 115, 120, 125, 130, 135, 140, 145, 150 }; // %

        // game:qmatch.prize_base
        public static int sPrizeBase = 100; // %
        
        /// <summary>
        /// Original implementation (Provide prizeTable & race limit laps)
        /// </summary>
        /// <param name="laps"></param>
        /// <param name="finishOrder">0 indexed finish order of the player</param>
        /// <param name="racerCount">Count of players that finished</param>
        /// <param name="prizeTable"></param>
        /// <param name="sRaceLimitLaps"></param>
        /// <returns></returns>
        public int GetQuickMatchReward(int laps, int finishOrder, int finishRacerCount, int[] prizeTable, int sRaceLimitLaps = 1)
        {
            // If prize table does not cover the player's position, its default to 0
            if (prizeTable.Length <= finishOrder)
                return 0;

            int value = prizeTable[finishOrder];
            float prize = (float)value;

            // No money if outside valid count ranges (1 alone player can still get a prize)
            if (finishRacerCount < 1 || finishRacerCount > 16)
                return 0;

            // More entries = extra credits
            float ratio = sEntriesNumTable[finishRacerCount - 1] / 100f;
            prize *= ratio;

            if (sRaceLimitLaps < 1)
                return 0;

            // laps will be equal to sRaceLimitLaps so no changes are made there
            ratio = laps / sRaceLimitLaps;
            prize *= ratio;

            // If we have a server option (in %) to give more or less money than usual
            ratio = sPrizeBase / 100f;
            prize *= ratio;

            // Daily login event bonus is included after this function

            return (int)prize;
        }
    }
}
