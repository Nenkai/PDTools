using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.LiveTimingApi.Entities;

public class LiveTimingRaceInfo
{
    /// <summary>
    /// Time at which the race has finished and chequered flag waved.
    /// </summary>
    public int ChequeredTime { get; set; }

    /// <summary>
    /// Internal course code.
    /// </summary>
    public int CourseCode { get; set; }

    /// <summary>
    /// Name of the course.
    /// </summary>
    public string CourseName { get; set; }

    public bool Established { get; set; }

    public int FixedTime { get; set; }

    /// <summary>
    /// Number of sectors in this race
    /// </summary>
    public int NbSector { get; set; }

    /// <summary>
    /// Number of laps in this race
    /// </summary>
    public int RaceLaps { get; set; }

    /// <summary>
    /// Number of minutes in this race
    /// </summary>
    public int RaceMinute { get; set; }

    /// <summary>
    /// Type of race being held.
    /// </summary>
    public int RaceType { get; set; }

    public int SessionType { get; set; }
}
