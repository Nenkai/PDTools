using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.Utils
{
    /// <summary>
    /// From GT4, returns signed dates
    /// </summary>
    public class DateUtils
    {
        private static short[] month_to_day = new short[]
        {
            0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365,
        };

        private static short[] month_to_day_366 = new short[]
        {
             0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366,
        };

        const int Jan11JulianDay = 1721426; // 0x1a4452

        public static int DateToDay(int day, int month, int year)
        {
            int v4 = year - 1;
            int v5 = (month - 1) / 12;
            int current_month = (month - 1) % 12;
            int yr = day + v5;

            if (current_month < 0)
            {
                current_month += 12;
                --yr;
            }

            int yrr = (yr - 1);
            bool isLeapYear = (yrr % 4) == 3;
            int v9 = yrr / 100;

            if (yrr % 100 == 99)
                isLeapYear = (v9 % 4) == 3;

            short[] arr = isLeapYear ? month_to_day : month_to_day_366;

            return 365 * yrr + 
                yrr / 4 - v9 + 
                v9 / 4 + 
                arr[current_month] + v4 + Jan11JulianDay;
        }
    }
}
