using System;
using System.Xml;
using System.ComponentModel;
using System.Collections.Generic;

using PDTools.Utils;
using PDTools.Enums;
using PDTools.Enums.PS3;

namespace PDTools.Structures.MGameParameter
{
    /// <summary>
    /// GT6 Only. Represents an entry with more options (compared to the regular entry, used in GT5).
    /// </summary>
    public class EntryBase
    {
        /// <summary>
        /// Car for this entry.
        /// </summary>
        public MCarThin Car { get; set; } = new MCarThin();

        /// <summary>
        /// Name of the driver.
        /// </summary>
        public string DriverName { get; set; }

        /// <summary>
        /// Region of the driver.
        /// </summary>
        public string DriverRegion { get; set; }

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        public byte RaceClassID { get; set; } = 0;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public sbyte ProxyDriverModel { get; set; } = -1;

        public List<sbyte> BoostRaceRatio { get; set; } = new List<sbyte>();
        public List<byte> BoostRatio { get; set; } = new List<byte>();

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public short AIBrakingSkill { get; set; } = -1;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public short AICorneringSkill { get; set; } = -1;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public sbyte AIAcceleratingSkill { get; set; } = -1;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public sbyte AIStartingSkill { get; set; } = -1;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public sbyte AIRoughness { get; set; } = -1;

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        public byte AIReaction { get; set; } = 0;

        /// <summary>
        /// Engine tuning stage. Defaults to <see cref="PARTS_NATUNE.NONE"/>.
        /// </summary>
        public PARTS_NATUNE EngineNaTuneStage { get; set; } = PARTS_NATUNE.NONE;

        /// <summary>
        /// Turbo tuning stage. Defaults to <see cref="PARTS_TURBINEKIT.NONE"/>.
        /// </summary>
        public PARTS_TURBINEKIT EngineTurboKit { get; set; } = PARTS_TURBINEKIT.NONE;

        /// <summary>
        /// Computer level. Defaults to <see cref="PARTS_COMPUTER.NONE"/>.
        /// </summary>
        public PARTS_COMPUTER EngineComputer { get; set; } = PARTS_COMPUTER.NONE;

        /// <summary>
        /// Exhaust level. Defaults to <see cref="PARTS_MUFFLER.UNSPECIFIED"/>.
        /// </summary>
        public PARTS_MUFFLER Muffler { get; set; } = PARTS_MUFFLER.UNSPECIFIED;

        /// <summary>
        /// Suspension level. Defaults to <see cref="PARTS_SUSPENSION.UNSPECIFIED"/>.
        /// </summary>
        public PARTS_SUSPENSION Suspension { get; set; } = PARTS_SUSPENSION.UNSPECIFIED;

        /// <summary>
        /// Suspension level. Defaults to <see cref="PARTS_GEAR.UNSPECIFIED"/>.
        /// </summary>
        public PARTS_GEAR Transmission { get; set; } = PARTS_GEAR.UNSPECIFIED;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public int GearMaxSpeed { get; set; } = -1;

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        public byte BallastWeight { get; set; }

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public sbyte BallastPosition { get; set; } = -1;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public int WheelID { get; set; } = -1;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public int WheelColor { get; set; } = -1;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public int WheelInchUp { get; set; } = -1;

        /// <summary>
        /// Defaults to <see cref="TireType.NONE_SPECIFIED"/> (car defaults).
        /// </summary>
        public TireType TireFront { get; set; } = TireType.NONE_SPECIFIED;

        /// <summary>
        /// Defaults to <see cref="TireType.NONE_SPECIFIED"/> (car defaults).
        /// </summary>
        public TireType TireRear { get; set; } = TireType.NONE_SPECIFIED;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public sbyte AeroWing { get; set; } = -1;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public sbyte Aero1_AeroKit { get; set; } = -1;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public sbyte Aero2_FlatFloor { get; set; } = -1;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public sbyte Aero3_AeroOther { get; set; } = -1;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public sbyte DownforceFront { get; set; } = -1;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public sbyte DownforceRear { get; set; } = -1;

        /// <summary>
        /// Car paint ID. Defaults to -1.
        /// </summary>
        public short PaintId { get; set; } = -1;

        /// <summary>
        /// Defaults to -1.
        /// </summary>
        public short DeckenNumber { get; set; } = -1;

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        public byte DeckenType { get; set; } = 0;

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        public byte DeckenCustomID { get; set; } = 0;

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        public byte DeckenCustomType { get; set; } = 0;

        // Following properties are not exposed in XMLs

        /// <summary>
        /// Not exposed in XMLs. Defaults to -1.
        /// </summary>
        public sbyte PowerLimiter { get; set; } = -1;

