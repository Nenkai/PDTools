using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;

using PDTools.Utils;
using PDTools.Enums;

namespace PDTools.Structures.MGameParameter;

public class Constraint
{
    /// <summary>
    /// Defaults to -1 (none specified)
    /// </summary>
    public int Transmission { get; set; } = -1;

    /// <summary>
    /// Defaults to -1 (none specified)
    /// </summary>
    public int DrivingLine { get; set; } = -1;

    /// <summary>
    /// Defaults to -1 (none specified)
    /// </summary>
    public int ASM { get; set; } = -1;

    /// <summary>
    /// Defaults to -1 (none specified)
    /// </summary>
    public int TCS { get; set; } = -1;

    /// <summary>
    /// Unused
    /// </summary>
    public int SuggestTCS { get; set; } = -1;

    /// <summary>
    /// Defaults to -1 (none specified)
    /// </summary>
    public int ABS { get; set; } = -1;

    /// <summary>
    /// Simulation Physics aka no Skid recovery force
    /// </summary>
    public int Simulation_SkidRecoveryForce { get; set; } = -1;

    /// <summary>
    /// Defaults to <see cref="TireType.NONE_SPECIFIED"/>
    /// </summary>
    public TireType LimitTireFront { get; set; } = TireType.NONE_SPECIFIED;

    /// <summary>
    /// Defaults to <see cref="TireType.NONE_SPECIFIED"/>
    /// </summary>
    public TireType NeedTireFront { get; set; } = TireType.NONE_SPECIFIED;

    /// <summary>
    /// Defaults to <see cref="TireType.NONE_SPECIFIED"/>
    /// </summary>
    public TireType SuggestTireFront { get; set; } = TireType.NONE_SPECIFIED;

    /// <summary>
    /// Defaults to <see cref="TireType.NONE_SPECIFIED"/>
    /// </summary>
    public TireType LimitTireRear { get; set; } = TireType.NONE_SPECIFIED;

    /// <summary>
    /// Defaults to <see cref="TireType.NONE_SPECIFIED"/>
    /// </summary>
    public TireType NeedTireRear { get; set; } = TireType.NONE_SPECIFIED;

    /// <summary>
    /// Defaults to <see cref="TireType.NONE_SPECIFIED"/>
    /// </summary>
    public TireType SuggestTireRear { get; set; } = TireType.NONE_SPECIFIED;

    /// <summary>
    /// Defaults to -1 (none specified)
    /// </summary>
    public int ActiveSteering { get; set; } = -1;

    /// <summary>
    /// Defaults to -1 (none specified)
    /// </summary>
    public int DriftType { get; set; } = -1;

    /// <summary>
    /// GT6 Only. Defaults to -1 (none specified)
    /// </summary>
    public int SuggestedGear { get; set; } = -1;

    /// <summary>
    /// GT6 Only. Defaults to -1 (none specified)
    /// </summary>
    public int InCarView { get; set; } = -1;

    /// <summary>
    /// Defaults to <see cref="TireType.NONE_SPECIFIED"/>
    /// </summary>
    public TireType EnemyTire { get; set; } = TireType.NONE_SPECIFIED;

    /// <summary>
    /// Defaults to -1 (none specified)
    /// </summary>
    public int PowerRestrictorLimit { get; set; } = -1;

    public List<MCarThin> Cars { get; set; } = [];

    public bool IsDefault()
    {
        var constraintsDefault = new Constraint();
        return Transmission == constraintsDefault.Transmission &&
            DrivingLine == constraintsDefault.DrivingLine &&
            ASM == constraintsDefault.ASM &&
            TCS == constraintsDefault.TCS &&
            Transmission == constraintsDefault.Transmission &&
            SuggestTCS == constraintsDefault.SuggestTCS &&
            ABS == constraintsDefault.ABS &&
            Simulation_SkidRecoveryForce == constraintsDefault.Simulation_SkidRecoveryForce &&
            LimitTireFront == constraintsDefault.LimitTireFront &&
            NeedTireFront == constraintsDefault.NeedTireFront &&
            SuggestTireFront == constraintsDefault.SuggestTireFront &&
            ActiveSteering == constraintsDefault.ActiveSteering &&
            DriftType == constraintsDefault.DriftType &&
            SuggestedGear == constraintsDefault.SuggestedGear &&
            InCarView == constraintsDefault.InCarView &&
            EnemyTire == constraintsDefault.EnemyTire &&
            PowerRestrictorLimit == constraintsDefault.PowerRestrictorLimit &&
            Cars.Count == 0;
    }

