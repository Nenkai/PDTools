using System;
using System.Collections.Generic;
using System.Text;
using PDTools.Enums;

using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class Entry
    {
        public MCarThin CarThin { get; set; }
        public byte[] CarParameterBlob { get; set; }
        public MCarParameter CarParameter { get; set; }

        /// <summary>
        /// Name of the driver.
        /// </summary>
        public string DriverName { get; set; }

        /// <summary>
        /// Region of the driver.
        /// </summary>
        public string DriverRegion { get; set; }

        public MCarDriverParameter[] DriverParameter { get; set; } = new MCarDriverParameter[4];

        public byte PilotID { get; set; } = 0;

        public EntryBase EntryBase { get; set; } = new EntryBase();

        /// <summary>
        /// Whether initial position parameters should be used, <see cref="InitialPosition"/> and <see cref="InitialVelocity"/>.
        /// </summary>
        public bool AvailableInitialPosition { get; set; }

        public int InitialPosition { get; set; } = 0;

        public int InitialVelocity { get; set; } = 0;

        public StartType StartType { get; set; } = StartType.NONE;

        public int Delay { get; set; } = 0;

        public byte RaceClassID { get; set; } = 0;

        public sbyte ProxyDriverModel { get; set; } = -1;

        public byte NoSuitableTire { get; set; } = 0;

        public short InitialFuel100 { get; set; } = -1;

        public List<int> BoostRaceRatio { get; set; } = new List<int>();
        public List<int> BoostRatio { get; set; } = new List<int>();

        /// <summary>
        /// AI Braking Skill. Defaults to -1.
        /// </summary>
        public short AISkillBraking { get; set; } = -1;

        /// <summary>
        /// AI Cornering Skill. Defaults to -1.
        /// </summary>
        public short AISkillCornering { get; set; } = -1;

        /// <summary>
        /// AI Accelerating Skill. Defaults to -1.
        /// </summary>
        public sbyte AISkillAccelerating { get; set; } = -1;

        /// <summary>
        /// AI Starting Skill. Defaults to -1.
        /// </summary>
        public sbyte AISkillStarting { get; set; } = -1;

        public sbyte AIRoughness { get; set; } = -1;

        public void ReadEntryFromCache(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5_E5_25_EE && magic != 0xE6_E6_25_EE)
                throw new System.IO.InvalidDataException($"Course magic did not match - Got {magic.ToString("X8")}, expected 0xE6E625EE");
            int version = reader.ReadInt32();

            int player_no = reader.ReadInt32();

            MCarThin car = new MCarThin(0);
            car.Read(ref reader);

            CarParameter = MCarParameter.ImportFromBlob(ref reader);
            DriverName = reader.ReadString4Aligned();
            DriverRegion = reader.ReadString4Aligned();

            for (int i = 0; i < 4; i++)
                DriverParameter[i].Deserialize(ref reader);

            if (version >= 1_08)
                EntryBase.Deserialize(ref reader);

            AvailableInitialPosition = reader.ReadBool(); // Available initial position
            RaceClassID = reader.ReadByte();
            ProxyDriverModel = reader.ReadSByte(); // Proxy driver model
            PilotID = reader.ReadByte(); // Pilot ID
            InitialPosition = reader.ReadInt32();
            InitialVelocity = reader.ReadInt32();
            StartType = (StartType)reader.ReadInt32();
            Delay = reader.ReadInt32();
            NoSuitableTire = reader.ReadByte();
            InitialFuel100 = reader.ReadInt16();

            int boost_rate_count = reader.ReadInt32();
            reader.ReadIntoByteArray(boost_rate_count, new byte[boost_rate_count], 8);
            reader.ReadIntoByteArray(boost_rate_count, new byte[boost_rate_count], 8);

            AISkillBraking = reader.ReadInt16();
            AISkillCornering = reader.ReadInt16();
            AISkillAccelerating = reader.ReadSByte();
            StartType = (StartType)reader.ReadSByte();
            AIRoughness = reader.ReadSByte();

            if (version >= 1_07)
                reader.ReadByte(); // Unk
        }


        public void WriteEntryToBuffer(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_25_EE);
            bs.WriteUInt32(1_08);

            bs.WriteInt32(0); // player_no

            // Write car (carthin)
            CarThin.Serialize(ref bs);
            CarParameter.Serialize(ref bs);

            bs.WriteNullStringAligned4(DriverName);
            bs.WriteNullStringAligned4(DriverRegion);

            for (int i = 0; i < 4; i++)
                DriverParameter[i].Serialize(ref bs);

            EntryBase.Serialize(ref bs);

            bs.WriteBool(AvailableInitialPosition);
            bs.WriteByte(RaceClassID);
            bs.WriteSByte(ProxyDriverModel);
            bs.WriteByte(PilotID);
            bs.WriteInt32(InitialPosition);
            bs.WriteInt32(InitialVelocity);
            bs.WriteInt32((int)StartType);
            bs.WriteInt32(Delay);
            bs.WriteByte(NoSuitableTire);
            bs.WriteInt16(-1); // initial_fuel100

            bs.WriteInt32(0); // boost_rate_count - ignore

            bs.WriteInt16(AISkillBraking);
            bs.WriteInt16(AISkillCornering);
            bs.WriteSByte(AISkillAccelerating);
            bs.WriteSByte(AISkillStarting);
            bs.WriteSByte(AIRoughness);
            bs.WriteByte(0); // unk  
        }
    }
}
