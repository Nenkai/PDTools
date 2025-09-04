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

public class Regulation
{
    public int LimitPP { get; set; } = -1;
    public int NeedPP { get; set; } = -1;

    public TireType LimitTireFront { get; set; } = TireType.NONE_SPECIFIED;
    public TireType NeedTireFront { get; set; } = TireType.NONE_SPECIFIED;
    public TireType LimitTireRear { get; set; } = TireType.NONE_SPECIFIED;
    public TireType NeedTireRear { get; set; } = TireType.NONE_SPECIFIED;

    public List<CarCategoryRestriction> CarCategories { get; set; } = [];
    public List<MCarThin> Cars { get; set; } = [];
    public List<MCarThin> BanCars { get; set; } = [];

    public int NeedLicense { get; set; } = -1;
    public int LimitPower { get; set; } = -1;

    /// <summary>
    /// GT5 Only
    /// </summary>
    public int NeedTorque { get; set; } = -1;

    /// <summary>
    /// GT5 Only
    /// </summary>
    public int LimitTorque { get; set; } = -1;

    /// <summary>
    /// GT5 Only, appears unused
    /// </summary>
    public int LimitDisplacement { get; set; } = -1;

    /// <summary>
    /// GT5 Only, appears unused
    /// </summary>
    public int NeedDisplacement { get; set; } = -1;

    public int NeedPower { get; set; } = -1;
    public int LimitWeight { get; set; } = -1;
    public int NeedWeight { get; set; } = -1;
    public int LimitLength { get; set; } = -1;
    public int NeedLength { get; set; } = -1;
    public DrivetrainBits NeedDrivetrain { get; set; } = DrivetrainBits.NONE_SPECIFIED;
    public AspirationBits NeedAspiration { get; set; } = AspirationBits.NONE_SPECIFIED;

    /// <summary>
    /// Appears unused
    /// </summary>
    public int Tuning { get; set; } = -1;

    /// <summary>
    /// GT6 Only
    /// </summary>
    public int NOS { get; set; } = -1;

    /// <summary>
    /// GT6 Only
    /// </summary>
    public int KartPermitted { get; set; } = -1;

    /// <summary>
    /// GT6 Only, appears unused
    /// </summary>
    public int CarTagID { get; set; } = -1;

    public int RestrictorLimit { get; set; } = -1;
    public int LimitYear { get; set; } = -1;
    public int NeedYear { get; set; } = -1;
    public List<Tuner> Tuners { get; set; } = [];
    public List<Country> Countries { get; set; } = [];

    public int LimitASpecLevel { get; set; } = -1;
    public int NeedASpecLevel { get; set; } = -1;
    public int LimitBSpecLevel { get; set; } = -1;
    public int NeedBSpecLevel { get; set; } = -1;
    public int LimitBSpecDriverCount { get; set; } = -1;
    public int NeedBSpecDriverCount { get; set; } = -1;
    public string? NeedEntitlement { get; set; }

