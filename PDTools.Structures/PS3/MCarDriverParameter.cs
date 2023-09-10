using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using PDTools.Utils;
using PDTools.Enums;

namespace PDTools.Structures.PS3
{
    public class MCarDriverParameter
    {
        public int Version { get; set; }
        public uint unk2;
        public short unk3;
        public string DisplayName { get; set; }
        public string OnlineID { get; set; }
        public string Region { get; set; }
        public byte Port { get; set; }
        public DriverType DriverType { get; set; }
        public byte ResidenceID { get; set; }
        public bool IsGhost { get; set; }
        public bool DisplayDrivingLine { get; set; }

        public DriverSettings Settings { get; set; } = new DriverSettings();

        public byte CorneringSkill { get; set; }
        public byte AcceleratingSkill { get; set; }

        /// <summary>
        /// Linked to <see cref="AIReactionLevel"/> (3 bits), <see cref="AIRoughness"/> (4 bits), <see cref="DisableBSpecSkill"/> (1 bit)
        /// </summary>
        public byte BrakingSkillFlags { get; set; }

        /// <summary>
        /// 3 bits value (max 8)
        /// </summary>
        public byte AIReactionLevel
        {
            get => (byte)(BrakingSkillFlags >> 5 & 0b111);
            set => BrakingSkillFlags |= (byte)((value & 0b111) << 5);
        }

        /// <summary>
        /// 4 bits value (max 15)
        /// </summary>
        public byte AIRoughness
        {
            get => (byte)((BrakingSkillFlags >> 1 & 0b1111) - 1);
            set => BrakingSkillFlags |= (byte)(((value & 0b1111) << 1) + 1);
        }

        public bool DisableBSpecSkill
        {
            get => (BrakingSkillFlags & 1) != 0;
            set => BrakingSkillFlags |= (byte)(value ? 1 : 0);
        }

        public byte SpecialAIType { get; set; }
        public byte StartingSkill { get; set; }
        public byte AILevel { get; set; }

        public short HeadCode { get; set; }
        public short BodyCode { get; set; }
        public short HeadColorCode { get; set; }
        public short BodyColorCode { get; set; }

        public GrowthParameter GrowthParameter { get; } = new GrowthParameter();

        public bool IsVacant()
        {
            return DriverType != DriverType.NONE;
        }

        public void CopyTo(MCarDriverParameter other)
        {
            other.Version = Version;
            other.unk2 = unk2;
            other.unk3 = unk3;
            other.DisplayName = DisplayName;
            other.OnlineID = OnlineID;
            other.Region = Region;
            other.Port = Port;
            other.DriverType = DriverType;
            other.ResidenceID = ResidenceID;
            other.IsGhost = IsGhost;
            other.DisplayDrivingLine = DisplayDrivingLine;
            Settings.CopyTo(other.Settings);
            other.CorneringSkill = CorneringSkill;
            other.AcceleratingSkill = AcceleratingSkill;
            other.BrakingSkillFlags = BrakingSkillFlags;
            other.AIRoughness = AIRoughness;
            other.DisableBSpecSkill = DisableBSpecSkill;
            other.SpecialAIType = SpecialAIType;
            other.StartingSkill = StartingSkill;
            other.AILevel = AILevel;
            other.HeadCode = HeadCode;
            other.BodyCode = BodyCode;
            other.HeadColorCode = HeadColorCode;
            other.BodyColorCode = BodyColorCode;
            GrowthParameter.CopyTo(other.GrowthParameter);
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteElementValue("type", DriverType.ToString());
            xml.WriteElementInt("port", Port);
            xml.WriteElementValue("display_name", DisplayName);
            xml.WriteElementValue("region", Region);
            xml.WriteElementBool("physics_pro", Settings.Physics_Pro);
            xml.WriteElementInt("head_code", HeadCode);
            xml.WriteElementInt("body_code", BodyCode);
            xml.WriteElementInt("head_color_code", HeadColorCode);
            xml.WriteElementInt("body_color_code", BodyColorCode);
            xml.WriteElementInt("braking_skill", BrakingSkillFlags);
            xml.WriteElementInt("cornering_skill", CorneringSkill);
            xml.WriteElementInt("accelerating_skill", AcceleratingSkill);
            xml.WriteElementInt("starting_skill", StartingSkill);
            xml.WriteElementInt("ai_roughness", AIRoughness);
            xml.WriteElementInt("special_ai_type", SpecialAIType);
            xml.WriteElementBool("display_driving_line", DisplayDrivingLine);
        }