        /// <summary>
        /// Not exposed in XMLs. Defaults to -1.
        /// </summary>
        public short BodyCode { get; set; } = -1;

        /// <summary>
        /// Not exposed in XMLs. Defaults to -1.
        /// </summary>
        public short HeadCode { get; set; } = -1;

        /// <summary>
        /// Not exposed in XMLs. Defaults to -1.
        /// </summary>
        public short BodyColorCode { get; set; } = -1;

        /// <summary>
        /// Not exposed in XMLs. Defaults to -1.
        /// </summary>
        public short HeadColorCode { get; set; } = -1;

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("entry_base");
            {
                xml.WriteStartElement("car");
                {
                    xml.WriteAttributeString("label", Car.CarLabel);
                    xml.WriteAttributeString("color", Car.Paint.ToString());
                }
                xml.WriteEndElement();

                xml.WriteElementValue("driver_name", DriverName);
                xml.WriteElementValue("driver_region", DriverName);
                xml.WriteElementInt("race_class_id", RaceClassID);
                xml.WriteElementInt("proxy_driver_model", ProxyDriverModel);

                xml.WriteStartElement("boost_race_ratio");
                foreach (var ratio in BoostRaceRatio)
                    xml.WriteElementInt("ratio", ratio);
                xml.WriteEndElement();

                xml.WriteStartElement("boost_ratio");
                foreach (var ratio in BoostRatio)
                    xml.WriteElementInt("ratio", ratio);
                xml.WriteEndElement();

                xml.WriteElementInt("ai_skill_breaking", AIBrakingSkill);
                xml.WriteElementInt("ai_skill_cornering", AICorneringSkill);
                xml.WriteElementInt("ai_skill_accelerating", AIAcceleratingSkill);
                xml.WriteElementInt("ai_skill_starting", AIStartingSkill);
                xml.WriteElementInt("ai_roughness", AIRoughness);
                xml.WriteElementInt("ai_reaction", AIReaction);

                xml.WriteElementValue("engine_na_tune_stage", EngineNaTuneStage.ToString());
                xml.WriteElementValue("engine_turbo_kit", EngineTurboKit.ToString());
                xml.WriteElementValue("engine_computer", EngineComputer.ToString());
                xml.WriteElementValue("muffler", Muffler.ToString());
                xml.WriteElementValue("suspension", Suspension.ToString());
                xml.WriteElementValue("transmission", Transmission.ToString());
                xml.WriteElementInt("gear_max_speed", GearMaxSpeed);
                xml.WriteElementInt("ballast_weight", BallastWeight);
                xml.WriteElementInt("ballast_position", BallastPosition);
                xml.WriteElementInt("wheel", WheelID);
                xml.WriteElementInt("wheel_color", WheelColor);
                xml.WriteElementInt("wheel_inch_up", WheelInchUp);
                xml.WriteElementValue("tire_f", TireFront.ToString());
                xml.WriteElementValue("tire_r", TireRear.ToString());
                xml.WriteElementInt("aero_wing", AeroWing);
                xml.WriteElementInt("aero_1", Aero1_AeroKit);
                xml.WriteElementInt("aero_2", Aero2_FlatFloor);
                xml.WriteElementInt("aero_3", Aero3_AeroOther);
                xml.WriteElementInt("downforce_f", DownforceFront);
                xml.WriteElementInt("downforce_r", DownforceRear);
                xml.WriteElementInt("paint_id", PaintId);
                xml.WriteElementInt("decken_number", DeckenNumber);
                xml.WriteElementInt("decken_type", DeckenType);
                xml.WriteElementInt("decken_custom_id", DeckenCustomID);
                xml.WriteElementInt("decken_custom_type", DeckenCustomType);
            }
        }

