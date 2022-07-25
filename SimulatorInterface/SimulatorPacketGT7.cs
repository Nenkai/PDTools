using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;
using Syroot.BinaryData.Memory;

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

        public float RelativeOrientationToNorth { get; set; }

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

        public float Unknown_0x100_GearRelated { get; set; }

        public float[] GearRatios { get; set; } = new float[7];

        public int CarCode { get; set; }

        public void Read(Span<byte> data)
        {
            SpanReader sr = new SpanReader(data);
            int magic = sr.ReadInt32();
            if (magic != 0x47375330) // 0S7G - G7S0
                throw new InvalidDataException($"Unexpected packet magic '{magic}'.");

            var dataPacket = new SimulatorPacketGT7();

            Position = new Vector3(sr.ReadSingle(), sr.ReadSingle(), sr.ReadSingle()); // Coords to track
            Acceleration = new Vector3(sr.ReadSingle(), sr.ReadSingle(), sr.ReadSingle());  // Accel in track pixels
            Rotation = new Vector3(sr.ReadSingle(), sr.ReadSingle(), sr.ReadSingle()); // Pitch/Yaw/Roll all -1 to 1
            RelativeOrientationToNorth = sr.ReadSingle();
            Unknown_0x2C = new Vector3(sr.ReadSingle(), sr.ReadSingle(), sr.ReadSingle());
            Unknown_0x38 = sr.ReadSingle();
            RPM = sr.ReadSingle();

            // Skip IV
            sr.Position += 8;

            Unknown_0x48 = sr.ReadSingle();
            MetersPerSecond = sr.ReadSingle();
            TurboBoost = sr.ReadSingle();
            Unknown_0x54 = sr.ReadSingle();
            Unknown_Always85_0x58 = sr.ReadSingle();
            Unknown_Always110_0x5C = sr.ReadSingle();
            TireSurfaceTemperatureFL = sr.ReadSingle();
            TireSurfaceTemperatureFR = sr.ReadSingle();
            TireSurfaceTemperatureRL = sr.ReadSingle();
            TireSurfaceTemperatureRR = sr.ReadSingle();
            TotalTimeTicks = sr.ReadInt32(); // can't be more than MAX_LAPTIME1000 - which is 1209599999, or else it's set to -1
            CurrentLap = sr.ReadInt32();
            BestLapTime = TimeSpan.FromMilliseconds(sr.ReadInt32());
            LastLapTime = TimeSpan.FromMilliseconds(sr.ReadInt32());
            DayProgressionMS = sr.ReadInt32();
            PreRaceStartPositionOrQualiPos = sr.ReadInt16();
            NumCarsAtPreRace = sr.ReadInt16();
            MinAlertRPM = sr.ReadInt16();
            MaxAlertRPM = sr.ReadInt16();
            CalculatedMaxSpeed = sr.ReadInt16();
            Flags = (SimulatorFlags)sr.ReadInt16();

            int bits = sr.ReadByte();
            CurrentGear = (byte)(bits & 0b1111);
            SuggestedGear = (byte)(bits >> 4);

            Throttle = sr.ReadByte();
            Brake = sr.ReadByte();

            //short throttleAndBrake = sr.ReadInt16();
            byte unknown = sr.ReadByte();

            TireFL_Unknown0x94_0 = sr.ReadSingle();
            TireFR_Unknown0x94_1 = sr.ReadSingle();
            TireRL_Unknown0x94_2 = sr.ReadSingle();
            TireRR_Unknown0x94_3 = sr.ReadSingle();
            TireFL_Accel = sr.ReadSingle();
            TireFR_Accel = sr.ReadSingle();
            TireRL_Accel = sr.ReadSingle();
            TireRR_Accel = sr.ReadSingle();
            TireFL_UnknownB4 = sr.ReadSingle();
            TireFR_UnknownB4 = sr.ReadSingle();
            TireRL_UnknownB4 = sr.ReadSingle();
            TireRR_UnknownB4 = sr.ReadSingle();
            TireFL_UnknownC4 = sr.ReadSingle();
            TireFR_UnknownC4 = sr.ReadSingle();
            TireRL_UnknownC4 = sr.ReadSingle();
            TireRR_UnknownC4 = sr.ReadSingle();

            sr.Position += sizeof(int) * 8; // Seems to be reserved - server does not set that

            dataPacket.Unknown_0xF4 = sr.ReadSingle();
            dataPacket.Unknown_0xF8 = sr.ReadSingle();
            dataPacket.RPMUnknown_0xFC = sr.ReadSingle();

            dataPacket.Unknown_0x100_GearRelated = sr.ReadSingle();
            for (var i = 0; i < 7; i++)
                dataPacket.GearRatios[i] = sr.ReadSingle();

            // Normally this one is not set at all. The game memcpy's the gear ratios without bound checking
            // The LC500 which has 10 gears even overrides the car code 😂
            float empty_or_gearRatio8 = sr.ReadSingle();

            dataPacket.CarCode = sr.ReadInt32();
        }
    }

    [Flags]
    public enum SimulatorFlags : short
    {
        None = 0,

        OnTrack = 1 << 0,
        Paused = 1 << 1,
        LoadingOrProcessing = 1 << 2,

        /// <summary>
        /// Needs more investigation
        /// </summary>
        HasThrottleControlMaybe = 1 << 3,

        HasTurbo = 1 << 4,
        RevLimiterBlinkAlertActive = 1 << 5,
        HandBrakeActive = 1 << 6,
        LightsActive = 1 << 7,
        HighBeamActive = 1 << 8,
        LowBeamActive = 1 << 9,
        ASMActive = 1 << 10,
        TCSActive = 1 << 11,
    }
}
