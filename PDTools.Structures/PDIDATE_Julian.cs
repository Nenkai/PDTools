using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.Utils
{
    /// <summary>
    /// From GT4, returns signed dates
    /// </summary>
    public class PDIDATE_Julian
    {
        private static short[] month_to_day = new short[]
        {
            0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365,
        };

        private static short[] month_to_day_366 = new short[]
        {
             0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366,
        };

        const int Jan11JulianDay = 1721426;

        public static int DateToDay(int year, int month, int day)
        {
            int monthIndex = month - 1;
            int dayIndex = year - 1;

            int current_month = monthIndex % 12;
            int yr = day + (monthIndex / 12);

            if (current_month < 0)
            {
                current_month += 12;
                --yr;
            }

            int yrIndex = yr - 1;

            bool isLeapYear = (yrIndex % 4 == 0 && (yrIndex % 100 != 0 || yrIndex % 400 == 0));

            short[] arr = isLeapYear ? month_to_day : month_to_day_366;

            return 365 * (yrIndex - 1)
                + (yrIndex - 1) / 4
                - yrIndex / 100
                + yrIndex / 100 / 4
                + arr[current_month]
                + dayIndex
                + Jan11JulianDay;
        }
    }
}
