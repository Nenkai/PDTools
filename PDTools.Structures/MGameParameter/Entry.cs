using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using PDTools.Utils;
using PDTools.Enums;
using System.Runtime.ConstrainedExecution;
using PDTools.Structures.PS3;

namespace PDTools.Structures.MGameParameter
{
    public class Entry
    {
        public MCarThin Car { get; set; } = new MCarThin();
        public MCarParameter CarParameter { get; set; }

        public int PlayerNumber { get; set; }

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

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public short InitialFuel100 { get; set; } = -1;

        public List<sbyte> BoostRaceRatio { get; set; } = new List<sbyte>();
        public List<byte> BoostRatio { get; set; } = new List<byte>();

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

        public Entry()
        {
            for (var i = 0; i < 4; i++)
                DriverParameter[i] = new MCarDriverParameter();
        }

        public void CopyTo(Entry other)
        {
            Car.CopyTo(other.Car);
            //CarParameter.CopyTo(other.CarParameter);
            other.PlayerNumber = PlayerNumber;
            other.DriverName = DriverName;
            other.DriverRegion = DriverRegion;

            for (int i = 0; i < 4; i++)
            {
                var driverParameter = new MCarDriverParameter();
                DriverParameter[i].CopyTo(driverParameter);
                other.DriverParameter[i] = driverParameter;
            }

            other.PilotID = PilotID;
            EntryBase.CopyTo(other.EntryBase);
            other.AvailableInitialPosition = AvailableInitialPosition;
            other.InitialPosition = InitialPosition;
            other.InitialVelocity = InitialVelocity;
            other.StartType = StartType;
            other.Delay = Delay;
            other.RaceClassID = RaceClassID;
            other.ProxyDriverModel = ProxyDriverModel;
            other.NoSuitableTire = NoSuitableTire;
            other.InitialFuel100 = InitialFuel100;

            for (int i = 0; i < BoostRaceRatio.Count; i++)
                other.BoostRaceRatio.Add(BoostRaceRatio[i]);

            for (int i = 0; i < BoostRatio.Count; i++)
                other.BoostRatio.Add(BoostRatio[i]);

            other.AISkillBraking = AISkillBraking;
            other.AISkillCornering = AISkillCornering;
            other.AISkillAccelerating = AISkillAccelerating;
            other.AISkillStarting = AISkillStarting;
            other.AIRoughness = AIRoughness;
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("entry");
            {
                xml.WriteElementInt("player_no", PlayerNumber);

                if (!string.IsNullOrEmpty(Car.CarLabel))
                {
                    xml.WriteStartElement("car");
                    {
                        xml.WriteAttributeString("label", Car.CarLabel);
                        xml.WriteAttributeString("color", Car.Paint.ToString());
                    }
                    xml.WriteEndElement();
                }

                // TODO: car_parameter

                if (!string.IsNullOrEmpty(DriverName))
                    xml.WriteElementValue("driver_name", DriverName);

                if (!string.IsNullOrEmpty(DriverRegion))
                    xml.WriteElementValue("driver_region", DriverRegion);

                if (!DriverParameter[0].IsVacant())
                {
                    xml.WriteStartElement("driver_parameter");
                    DriverParameter[0].WriteToXml(xml);
                    xml.WriteEndElement();
                }

                xml.WriteElementIntIfNotDefault("pilot_id", PilotID, defaultValue: 0);
                if (!string.IsNullOrEmpty(EntryBase.Car.CarLabel))
                    EntryBase.WriteToXml(xml);

                xml.WriteElementIntIfNotDefault("initial_position", InitialPosition, defaultValue: 0);
                xml.WriteElementIntIfNotDefault("initial_velocity", InitialVelocity, defaultValue: 0);
                xml.WriteElementValue("start_type", StartType.ToString());
                xml.WriteElementIntIfNotDefault("delay", Delay, defaultValue: 0);
                xml.WriteElementIntIfNotDefault("race_class_id", RaceClassID, defaultValue: 0);
                xml.WriteElementIntIfNotDefault("proxy_driver_model", ProxyDriverModel);
                xml.WriteElementIntIfNotDefault("no_suitable_tire", NoSuitableTire, defaultValue: 0);
                xml.WriteElementIntIfNotDefault("initial_fuel100", InitialFuel100);

                if (BoostRaceRatio.Count > 0)
                {
                    xml.WriteStartElement("boost_race_ratio");
                    foreach (var ratio in BoostRaceRatio)
                        xml.WriteElementInt("ratio", ratio);
                    xml.WriteEndElement();
                }

                if (BoostRatio.Count > 0)
                {
                    xml.WriteStartElement("boost_ratio");
                    foreach (var ratio in BoostRatio)
                        xml.WriteElementInt("ratio", ratio);
                    xml.WriteEndElement();
                }

                xml.WriteElementIntIfNotDefault("ai_skill_breaking", AISkillBraking);
                xml.WriteElementIntIfNotDefault("ai_skill_cornering", AISkillCornering);
                xml.WriteElementIntIfNotDefault("ai_skill_accelerating", AISkillAccelerating);
                xml.WriteElementIntIfNotDefault("ai_skill_starting", AISkillStarting);
                xml.WriteElementIntIfNotDefault("ai_roughness", AIRoughness);
            }
            xml.WriteEndElement();
        }