        public void ParseFromXml(XmlNode entryNode)
        {
            foreach (XmlNode entryDetailNode in entryNode)
            {
                switch (entryDetailNode.Name)
                {
                    case "type":
                        DriverType = entryDetailNode.ReadValueEnum<DriverType>(); break;
                    case "port":
                        Port = entryDetailNode.ReadValueByte(); break;
                    case "display_name":
                        DisplayName = entryDetailNode.ReadValueString(); break;
                    case "region":
                        Region = entryDetailNode.ReadValueString(); break;
                    case "head_code":
                        HeadColorCode = entryDetailNode.ReadValueShort(); break;
                    case "body_code":
                        BodyCode = entryDetailNode.ReadValueShort(); break;
                    case "head_color_code":
                        HeadColorCode = entryDetailNode.ReadValueShort(); break;
                    case "body_color_code":
                        BodyColorCode = entryDetailNode.ReadValueShort(); break;

                    case "braking_skill":
                        BrakingSkillFlags = entryDetailNode.ReadValueByte(); break;
                    case "cornering_skill":
                        CorneringSkill = entryDetailNode.ReadValueByte(); break;
                    case "accelerating_skill":
                        AcceleratingSkill = entryDetailNode.ReadValueByte(); break;
                    case "starting_skill":
                        StartingSkill = entryDetailNode.ReadValueByte(); break;
                    case "ai_roughness":
                        AIRoughness = entryDetailNode.ReadValueByte(); break;
                    case "special_ai_type":
                        SpecialAIType = entryDetailNode.ReadValueByte(); break;
                    case "display_driving_line":
                        DisplayDrivingLine = entryDetailNode.ReadValueBool(); break;
                }
            }
        }

        public void Deserialize(ref BitStream reader)
        {
            int basePos = reader.Position;
            Version = reader.ReadInt32();
            int cdp_size = reader.ReadInt32();

            Port = (byte)reader.ReadBits(4);
            DriverType = (DriverType)reader.ReadBits(4);
            ResidenceID = (byte)reader.ReadBits(6);
            IsGhost = reader.ReadBoolBit();
            DisplayDrivingLine = reader.ReadBoolBit();

            byte[] name = new byte[Version >= 110 ? 32 : 64];
            reader.ReadIntoByteArray(name.Length, name, BitStream.Byte_Bits);
            DisplayName = Encoding.ASCII.GetString(name).TrimEnd((char)0);
            if (Version >= 109)
            {
                byte[] onlineIdBuffer = new byte[18];
                reader.ReadIntoByteArray(18, onlineIdBuffer, BitStream.Byte_Bits);
            }

            byte[] region = new byte[4];
            reader.ReadIntoByteArray(4, region, BitStream.Byte_Bits);
            Region = Encoding.ASCII.GetString(region).TrimEnd((char)0);

            Settings.Deserialize(ref reader);

            CorneringSkill = reader.ReadByte();
            AcceleratingSkill = reader.ReadByte();
            reader.ReadBits(4); // ai_pit_decision_10_vitality_before_race
            reader.ReadBits(4); // ai_pit_decision_10_tire_before_race
            BrakingSkillFlags = reader.ReadByte();
            SpecialAIType = reader.ReadByte();
            StartingSkill = reader.ReadByte();
            reader.ReadBits(3); // ai_reaction_level
            reader.ReadBits(4); // unk6
            reader.ReadBits(1); // disable_bspec_skill
            AILevel = reader.ReadByte();

            GrowthParameter.Deserialize(ref reader);

            // TODO: What is this? Adhoc doesn't expose it
            if (Version < 113)
            {
                if (Version >= 110)
                {
                    // All use ReadBitsSafe, but for the purposes of making it more clean we arent
                    HeadCode = reader.ReadInt16(); // Head Code
                    BodyCode = reader.ReadInt16(); // Body Code
                    HeadColorCode = reader.ReadInt16(); // Head Color Code
                    BodyColorCode = reader.ReadInt16(); // Body Color Code
                    reader.ReadBits(2);
                    reader.ReadBits(1);
                    reader.ReadBits(5);
                    reader.ReadByte();
                    reader.ReadInt16();

                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();

                    // x2
                    reader.ReadInt16();
                    reader.ReadInt16();
                    reader.ReadInt16();
                    reader.ReadInt16();
                    reader.ReadBits(2); // pDVar2->field_0x88 & 0x3fffffff | iVar4 << 0x1e;
                    reader.ReadBits(1); // pDVar2->field_0x88 & 0xdfffffff | (uVar5 & 0x1) << 0x1d;
                    reader.ReadBits(5); // pDVar2->field_0x88 & 0xe0ffffff | (uVar5 & 0x1f) << 0x18;
                    reader.ReadByte();
                    reader.ReadInt16();

                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                }
                else if (Version <= 106)
                {
                    reader.ReadByte();
                    reader.ReadByte();

                    V106_Flag = (byte)reader.ReadBits(2);
                    if (V106_Flag != 0)
                        reader.ReadBits(2);
                    reader.ReadBits(2);
                    reader.ReadBits(1);

                    V106_Flag2 = (byte)reader.ReadBits(2);
                    if (V106_Flag2 != 0)
                        reader.ReadByte();

                    reader.ReadByte();

                    if (Version == 106)
                    {
                        reader.ReadBits(6);
                        reader.ReadBits(6);

                        reader.ReadBits(6);
                        reader.ReadBits(6);

                        reader.ReadBits(1);
                        reader.ReadBits(1);

                        reader.ReadBits(6);
                    }
                }
            }

            reader.Position = basePos + cdp_size;
        }

