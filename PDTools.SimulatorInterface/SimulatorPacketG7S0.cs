using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using System.Numerics;
using Syroot.BinaryData.Memory;

namespace PDTools.SimulatorInterface
{
    /// <summary>
    /// Used by GT7 and GT Sport.
    /// </summary>
    public class SimulatorPacketG7S0 : SimulatorPacketBase
    {
        /// <summary>
        /// Position on the track. Track units are in meters.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Velocity in track units (which are meters) for each axis.
        /// </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Rotation (Pitch/Yaw/Roll) from -1 to 1.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Orientation to North. 1.0 is north, 0.0 is south.
        /// </summary>
        public float RelativeOrientationToNorth { get; set; }

        /// <summary>
        /// How fast the car turns around axes. (In radians/second, -1 to 1).
        /// </summary>
        public Vector3 AngularVelocity { get; set; }

        /// <summary>
        /// Body height.
        /// </summary>
        public float BodyHeight { get; set; }

        /// <summary>
        /// Engine revolutions per minute
        /// </summary>
        public float EngineRPM { get; set; }

        /// <summary>
        /// Stays at 100, not fuel, nor tyre wear
        /// </summary>
        public float Unknown_0x48 { get; set; }

        /// <summary>
        /// Current speed in meters per second. <see cref="MetersPerSecond * 3.6"/> to get it in KPH.
        /// </summary>
        public float MetersPerSecond { get; set; }

        /// <summary>
        /// Value below 1.0 is below 0 ingame, so 2.0 = 1 x 100kPa
        /// </summary>
        public float TurboBoost { get; set; }

        /// <summary>
        /// Oil Pressure (in Bars)
        /// </summary>
        public float OilPressure { get; set; }

        /// <summary>
        /// Games will always send 85.
        /// </summary>
        public float WaterTemperature { get; set; }

        /// <summary>
        /// Games will always send 110.
        /// </summary>
        public float OilTemperature { get; set; }

        /// <summary>
        /// Front Left Tire - Surface Temperature (in °C)
        /// </summary>
        public float TireFL_SurfaceTemperature { get; set; }

        /// <summary>
        /// Front Right - Surface Temperature (in °C)
        /// </summary>
        public float TireFR_SurfaceTemperature { get; set; }

        /// <summary>
        /// Rear Left - Surface Temperature (in °C)
        /// </summary>
        public float TireRL_SurfaceTemperature { get; set; }

        /// <summary>
        /// Rear Right - Surface Temperature (in °C)
        /// </summary>
        public float TireRR_SurfaceTemperature { get; set; }

        /// <summary>
        /// Id of the packet for proper ordering.
        /// </summary>
        public int PacketId { get; set; }

        /// <summary>
        /// Current lap count.
        /// </summary>
        public short LapCount { get; set; }

        /// <summary>
        /// Laps to finish.
        /// </summary>
        public short LapsInRace { get; set; }

        /// <summary>
        /// Best Lap Time. Defaults to -1 millisecond when not set.
        /// </summary>
        public TimeSpan BestLapTime { get; set; }

        /// <summary>
        /// Last Lap Time. Defaults to -1 millisecond when not set.
        /// </summary>
        public TimeSpan LastLapTime { get; set; }

        /// <summary>
        /// Current time of day on the track.
        /// </summary>
        public TimeSpan TimeOfDayProgression { get; set; }

        /// <summary>
        /// Needs more investigation
        /// </summary>
        public short PreRaceStartPositionOrQualiPos { get; set; }

        /// <summary>
        /// Needs more investigation
        /// </summary>
        public short NumCarsAtPreRace { get; set; }

        /// <summary>
        /// Minimum RPM to which the car shows an alert.
        /// </summary>
        public short MinAlertRPM { get; set; }

        /// <summary>
        /// Maximum RPM to the alert.
        /// </summary>
        public short MaxAlertRPM { get; set; }

        /// <summary>
        /// Updates weirdly, needs investigation
        /// </summary>
        public short CalculatedMaxSpeed { get; set; }

        /// <summary>
        /// Packet flags.
        /// </summary>
        public SimulatorFlags Flags { get; set; }