    public bool IsDefault()
    {
        var defaultRegulations = new Regulation();
        return LimitPP == defaultRegulations.LimitPP &&
            NeedPP == defaultRegulations.NeedPP &&
            NeedPP == defaultRegulations.NeedPP &&
            LimitTireFront == defaultRegulations.LimitTireFront &&
            NeedTireRear == defaultRegulations.NeedTireRear &&
            LimitTireFront == defaultRegulations.LimitTireFront &&
            LimitTireRear == defaultRegulations.LimitTireRear &&
            CarCategories.Count == 0 &&
            Cars.Count == 0 &&
            BanCars.Count == 0 &&
            NeedLicense == defaultRegulations.NeedLicense &&
            LimitPower == defaultRegulations.LimitPower &&
            NeedPower == defaultRegulations.NeedPower &&
            LimitTorque == defaultRegulations.LimitTorque &&
            NeedTorque == defaultRegulations.NeedTorque &&
            LimitDisplacement == defaultRegulations.LimitDisplacement &&
            NeedDisplacement == defaultRegulations.NeedDisplacement &&
            LimitWeight == defaultRegulations.LimitWeight &&
            NeedWeight == defaultRegulations.NeedWeight &&
            LimitLength == defaultRegulations.LimitLength &&
            NeedLength == defaultRegulations.NeedLength &&
            NeedDrivetrain == defaultRegulations.NeedDrivetrain &&
            NeedAspiration == defaultRegulations.NeedAspiration &&
            Tuning == defaultRegulations.Tuning &&
            NOS == defaultRegulations.NOS &&
            KartPermitted == defaultRegulations.KartPermitted &&
            CarTagID == defaultRegulations.CarTagID &&
            RestrictorLimit == defaultRegulations.RestrictorLimit &&
            LimitYear == defaultRegulations.LimitYear &&
            NeedYear == defaultRegulations.NeedYear &&
            Tuners.Count == 0 &&
            Countries.Count == 0 &&
            LimitASpecLevel == defaultRegulations.LimitASpecLevel &&
            NeedASpecLevel == defaultRegulations.NeedASpecLevel &&
            LimitBSpecLevel == defaultRegulations.LimitBSpecLevel &&
            NeedBSpecLevel == defaultRegulations.NeedBSpecLevel &&
            LimitBSpecDriverCount == defaultRegulations.LimitBSpecDriverCount &&
            NeedBSpecDriverCount == defaultRegulations.NeedBSpecDriverCount &&
            NeedEntitlement == defaultRegulations.NeedEntitlement;
    }

    public void CopyTo(Regulation other)
    {
        other.LimitPP = LimitPP;
        other.NeedPP = NeedPP;
        other.NeedPP = NeedPP;
        other.LimitTireFront = LimitTireFront;
        other.NeedTireRear = NeedTireRear;
        other.LimitTireFront = LimitTireFront;
        other.LimitTireRear = LimitTireRear;

        foreach (var category in CarCategories)
            other.CarCategories.Add(category);

        foreach (var car in Cars)
        {
            var mcarThin = new MCarThin();
            car.CopyTo(mcarThin);
            other.Cars.Add(mcarThin);
        }

        foreach (var car in BanCars)
        {
            var mcarThin = new MCarThin();
            car.CopyTo(mcarThin);
            other.BanCars.Add(mcarThin);
        }

        other.NeedLicense = NeedLicense;
        other.LimitPower = LimitPower;
        other.NeedPower = NeedPower;
        other.LimitTorque = LimitTorque;
        other.NeedTorque = NeedTorque;
        other.LimitDisplacement = LimitDisplacement;
        other.NeedDisplacement = NeedDisplacement;
        other.LimitWeight = LimitWeight;
        other.NeedWeight = NeedWeight;
        other.LimitLength = LimitLength;
        other.NeedLength = NeedLength;
        other.NeedDrivetrain = NeedDrivetrain;
        other.NeedAspiration = NeedAspiration;
        other.Tuning = Tuning;
        other.NOS = NOS;
        other.KartPermitted = KartPermitted;
        other.CarTagID = CarTagID;
        other.RestrictorLimit = RestrictorLimit;
        other.LimitYear = LimitYear;
        other.NeedYear = NeedYear;

        foreach (var tuner in Tuners)
            other.Tuners.Add(tuner);

        foreach (var country in Countries)
            other.Countries.Add(country);

        other.LimitASpecLevel = LimitASpecLevel;
        other.NeedASpecLevel = NeedASpecLevel;
        other.LimitBSpecLevel = LimitBSpecLevel;
        other.NeedBSpecLevel = NeedBSpecLevel;
        other.LimitBSpecDriverCount = LimitBSpecDriverCount;
        other.NeedBSpecDriverCount = NeedBSpecDriverCount;
        other.NeedEntitlement = NeedEntitlement;
    }

