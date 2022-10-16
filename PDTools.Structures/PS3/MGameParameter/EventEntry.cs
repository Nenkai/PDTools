using System;
using System.Xml;
using System.ComponentModel;

using PDTools.Utils;
using PDTools.Structures.PS3;

namespace PDTools.Structures.PS3.MGameParameter
{
    public class EventEntry
    {
        public bool IsAI { get; set; }
        public bool IsPresentEntry { get; set; }

        public string DriverName { get; set; } = "Unnamed";
        public string DriverRegion { get; set; } = "PDI";

        #region Skill Properties
        private short _baseSkill = 80;
        public short BaseSkill
        {
            get => _baseSkill;
            set
            {
                if (value <= 200 && value >= -1)
                    _baseSkill = value;
            }
        }

        private short _brakingSkill = 80;
        public short BrakingSkill
        {
            get => _brakingSkill;
            set
            {
                if (value <= 200 && value >= -1)
                    _brakingSkill = value;
            }
        }

        private short _corneringSKill = 80;
        public short CorneringSkill
        {
            get => _corneringSKill;
            set
            {
                if (value <= 200 && value >= -1)
                    _corneringSKill = value;
            }
        }

        private sbyte _accelSkill = 80;
        public sbyte AccelSkill
        {
            get => _accelSkill;
            set
            {
                if (value <= 100 && value >= -1)
                    _accelSkill = value;
            }
        }

        private sbyte _startSkill = 80;
        public sbyte StartingSkill
        {
            get => _startSkill;
            set
            {
                if (value <= 100 && value >= -1)
                    _startSkill = value;
            }
        }

        private sbyte _roughness = -1;
        public sbyte Roughness
        {
            get => _roughness;
            set
            {
                if (value <= 10 && value >= -1)
                    _roughness = value;
            }
        }
        #endregion

        private int _delay = 0;
        public int Delay
        {
            get => _delay;
            set
            {
                if (value <= 3_600_000 && value >= 0)
                    _delay = value;
            }
        }

        private int _initialVelocity = -1;
        public int InitialVelocity
        {
            get => _initialVelocity;
            set
            {
                if (value <= 1000 && value >= -1)
                    _initialVelocity = value;
            }
        }

        private int _initialVCoord = -1;
        public int InitialVCoord
        {
            get => _initialVCoord;
            set
            {
                if (value <= 99999)
                    _initialVCoord = value;
            }
        }

        #region Car Setting Related Properties
        private int _maxGearSpeed;
        public int MaxGearSpeed
        {
            get => _maxGearSpeed;
            set
            {
                if (value < 0)
                    value = 0;
                _maxGearSpeed = value;
            }
        }

