
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.LiveTimingApi.Entities;

public class LiveTimingCondition
{
    /// <summary>
    /// Air Temperature (in °C).
    /// </summary>
    public int AirTemperature { get; set; }

    /// <summary>
    /// Humidity (in %).
    /// </summary>
    public int Humidity { get; set; }

    /// <summary>
    /// Whether the track is wet.
    /// </summary>
    public bool IsWet { get; set; }

    public float Precipitation { get; set; }

    /// <summary>
    /// Pressure (in hPa/hectopascal).
    /// </summary>
    public int Pressure { get; set; }

    public int PuddleRate { get; set; }

    /// <summary>
    /// Track temperature (in °C).
    /// </summary>
    public int TrackTemperature { get; set; }

    public int WetRate { get; set; }

    public WindDirection WindDirection { get; set; }

    /// <summary>
    /// Wind Speed (in kmh).
    /// </summary>
    public int WindSpeed { get; set; }
}

public enum WindDirection : int
{
    DIRECTION_S,
    DIRECTION_SSW,
    DIRECTION_SW,
    DIRECTION_WSW,
    DIRECTION_W,
    DIRECTION_WNW,
    DIRECTION_NW,
    DIRECTION_NNW,
    DIRECTION_N,
    DIRECTION_NNE,
    DIRECTION_NE,
    DIRECTION_ENE,
    DIRECTION_E,
    DIRECTION_ESE,
    DIRECTION_SE,
    DIRECTION_SSE,
}
