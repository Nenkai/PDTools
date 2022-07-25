using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;

namespace SimulatorInterface
{
    public class SimulatorPacketGT7
    {
        /// <summary>
        /// Position on the track.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Acceleration in track units for each axis.
        /// </summary>
        public Vector3 Acceleration { get; set; }

        /// <summary>
        /// Rotation (Pitch/Yaw/Roll) from -1 to 1.
        /// </summary>
        public Vector3 Rotation { get; set; }

        public float Unknown_0x28 { get; set; }

        /// <summary>
        /// Unknown. May be related to orientation.
        /// </summary>
        public Vector3 Unknown_0x2C { get; set; }

        public float Unknown_0x38 { get; set; }

        public float RPM { get; set; }

        /// <summary>
        /// Stays at 100, not fuel, nor tyre wear
        /// </summary>
        public float Unknown_0x48 { get; set; }

        public float MetersPerSecond { get; set; }

        /// <summary>
        /// Value below 1.0 is below 0 ingame, so 2.0 = 1 x 100kPa
        /// </summary>
        public float TurboBoost { get; set; }

        public float Unknown_0x54 { get; set; }

        /// <summary>
        /// Game will always send this.
        /// </summary>
        public float Unknown_Always85_0x58 { get; set; }

        /// <summary>
        /// Game will always send this.
        /// </summary>
        public float Unknown_Always110_0x5C { get; set; }

        /// <summary>
        /// Front Left
        /// </summary>
        public float TireSurfaceTemperatureFL { get; set; }

        /// <summary>
        /// Front Right
        /// </summary>
        public float TireSurfaceTemperatureFR { get; set; }

        /// <summary>
        /// Rear Left
        /// </summary>
        public float TireSurfaceTemperatureRL { get; set; }

        /// <summary>
        /// Rear Right
        /// </summary>
        public float TireSurfaceTemperatureRR { get; set; }

        /// <summary>
        /// Can't be more than 1000 laps worth - which is 1209599999, or else it's set to -1
        /// </summary>
        public int TotalTimeTicks { get; set; }

        public int CurrentLap { get; set; }

        public TimeSpan BestLapTime { get; set; }

        public TimeSpan LastLapTime { get; set; }

        public int DayProgressionMS { get; set; }

        /// <summary>
        /// Needs more investigation
        /// </summary>
        public short PreRaceStartPositionOrQualiPos { get; set; }

        /// <summary>
        /// Needs more investigation
        /// </summary>
        public short NumCarsAtPreRace { get; set; }

        public short MinAlertRPM { get; set; }

        public short MaxAlertRPM { get; set; }

        /// <summary>
        /// Updates weirdly, needs investigation
        /// </summary>
        public short CalculatedMaxSpeed { get; set; }

        public SimulatorFlags Flags { get; set; }

        public byte CurrentGear { get; set; }
        public byte SuggestedGear { get; set; }

        public byte Throttle { get; set; }
        public byte Brake { get; set; }

        public float TireFL_Unknown0x94_0 { get; set; }
        public float TireFR_Unknown0x94_1 { get; set; }
        public float TireRL_Unknown0x94_2 { get; set; }
        public float TireRR_Unknown0x94_3 { get; set; }

        public float TireFL_Accel { get; set; }
        public float TireFR_Accel { get; set; }
        public float TireRL_Accel { get; set; }
        public float TireRR_Accel { get; set; }

        public float TireFL_UnknownB4 { get; set; }
        public float TireFR_UnknownB4 { get; set; }
        public float TireRL_UnknownB4 { get; set; }
        public float TireRR_UnknownB4 { get; set; }

        public float TireFL_UnknownC4 { get; set; }
        public float TireFR_UnknownC4 { get; set; }
        public float TireRL_UnknownC4 { get; set; }
        public float TireRR_UnknownC4 { get; set; }

        /// <summary>
        /// Seems to be related to assist, set to 1 when shifting, or handbraking
        /// </summary>
        public float Unknown_0xF4 { get; set; }

        /// <summary>
        /// The opposite? 0 instead of 1 - when this is set, RPM is set for the next value
        /// </summary>
        public float Unknown_0xF8 { get; set; }

        /// <summary>
        /// Depends on 0xF8
        /// </summary>
        public float RPMUnknown_0xFC { get; set; }

        /// <summary>
        /// Constant depending on the car? Car dimensions? Actual array of 7 elements
        /// </summary>
        public float[] Unknown_0x100 { get; set; } = new float[7];

        public int CarCode { get; set; }
    }

    [Flags]
    public enum SimulatorFlags : short
    {
        OnTrack = 1 << 0,
        Paused = 1 << 1,
        Unknown3 = 1 << 2,

        /// <summary>
        /// Needs more investigation
        /// </summary>
        NoAssistsActive = 1 << 3,
        Unknown5 = 1 << 4,
        RevLimiterActive = 1 << 5,
        HandBrakeActive = 1 << 6,
        LightsActive = 1 << 7,
        HighBeamActive = 1 << 8,
        LowBeamActive = 1 << 9,
        Unknown11 = 1 << 10,
        TCSActive = 1 << 11,
    }
}