        public void ReadFromXml(XmlNode entryNode)
        {
            foreach (XmlNode entryDetailNode in entryNode)
            {
                switch (entryDetailNode.Name)
                {
                    case "player_no":
                        PlayerNumber = entryDetailNode.ReadValueInt(); break;
                    case "car":
                        Car.CarLabel = entryDetailNode.Attributes["label"].Value;
                        Car.Paint = short.Parse(entryDetailNode.Attributes["color"].Value);
                        break;
                    case "car_parameter":
                        throw new NotImplementedException("Implement car_parameter parsing!");
                    case "driver_name":
                        DriverName = entryDetailNode.ReadValueString(); break;
                    case "driver_region":
                        DriverRegion = entryDetailNode.ReadValueString(); break;
                    case "driver_parameter":
                        DriverParameter[0].ParseFromXml(entryDetailNode); break;
                    case "pilot_id":
                        PilotID = entryDetailNode.ReadValueByte(); break;
                    case "entry_base":
                        EntryBase.ReadFromXml(entryDetailNode); break;
                    case "initial_position":
                        InitialPosition = entryDetailNode.ReadValueInt(); break;
                    case "initial_velocity":
                        InitialVelocity = entryDetailNode.ReadValueInt(); break;
                    case "start_type":
                        StartType = entryDetailNode.ReadValueEnum<StartType>(); break;
                    case "delay":
                        Delay = entryDetailNode.ReadValueInt(); break;
                    case "race_class_id":
                        RaceClassID = entryDetailNode.ReadValueByte(); break;
                    case "proxy_driver_model":
                        ProxyDriverModel = entryDetailNode.ReadValueSByte(); break;
                    case "no_suitable_tire":
                        NoSuitableTire = entryDetailNode.ReadValueByte(); break;
                    case "initial_fuel100":
                        InitialFuel100 = entryDetailNode.ReadValueByte(); break;

                    case "boost_race_ratio":
                        foreach (XmlNode race_ratio_node in entryDetailNode.SelectNodes("race_ratio"))
                            BoostRaceRatio.Add(race_ratio_node.ReadValueSByte());
                        break;
                    case "boost_ratio":
                        foreach (XmlNode ratio_node in entryDetailNode.SelectNodes("ratio"))
                            BoostRatio.Add(ratio_node.ReadValueByte());
                        break;

                    case "ai_skill_breaking":
                        AISkillBraking = entryDetailNode.ReadValueShort(); break;
                    case "ai_skill_cornering":
                        AISkillCornering = entryDetailNode.ReadValueShort(); break;
                    case "ai_skill_accelerating":
                        AISkillAccelerating = entryDetailNode.ReadValueSByte(); break;
                    case "ai_skill_starting":
                        AISkillStarting = entryDetailNode.ReadValueSByte(); break;
                    case "ai_roughness":
                        AIRoughness = entryDetailNode.ReadValueSByte(); break;
                }
            }
        }

        public void Deserialize(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5_E5_25_EE && magic != 0xE6_E6_25_EE)
                throw new System.IO.InvalidDataException($"Course magic did not match - Got {magic.ToString("X8")}, expected 0xE6E625EE");
            int version = reader.ReadInt32();

            int player_no = reader.ReadInt32();

            Car.Read(ref reader);

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


        public void Serialize(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_25_EE);
            bs.WriteUInt32(1_08);

            bs.WriteInt32(PlayerNumber); // player_no

            // Write car (carthin)
            Car.Serialize(ref bs);
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
            bs.WriteInt16(InitialFuel100); // initial_fuel100

            bs.WriteInt32(BoostRatio.Count);
            for (var i = 0; i < BoostRatio.Count; i++)
            {
                bs.WriteByte(BoostRatio[i]);
                if (i < BoostRaceRatio.Count)
                    bs.WriteSByte(BoostRaceRatio[i]);
                else
                    bs.WriteSByte(0);
            }

            bs.WriteInt16(AISkillBraking);
            bs.WriteInt16(AISkillCornering);
            bs.WriteSByte(AISkillAccelerating);
            bs.WriteSByte(AISkillStarting);
            bs.WriteSByte(AIRoughness);
            bs.WriteByte(0); // unk  
        }
    }
}