        public byte V106_Flag { get; set; }
        public byte V106_Flag2 { get; set; }

        public void Serialize(ref BitStream bs)
        {
            int baseOffset = bs.Position;

            bs.WriteInt32(Version);
            bs.WriteInt32(0xC0); // Size, game hardcodes it

            bs.WriteBits(Port, 4);
            bs.WriteBits((byte)DriverType, 4);
            bs.WriteBits(ResidenceID, 6);
            bs.WriteBoolBit(IsGhost);
            bs.WriteBoolBit(DisplayDrivingLine);

            byte[] name = new byte[Version >= 110 ? 32 : 64];
            Encoding.UTF8.GetBytes(DisplayName).AsSpan().CopyTo(name);
            bs.WriteByteData(name);

            if (Version >= 109)
            {
                byte[] onlineIdBuffer = new byte[18];
                Encoding.UTF8.GetBytes(OnlineID).AsSpan().CopyTo(onlineIdBuffer);
                bs.WriteByteData(onlineIdBuffer);
            }

            byte[] region = new byte[4];
            Encoding.UTF8.GetBytes(Region).AsSpan().CopyTo(region);
            bs.WriteByteData(region);

            Settings.Serialize(ref bs);

            bs.WriteByte(CorneringSkill);
            bs.WriteByte(AcceleratingSkill);
            bs.WriteBits(0, 4);
            bs.WriteBits(0, 4);
            bs.WriteByte(BrakingSkillFlags);
            bs.WriteByte(SpecialAIType);
            bs.WriteByte(StartingSkill);
            bs.WriteBits(0, 3);
            bs.WriteBits(0, 4);
            bs.WriteBits(0, 1);
            bs.WriteByte(AILevel);

            GrowthParameter.Serialize(ref bs);

            // TODO: What is this?
            if (Version < 113)
            {
                if (Version >= 110)
                {
                    bs.WriteInt16(HeadCode);
                    bs.WriteInt16(BodyCode);
                    bs.WriteInt16(HeadColorCode);
                    bs.WriteInt16(BodyColorCode);
                    bs.WriteBits(0, 2);
                    bs.WriteBits(0, 1);
                    bs.WriteBits(0, 5);
                    bs.WriteByte(0);
                    bs.WriteInt16(0);

                    bs.WriteInt32(0);
                    bs.WriteInt32(0);
                    bs.WriteInt32(0);
                    bs.WriteInt32(0);
                    bs.WriteInt32(0);

                    // x2
                    bs.WriteInt16(0);
                    bs.WriteInt16(0);
                    bs.WriteInt16(0);
                    bs.WriteInt16(0);
                    bs.WriteBits(0, 2);
                    bs.WriteBits(0, 1);
                    bs.WriteBits(0, 5);
                    bs.WriteByte(0);
                    bs.WriteInt16(0);

                    bs.WriteInt32(0);
                    bs.WriteInt32(0);
                    bs.WriteInt32(0);
                    bs.WriteInt32(0);
                    bs.WriteInt32(0);
                }
                else if (Version <= 106)
                {
                    bs.WriteByte(0);
                    bs.WriteByte(0);
                    bs.WriteBits(V106_Flag, 2);
                    if (V106_Flag != 0)
                        bs.WriteBits(0, 2);
                    bs.WriteBits(0, 2);
                    bs.WriteBits(0, 1);

                    bs.WriteBits(V106_Flag2, 2);
                    if (V106_Flag2 != 0)
                        bs.WriteByte(0);

                    bs.WriteByte(0);

                    if (Version == 106)
                    {
                        bs.WriteBits(0, 6);
                        bs.WriteBits(0, 6);

                        bs.WriteBits(0, 6);
                        bs.WriteBits(0, 6);

                        bs.WriteBits(0, 1);
                        bs.WriteBits(0, 1);

                        bs.WriteBits(0, 6);
                    }
                }
            }

            bs.Position = baseOffset + 0xC0;

        }
    }