    public void WriteToXml(XmlWriter xml)
    {
        xml.WriteElementInt("limit_pp", LimitPP);
        xml.WriteElementInt("need_pp", NeedPP);
        xml.WriteElementEnumInt("limit_tire_f", LimitTireFront);
        xml.WriteElementEnumInt("need_tire_f", NeedTireFront);
        xml.WriteElementEnumInt("limit_tire_r", LimitTireRear);
        xml.WriteElementEnumInt("need_tire_r", NeedTireRear);

        xml.WriteStartElement("car_categories");
        foreach (CarCategoryRestriction category in CarCategories)
            xml.WriteElementValue("category", category.ToString());
        xml.WriteEndElement();

        xml.WriteStartElement("cars");
        foreach (MCarThin vehicle in Cars)
        {
            xml.WriteStartElement("car");
            xml.WriteAttributeString("label", vehicle.CarLabel);
            xml.WriteEndElement();
        }
        xml.WriteEndElement();

        xml.WriteStartElement("ban_cars");
        foreach (MCarThin vehicle in BanCars)
        {
            xml.WriteStartElement("car");
            xml.WriteAttributeString("label", vehicle.CarLabel);
            xml.WriteEndElement();
        }
        xml.WriteEndElement();

        xml.WriteElementInt("need_license", NeedLicense);

        xml.WriteElementInt("limit_power", LimitPower);
        xml.WriteElementInt("need_power", NeedPower);
        xml.WriteElementInt("limit_torque", LimitTorque);
        xml.WriteElementInt("need_torque", NeedTorque);
        xml.WriteElementInt("limit_displacement", LimitDisplacement);
        xml.WriteElementInt("need_displacement", NeedDisplacement);
        xml.WriteElementInt("limit_weight", LimitWeight);
        xml.WriteElementInt("need_weight", NeedWeight);
        xml.WriteElementInt("limit_length", LimitLength);
        xml.WriteElementInt("need_length", NeedLength);
        xml.WriteElementInt("need_drivetrain", (int)NeedDrivetrain);
        xml.WriteElementInt("need_aspiration", (int)NeedAspiration);
        xml.WriteElementInt("tuning", Tuning);
        xml.WriteElementInt("NOS", NOS);
        xml.WriteElementInt("kart_permitted", KartPermitted);
        xml.WriteElementInt("car_tag_id", CarTagID);
        xml.WriteElementInt("restrictor_limit", RestrictorLimit);
        xml.WriteElementInt("limit_year", LimitYear);
        xml.WriteElementInt("need_year", LimitYear);

        xml.WriteStartElement("tuners");
        foreach (Tuner tuner in Tuners)
            xml.WriteElementValue("tuner", tuner.ToString());
        xml.WriteEndElement();

        xml.WriteStartElement("countries");
        foreach (Country country in Countries)
            xml.WriteElementValue("country", country.ToString());
        xml.WriteEndElement();

        xml.WriteElementInt("limit_aspec_level", LimitASpecLevel);
        xml.WriteElementInt("need_aspec_level", NeedASpecLevel);
        xml.WriteElementInt("limit_bspec_level", LimitBSpecLevel);
        xml.WriteElementInt("need_bspec_level", NeedBSpecLevel);
        xml.WriteElementInt("limit_bspec_driver_count", LimitBSpecDriverCount);
        xml.WriteElementInt("need_bspec_driver_count", NeedBSpecDriverCount);

        if (!string.IsNullOrEmpty(NeedEntitlement))
            xml.WriteElementValue("need_entitlement", NeedEntitlement);
    }

