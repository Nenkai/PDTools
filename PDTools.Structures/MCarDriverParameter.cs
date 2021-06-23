using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;
using PDTools.Utils;

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

        public DriverSettings Settings { get; set; } = new DriverSettings();

        public byte CorneringSkill { get; set; }
        public byte AcceleratingSkill { get; set; }
        public byte BrakingSkill { get; set; }
        public byte SpecialDriverType { get; set; }
        public byte StartingSkill { get; set; }
        public byte AILevel { get; set; }

        public GrowthParameter GrowthParameter { get; set; } = new GrowthParameter();

        public static MCarDriverParameter Read(ref SpanReader buffer)
        {
            var driver = new MCarDriverParameter();
            BitStream reader = new BitStream(/*buffer.Span.Slice(buffer.Position)*/ Span<byte>.Empty); // FIX ME
            reader.BufferByteSize = 0xC0;

            driver.Version = reader.ReadInt32();
            driver.unk2 = reader.ReadUInt32();
            reader.ReadBits(4); // *puVar13 & 0xfffffff | iVar5 << 0x1c;
            reader.ReadBits(4); // *puVar13 & 0xf0ffffff | (uVar6 & 0xf) << 0x18;
            reader.ReadBits(6); // *puVar13 & 0xff03ffff | (uVar6 & 0x3f) << 0x12;
            reader.ReadBits(1); // *puVar13 & 0xfffdffff | (uVar6 & 0x1) << 0x11;
            reader.ReadBits(1); // *puVar13 & 0xfffeffff | (uVar6 & 0x1) << 0x10;
            byte[] name = new byte[driver.Version >= 110 ? 32 : 64];
            reader.ReadIntoByteArray(name.Length, name, BitStream.Byte_Bits);
            driver.PlayerName = Encoding.ASCII.GetString(name).TrimEnd((char)0);
            if (driver.Version >= 109)
            {
                byte[] onlineIdBuffer = new byte[18];
                reader.ReadIntoByteArray(18, onlineIdBuffer, BitStream.Byte_Bits);
            }

            byte[] region = new byte[4];
            reader.ReadIntoByteArray(4, region, BitStream.Byte_Bits);
            driver.Region = Encoding.ASCII.GetString(region).TrimEnd((char)0);

            driver.Settings.Read(ref reader);
            driver.CorneringSkill = reader.ReadByte();
            driver.AcceleratingSkill = reader.ReadByte();
            reader.ReadBits(4); // pDVar2->field_0x53 & 0xf | (byte)(iVar5 << 0x4);
            reader.ReadBits(4); // pDVar2->field_0x53 & 0xf0 | bVar11 & 0xf;
            driver.BrakingSkill = reader.ReadByte();
            driver.SpecialDriverType = reader.ReadByte();
            driver.StartingSkill = reader.ReadByte();
            reader.ReadBits(3); // pDVar2->field_0x57 & 0x1f | (byte)(iVar5 << 0x5);
            reader.ReadBits(4); // pDVar2->field_0x57 & 0xe0 | (byte)(((pDVar2->field_0x57 & 0x1) << 0x1b) >> 0x1b) | (byte)(iVar5 << 0x1) & 0x1e;
            reader.ReadBits(1); // pDVar2->field_0x57 & 0xfe | bVar11 & 0x1;
            driver.AILevel = reader.ReadByte();

            driver.GrowthParameter.Read(ref reader);

            if (driver.Version < 113)
            {
                if (driver.Version >= 110)
                {
                    // All use ReadBitsSafe, but for the purposes of making it more clean we arent
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
                else if (driver.Version <= 106)
                {
                    reader.ReadByte();
                    reader.ReadByte();
                    if (reader.ReadBits(2) != 0)
                        reader.ReadBits(2);
                    reader.ReadBits(2);
                    reader.ReadBits(1);

                    if (reader.ReadBits(2) != 0) 
                        reader.ReadByte();

                    reader.ReadByte();

                    if (driver.Version == 106)
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
            return driver;
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
        public byte Competition_Flags { get; set; }
        public byte Pad_Yaw_Gain { get; set; }
        public byte Assist_4was { get; set; }
        private byte unk2;
        public byte RTAUnadjustable { get; set; }

        public void Read(ref BitStream reader)
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

        public void Read(ref BitStream reader)
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
    }
}