        public void ReadFromXml(XmlNode entryNode)
        {
            foreach (XmlNode entryDetailNode in entryNode)
            {
                switch (entryDetailNode.Name)
                {
                    case "car":
                        Car.CarLabel = entryDetailNode.Attributes["label"].Value;
                        Car.Paint = short.Parse(entryDetailNode.Attributes["color"].Value);
                        break;

                    case "driver_name":
                        DriverName = entryDetailNode.ReadValueString(); break;
                    case "driver_region":
                        DriverRegion = entryDetailNode.ReadValueString(); break;
                    case "race_class_id":
                        RaceClassID = entryDetailNode.ReadValueByte(); break;
                    case "proxy_driver_model":
                        ProxyDriverModel = entryDetailNode.ReadValueSByte(); break;
                    case "boost_race_ratio":
                        foreach (XmlNode race_ratio_node in entryDetailNode.SelectNodes("race_ratio"))
                            BoostRaceRatio.Add(race_ratio_node.ReadValueSByte());
                        break;
                    case "boost_ratio":
                        foreach (XmlNode ratio_node in entryDetailNode.SelectNodes("ratio"))
                            BoostRatio.Add(ratio_node.ReadValueByte());
                        break;
                    case "ai_skill_breaking":

                        AIBrakingSkill = entryDetailNode.ReadValueShort(); break;
                    case "ai_skill_cornering":
                        AICorneringSkill = entryDetailNode.ReadValueShort(); break;
                    case "ai_skill_accelerating":
                        AIAcceleratingSkill = entryDetailNode.ReadValueSByte(); break;
                    case "ai_skill_starting":
                        AIStartingSkill = entryDetailNode.ReadValueSByte(); break;
                    case "ai_roughness":
                        AIRoughness = entryDetailNode.ReadValueSByte(); break;

                    case "engine_na_tune_stage":
                        EngineNaTuneStage = entryDetailNode.ReadValueEnum<PARTS_NATUNE>(); break;
                    case "engine_turbo_kit":
                        EngineTurboKit = entryDetailNode.ReadValueEnum<PARTS_TURBINEKIT>(); break;
                    case "engine_computer":
                        EngineComputer = entryDetailNode.ReadValueEnum<PARTS_COMPUTER>(); break;
                    case "muffler":
                        Muffler = entryDetailNode.ReadValueEnum<PARTS_MUFFLER>(); break;
                    case "suspension":
                        Suspension = entryDetailNode.ReadValueEnum<PARTS_SUSPENSION>(); break;
                    case "transmission":
                        Transmission = entryDetailNode.ReadValueEnum<PARTS_GEAR>(); break;

                    case "gear_max_speed":
                        GearMaxSpeed = entryDetailNode.ReadValueInt(); break;
                    case "ballast_weight":
                        BallastWeight = entryDetailNode.ReadValueByte(); break;
                    case "ballast_position":
                        BallastPosition = entryDetailNode.ReadValueSByte(); break;
                    case "wheel":
                        WheelID = entryDetailNode.ReadValueInt(); break;
                    case "wheel_color":
                        WheelColor = entryDetailNode.ReadValueShort(); break;
                    case "wheel_inch_up":
                        WheelInchUp = entryDetailNode.ReadValueInt(); break;
                    case "tire_f":
                        TireFront = entryDetailNode.ReadValueEnum<TireType>(); break;
                    case "tire_r":
                        TireRear = entryDetailNode.ReadValueEnum<TireType>(); break;
                    case "aero_1":
                        Aero1_AeroKit = entryDetailNode.ReadValueSByte(); break;
                    case "aero_2":
                        Aero2_FlatFloor = entryDetailNode.ReadValueSByte(); break;
                    case "aero_3":
                        Aero3_AeroOther = entryDetailNode.ReadValueSByte(); break;
                    case "downforce_f":
                        DownforceFront = entryDetailNode.ReadValueSByte(); break;
                    case "downforce_r":
                        DownforceRear = entryDetailNode.ReadValueSByte(); break;
                    case "paint_id":
                        PaintId = entryDetailNode.ReadValueShort(); break;

                    case "decken_number":
                        DeckenNumber = entryDetailNode.ReadValueShort(); break;
                    case "decken_type":
                        DeckenType = entryDetailNode.ReadValueByte(); break;
                    case "decken_custom_id":
                        DeckenCustomID = entryDetailNode.ReadValueByte(); break;
                    case "decken_custom_type":
                        DeckenCustomType = entryDetailNode.ReadValueByte(); break;
                }
            }
        }



        public void Deserialize(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5_EB_45_F8 && magic != 0xE6_EB_45_F8)
                throw new System.IO.InvalidDataException($"Course magic did not match - Got {magic.ToString("X8")}, expected 0xE6EB45F8");

            int version = reader.ReadInt32();

            Car.Read(ref reader);

            DriverName = reader.ReadString4Aligned();
            DriverRegion = reader.ReadString4Aligned();
            RaceClassID = reader.ReadByte();
            reader.ReadSByte();

            int boost_ratio_count = reader.ReadInt32();
            if (boost_ratio_count > 0)
            {
                for (int i = 0; i < boost_ratio_count; i++)
                {
                    BoostRatio.Add(reader.ReadByte());
                    BoostRaceRatio.Add(reader.ReadSByte());
                }
            }

            AIBrakingSkill = reader.ReadInt16();
            AICorneringSkill = reader.ReadInt16();
            AIAcceleratingSkill = reader.ReadSByte();
            AIStartingSkill = reader.ReadSByte();
            AIRoughness = reader.ReadSByte();