        /// <summary>
        /// Current Gear for the car.
        /// </summary>
        public byte CurrentGear { get; set; }

        /// <summary>
        /// (Assist) Suggested Gear to downshift to.
        /// </summary>
        public byte SuggestedGear { get; set; }

        /// <summary>
        /// Throttle (0-255)
        /// </summary>
        public byte Throttle { get; set; }

        /// <summary>
        /// Brake Pedal (0-255)
        /// </summary>
        public byte Brake { get; set; }

        public byte Unknown0x93 { get; set; }

        public float TireFL_Unknown0x94_0 { get; set; }
        public float TireFR_Unknown0x94_1 { get; set; }
        public float TireRL_Unknown0x94_2 { get; set; }
        public float TireRR_Unknown0x94_3 { get; set; }

        /// <summary>
        /// Front Left Tire - Revolutions Per Second (in Radians)
        /// </summary>
        public float TireFL_RevPerSecond { get; set; }

        /// <summary>
        /// Front Right Tire - Revolutions Per Second (in Radians)
        /// </summary>
        public float TireFR_RevPerSecond { get; set; }

        /// <summary>
        /// Rear Left Tire - Revolutions Per Second (in Radians)
        /// </summary>
        public float TireRL_RevPerSecond { get; set; }

        /// <summary>
        /// Rear Right Tire - Revolutions Per Second (in Radians)
        /// </summary>
        public float TireRR_RevPerSecond { get; set; }

        /// <summary>
        /// Front Left Tire - Tire Radius
        /// </summary>
        public float TireFL_TireRadius { get; set; }

        /// <summary>
        /// Front Right Tire - Tire Radius
        /// </summary>
        public float TireFR_TireRadius { get; set; }

        /// <summary>
        /// Rear Left Tire - Tire Radius
        /// </summary>
        public float TireRL_TireRadius { get; set; }

        /// <summary>
        /// Rear Right Tire - Tire Radius
        /// </summary>
        public float TireRR_TireRadius { get; set; }

        /// <summary>
        /// Front Left Tire - Suspension Height
        /// </summary>
        public float TireFL_SusHeight { get; set; }

        /// <summary>
        /// Front Right Tire - Suspension Height
        /// </summary>
        public float TireFR_SusHeight { get; set; }

        /// <summary>
        /// Rear Left Tire - Suspension Height
        /// </summary>
        public float TireRL_SusHeight { get; set; }

        /// <summary>
        /// Rear Right Tire - Suspension Height
        /// </summary>
        public float TireRR_SusHeight { get; set; }

        /// <summary>
        /// 0.0 to 1.0
        /// </summary>
        public float ClutchPedal { get; set; }

        /// <summary>
        /// 0.0 to 1.0
        /// </summary>
        public float ClutchEngagement { get; set; }

        /// <summary>
        /// Basically same as engine rpm when in gear and the clutch pedal is not depressed.
        /// </summary>
        public float RPMFromClutchToGearbox { get; set; }

        public float Unknown_0x100_GearRelated { get; set; }

        /// <summary>
        /// Gear ratios for the car. Up to 7.
        /// </summary>
        public float[] GearRatios { get; set; } = new float[7];

        /// <summary>
        /// Internal code that identifies the car. This value may be overriden if using a car that uses 9 or more gears.
        /// </summary>
        public int CarCode { get; set; }