    public void ParseFromXml(XmlNode node)
    {
        foreach (XmlNode regulationNode in node.ChildNodes)
        {
            switch (regulationNode.Name)
            {
                case "limit_pp":
                    LimitPP = regulationNode.ReadValueInt(); break;
                case "need_pp":
                    NeedPP = regulationNode.ReadValueInt(); break;
                case "limit_tire_f":
                    LimitTireFront = regulationNode.ReadValueEnum<TireType>(); break;
                case "need_tire_f":
                    NeedTireFront = regulationNode.ReadValueEnum<TireType>(); break;
                case "limit_tire_r":
                    LimitTireRear = regulationNode.ReadValueEnum<TireType>(); break;
                case "need_tire_r":
                    NeedTireRear = regulationNode.ReadValueEnum<TireType>(); break;
                case "car_categories":
                    ParseAllowedCategories(regulationNode);
                    break;
                case "cars":
                    ParseRaceAllowedVehicles(regulationNode);
                    break;
                case "ban_cars":
                    ParseRaceDisallowedVehicles(regulationNode);
                    break;
                case "need_license":
                    NeedLicense = regulationNode.ReadValueInt();
                    break;
                case "limit_power":
                    LimitPower = regulationNode.ReadValueInt(); break;
                case "need_power":
                    NeedPower = regulationNode.ReadValueInt(); break;
                case "limit_torque":
                    LimitTorque = regulationNode.ReadValueInt(); break;
                case "need_torque":
                    NeedTorque = regulationNode.ReadValueInt(); break;
                case "limit_displacement":
                    LimitDisplacement = regulationNode.ReadValueInt(); break;
                case "need_displacement":
                    NeedDisplacement = regulationNode.ReadValueInt(); break;
                case "limit_weight":
                    LimitWeight = regulationNode.ReadValueInt(); break;
                case "need_weight":
                    NeedWeight = regulationNode.ReadValueInt(); break;
                case "limit_length":
                    LimitLength = regulationNode.ReadValueInt();
                    break;
                case "need_length":
                    NeedLength = regulationNode.ReadValueInt();
                    break;
                case "need_drivetrain":
                    int val2 = regulationNode.ReadValueInt();
                    NeedDrivetrain = (DrivetrainBits)regulationNode.ReadValueInt();
                    break;
                case "need_aspiration":
                    int val = regulationNode.ReadValueInt();
                    NeedAspiration = (AspirationBits)regulationNode.ReadValueInt();
                    break;
                case "tuning":
                    Tuning = regulationNode.ReadValueInt();
                    break;
                case "NOS":
                    NOS = regulationNode.ReadValueInt();
                    break;
                case "kart_permitted":
                    KartPermitted = regulationNode.ReadValueInt();
                    break;
                case "car_tag_id":
                    CarTagID = regulationNode.ReadValueInt();
                    break;
                case "restrictor_limit":
                    RestrictorLimit = regulationNode.ReadValueInt();
                    break;
                case "limit_year":
                    LimitYear = regulationNode.ReadValueInt(); break;
                case "need_year":
                    NeedYear = regulationNode.ReadValueInt(); break;
                case "tuners":
                    ParseRaceAllowedManufacturers(regulationNode);
                    break;
                case "countries":
                    ParseAllowedCountries(regulationNode);
                    break;
                case "limit_aspec_level":
                    LimitASpecLevel = regulationNode.ReadValueInt(); break;
                case "need_aspec_level":
                    NeedASpecLevel = regulationNode.ReadValueInt(); break;
                case "limit_bspec_level":
                    LimitBSpecLevel = regulationNode.ReadValueInt(); break;
                case "need_bspec_level":
                    NeedBSpecLevel = regulationNode.ReadValueInt(); break;
                case "limit_bspec_driver_count":
                    LimitBSpecDriverCount = regulationNode.ReadValueInt(); break;
                case "need_bspec_driver_count":
                    NeedBSpecDriverCount = regulationNode.ReadValueInt(); break;
                case "need_entitlement":
                    NeedEntitlement = regulationNode.ReadValueString(); break;
            }
        }
    }

    private void ParseRaceAllowedManufacturers(XmlNode node)
    {
        Tuners = [];

        XmlNodeList? tunerNodes = node.SelectNodes("tuner");
        if (tunerNodes is null)
            return;

        foreach (XmlNode? manufacturerNode in tunerNodes)
        {
            if (manufacturerNode is null)
                continue;

            Tuners.Add(node.ReadValueEnum<Tuner>());
        }
    }