    public void CopyTo(Constraint other)
    {
        other.Transmission = Transmission;
        other.DrivingLine = DrivingLine;
        other.ASM = ASM;
        other.TCS = TCS;
        other.Transmission = Transmission;
        other.SuggestTCS = SuggestTCS;
        other.ABS = ABS;
        other.Simulation_SkidRecoveryForce = Simulation_SkidRecoveryForce;
        other.LimitTireFront = LimitTireFront;
        other.NeedTireFront = NeedTireFront;
        other.SuggestTireFront = SuggestTireFront;
        other.ActiveSteering = ActiveSteering;
        other.DriftType = DriftType;
        other.SuggestedGear = SuggestedGear;
        other.InCarView = InCarView;
        other.EnemyTire = EnemyTire;
        other.PowerRestrictorLimit = PowerRestrictorLimit;

        foreach (var car in Cars)
        {
            var mcarThin = new MCarThin();
            car.CopyTo(mcarThin);
            other.Cars.Add(mcarThin);
        }
    }

    public void ParseFromXml(XmlNode node)
    {
        foreach (XmlNode constraintNode in node.ChildNodes)
        {
            switch (constraintNode.Name)
            {
                case "transmission":
                    Transmission = constraintNode.ReadValueInt(); break;
                case "driving_line":
                    DrivingLine = constraintNode.ReadValueInt(); break;
                case "asm":
                    ASM = constraintNode.ReadValueInt(); break;
                case "tcs":
                    TCS = constraintNode.ReadValueInt(); break;
                case "suggest_tcs":
                    SuggestTCS = constraintNode.ReadValueInt(); break;
                case "abs":
                    ABS = constraintNode.ReadValueInt(); break;
                case "simulation":
                    Simulation_SkidRecoveryForce = constraintNode.ReadValueInt(); break;
                case "limit_tire_f":
                    LimitTireFront = constraintNode.ReadValueEnum<TireType>(); break;
                case "need_tire_f":
                    NeedTireFront = constraintNode.ReadValueEnum<TireType>(); break;
                case "suggest_tire_f":
                    SuggestTireFront = constraintNode.ReadValueEnum<TireType>(); break;
                case "limit_tire_r":
                    LimitTireRear = constraintNode.ReadValueEnum<TireType>(); break;
                case "need_tire_r":
                    NeedTireRear = constraintNode.ReadValueEnum<TireType>(); break;
                case "suggest_tire_r":
                    SuggestTireRear = constraintNode.ReadValueEnum<TireType>(); break;
                case "active_steering":
                    ActiveSteering = constraintNode.ReadValueInt(); break;
                case "drift_type":
                    DriftType = constraintNode.ReadValueInt(); break;
                case "in_car_view":
                    InCarView = constraintNode.ReadValueInt(); break;
                case "enemy_tire":
                    EnemyTire = constraintNode.ReadValueEnum<TireType>(); break;
                case "restrictor_limit": // Not read, but we do anyway
                    PowerRestrictorLimit = constraintNode.ReadValueInt(); break;
            }
        }
    }

