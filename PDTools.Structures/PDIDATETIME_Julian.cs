using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.Structures;

/// <summary>
/// 64 bits julian date + time 
/// </summary>
public class PDIDATETIME_Julian
{
    public static readonly uint[] DaysInYear = [0, 365, 730, 1095, 1461];

    public static uint[][] Days_To_Month =
    [
        // non-leap year
        [0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334],
        // leap year
        [0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335],
    ];

    const int HalfSecondsInDay = 43200; // 0xA8C0

    const int SecondsInDay = 86400; // 0x15180
    const int SecondsInHour = 3600;
    const int SecondsInMinute = 60;
    const int HoursInDay = 24;

    const int MinutesInDay = 1460; // 5b4

    const int DaysPer400Years = 0x23ab1;
    const int DaysPer100Years = 0x8eac;
    const int DaysPer4Years = 0x5b5;


    const int Jan11JulianDay = 1721426; // 0x1a4452
    public static DateTime FromJulianDateValue(ulong julianTime)
    {
        int dmyValue = (int)((ulong)(julianTime + HalfSecondsInDay) / SecondsInDay);
        var (Day, Month, Year) = GetDayMonthYear(dmyValue);
        var (Seconds, Minutes, Hours) = GetSecondMinuteHours((int)(julianTime + HalfSecondsInDay) + dmyValue * -SecondsInDay);

        return new DateTime(Year, Month, Day, Hours, Minutes, Seconds);
    }

    public static double DateTimeToJulian(DateTime dateTime)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        TimeSpan diff = dateTime.ToUniversalTime() - origin;
        double unixTime = Math.Floor(diff.TotalSeconds);
        double julianDate = ((unixTime / 86400) + 2440587.5) * 86400;

        return julianDate;
    }

    // For further cleaning, https://github.com/mikesmithjr/ora-jdbc-source/blob/8a8331bc7112ecd8dfcf97afe9d27cb9c30e8b0d/OracleJDBC/src/oracle/jdbc/driver/DateCommonBinder.java#L90
    private static (byte Day, byte Month, short Year) GetDayMonthYear(int dmy)
    {
        int n100 = ((dmy + -Jan11JulianDay) % DaysPer400Years) / DaysPer100Years;
        int dayOfYear = ((dmy + -Jan11JulianDay) % DaysPer400Years) % DaysPer100Years;

        int minutes = MinutesInDay;
        int hours = HoursInDay;
        if (n100 == 4)
            n100 = 3;
        else
        {
            hours = dayOfYear / DaysPer4Years;
            minutes = dayOfYear % DaysPer4Years;
        }

        int yearIndex = BSearch((uint)minutes, DaysInYear, DaysInYear.Length);
        bool isLeap = yearIndex == 3;
        if (hours == HoursInDay)
            isLeap = yearIndex == 3 && n100 == 3;

        int monthIndex = BSearch((uint)minutes - (uint)DaysInYear[yearIndex], Days_To_Month[isLeap ? 1 : 0], Days_To_Month[isLeap ? 1 : 0].Length);
        byte month = Convert.ToByte(monthIndex + 1);
        byte day = Convert.ToByte( (((uint)minutes - (uint)DaysInYear[yearIndex]) - Days_To_Month[isLeap ? 1 : 0][monthIndex]) + 1);
        short year = Convert.ToInt16((((short)n100 + (short)((dmy + -Jan11JulianDay) / DaysPer400Years) * 4) * 0x19 + (short)hours) * 4 + (short)yearIndex + 1);

        return (day, month, year);
    }

    private static (byte Seconds, byte Minutes, byte Hours) GetSecondMinuteHours(int smh)
    {
        byte hours = Convert.ToByte(smh / SecondsInHour);
        byte minutes = Convert.ToByte((smh % SecondsInHour) / SecondsInMinute);
        byte seconds = Convert.ToByte((smh % SecondsInHour) + minutes * -SecondsInMinute);

        return (seconds, minutes, hours);
    }

    private static int BSearch(uint value, uint[] values, int max)
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