    private void ParseRaceAllowedVehicles(XmlNode node)
    {
        Cars = [];
        XmlNodeList? carNodes = node.SelectNodes("car");
        if (carNodes is null)
            return;

        foreach (XmlNode? vehicleNode in carNodes)
        {
            if (vehicleNode?.Attributes is null)
                continue;

            XmlAttribute? label = vehicleNode.Attributes["label"];
            if (label is null)
                continue;

            Cars.Add(new MCarThin(label.Value));
        }
    }

    private void ParseRaceDisallowedVehicles(XmlNode node)
    {
        BanCars = [];
        XmlNodeList? carNodes = node.SelectNodes("car");
        if (carNodes is null)
            return;

        foreach (XmlNode? vehicleNode in carNodes)
        {
            if (vehicleNode?.Attributes is null)
                continue;

            XmlAttribute? label = vehicleNode.Attributes["label"];
            if (label is null)
                continue;

            BanCars.Add(new MCarThin(label.Value));
        }
    }

    private void ParseAllowedCountries(XmlNode node)
    {
        Countries = [];

        XmlNodeList? countryNodes = node.SelectNodes("country");
        if (countryNodes is null)
            return;

        foreach (XmlNode? countryNode in countryNodes)
        {
            if (countryNode is null)
                continue;

            Countries.Add(node.ReadValueEnum<Country>());
        }
    }

    private void ParseAllowedCategories(XmlNode node)
    {
        CarCategories = [];

        XmlNodeList? categoryNodes = node.SelectNodes("category");
        if (categoryNodes is null)
            return;

        foreach (XmlNode? categoryNode in categoryNodes)
        {
            if (categoryNode is null)
                continue;

            CarCategories.Add(node.ReadValueEnum<CarCategoryRestriction>());
        }
    }

    public void Deserialize(ref BitStream reader)
    {
        uint magic = reader.ReadUInt32();
        if (magic != 0xE5E561AB && magic != 0xE6E661AB)
            throw new System.IO.InvalidDataException($"Regulation magic did not match - Got {magic.ToString("X8")}, expected 0xE6E661AB");

        uint regulationVersion = reader.ReadUInt32();
        LimitPP = reader.ReadInt32();
        NeedPP = reader.ReadInt32();
        LimitTireFront = (TireType)reader.ReadInt32();
        NeedTireFront = (TireType)reader.ReadInt32();
        LimitTireRear = (TireType)reader.ReadInt32();
        NeedTireRear = (TireType)reader.ReadInt32();

        int carCategoryCount = reader.ReadInt32();
        for (int i = 0; i < carCategoryCount; i++)
            CarCategories.Add((CarCategoryRestriction)reader.ReadInt32());

        int carCount = reader.ReadInt32();
        for (int i = 0; i < carCount; i++)
        {
            var car = new MCarThin();
            car.Read(ref reader);
            Cars.Add(car);
        }

        NeedLicense = reader.ReadInt32();
        LimitPower = reader.ReadInt32();
        NeedPower = reader.ReadInt32();
        if (regulationVersion < 101)
        {
            reader.ReadInt16();
            reader.ReadInt16();
        }

        LimitWeight = reader.ReadInt32();
        NeedWeight = reader.ReadInt32();
        LimitLength = reader.ReadInt32();
        NeedLength = reader.ReadInt32();
        NeedDrivetrain = (DrivetrainBits)reader.ReadInt32();
        NeedAspiration = (AspirationBits)reader.ReadInt32();
        LimitYear = reader.ReadInt32();
        NeedYear = reader.ReadInt32();

        LimitASpecLevel = reader.ReadInt32();
        NeedASpecLevel = reader.ReadInt32();
        LimitBSpecLevel = reader.ReadInt32();
        NeedBSpecLevel = reader.ReadInt32();
        LimitBSpecDriverCount = reader.ReadInt32();
        NeedBSpecDriverCount = reader.ReadInt32();

        int tunersCount = reader.ReadInt32();
        for (int i = 0; i < tunersCount; i++)
        {
            Tuner tuner = (Tuner)reader.ReadInt32();
            Tuners.Add(tuner);
        }

        int countriesCount = reader.ReadInt32();
        for (int i = 0; i < countriesCount; i++)
        {
            Country country = (Country)reader.ReadInt32();
            Countries.Add(country);
        }

        NeedEntitlement = reader.ReadString4();
        Tuning = reader.ReadInt32();

        if (regulationVersion >= 101)
            NOS = reader.ReadInt32();

        if (regulationVersion >= 102)
            KartPermitted = reader.ReadInt32();

        int banCars = reader.ReadInt32();
        for (int i = 0; i < banCars; i++)
        {
            var car = new MCarThin();
            car.Read(ref reader);
            BanCars.Add(car);
        }

        CarTagID = reader.ReadInt32();
        RestrictorLimit = reader.ReadInt32();
    }

