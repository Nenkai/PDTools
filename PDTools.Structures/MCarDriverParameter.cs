using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;
using PDTools.Utils;
using PDTools.Enums;

namespace PDTools.Structures
{
    public class MCarDriverParameter
    {
        public int Version { get; set; }
        public uint unk2;
        public short unk3;
        public string PlayerName { get; set; }
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
        public byte BrakingSkill { get; set; }
        public byte SpecialDriverType { get; set; }
        public byte StartingSkill { get; set; }
        public byte AILevel { get; set; }

        public GrowthParameter GrowthParameter { get; set; } = new GrowthParameter();

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
            PlayerName = Encoding.ASCII.GetString(name).TrimEnd((char)0);
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
            BrakingSkill = reader.ReadByte();
            SpecialDriverType = reader.ReadByte();
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
                    reader.ReadInt16(); // Head Code
                    reader.ReadInt16(); // Body Code
                    reader.ReadInt16(); // Head Color Code
                    reader.ReadInt16(); // Body Color Code
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
            Encoding.UTF8.GetBytes(PlayerName).AsSpan().CopyTo(name);
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
            bs.WriteByte(BrakingSkill);
            bs.WriteByte(SpecialDriverType);
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
        public byte GTBehavior_version;
        public bool Manual { get; set; }
        public bool Assist_TCS { get; set; }
        public bool Assist_ASM { get; set; }
        public byte Steering_Assist_Type { get; set; }
        private byte unk;
        public bool Active_Steering { get; set; }
        public byte Active_Brake_Level;
        public bool Physics_Pro { get; set; }
        public byte Competition_Flags { get; set; } // 'academy_flag' in GT5, 'competition_flags' in GT6
        public byte Pad_Yaw_Gain { get; set; }
        public byte Assist_4was { get; set; }
        private byte unk2;
        public byte RTAUnadjustable { get; set; }

        public void Deserialize(ref BitStream reader)
        {
            GTBehavior_version = reader.ReadByte();
            Manual = reader.ReadByte() == 1;
            Assist_TCS = reader.ReadByte() == 1;
            Assist_ASM = reader.ReadByte() == 1;
            Steering_Assist_Type = reader.ReadByte();
            reader.ReadByte();
            Active_Steering = reader.ReadByte() == 1;
            Active_Brake_Level = reader.ReadByte();
            Physics_Pro = reader.ReadByte() == 1;
            Competition_Flags = reader.ReadByte();
            Pad_Yaw_Gain = reader.ReadByte();
            Assist_4was = reader.ReadByte();
            reader.ReadByte();
            RTAUnadjustable = reader.ReadByte();
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteByte(GTBehavior_version);
            bs.WriteByte(Manual ? (byte)1 : (byte)0);
            bs.WriteByte(Assist_TCS ? (byte)1 : (byte)0);
            bs.WriteByte(Assist_ASM ? (byte)1 : (byte)0);
            bs.WriteByte(Steering_Assist_Type);
            bs.WriteByte(0);
            bs.WriteByte(Active_Steering ? (byte)1 : (byte)0);
            bs.WriteByte(Active_Brake_Level);
            bs.WriteByte(Physics_Pro ? (byte)1 : (byte)0);
            bs.WriteByte(Competition_Flags);
            bs.WriteByte(Pad_Yaw_Gain);
            bs.WriteByte(Assist_4was);
            bs.WriteByte(0);
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
