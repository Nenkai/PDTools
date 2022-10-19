﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.Structures
{
    /// <summary>
    /// 64 bits julian date + time 
    /// </summary>
    public class PDIDATETIME
    {
        public static readonly int[] DaysInYear = new int[] { 0, 365, 730, 1095, 1461 };

        public static int[][] Days_To_Month = new int[2][]
        {
            // non-leap year
            new int[12] { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334 },
            // leap year
            new int[12] { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335 },
        };

        const int HalfSecondsInDay = 43200; // 0xA8C0

        const int SecondsInDay = 86400; // 0x15180
        const int SecondsInHour = 3600;
        const int SecondsInMinute = 60;

        const int DaysPer400Years = 146097;
        const int DaysPer100Years = 36524;
        const int DaysPer4Years = 1461;


        const int Jan11JulianDay = 1721426; // 0x1a4452

        ///////////////////////////////////////////////////////////
        /////////////////// PDIDATETIME ///////////////////////////
        ///////////////////////////////////////////////////////////
        // Handles Sec/Min/Hr/Day/Month/Year/ <-> 64 bit integer translation.

        /// <summary>
        /// Converts a packed PDIDATETIME julian value into a DateTime.
        /// </summary>
        /// <param name="julianTime"></param>
        /// <returns></returns>
        public static DateTime JulianToDateTime_64(ulong julianTime)
        {
            int dmyValue = (int)((ulong)(julianTime + HalfSecondsInDay) / SecondsInDay);
            var dmy = DayToDate(dmyValue);
            var smh = GetSecondMinuteHours((int)(julianTime + HalfSecondsInDay) + dmyValue * -SecondsInDay);

            return new DateTime(dmy.Year, dmy.Month, dmy.Day, smh.Hours, smh.Minutes, smh.Seconds);
        }

        /// <summary>
        /// Converts a datetime into a packed PDIDATETIME julian value.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static double DateTimeToJulian_64(DateTime dateTime)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = dateTime.ToUniversalTime() - origin;
            double unixTime = Math.Floor(diff.TotalSeconds);
            double julianDate = ((unixTime / 86400) + 2440587.5) * 86400;

            return julianDate;
        }

        private static (byte Seconds, byte Minutes, byte Hours) GetSecondMinuteHours(int smh)
        {
            byte hours = Convert.ToByte(smh / SecondsInHour);
            byte minutes = Convert.ToByte((smh % SecondsInHour) / SecondsInMinute);
            byte seconds = Convert.ToByte((smh % SecondsInHour) + minutes * -SecondsInMinute);

            return (seconds, minutes, hours);
        }

        ///////////////////////////////////////////////////////
        /////////////////// PDIDATE ///////////////////////////
        ///////////////////////////////////////////////////////
        // Handles Day/Month/Year <-> 32 bit integer translation only.

#if NET6_0_OR_GREATER
        public static DateOnly DayToDateOnly(int value)
        {
            (int Year, int Month, int Day) = DayToDate(value);
            return new DateOnly(Year, Month, Day);
        }
#endif

        /// <summary>
        /// Converts a 32 bit packed integer into a date time (d/m/y).
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime DayToDateTime(int value)
        {
            (int Year, int Month, int Day) = DayToDate(value);
            if (Year < 1)
                Year = 1;

            if (Day > 31)
                Day = 31;

            return new DateTime(Year, Month, Day);
        }

        /// <summary>
        /// Converts a packed 32 bit integer into a PDIDATE (d/m/y). Original Impl
        /// </summary>
        public static (int Year, int Month, int Day) DayToDate(int value)
        {
            int dayOfYear = value - (Jan11JulianDay - 1);
            int n400 = dayOfYear / DaysPer400Years;
            dayOfYear %= DaysPer400Years;

            int n100 = dayOfYear / DaysPer100Years;
            int v6 = dayOfYear % DaysPer100Years;

            if (dayOfYear / DaysPer100Years == 4)
            {
                n100 = 3;
                v6 = DaysPer100Years;
            }

            int n4 = v6 / DaysPer4Years;
            int day = v6 % DaysPer4Years;
            int n1 = search_day(day, DaysInYear, 5);
            bool isLeapYear = n1 == 3;

            int year = 4 * (25 * (4 * n400 + n100) + n4) + n1 + 1;
            day -= DaysInYear[n1];


            if (n4 == 24)
                isLeapYear = isLeapYear & (n100 == 3);

            int _month = search_day(day, Days_To_Month[isLeapYear ? 1 : 0], 13);
            int month = _month + 1;
            day = (byte)day - Days_To_Month[isLeapYear ? 1 : 0][_month] + 1;

            return (year, month, day);
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Converts a date only into a 32 bit packed integer (d/m/y).
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int DayToDateOnly(DateOnly date)
        {
            return DateToDay(date.Year, date.Month, date.Day);
        }
#endif

        /// <summary>
        /// Converts a date time into a 32 bit packed integer (d/m/y).
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int DateTimeToDay(DateTime date)
        {
            return DateToDay(date.Year, date.Month, date.Day);
        }

        /// <summary>
        /// Converts a date only (d/m/y) into a 32 bit packed integer. Original Impl
        /// </summary>
        /// <param name="year"></param>
        /// <param name="months"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static int DateToDay(int year, int months, int day)
        {
            int monthIndex = months - 1;
            int dayIndex = day - 1;

            int month = monthIndex % 12;
            int yr = year + (monthIndex / 12);

            if (month < 0)
            {
                month += 12;
                --yr;
            }


            bool isLeapYear = (yr % 4 == 0 && (yr % 100 != 0 || yr % 400 == 0));

            int yrIndex = yr - 1;
            return 365 * yrIndex
                + (yrIndex - 1) / 4
                - yrIndex / 100
                + yrIndex / 100 / 4
                + Days_To_Month[isLeapYear ? 1 : 0][month]
                + dayIndex
                + (Jan11JulianDay - 1);
        }

        private static int search_day(int value, int[] values, int max)
        {
            int min = 0;

            do
            {
                int mid = (min + max) / 2;
                if (value < values[mid])
                {
                    max = mid;
                    mid = min;
                }

                min = mid;
            } while (min + 1 < max);

            return min;
        }
    }
}