    public class DriverSettings
    {
        /// <summary>
        /// Should match PDISTD::getGtbVersion
        /// GT6 1.22 is 131
        /// </summary>
        public byte GTBehavior_version;
        public bool Manual { get; set; }

        /// <summary>
        /// Defaults to 1
        /// </summary>
        public bool Assist_TCS { get; set; } = true;
        public bool Assist_ASM { get; set; }

        /// <summary>
        /// Defaults to 1
        /// </summary>
        public byte Steering_Assist_Type { get; set; } = 1;

        private byte unk;
        public bool Active_Steering { get; set; }
        public byte Active_Brake_Level;
        public bool Physics_Pro { get; set; }
        public byte Competition_Flags { get; set; } // 'academy_flag' in GT5, 'competition_flags' in GT6
        public byte Pad_Yaw_Gain { get; set; }

        /// <summary>
        /// Defaults to 1
        /// </summary>
        public byte Assist_4was { get; set; } = 1;
        private byte unk2;
        public byte RTAUnadjustable { get; set; }

        public void CopyTo(DriverSettings other)
        {
            other.GTBehavior_version = GTBehavior_version;
            other.Manual = Manual;
            other.Assist_TCS = Assist_TCS;
            other.Assist_ASM = Assist_ASM;
            other.Steering_Assist_Type = Steering_Assist_Type;
            other.unk = unk;
            other.Active_Steering = Active_Steering;
            other.Active_Brake_Level = Active_Brake_Level;
            other.Physics_Pro = Physics_Pro;
            other.Competition_Flags = Competition_Flags;
            other.Pad_Yaw_Gain = Pad_Yaw_Gain;
            other.Assist_4was = Assist_4was;
            other.unk2 = unk2;
            other.RTAUnadjustable = RTAUnadjustable;
        }

        public void Deserialize(ref BitStream reader)
        {
            GTBehavior_version = reader.ReadByte();
            Manual = reader.ReadByte() == 1;
            Assist_TCS = reader.ReadByte() == 1;
            Assist_ASM = reader.ReadByte() == 1;
            Steering_Assist_Type = reader.ReadByte();
            unk = reader.ReadByte();
            Active_Steering = reader.ReadByte() == 1;
            Active_Brake_Level = reader.ReadByte();
            Physics_Pro = reader.ReadByte() == 1;
            Competition_Flags = reader.ReadByte();
            Pad_Yaw_Gain = reader.ReadByte();
            Assist_4was = reader.ReadByte();
            unk2 = reader.ReadByte();
            RTAUnadjustable = reader.ReadByte();
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteByte(GTBehavior_version);
            bs.WriteByte(Manual ? (byte)1 : (byte)0);
            bs.WriteByte(Assist_TCS ? (byte)1 : (byte)0);
            bs.WriteByte(Assist_ASM ? (byte)1 : (byte)0);
            bs.WriteByte(Steering_Assist_Type);
            bs.WriteByte(unk);
            bs.WriteByte(Active_Steering ? (byte)1 : (byte)0);
            bs.WriteByte(Active_Brake_Level);
            bs.WriteByte(Physics_Pro ? (byte)1 : (byte)0);
            bs.WriteByte(Competition_Flags);
            bs.WriteByte(Pad_Yaw_Gain);
            bs.WriteByte(Assist_4was);
            bs.WriteByte(unk2);
            bs.WriteByte(RTAUnadjustable);
        }
    }