    public void Deserialize(ref BitStream reader)
    {
        uint magic = reader.ReadUInt32();
        if (magic != 0xE5E5F33D && magic != 0xE6E6F33D)
            throw new System.IO.InvalidDataException($"Constraint magic did not match - Got {magic:X8}, expected 0xE6E6F33D");

        uint contraintVersion = reader.ReadUInt32();
        Transmission = reader.ReadInt32();
        DrivingLine = reader.ReadInt32();
        ASM = reader.ReadInt32();
        TCS = reader.ReadInt32();
        SuggestTCS = reader.ReadInt32();
        ABS = reader.ReadInt32();
        LimitTireFront = (TireType)reader.ReadInt32();
        NeedTireFront = (TireType)reader.ReadInt32();
        SuggestTireFront = (TireType)reader.ReadInt32();
        LimitTireRear = (TireType)reader.ReadInt32();
        NeedTireRear = (TireType)reader.ReadInt32();
        SuggestTireRear = (TireType)reader.ReadInt32();
        Simulation_SkidRecoveryForce = reader.ReadInt32();
        ActiveSteering = reader.ReadInt32();
        int cars = reader.ReadInt32();
        for (int i = 0; i < cars; i++)
        {
            // Car Thin
            var carThin = new MCarThin();
            carThin.Read(ref reader);
            Cars.Add(carThin);
        }

        DriftType = reader.ReadInt32();
        SuggestedGear = reader.ReadInt32();
        InCarView = reader.ReadInt32();
        EnemyTire = (TireType)reader.ReadInt32();

        if (contraintVersion >= 101)
            PowerRestrictorLimit = reader.ReadInt32();
    }

    public void Serialize(ref BitStream bs)
    {
        bs.WriteUInt32(0xE6_E6_F3_3D);
        bs.WriteUInt32(1_01);
        bs.WriteInt32(Transmission);
        bs.WriteInt32(DrivingLine);
        bs.WriteInt32(ASM);
        bs.WriteInt32(TCS);
        bs.WriteInt32(SuggestTCS);
        bs.WriteInt32(ABS);
        bs.WriteInt32((int)LimitTireFront);
        bs.WriteInt32((int)NeedTireFront);
        bs.WriteInt32((int)SuggestTireFront);
        bs.WriteInt32((int)LimitTireRear);
        bs.WriteInt32((int)NeedTireRear);
        bs.WriteInt32((int)SuggestTireRear);
        bs.WriteInt32(Simulation_SkidRecoveryForce);
        bs.WriteInt32(ActiveSteering);

        for (int i = 0; i < Cars.Count; i++)
        {
            // Car Thin
            Cars[i].Serialize(ref bs);
        }

        bs.WriteInt32(DriftType);
        bs.WriteInt32(SuggestedGear);
        bs.WriteInt32(InCarView);
        bs.WriteInt32((int)EnemyTire);
        bs.WriteInt32(PowerRestrictorLimit);

    }

    public void WriteToXml(XmlWriter xml)
    {
        xml.WriteElementInt("transmission", Transmission);
        xml.WriteElementInt("driving_line", DrivingLine);
        xml.WriteElementInt("asm", ASM);
        xml.WriteElementInt("tcs", TCS);
        xml.WriteElementIntIfNotDefault("suggest_tcs", SuggestTCS);
        xml.WriteElementInt("simulation", Simulation_SkidRecoveryForce);
        xml.WriteElementEnumInt("limit_tire_f", LimitTireFront);
        xml.WriteElementEnumInt("need_tire_f", NeedTireFront);
        xml.WriteElementEnumInt("suggest_tire_f", SuggestTireFront);
        xml.WriteElementEnumInt("limit_tire_r", LimitTireRear);
        xml.WriteElementEnumInt("need_tire_r", NeedTireRear);
        xml.WriteElementEnumInt("suggest_tire_r", SuggestTireRear);
        xml.WriteElementInt("active_steering", ActiveSteering);
        xml.WriteElementInt("drift_type", DriftType);
        xml.WriteElementIntIfNotDefault("suggested_gear", SuggestedGear);
        xml.WriteElementIntIfNotDefault("in_car_view", InCarView);

        if (EnemyTire != TireType.NONE_SPECIFIED)
            xml.WriteElementEnumInt("enemy_tire", EnemyTire);
    }
}