    public void Serialize(ref BitStream bs)
    {
        bs.WriteUInt32(0xE6_E6_61_AB);
        bs.WriteUInt32(1_03); // Version

        bs.WriteInt32(LimitPP);
        bs.WriteInt32(NeedPP);

        bs.WriteInt32((int)LimitTireFront);
        bs.WriteInt32((int)NeedTireFront);
        bs.WriteInt32((int)LimitTireRear);
        bs.WriteInt32((int)NeedTireRear);

        bs.WriteInt32(CarCategories.Count);
        foreach (var cat in CarCategories)
            bs.WriteInt32((int)cat);

        bs.WriteInt32(Cars.Count);
        foreach (var car in Cars)
            car.Serialize(ref bs);

        bs.WriteInt32(NeedLicense);
        bs.WriteInt32(LimitPower);
        bs.WriteInt32(NeedPower);
        bs.WriteInt32(LimitWeight);
        bs.WriteInt32(NeedWeight);
        bs.WriteInt32(LimitLength);
        bs.WriteInt32(NeedLength);
        bs.WriteInt32(NeedDrivetrain == 0 ? -1 : (int)NeedDrivetrain);
        bs.WriteInt32(NeedAspiration == 0 ? -1 : (int)NeedAspiration);
        bs.WriteInt32(LimitYear);
        bs.WriteInt32(NeedYear);
        bs.WriteInt32(LimitASpecLevel);
        bs.WriteInt32(NeedASpecLevel);
        bs.WriteInt32(LimitBSpecLevel);
        bs.WriteInt32(NeedBSpecLevel);
        bs.WriteInt32(LimitBSpecDriverCount);
        bs.WriteInt32(NeedBSpecDriverCount);

        bs.WriteInt32(Tuners.Count);
        foreach (Tuner tuner in Tuners)
            bs.WriteInt32((int)tuner);

        bs.WriteInt32(Countries.Count);
        foreach (Country country in Countries)
            bs.WriteInt32((int)country);

        bs.WriteNullStringAligned4(string.Empty); // Entitlement
        bs.WriteInt32(Tuning);
        bs.WriteInt32(NOS);
        bs.WriteInt32(KartPermitted);

        bs.WriteInt32(BanCars.Count);
        foreach (var banCar in BanCars)
            banCar.Serialize(ref bs);

        bs.WriteInt32(CarTagID);
        bs.WriteInt32(RestrictorLimit);
    }
}

public enum CarCategoryRestriction
{
    [Description("Normal Cars")]
    NORMAL,

    [Description("Racing Cars")]
    RACING,

    [Description("Tuned Cars")]
    TUNING,

    [Description("Concept Cars")]
    CONCEPT
}

[Flags]
public enum AspirationBits
{
    NONE_SPECIFIED = -1,
    NA = 0x02,
    Turbo = 0x04,
    Supercharger = 0x08,
}


[Flags]
public enum DrivetrainBits
{
    NONE_SPECIFIED = -1,

    FR = 0x02,
    FF = 0x04,
    AWD = 0x08,
    MR = 0x10,
    RR = 0x20,

    All = FR | FF | AWD | MR | RR,
}