            EngineNaTuneStage = (PARTS_NATUNE)reader.ReadSByte();
            EngineTurboKit = (PARTS_TURBINEKIT)reader.ReadSByte();
            EngineComputer = (PARTS_COMPUTER)reader.ReadSByte();
            Muffler = (PARTS_MUFFLER)reader.ReadSByte();
            Suspension = (PARTS_SUSPENSION)reader.ReadSByte();
            Transmission = (PARTS_GEAR)reader.ReadSByte();

            WheelID = reader.ReadInt16();

            if (version >= 1_02)
            {
                WheelColor = reader.ReadInt16();
                WheelInchUp = reader.ReadInt16();
            }

            TireFront = (TireType)reader.ReadSByte();
            TireRear = (TireType)reader.ReadSByte();

            // Aero
            AeroWing = reader.ReadSByte();
            Aero1_AeroKit = reader.ReadSByte();
            Aero2_FlatFloor = reader.ReadSByte();
            Aero3_AeroOther = reader.ReadSByte();

            PowerLimiter = reader.ReadSByte();
            DownforceFront = reader.ReadSByte();
            DownforceRear = reader.ReadSByte();
            PaintId = reader.ReadInt16();

            if (version >= 1_02)
            {
                reader.ReadInt16();
                DeckenNumber = reader.ReadInt16();
            }

            if (version >= 1_03)
            {
                HeadCode = reader.ReadInt16();
                BodyCode = reader.ReadInt16();
                HeadColorCode = reader.ReadInt16();
                BodyColorCode = reader.ReadInt16();
            }

            if (version >= 1_04)
            {
                AIReaction = reader.ReadByte(); // AI Reaction
                BallastWeight = reader.ReadByte();
                BallastPosition = reader.ReadSByte();
            }

            if (version >= 1_05)
            {
                DeckenType = reader.ReadByte(); // Decken Type
                DeckenCustomID = reader.ReadByte(); // Decken Custom ID
            }

            if (version >= 1_06)
                DeckenCustomType = reader.ReadByte(); // Decken Custom Type
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_EB_45_F8);
            bs.WriteUInt32(1_06);

            // Write car (carthin)
            Car.Serialize(ref bs);

            bs.WriteNullStringAligned4(DriverName);
            bs.WriteNullStringAligned4(DriverRegion);
            bs.WriteByte(RaceClassID);
            bs.WriteSByte(ProxyDriverModel);

            bs.WriteInt32(BoostRatio.Count);
            for (var i = 0; i < BoostRatio.Count; i++)
            {
                bs.WriteByte(BoostRatio[i]);
                if (i < BoostRaceRatio.Count)
                    bs.WriteSByte(BoostRaceRatio[i]);
                else
                    bs.WriteSByte(0);
            }

            bs.WriteInt16(AIBrakingSkill);
            bs.WriteInt16(AICorneringSkill);
            bs.WriteSByte(AIAcceleratingSkill);
            bs.WriteSByte(AIStartingSkill);
            bs.WriteSByte(AIRoughness);

            bs.WriteSByte((sbyte)EngineNaTuneStage);
            bs.WriteSByte((sbyte)EngineTurboKit);
            bs.WriteSByte((sbyte)EngineComputer);
            bs.WriteSByte((sbyte)Muffler);
            bs.WriteSByte((sbyte)Suspension);
            bs.WriteSByte((sbyte)Transmission);

            bs.WriteInt16((sbyte)WheelID);
            bs.WriteInt16((sbyte)WheelColor);
            bs.WriteInt16((sbyte)WheelInchUp);
            bs.WriteSByte((sbyte)TireFront);
            bs.WriteSByte((sbyte)TireRear);

            // Aero stuff
            bs.WriteSByte(AeroWing);
            bs.WriteSByte(Aero1_AeroKit);
            bs.WriteSByte(Aero2_FlatFloor);
            bs.WriteSByte(Aero3_AeroOther);

            bs.WriteSByte(PowerLimiter);
            bs.WriteSByte(DownforceFront);
            bs.WriteSByte(DownforceRear);
            bs.WriteInt16(PaintId);
            bs.WriteInt16(-1); // Unk
            bs.WriteInt16(DeckenNumber);

            bs.WriteInt16(HeadCode);
            bs.WriteInt16(BodyCode);
            bs.WriteInt16(HeadColorCode);
            bs.WriteInt16(BodyColorCode);

            bs.WriteByte(AIReaction);
            bs.WriteByte(BallastWeight);
            bs.WriteSByte(BallastPosition);
            bs.WriteByte(DeckenType);
            bs.WriteByte(DeckenCustomID);
            bs.WriteByte(DeckenType);
        }
    }
}