        public override void Read(Span<byte> data)
        {
            SpanReader sr = new SpanReader(data);
            int magic = sr.ReadInt32();
            if (magic != 0x47375330) // 0S7G - G7S0
                throw new InvalidDataException($"Unexpected packet magic '{magic}'.");

            Position = new Vector3(sr.ReadSingle(), sr.ReadSingle(), sr.ReadSingle());
            Velocity = new Vector3(sr.ReadSingle(), sr.ReadSingle(), sr.ReadSingle());
            Rotation = new Vector3(sr.ReadSingle(), sr.ReadSingle(), sr.ReadSingle());
            RelativeOrientationToNorth = sr.ReadSingle();
            AngularVelocity = new Vector3(sr.ReadSingle(), sr.ReadSingle(), sr.ReadSingle());
            BodyHeight = sr.ReadSingle();
            EngineRPM = sr.ReadSingle();

            // Skip IV
            sr.Position += 8;

            Unknown_0x48 = sr.ReadSingle();
            MetersPerSecond = sr.ReadSingle();
            TurboBoost = sr.ReadSingle();
            OilPressure = sr.ReadSingle();
            WaterTemperature = sr.ReadSingle();
            OilTemperature = sr.ReadSingle();
            TireFL_SurfaceTemperature = sr.ReadSingle();
            TireFR_SurfaceTemperature = sr.ReadSingle();
            TireRL_SurfaceTemperature = sr.ReadSingle();
            TireRR_SurfaceTemperature = sr.ReadSingle();
            PacketId = sr.ReadInt32();
            LapCount = sr.ReadInt16();
            LapsInRace = sr.ReadInt16();
            BestLapTime = TimeSpan.FromMilliseconds(sr.ReadInt32());
            LastLapTime = TimeSpan.FromMilliseconds(sr.ReadInt32());
            TimeOfDayProgression = TimeSpan.FromMilliseconds(sr.ReadInt32());
            PreRaceStartPositionOrQualiPos = sr.ReadInt16();
            NumCarsAtPreRace = sr.ReadInt16();
            MinAlertRPM = sr.ReadInt16();
            MaxAlertRPM = sr.ReadInt16();
            CalculatedMaxSpeed = sr.ReadInt16();
            Flags = (SimulatorFlags)sr.ReadInt16();

            int bits = sr.ReadByte();
            CurrentGear = (byte)(bits & 0b1111); // 4 bits
            SuggestedGear = (byte)(bits >> 4); // Also 4 bits

            Throttle = sr.ReadByte();
            Brake = sr.ReadByte();

            Unknown0x93 = sr.ReadByte();

            TireFL_Unknown0x94_0 = sr.ReadSingle();
            TireFR_Unknown0x94_1 = sr.ReadSingle();
            TireRL_Unknown0x94_2 = sr.ReadSingle();
            TireRR_Unknown0x94_3 = sr.ReadSingle();
            TireFL_RevPerSecond = sr.ReadSingle();
            TireFR_RevPerSecond = sr.ReadSingle();
            TireRL_RevPerSecond = sr.ReadSingle();
            TireRR_RevPerSecond = sr.ReadSingle();
            TireFL_TireRadius = sr.ReadSingle();
            TireFR_TireRadius = sr.ReadSingle();
            TireRL_TireRadius = sr.ReadSingle();
            TireRR_TireRadius = sr.ReadSingle();
            TireFL_SusHeight = sr.ReadSingle();
            TireFR_SusHeight = sr.ReadSingle();
            TireRL_SusHeight = sr.ReadSingle();
            TireRR_SusHeight = sr.ReadSingle();

            sr.Position += sizeof(int) * 8; // Seems to be reserved - server does not set that

            ClutchPedal = sr.ReadSingle();
            ClutchEngagement = sr.ReadSingle();
            RPMFromClutchToGearbox = sr.ReadSingle();

            Unknown_0x100_GearRelated = sr.ReadSingle();

            for (var i = 0; i < 7; i++)
                GearRatios[i] = sr.ReadSingle();

            // Normally this one is not set at all. The game memcpy's the gear ratios without bound checking
            // The LC500 which has 10 gears even overrides the car code 😂
            float empty_or_gearRatio8 = sr.ReadSingle();

            CarCode = sr.ReadInt32();
        }

        public override void PrintPacket(bool debug = false)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Simulator Interface Packet                    ");
            Console.WriteLine("[Car Data]          ");
            Console.WriteLine($"- Car Code: {CarCode}         ");
            Console.WriteLine($"- Throttle: {Throttle}   ");
            Console.WriteLine($"- Brake: {Brake}   ");
            Console.WriteLine($"- RPM: {EngineRPM} - KPH: {Math.Round(MetersPerSecond * 3.6, 2)}     ");
            Console.WriteLine($"- Turbo Boost: {((TurboBoost - 1.0) * 100.0):F2}kPa   ");
            Console.WriteLine($"- Oil Pressure: {OilPressure:F2}   ");
            Console.WriteLine($"- Body Height: {BodyHeight:F2}   ");
            Console.WriteLine($"- Clutch Pedal: {ClutchPedal:F2}   ");
            Console.WriteLine($"- Clutch Engagement: {ClutchEngagement:F2}   ");
            Console.WriteLine($"- RPM From Clutch To Gearbox: {RPMFromClutchToGearbox}   ");