        private uint _powerLimiter = 1000;
        public uint PowerLimiter
        {
            get => _powerLimiter;
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1000)
                    value = 1000;
                _powerLimiter = value;
            }
        }

        public byte BallastWeight { get; set; }
        public sbyte BallastPosition { get; set; } = -1;
        public sbyte DownforceRear { get; set; } = -1;
        public sbyte DownforceFront { get; set; } = -1;

        public short BodyPaintID { get; set; } = -1;
        public short WheelPaintID { get; set; } = -1;
        public int WheelID { get; set; } = -1;
        public int WheelInchUp { get; set; } = -1;
        public int AeroKit { get; set; } = -1;
        public int FlatFloor { get; set; } = -1;
        public int AeroOther { get; set; } = -1;

        #endregion

        #region Default Parameters
        private static readonly byte[] defaultCarParameter = new byte[]
        {
            0x00, 0x00, 0x00, 0x6D, 0x7E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x4F, 0x9D, 0xBE, 0x7C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4F, 0x00, 0x00, 0x00,
            0x00, 0x64, 0x00, 0x00, 0x00, 0x64, 0x05, 0x05, 0x01, 0x00, 0x00, 0x00, 0x00, 0xBE, 0x03, 0xE8,
            0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x01, 0x80, 0x00, 0x00, 0x00, 0x7F, 0xFF, 0xFF, 0xFF, 0x80, 0x00, 0x1F, 0xC0, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        public static readonly byte[] defaultDriverParameter = new byte[]
        {
            0x00, 0x00, 0x00, 0x70, 0x00, 0x00, 0x00, 0xC0, 0xF0, 0xFD, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x83, 0x00, 0x01, 0x00, 0x01, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x64, 0x64,
            0x00, 0x64, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x20, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        #endregion

        public int? CarCode { get; set; }
        public string CarLabel { get; set; }

        public string ActualCarName { get; set; }
        public StartType StartType { get; set; } = StartType.GRID;

        public byte RaceClassID { get; set; }

        public short ColorIndex { get; set; }
        public TireType TireFront { get; set; } = TireType.NONE_SPECIFIED;
        public TireType TireRear { get; set; } = TireType.NONE_SPECIFIED;

        public EngineNATuneState EngineStage { get; set; } = EngineNATuneState.NONE;
        public EngineTurboKit TurboKit { get; set; } = EngineTurboKit.NONE;
        public EngineComputer Computer { get; set; } = EngineComputer.NONE;
        public Muffler Exhaust { get; set; } = Muffler.UNSPECIFIED;
        public Suspension Suspension { get; set; } = Suspension.UNSPECIFIED;
        public Transmission Transmission { get; set; } = Transmission.UNSPECIFIED;

        // For reading
        public MCarParameter CarParameter { get; set; }

        public EventEntry(bool isPlayer = false)
        {
            if (isPlayer)
                SetAsPlayerSkills();
        }

        public void SetAsPlayerSkills()
        {
            BaseSkill = 100;

            AccelSkill = -1;
            BrakingSkill = -1;
            CorneringSkill = -1;
            StartingSkill = -1;
            Roughness = -1;
        }

        public void WriteToXml(XmlWriter xml, bool isFixed)
        {
            if (isFixed)
                xml.WriteStartElement("entry");
            else
                xml.WriteStartElement("entry_base");

            if (!string.IsNullOrEmpty(CarLabel) && (IsAI || isFixed)) // Pointless to write it if its a player as entry_base; it gets ignored
            {
                xml.WriteStartElement("car");
                xml.WriteAttributeString("color", ColorIndex.ToString());
                xml.WriteAttributeString("label", CarLabel);
                xml.WriteEndElement();
            }

            if (!IsPresentEntry)
            {
                if (isFixed)
                {
                    if (InitialVCoord != -1)
                        xml.WriteElementInt("initial_position", InitialVCoord);
                    if (InitialVelocity != -1)
                        xml.WriteElementInt("initial_velocity", InitialVelocity);

                    xml.WriteElementInt("delay", Delay);
                    if (StartType != StartType.GRID)
                        xml.WriteElementValue("start_type", StartType.ToString());
                }

                xml.WriteElementValue("driver_name", IsAI ? DriverName : "Player");

                xml.WriteElementInt("player_no", IsAI ? -1 : 0);

                if (IsAI)
                    xml.WriteElementValue("driver_region", DriverRegion);

                xml.WriteElementInt("race_class_id", RaceClassID);

                if (IsAI)
                {
                    xml.WriteElementInt("ai_skill", BaseSkill);
                    xml.WriteElementInt("ai_skill_accelerating", AccelSkill);
                    xml.WriteElementInt("ai_skill_breaking", BrakingSkill);
                    xml.WriteElementInt("ai_skill_cornering", CorneringSkill);
                    xml.WriteElementInt("ai_skill_starting", StartingSkill);
                    xml.WriteElementInt("ai_roughness", Roughness);
                }
            }

            // Fixed entries can have a child entry_base.
            if (isFixed)
                xml.WriteStartElement("entry_base");

            if (EngineStage != EngineNATuneState.NONE)
                xml.WriteElementValue("engine_na_tune_stage", EngineStage.ToString());
            if (TurboKit != EngineTurboKit.NONE)
                xml.WriteElementValue("engine_turbo_kit", TurboKit.ToString());
            if (Computer != EngineComputer.NONE)
                xml.WriteElementValue("engine_computer", Computer.ToString());
            if (Exhaust != Muffler.UNSPECIFIED)
                xml.WriteElementValue("muffler", Exhaust.ToString());
            if (Suspension != Suspension.UNSPECIFIED)
                xml.WriteElementValue("suspension", Suspension.ToString());
            if (Transmission != Transmission.UNSPECIFIED)
                xml.WriteElementValue("transmission", Transmission.ToString());

            if (PowerLimiter != 0 && PowerLimiter != 1000)
                xml.WriteElementUInt("power_limiter", PowerLimiter);

            if (MaxGearSpeed != 0)
                xml.WriteElementInt("gear_max_speed", MaxGearSpeed);

            if (BodyPaintID != -1)
                xml.WriteElementInt("paint_id", BodyPaintID);

            if (WheelID != -1)
                xml.WriteElementInt("wheel", WheelID);

            if (WheelPaintID != -1)
                xml.WriteElementInt("wheel_color", WheelPaintID);

            if (WheelInchUp != -1)
                xml.WriteElementInt("wheel_inch_up", WheelInchUp);

            if (BallastWeight != 0)
                xml.WriteElementInt("ballast_weight", BallastWeight);

            if (BallastPosition != -1)
                xml.WriteElementInt("ballast_position", BallastPosition);

            if (DownforceFront != -1)
                xml.WriteElementInt("downforce_f", DownforceFront);

            if (DownforceRear != -1)
                xml.WriteElementInt("downforce_r", DownforceRear);

            if (AeroKit != -1)
                xml.WriteElementInt("aero_1", AeroKit);
            if (FlatFloor != -1)
                xml.WriteElementInt("aero_2", FlatFloor);
            if (AeroOther != -1)
                xml.WriteElementInt("aero_3", AeroOther);

            if (TireFront != TireType.NONE_SPECIFIED)
                xml.WriteElementValue("tire_f", TireFront.ToString());
            if (TireRear != TireType.NONE_SPECIFIED)
                xml.WriteElementValue("tire_r", TireRear.ToString());

            if (isFixed)
                xml.WriteEndElement();

            xml.WriteEndElement();
        }

        public void ReadEntryBaseFromCache(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5_EB_45_F8 && magic != 0xE6_EB_45_F8)
                throw new System.IO.InvalidDataException($"Course magic did not match - Got {magic.ToString("X8")}, expected 0xE6EB45F8");

            int version = reader.ReadInt32();

            CarCode = reader.ReadInt32();
            ColorIndex = reader.ReadInt16();
            reader.ReadInt16();
            reader.ReadInt32();

            DriverName = reader.ReadString4Aligned();
            DriverRegion = reader.ReadString4Aligned();
            RaceClassID = reader.ReadByte();
            reader.ReadSByte();

            int boost_rate_count = reader.ReadInt32();
            if (boost_rate_count > 0)
            {
                for (int i = 0; i < boost_rate_count; i++)
                    reader.ReadByte();
            }

            BrakingSkill = reader.ReadInt16();
            CorneringSkill = reader.ReadInt16();
            AccelSkill = reader.ReadSByte();
            StartingSkill = reader.ReadSByte();
            Roughness = reader.ReadSByte();

            EngineStage = (EngineNATuneState)reader.ReadSByte();
            TurboKit = (EngineTurboKit)reader.ReadSByte();
            Computer = (EngineComputer)reader.ReadSByte();
            Exhaust = (Muffler)reader.ReadSByte();
            Suspension = (Suspension)reader.ReadSByte();
            Transmission = (Transmission)reader.ReadSByte();

            WheelID = reader.ReadInt16();

            if (version >= 1_02)
            {
                WheelPaintID = reader.ReadInt16();
                WheelInchUp = reader.ReadInt16();
            }

            TireFront = (TireType)reader.ReadSByte();
            TireRear = (TireType)reader.ReadSByte();

            // Aero
            reader.ReadSByte();
            reader.ReadSByte();
            reader.ReadSByte();
            reader.ReadSByte();

            PowerLimiter = (uint)reader.ReadSByte() * 10;
            DownforceFront = reader.ReadSByte();
            DownforceRear = reader.ReadSByte();
            BodyPaintID = reader.ReadInt16();

            if (version >= 1_02)
            {
                reader.ReadInt16();
                reader.ReadInt16(); // Decken Number
            }

            if (version >= 1_03)
            {
                reader.ReadInt16(); // Head Code
                reader.ReadInt16(); // Body Code
                reader.ReadInt16(); // Head Color
                reader.ReadInt16(); // Body Color
            }

            if (version >= 1_04)
            {
                reader.ReadByte(); // AI Reaction
                BallastWeight = reader.ReadByte();
                BallastPosition = reader.ReadSByte();
            }

            if (version >= 1_05)
            {
                reader.ReadByte(); // Decken Type
                reader.ReadByte(); // Decken Custom ID
            }

            if (version >= 1_06)
                reader.ReadByte(); // Decken Custom Type
        }

        public void ReadEntryFromCache(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5_E5_25_EE && magic != 0xE6_E6_25_EE)
                throw new System.IO.InvalidDataException($"Course magic did not match - Got {magic.ToString("X8")}, expected 0xE6E625EE");
            int version = reader.ReadInt32();

            int player_no = reader.ReadInt32();
            if (player_no != 0)
                IsAI = true;

            MCarThin car = new MCarThin(0);
            car.Read(ref reader);

            CarParameter = MCarParameter.ImportFromBlob(ref reader);
            DriverName = reader.ReadString4Aligned();
            DriverRegion = reader.ReadString4Aligned();

            for (int i = 0; i < 4; i++)
                MCarDriverParameter.Read(ref reader);

            var entry = new EventEntry();
            if (version >= 1_08)
                entry.ReadEntryBaseFromCache(ref reader);

            reader.ReadBool(); // Available initial position
            RaceClassID = reader.ReadByte();
            reader.ReadByte(); // Proxy driver model
            reader.ReadByte(); // Pilot ID
            InitialVCoord = reader.ReadInt32();
            InitialVelocity = reader.ReadInt32();
            StartType = (StartType)reader.ReadInt32();
            Delay = reader.ReadInt32();
            reader.ReadByte(); // No suitable tyre
            reader.ReadInt16(); // Initial fuel 100

            int boost_rate_count = reader.ReadInt32();
            reader.ReadIntoByteArray(boost_rate_count, new byte[boost_rate_count], 8);
            reader.ReadIntoByteArray(boost_rate_count, new byte[boost_rate_count], 8);

            BrakingSkill = reader.ReadInt16();
            CorneringSkill = reader.ReadInt16();
            AccelSkill = reader.ReadSByte();
            StartingSkill = reader.ReadSByte();
            Roughness = reader.ReadSByte();

            if (version >= 1_07)
                reader.ReadByte(); // Unk
        }

        public void WriteEntryBaseToBuffer(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_EB_45_F8);
            bs.WriteUInt32(1_06);

            // Write car (carthin)
            bs.WriteInt32(CarCode is null ? -1 : CarCode.Value);
            bs.WriteInt16(ColorIndex);
            bs.WriteInt16(0);
            bs.WriteInt32(-1);

            bs.WriteNullStringAligned4(DriverName);
            bs.WriteNullStringAligned4(DriverRegion);
            bs.WriteByte(RaceClassID);
            bs.WriteSByte(-1); // Proxy Driver Model

            // boost_rate count, ignore with empty list
            bs.WriteInt32(0);

            bs.WriteInt16(BrakingSkill);
            bs.WriteInt16(CorneringSkill);
            bs.WriteSByte(AccelSkill);
            bs.WriteSByte(StartingSkill);
            bs.WriteSByte(Roughness);

            bs.WriteSByte((sbyte)EngineStage);
            bs.WriteSByte((sbyte)TurboKit);
            bs.WriteSByte((sbyte)Computer);
            bs.WriteSByte((sbyte)Exhaust);
            bs.WriteSByte((sbyte)Suspension);
            bs.WriteSByte((sbyte)Transmission);

            bs.WriteInt16((sbyte)WheelID);
            bs.WriteInt16((sbyte)WheelPaintID);
            bs.WriteInt16((sbyte)WheelInchUp);
            bs.WriteSByte((sbyte)TireFront);
            bs.WriteSByte((sbyte)TireRear);

            // Aero stuff
            bs.WriteSByte(-1);
            bs.WriteSByte(-1);
            bs.WriteSByte(-1);
            bs.WriteSByte(-1);

            sbyte powLimiter = -1;
            if (PowerLimiter != 1000 && PowerLimiter != 0) // Ensure to guard against nulled engines..
                powLimiter = (sbyte)((int)PowerLimiter / 10);
            bs.WriteSByte(powLimiter);
            bs.WriteSByte(DownforceFront);
            bs.WriteSByte(DownforceRear);
            bs.WriteInt16(BodyPaintID);
            bs.WriteInt16(-1); // Unk
            bs.WriteInt16(-1); // decken_number

            bs.WriteInt16(-1); // head/body codes
            bs.WriteInt16(-1);
            bs.WriteInt16(-1);
            bs.WriteInt16(-1);

            bs.WriteByte(0); // ai_reaction
            bs.WriteByte(BallastWeight);
            bs.WriteSByte(BallastPosition);
            bs.WriteSByte(-1); // Decken Type
            bs.WriteSByte(-1); // Decken Custom ID
            bs.WriteSByte(-1); // Decken Custom Type
        }

        public void WriteEntryToBuffer(ref BitStream bs, bool isPlayer)
        {
            bs.WriteUInt32(0xE6_E6_25_EE);
            bs.WriteUInt32(1_08);

            bs.WriteInt32(0); // player_no

            // Write car (carthin)
            bs.WriteInt32(CarCode is null ? -1 : CarCode.Value);
            bs.WriteInt16(ColorIndex);
            bs.WriteInt16(0);
            bs.WriteInt32(-1);

            bs.WriteByteData(defaultCarParameter, false);

            bs.WriteNullStringAligned4(DriverName);
            bs.WriteNullStringAligned4(DriverRegion);

            for (int i = 0; i < 4; i++)
                bs.WriteByteData(defaultDriverParameter, false);

            new EventEntry(isPlayer).WriteEntryBaseToBuffer(ref bs);

            // available_initial_position - Flicked if the positions are available
            bs.WriteBool(InitialVCoord != -1 && InitialVelocity != -1);

            bs.WriteSByte(0); // race_class_id
            bs.WriteSByte(-1); // Proxy driver model
            bs.WriteSByte(0); // Pilot id

            // If a position is available, write them, else, they're defaulted to 0 rather than 1
            if (InitialVCoord != -1 && InitialVelocity != -1)
            {
                bs.WriteInt32(InitialVCoord);
                bs.WriteInt32(InitialVelocity);
            }
            else
            {
                bs.WriteInt32(0);
                bs.WriteInt32(0);
            }

            bs.WriteInt32((int)StartType); // Start Type
            bs.WriteInt32(Delay == 0 ? -1 : Delay);
            bs.WriteSByte(0); // no_suitable_tyre
            bs.WriteInt16(-1); // initial_fuel100

            bs.WriteInt32(0); // boost_rate_count - ignore

            bs.WriteInt16(BrakingSkill); // Braking
            bs.WriteInt16(CorneringSkill); // cornering
            bs.WriteSByte(AccelSkill); // Accelerating
            bs.WriteSByte(StartingSkill); // Starting
            bs.WriteSByte(Roughness); // AI Roughness
            bs.WriteByte(0); // unk

        }
    }

    public enum EngineNATuneState // PARTS_NATUNE
    {
        [Description("Default")]
        NONE = -1,

        [Description("(Placeholder Level 0)")]
        LEVEL0 = 0,

        [Description("Stage 1")]
        LEVEL1 = 1,

        [Description("Stage 2")]
        LEVEL2,

        [Description("Stage 3")]
        LEVEL3,

        [Description("Stage 4 (Normally Unavailable)")]
        LEVEL4,

        [Description("Stage 5 (Normally Unavailable)")]
        LEVEL5,
    }

    public enum EngineTurboKit // PARTS_TURBINEKIT
    {
        [Description("Default")]
        NONE = -1,

        [Description("NO (?)")]
        NO,

        [Description("Low RPM Range Turbo Kit")]
        LEVEL1,

        [Description("Mid RPM Range Turbo Kit")]
        LEVEL2,

        [Description("High RPM Range Turbo Kit")]
        LEVEL3,

        [Description("Super RPM Range Turbo Kit (Normally Unavailable)")]
        LEVEL4,

        [Description("Ultra RPM Range Turbo Kit (Normally Unavailable)")]
        LEVEL5,
    }

    public enum EngineComputer // PARTS_COMPUTER
    {
        [Description("Default")]
        NONE = -1,

        [Description("Sports Computer")]
        LEVEL1 = 1,
        LEVEL2,
    }

    public enum Muffler // PARTS_MUFFLER
    {
        [Description("Unspecified")]
        UNSPECIFIED = -1,

        [Description("Default")]
        NONE,

        [Description("Sports Exhaust")]
        SPORTS,

        [Description("Semi-Racing Exhaust")]
        SEMIRACING,

        [Description("Racing Exhaust")]
        RACING,
    }

    public enum Suspension // PARTS_SUSPENSION
    {
        [Description("Unspecified")]
        UNSPECIFIED = -1,

        [Description("Default")]
        NORMAL,

        [Description("Racing Suspension: Soft")]
        SPORTS1,

        [Description("Racing Suspension: Hard")]
        SPORTS2,

        [Description("Suspension: Rally")]
        SPORTS3,

        [Description("Height-Adjustable, Fully Customisable Suspension")]
        RACING,

        [Description("Full Active (?)")]
        FULL_ACTIVE,
    }

    public enum Transmission // PARTS_GEAR
    {
        [Description("Unspecified")]
        UNSPECIFIED = -1,

        [Description("Default")]
        NORMAL,

        [Description("Five-Speed Transmission")]
        CLOSE,

        [Description("Six-Speed Transmission")]
        SUPER_CLOSE,

        [Description("Fully Customisable Transmission")]
        VARIABLE,
    }
}
