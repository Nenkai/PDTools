using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.LiveTimingApi.Entities
{
    public class LiveTimingEntry
    {
        public int AfterPenaltySec { get; set; }

        public int BestLapTime { get; set; }

        /// <summary>
        /// All best sector times for this course.
        /// </summary>
        public int[] BestSectorTime { get; set; }

        /// <summary>
        /// Internal car code.
        /// </summary>
        public long CarCode { get; set; }

        public long CarColor { get; set; }

        /// <summary>
        /// Name of the car for this entry.
        /// </summary>
        public string CarName { get; set; }

        /// <summary>
        /// Whether this entry has gotten past the chequered flag.
        /// </summary>
        public bool Chequered { get; set; }

        public bool Cleared { get; set; }

        /// <summary>
        /// Country for the driver for this entry.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Whether this entry did not finish.
        /// </summary>
        public bool DNF { get; set; }

        public int DelaySeconds { get; set; }

        /// <summary>
        /// Whether the whole race is clean.
        /// </summary>
        public bool DirtyRun { get; set; }

        /// <summary>
        /// Whether the last sector was clean.
        /// </summary>
        public bool DirtySector { get; set; }

        /// <summary>
        /// Whether this entry is disqualified from the race.
        /// </summary>
        public bool Disqualified { get; set; }

        /// <summary>
        /// Driver index within this race.
        /// </summary>
        public int DriverIndex { get; set; }

        /// <summary>
        /// Name of the entry driving this car.
        /// </summary>
        public string DriverName { get; set; }

        /// <summary>
        /// Whether this entry is enabled.
        /// </summary>
        public int Enabled { get; set; }

        /// <summary>
        /// Front Tire type in use.
        /// </summary>
        public int FrontTire { get; set; }

        /// <summary>
        /// Whether this entry is in the pits.
        /// </summary>
        public bool InPit { get; set; }

        public int LastLapTime { get; set; }

        /// <summary>
        /// Number of laps leading.
        /// </summary>
        public int LeadLaps { get; set; }

        public int MaxSpeed { get; set; }

        public int MyBestLap { get; set; }

        public int MyBestSector { get; set; }

        /// <summary>
        /// Number of penalties issued.
        /// </summary>
        public int NbPenalty { get; set; }

        /// <summary>
        /// Number of times this entry has pitted in.
        /// </summary>
        public int NbPitIn { get; set; }

        /// <summary>
        /// Car Number.
        /// </summary>
        public int Number { get; set; }

        public int OverallBestLap { get; set; }
        public int OverallBestSector { get; set; }

        /// <summary>
        /// Rear Tire type in use.
        /// </summary>
        public int RearTire { get; set; }

        public int RunningState { get; set; }
        public int Sector { get; set; }
        public int[] SectorTime { get; set; }

        /// <summary>
        /// Internal index/ID of this entry in the race.
        /// </summary>
        public int SlotID { get; set; }

        public int StartingGrid { get; set; }

        /// <summary>
        /// Current total laps.
        /// </summary>
        public int TotalLaps { get; set; }

        /// <summary>
        /// Total race time.
        /// </summary>
        public int TotalTime { get; set; }

        public bool Violation { get; set; }
        public bool ViolationTire { get; set; }
    }
}