            if (SuggestedGear == 15)
                Console.WriteLine($"- Gear: {CurrentGear}                                    ");
            else
                Console.WriteLine($"- Gear: {CurrentGear} (Suggested: {SuggestedGear})");
            Console.WriteLine($"- Calculated Max Speed: {CalculatedMaxSpeed}  ");
            Console.WriteLine($"- Min/Max RPM Alerts: {MinAlertRPM} - {MaxAlertRPM}  ");

            Console.WriteLine($"- Flags: {Flags,-100}");
            Console.WriteLine($"- Gear Ratios: {string.Join(", ", GearRatios)}");
            Console.WriteLine($"- Tire Height / FL:{TireFL_SusHeight:F2} FR:{TireFR_SusHeight:F2} RL:{TireRL_SusHeight:F2} RR:{TireRR_SusHeight:F2}");
            Console.WriteLine($"- Tire RPS / FL:{TireFL_RevPerSecond:F2} FR:{TireFR_RevPerSecond:F2} RL:{TireRL_RevPerSecond:F2} RR:{TireRR_RevPerSecond:F2}");
            Console.WriteLine($"- Tire Radius / FL:{TireFL_TireRadius:F2} FR:{TireFR_TireRadius:F2} RL:{TireRL_TireRadius:F2} RR:{TireRR_TireRadius:F2}");

            Console.WriteLine($"- Tire Temperature");
            Console.WriteLine($"    FL: {TireFL_SurfaceTemperature:F2}°C | FR: {TireFR_SurfaceTemperature:F2}°C   ");
            Console.WriteLine($"    RL: {TireRL_SurfaceTemperature:F2}°C | RR: {TireRR_SurfaceTemperature:F2}°C   ");

            Console.WriteLine();
            Console.WriteLine("[Race Data]");

            Console.WriteLine($"- Packet Id: {PacketId}     ");
            Console.WriteLine($"- Current Lap: {LapCount}  ");

            if (BestLapTime.TotalMilliseconds == -1)
                Console.WriteLine($"- Best: N/A      ");
            else
                Console.WriteLine($"- Best: {BestLapTime:mm\\:ss\\.fff}     ");

            if (LastLapTime.TotalMilliseconds == -1)
                Console.WriteLine($"- Last: N/A      ");
            else
                Console.WriteLine($"- Last: {LastLapTime:mm\\:ss\\.fff}     ");

            Console.WriteLine($"- Time of Day: {TimeOfDayProgression:hh\\:mm\\:ss}     ");
            Console.WriteLine($"- PreRaceStartPositionOrQualiPos: {PreRaceStartPositionOrQualiPos}");
            Console.WriteLine($"- NumCarsAtPreRace: {NumCarsAtPreRace}");

            Console.WriteLine();
            Console.WriteLine("[Positional Information]");
            Console.WriteLine($"- Position: {Position:F3}     ");
            Console.WriteLine($"- Velocity: {Velocity:F3}    ");
            Console.WriteLine($"- Rotation: {Rotation:F3}     ");
            Console.WriteLine($"- Angular Velocity: {AngularVelocity:F2}   ");

            if (debug)
            {
                Console.WriteLine();
                Console.WriteLine("[Unknowns]");
                Console.WriteLine($"0x48 (Float): {Unknown_0x48:F2}   ");
                Console.WriteLine($"0x93 (byte): {Unknown0x93:F2}   ");
                Console.WriteLine($"0x94 Tire/Wheel Related (Float): {TireFL_Unknown0x94_0:F2} {TireFR_Unknown0x94_1:F2} {TireRL_Unknown0x94_2:F2} {TireRR_Unknown0x94_3:F2}   ");
                Console.WriteLine($"0x100 Gear Ratio Related (Float): {Unknown_0x100_GearRelated}   ");

            }
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