    public class GrowthParameter
    {
        public byte Growable { get; set; }
        public byte SpecID { get; set; }
        public byte Fatigue { get; set; }
        public byte Level { get; set; }
        public int Experience { get; set; }
        public short WinCount { get; set; }
        public short RaceCount { get; set; }
        public short Days { get; set; }
        public byte DecMasterRate { get; set; }
        public short Stamina { get; set; }
        public byte Mentality { get; set; }
        public byte Condition { get; set; }
        public byte Temper { get; set; }
        public byte Flexibility { get; set; }
        public byte SkillBaseBraking { get; set; }
        public byte SkillBaseCornering { get; set; }
        public byte SkillBaseShiftUp { get; set; }
        public byte SkillBaseCornerout { get; set; }
        public byte SkillBaseLineTrace { get; set; }
        public byte SkillBaseSteerReaction { get; set; }
        public byte SkillHeatOffset { get; set; }
        public byte Unk { get; set; }
        public short Unk2 { get; set; }
        public byte BonusSkillB { get; set; }
        public byte BonusSkillC { get; set; }
        public byte BonusSkillS { get; set; }
        public byte BonusSkillO { get; set; }

        public void CopyTo(GrowthParameter other)
        {
            other.Growable = Growable;
            other.SpecID = SpecID;
            other.Fatigue = Fatigue;
            other.Level = Level;
            other.Experience = Experience;
            other.WinCount = WinCount;
            other.RaceCount = RaceCount;
            other.Days = Days;
            other.DecMasterRate = DecMasterRate;
            other.Stamina = Stamina;
            other.Mentality = Mentality;
            other.Condition = Condition;
            other.Temper = Temper;
            other.Flexibility = Flexibility;
            other.SkillBaseBraking = SkillBaseBraking;
            other.SkillBaseCornering = SkillBaseCornering;
            other.SkillBaseShiftUp = SkillBaseShiftUp;
            other.SkillBaseCornerout = SkillBaseCornerout;
            other.SkillBaseLineTrace = SkillBaseLineTrace;
            other.SkillBaseSteerReaction = SkillBaseSteerReaction;
            other.SkillHeatOffset = SkillHeatOffset;
            other.Unk = Unk;
            other.Unk2 = Unk2;
            other.BonusSkillB = BonusSkillB;
            other.BonusSkillC = BonusSkillC;
            other.BonusSkillS = BonusSkillS;
            other.BonusSkillO = BonusSkillO;
        }

        public void Deserialize(ref BitStream reader)
        {
            Growable = reader.ReadByte();
            SpecID = reader.ReadByte();
            Fatigue = reader.ReadByte();
            Level = reader.ReadByte();
            Experience = reader.ReadInt32();
            WinCount = reader.ReadInt16();
            RaceCount = reader.ReadInt16();
            Days = reader.ReadInt16();
            DecMasterRate = reader.ReadByte();
            Stamina = reader.ReadInt16();
            Mentality = reader.ReadByte();
            Condition = reader.ReadByte();
            Temper = reader.ReadByte();
            Flexibility = reader.ReadByte();
            SkillBaseBraking = reader.ReadByte();
            SkillBaseCornering = reader.ReadByte();
            SkillBaseShiftUp = reader.ReadByte();
            SkillBaseCornerout = reader.ReadByte();
            SkillBaseLineTrace = reader.ReadByte();
            SkillBaseSteerReaction = reader.ReadByte();
            SkillHeatOffset = reader.ReadByte();
            Unk = reader.ReadByte();
            Unk2 = reader.ReadInt16();
            BonusSkillB = reader.ReadByte();
            BonusSkillC = reader.ReadByte();
            BonusSkillS = reader.ReadByte();
            BonusSkillO = reader.ReadByte();
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteByte(Growable);
            bs.WriteByte(SpecID);
            bs.WriteByte(Fatigue);
            bs.WriteByte(Level);
            bs.WriteInt32(Experience);
            bs.WriteInt16(WinCount);
            bs.WriteInt16(RaceCount);
            bs.WriteInt16(Days);
            bs.WriteByte(DecMasterRate);
            bs.WriteInt16(Stamina);
            bs.WriteByte(Mentality);
            bs.WriteByte(Condition);
            bs.WriteByte(Temper);
            bs.WriteByte(Flexibility);
            bs.WriteByte(SkillBaseBraking);
            bs.WriteByte(SkillBaseCornering);
            bs.WriteByte(SkillBaseShiftUp);
            bs.WriteByte(SkillBaseCornerout);
            bs.WriteByte(SkillBaseLineTrace);
            bs.WriteByte(SkillBaseSteerReaction);
            bs.WriteByte(SkillHeatOffset);
            bs.WriteByte(Unk);
            bs.WriteInt16(Unk2);
            bs.WriteByte(BonusSkillB);
            bs.WriteByte(BonusSkillC);
            bs.WriteByte(BonusSkillS);
            bs.WriteByte(BonusSkillO);
        }
    }
}
