using System;
using System.Xml;
using System.ComponentModel;
using System.Collections.Generic;

using PDTools.Utils;
using PDTools.Enums;
using PDTools.Enums.PS3;

namespace PDTools.Structures.MGameParameter;

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
    public string? DriverName { get; set; }

    /// <summary>
    /// Region of the driver.
    /// </summary>
    public string? DriverRegion { get; set; }

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

    public bool IsDefault()
    {
        var defaultEntryBase = new EntryBase();
        return
            this.Car.CarLabel == defaultEntryBase.Car.CarLabel &&
            this.Car.Paint == defaultEntryBase.Car.Paint &&
            this.DriverName == defaultEntryBase.DriverName &&
            this.DriverRegion == defaultEntryBase.DriverRegion &&
            this.RaceClassID == defaultEntryBase.RaceClassID &&
            this.ProxyDriverModel == defaultEntryBase.ProxyDriverModel &&
            BoostRaceRatio.Count > 0 &&
            BoostRatio.Count > 0 &&
            this.AIBrakingSkill == defaultEntryBase.AIBrakingSkill &&
            this.AIAcceleratingSkill == defaultEntryBase.AIAcceleratingSkill &&
            this.AICorneringSkill == defaultEntryBase.AICorneringSkill &&
            this.AIStartingSkill == defaultEntryBase.AIStartingSkill &&
            this.AIReaction == defaultEntryBase.AIReaction &&
            this.AIRoughness == defaultEntryBase.AIRoughness &&
            this.EngineNaTuneStage == defaultEntryBase.EngineNaTuneStage &&
            this.EngineComputer == defaultEntryBase.EngineComputer &&
            this.Suspension == defaultEntryBase.Suspension &&
            this.Transmission == defaultEntryBase.Transmission &&
            this.Muffler == defaultEntryBase.Muffler &&
            this.GearMaxSpeed == defaultEntryBase.GearMaxSpeed &&
            this.BallastWeight == defaultEntryBase.BallastWeight &&
            this.BallastPosition == defaultEntryBase.BallastPosition &&
            this.WheelID == defaultEntryBase.WheelID &&
            this.WheelColor == defaultEntryBase.WheelColor &&
            this.WheelInchUp == defaultEntryBase.WheelInchUp &&
            this.TireFront == defaultEntryBase.TireFront &&
            this.TireRear == defaultEntryBase.TireRear &&
            this.AeroWing == defaultEntryBase.AeroWing &&
            this.Aero1_AeroKit == defaultEntryBase.Aero1_AeroKit &&
            this.Aero2_FlatFloor == defaultEntryBase.Aero2_FlatFloor &&
            this.Aero3_AeroOther == defaultEntryBase.Aero3_AeroOther &&
            this.DownforceFront == defaultEntryBase.DownforceFront &&
            this.DownforceRear == defaultEntryBase.DownforceRear &&
            this.PaintId == defaultEntryBase.PaintId &&
            this.DeckenNumber == defaultEntryBase.DeckenNumber &&
            this.DeckenType == defaultEntryBase.DeckenType &&
            this.DeckenCustomID == defaultEntryBase.DeckenCustomID &&
            this.DeckenCustomType == defaultEntryBase.DeckenCustomType;
    }

    public void CopyTo(EntryBase other)
    {
        Car.CopyTo(other.Car);
        other.DriverName = DriverName;
        other.DriverRegion = DriverRegion;
        other.RaceClassID = RaceClassID;
        other.ProxyDriverModel = ProxyDriverModel;

        for (int i = 0; i < BoostRaceRatio.Count; i++)
            other.BoostRaceRatio.Add(BoostRaceRatio[i]);

        for (int i = 0; i < BoostRatio.Count; i++)
            other.BoostRatio.Add(BoostRatio[i]);

        other.AIBrakingSkill = AIBrakingSkill;
        other.AIAcceleratingSkill = AIAcceleratingSkill;
        other.AICorneringSkill = AICorneringSkill;
        other.AIStartingSkill = AIStartingSkill;

        other.EngineNaTuneStage = EngineNaTuneStage;
        other.EngineComputer = EngineComputer;
        other.Suspension = Suspension;
        other.Transmission = Transmission;
        other.Muffler = Muffler;
        other.GearMaxSpeed = GearMaxSpeed;
        other.BallastWeight = BallastWeight;
        other.BallastPosition = BallastPosition;
        other.WheelID = WheelID;
        other.WheelColor = WheelColor;
        other.WheelInchUp = WheelInchUp;
        other.TireFront = TireFront;
        other.TireRear = TireRear;
        other.AeroWing = AeroWing;
        other.Aero1_AeroKit = Aero1_AeroKit;
        other.Aero2_FlatFloor = Aero2_FlatFloor;
        other.Aero3_AeroOther = Aero3_AeroOther;
        other.DownforceFront = DownforceFront;
        other.DownforceRear = DownforceRear;
        other.PaintId = PaintId;
        other.DeckenNumber = DeckenNumber;
        other.DeckenType = DeckenType;
        other.DeckenCustomID = DeckenCustomID;
        other.DeckenCustomType = DeckenCustomType;
    }

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

            if (!string.IsNullOrEmpty(DriverName))
                xml.WriteElementValue("driver_name", DriverName);

            if (!string.IsNullOrEmpty(DriverRegion))
                xml.WriteElementValue("driver_region", DriverRegion);

            xml.WriteElementIntIfNotDefault("race_class_id", RaceClassID, defaultValue: 0);
            xml.WriteElementIntIfNotDefault("proxy_driver_model", ProxyDriverModel);

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

            xml.WriteElementIntIfNotDefault("ai_skill_breaking", AIBrakingSkill);
            xml.WriteElementIntIfNotDefault("ai_skill_cornering", AICorneringSkill);
            xml.WriteElementIntIfNotDefault("ai_skill_accelerating", AIAcceleratingSkill);
            xml.WriteElementIntIfNotDefault("ai_skill_starting", AIStartingSkill);
            xml.WriteElementIntIfNotDefault("ai_roughness", AIRoughness);
            xml.WriteElementIntIfNotDefault("ai_reaction", AIReaction, defaultValue: 0);

            if (EngineNaTuneStage != PARTS_NATUNE.NONE)
                xml.WriteElementValue("engine_na_tune_stage", EngineNaTuneStage.ToString());

            if (EngineTurboKit != PARTS_TURBINEKIT.NONE)
                xml.WriteElementValue("engine_turbo_kit", EngineTurboKit.ToString());

            if (EngineComputer != PARTS_COMPUTER.NONE)
                xml.WriteElementValue("engine_computer", EngineComputer.ToString());

            if (Muffler != PARTS_MUFFLER.UNSPECIFIED)
                xml.WriteElementValue("muffler", Muffler.ToString());

            if (Suspension != PARTS_SUSPENSION.UNSPECIFIED)
                xml.WriteElementValue("suspension", Suspension.ToString());

            if (Transmission != PARTS_GEAR.UNSPECIFIED)
                xml.WriteElementValue("transmission", Transmission.ToString());

            xml.WriteElementIntIfNotDefault("gear_max_speed", GearMaxSpeed);
            xml.WriteElementIntIfNotDefault("ballast_weight", BallastWeight, defaultValue: 0);
            xml.WriteElementIntIfNotDefault("ballast_position", BallastPosition);
            xml.WriteElementIntIfNotDefault("wheel", WheelID);
            xml.WriteElementIntIfNotDefault("wheel_color", WheelColor);
            xml.WriteElementIntIfNotDefault("wheel_inch_up", WheelInchUp);

            if (TireFront != TireType.NONE_SPECIFIED)
                xml.WriteElementValue("tire_f", TireFront.ToString());

            if (TireRear != TireType.NONE_SPECIFIED)
                xml.WriteElementValue("tire_r", TireRear.ToString());
            xml.WriteElementIntIfNotDefault("aero_wing", AeroWing);
            xml.WriteElementIntIfNotDefault("aero_1", Aero1_AeroKit);
            xml.WriteElementIntIfNotDefault("aero_2", Aero2_FlatFloor);
            xml.WriteElementIntIfNotDefault("aero_3", Aero3_AeroOther);
            xml.WriteElementIntIfNotDefault("downforce_f", DownforceFront);
            xml.WriteElementIntIfNotDefault("downforce_r", DownforceRear);
            xml.WriteElementIntIfNotDefault("paint_id", PaintId);
            xml.WriteElementIntIfNotDefault("decken_number", DeckenNumber);
            xml.WriteElementIntIfNotDefault("decken_type", DeckenType, defaultValue: 0);
            xml.WriteElementIntIfNotDefault("decken_custom_id", DeckenCustomID, defaultValue: 0);
            xml.WriteElementIntIfNotDefault("decken_custom_type", DeckenCustomType, defaultValue: 0);
        }
        xml.WriteEndElement();
    }

    public void ReadFromXml(XmlNode entryNode)
    {
        foreach (XmlNode entryDetailNode in entryNode)
        {
            switch (entryDetailNode.Name)
            {
                case "car":
                    {
                        Car.CarLabel = entryDetailNode.Attributes?["label"]?.Value ?? string.Empty;

                        var colorStr = entryDetailNode?.Attributes?["color"];
                        if (colorStr is not null && short.TryParse(colorStr.Value, out short color))
                            Car.Paint = color;
                    }
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
                    {
                        var nodes = entryDetailNode.SelectNodes("race_ratio");
                        if (nodes is not null)
                        {
                            foreach (XmlNode race_ratio_node in nodes)
                                BoostRaceRatio.Add(race_ratio_node.ReadValueSByte());
                        }
                    }
                    break;
                case "boost_ratio":
                    {
                        var nodes = entryDetailNode.SelectNodes("ratio");
                        if (nodes is not null)
                        {
                            foreach (XmlNode race_ratio_node in nodes)
                                BoostRatio.Add(race_ratio_node.ReadValueByte());
                        }
                    }
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

        DriverName = reader.ReadString4Aligned(align: 4);
        DriverRegion = reader.ReadString4Aligned(align: 4);
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
