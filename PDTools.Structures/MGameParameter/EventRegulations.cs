using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;

using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class EventRegulations
    {
        public static Dictionary<string, string> CountryDefinitions = new Dictionary<string, string>()
        {
            {"DE", "Germany"},
            {"FR", "France"},
            {"GB", "United Kingdom"},
            {"IT", "Italy"},
            {"JP", "Japan"},
            {"SE", "Sweden" },
            {"US", "USA" },
            {"AU", "Australia" },
            {"BE", "Belgium" },
            {"ES", "Spain" },
            {"KR", "South Korea" },
            {"NL", "Netherlands" },
            {"CA", "Canada" },
            {"AE", "UAE" },
            {"AR", "Argentina" },
            {"AT", "Austria" },
            {"CH", "Switzerland" },
            {"PT", "Portugal" },
            {"NZ", "New Zealand" },
        };

        public static Dictionary<CarCategoryRestriction, string> CategoryDefinitions = new Dictionary<CarCategoryRestriction, string>()
        {
            {CarCategoryRestriction.NORMAL, "Normal Cars" },
            {CarCategoryRestriction.RACING, "Racing Cars"},
            {CarCategoryRestriction.TUNING, "Tuned Cars"},
            {CarCategoryRestriction.CONCEPT, "Concept Cars"},
        };

        public bool NeedsPopulating { get; set; } = true;

        public List<(int Code, string label)> AllowedManufacturers { get; set; }
        public List<MCarThin> RestrictedVehicles { get; set; }
        public List<MCarThin> AllowedVehicles { get; set; }
        public List<string> AllowedCountries { get; set; }
        public List<CarCategoryRestriction> AllowedCategories { get; set; }

        private int _ppMax = -1;
        public int PPMax { get => _ppMax; set => _ppMax = value > -1 ? value : -1; }
        private int _ppMin = -1;
        public int PPMin { get => _ppMin; set => _ppMin = value > -1 ? value : -1; }

        private int _yearMax = -1;
        public int YearMax { get => _yearMax; set => _yearMax = value > -1 ? value : -1; }
        private int _yearMin = -1;
        public int YearMin { get => _yearMin; set => _yearMin = value > -1 ? value : -1; }

        private int _torqueMin = -1;
        public int TorqueMin { get => _torqueMin; set => _torqueMin = value > -1 ? value : -1; }
        private int _torqueMax = -1;
        public int TorqueMax { get => _torqueMax; set => _torqueMax = value > -1 ? value : -1; }

        private int _powerMax = -1;
        public int PowerMax { get => _powerMax; set => _powerMax = value > -1 ? value : -1; }

        private int _powerMin = -1;
        public int PowerMin { get => _powerMin; set => _powerMin = value > -1 ? value : -1; }

        private int _weightMax = -1;
        public int WeightMax { get => _weightMax; set => _weightMax = value > -1 ? value : -1; }

        private int _weightMin = -1;
        public int WeightMin { get => _weightMin; set => _weightMin = value > -1 ? value : -1; }

        private int _carLengthMax = -1;
        public int CarLengthMax { get => _carLengthMax; set => _carLengthMax = value > -1 ? value : -1; }

        private int _carLengthMin = -1;
        public int CarLengthMin { get => _carLengthMin; set => _carLengthMin = value > -1 ? value : -1; }

        private int _aspecLevelMax = -1;
        public int ASpecLevelMax { get => _aspecLevelMax; set => _aspecLevelMax = value > -1 ? value : -1; }

        private int _aspecLevelMin = -1;
        public int ASpecLevelMin { get => _aspecLevelMin; set => _aspecLevelMin = value > -1 ? value : -1; }

        private int _bspecLevelMax = -1;
        public int BSpecLevelMax { get => _bspecLevelMax; set => _bspecLevelMax = value > -1 ? value : -1; }

        private int _bspecLevelMin = -1;
        public int BSpecLevelMin { get => _bspecLevelMin; set => _bspecLevelMin = value > -1 ? value : -1; }

        private int _bspecDriverCountMax = -1;
        public int BSpecDriverCountMax { get => _bspecDriverCountMax; set => _bspecDriverCountMax = value > -1 ? value : -1; }

        private int _bspecDriverCountMin = -1;
        public int BSpecDriverCountMin { get => _bspecDriverCountMin; set => _bspecDriverCountMin = value > -1 ? value : -1; }

        private int _restrictorLimit = -1;
        public int RestrictorLimit { get => _restrictorLimit; set => _restrictorLimit = value > -1 ? value : -1; }

        public bool? KartPermitted { get; set; }
        public bool? NeedLicense { get; set; }
        public bool? Tuning { get; set; }
        public bool NOSRegulated { get; set; }
        private bool? _nosNeeded;

        public sbyte CarTagID { get; set; } = -1;
        public bool? NOSNeeded
        {
            get => NOSRegulated ? _nosNeeded : null;
            set
            {
                _nosNeeded = value;
                NOSRegulated = _nosNeeded.HasValue;
            }
        }

        public TireType TireCompoundMinFront { get; set; } = TireType.NONE_SPECIFIED;
        public TireType TireCompoundMinRear { get; set; } = TireType.NONE_SPECIFIED;

        public TireType TireCompoundMaxFront { get; set; } = TireType.NONE_SPECIFIED;
        public TireType TireCompoundMaxRear { get; set; } = TireType.NONE_SPECIFIED;

        public AspirationBits AspirationNeeded { get; set; } = AspirationBits.None;
        public DrivetrainBits DrivetrainNeeded { get; set; } = DrivetrainBits.None;

        public EventRegulations()
        {
            AllowedManufacturers = new List<(int, string)>();
            RestrictedVehicles = new List<MCarThin>();
            AllowedVehicles = new List<MCarThin>();
            AllowedCountries = new List<string>();
            AllowedCategories = new List<CarCategoryRestriction>();
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("regulation");

            xml.WriteStartElement("countries");
            foreach (string country in AllowedCountries)
                xml.WriteElementValue("country", country);
            xml.WriteEndElement();

            xml.WriteStartElement("car_categories");
            foreach (CarCategoryRestriction category in AllowedCategories)
                xml.WriteElementValue("category", category.ToString());
            xml.WriteEndElement();

            xml.WriteElementInt("limit_aspec_level", -1);
            xml.WriteElementInt("limit_bspec_level", -1);
            xml.WriteElementInt("limit_power", PowerMax);
            xml.WriteElementInt("limit_pp", PPMax);
            xml.WriteElementInt("limit_torque", TorqueMax);
            xml.WriteElementInt("limit_weight", WeightMax);
            xml.WriteElementEnumInt("limit_tire_f", TireCompoundMaxFront);
            xml.WriteElementEnumInt("limit_tire_r", TireCompoundMaxRear);
            xml.WriteElementInt("limit_year", YearMax);
            xml.WriteElementInt("need_aspec_level", -1);
            xml.WriteElementInt("need_bspec_level", -1);
            xml.WriteElementInt("need_power", PowerMin);
            xml.WriteElementInt("need_pp", PPMin);
            xml.WriteElementInt("need_torque", TorqueMin);
            xml.WriteElementInt("need_weight", WeightMin);
            xml.WriteElementEnumInt("need_tire_f", TireCompoundMinFront);
            xml.WriteElementEnumInt("need_tire_r", TireCompoundMinRear);
            xml.WriteElementInt("need_year", YearMin);
            xml.WriteElementInt("need_aspiration", AspirationNeeded > 0 ? (int)AspirationNeeded : -1 );
            xml.WriteElementInt("need_drivetrain", DrivetrainNeeded > 0 ? (int)DrivetrainNeeded : -1 );
            xml.WriteElementInt("need_license", -1);
            xml.WriteElementInt("limit_length", CarLengthMax);
            xml.WriteElementInt("need_length", CarLengthMin);

            xml.WriteElementIntIfSet("limit_aspec_level", ASpecLevelMax);
            xml.WriteElementIntIfSet("need_aspec_level", ASpecLevelMin);
            xml.WriteElementIntIfSet("limit_bspec_level", BSpecLevelMax);
            xml.WriteElementIntIfSet("need_bspec_level", BSpecLevelMin);
            xml.WriteElementIntIfSet("limit_bspec_driver_count", BSpecDriverCountMax);
            xml.WriteElementIntIfSet("need_bspec_driver_count", BSpecDriverCountMin);

            xml.WriteElementBoolOrNull("kart_permitted", KartPermitted);
            xml.WriteElementBoolOrNull("tuning", Tuning);
            xml.WriteElementInt("restrictor_limit", -1);
            xml.WriteElementBoolIfSet("NOS", NOSNeeded);

            xml.WriteStartElement("cars");
            foreach (MCarThin vehicle in AllowedVehicles)
            {
                xml.WriteStartElement("car");
                xml.WriteAttributeString("label", vehicle.CarLabel);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();

            xml.WriteStartElement("ban_cars");
            foreach (MCarThin vehicle in RestrictedVehicles)
            {
                xml.WriteStartElement("car");
                xml.WriteAttributeString("label", vehicle.CarLabel);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();

            xml.WriteStartElement("tuners");
            foreach (var manufacturer in AllowedManufacturers)
                xml.WriteElementValue("tuner", manufacturer.Item2);
            xml.WriteEndElement();

            xml.WriteEndElement();
        }

        public void ParseRegulations(XmlNode node)
        {
            foreach (XmlNode regulationNode in node.ChildNodes)
            {
                switch (regulationNode.Name)
                {
                    case "ban_cars":
                        ParseRaceDisallowedVehicles(regulationNode);
                        break;

                    case "cars":
                        ParseRaceAllowedVehicles(regulationNode);
                        break;

                    case "countries":
                        ParseAllowedCountries(regulationNode);
                        break;
                    case "car_categories":
                        ParseAllowedCategories(regulationNode);
                        break;

                    case "limit_length":
                        CarLengthMax = regulationNode.ReadValueInt();
                        break;
                    case "need_length":
                        CarLengthMin = regulationNode.ReadValueInt();
                        break;

                    case "need_year":
                        YearMin = regulationNode.ReadValueInt(); break;
                    case "limit_year":
                        YearMax = regulationNode.ReadValueInt(); break;

                    case "need_torque":
                        TorqueMin = regulationNode.ReadValueInt(); break;
                    case "limit_torque":
                        TorqueMax = regulationNode.ReadValueInt(); break;

                    case "need_power":
                        PowerMin = regulationNode.ReadValueInt(); break;
                    case "limit_power":
                        PowerMax = regulationNode.ReadValueInt(); break;

                    case "need_weight":
                        WeightMin = regulationNode.ReadValueInt(); break;
                    case "limit_weight":
                        WeightMax = regulationNode.ReadValueInt(); break;

                    case "need_pp":
                        PPMin = regulationNode.ReadValueInt(); break;
                    case "limit_pp":
                        PPMax = regulationNode.ReadValueInt(); break;

                    case "limit_tire_f":
                        TireCompoundMaxFront = regulationNode.ReadValueEnum<TireType>(); break;

                    case "limit_tire_r":
                        TireCompoundMaxRear = regulationNode.ReadValueEnum<TireType>(); break;

                    case "need_tire_f":
                        TireCompoundMinFront = regulationNode.ReadValueEnum<TireType>();
                        break;

                    case "need_tire_r":
                        TireCompoundMinRear = regulationNode.ReadValueEnum<TireType>(); break;

                    case "need_aspiration":
                        int val = regulationNode.ReadValueInt();
                        AspirationNeeded = (AspirationBits)(val == -1 ? 0 : val);
                        break;

                    case "need_drivetrain":
                        int val2 = regulationNode.ReadValueInt();
                        DrivetrainNeeded = (DrivetrainBits)(val2 == -1 ? 0 : val2);
                        break;

                    case "limit_aspec_level":
                        ASpecLevelMax = regulationNode.ReadValueInt(); break;
                    case "need_aspec_level":
                        ASpecLevelMin = regulationNode.ReadValueInt(); break;
                    case "limit_bspec_level":
                        BSpecLevelMax = regulationNode.ReadValueInt(); break;
                    case "need_bspec_level":
                        BSpecLevelMin = regulationNode.ReadValueInt(); break;
                    case "limit_bspec_driver_count":
                        BSpecDriverCountMax = regulationNode.ReadValueInt(); break;
                    case "need_bspec_driver_count":
                        BSpecDriverCountMin = regulationNode.ReadValueInt(); break;

                    case "tuning":
                        Tuning = regulationNode.ReadValueBoolNull();
                        break;

                    case "NOS":
                        NOSNeeded = regulationNode.ReadValueBoolNull();
                        break;

                    case "tuners":
                        ParseRaceAllowedManufacturers(regulationNode);
                        break;
                }
            }
        }

        private void ParseRaceAllowedManufacturers(XmlNode node)
        {
            AllowedManufacturers = new List<(int, string)>();
            foreach (XmlNode manufacturerNode in node.SelectNodes("tuner"))
                AllowedManufacturers.Add((0, manufacturerNode.ReadValueString()));
        }

        private void ParseRaceAllowedVehicles(XmlNode node)
        {
            AllowedVehicles = new List<MCarThin>();
            foreach (XmlNode vehicleNode in node.SelectNodes("car"))
            {
                string label = vehicleNode.Attributes["label"].Value;
                AllowedVehicles.Add(new MCarThin(label));
            }
        }

        private void ParseRaceDisallowedVehicles(XmlNode node)
        {
            RestrictedVehicles = new List<MCarThin>();
            foreach (XmlNode vehicleNode in node.SelectNodes("car"))
            {
                string label = vehicleNode.Attributes["label"].Value;
                RestrictedVehicles.Add(new MCarThin(label));
            }
        }

        private void ParseAllowedCountries(XmlNode node)
        {
            AllowedCountries = new List<string>();
            foreach (XmlNode countryNode in node.SelectNodes("country"))
                AllowedCountries.Add(countryNode.ReadValueString());
        }

        private void ParseAllowedCategories(XmlNode node)
        {
            AllowedCategories = new List<CarCategoryRestriction>();
            foreach (XmlNode countryNode in node.SelectNodes("category"))
            {
                if (Enum.TryParse(countryNode.ReadValueString(), out CarCategoryRestriction category))
                    AllowedCategories.Add(category);
            }
        }

        public void ReadFromCache(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5E561AB && magic != 0xE6E661AB)
                throw new System.IO.InvalidDataException($"Regulation magic did not match - Got {magic.ToString("X8")}, expected 0xE6E661AB");

            uint regulationVersion = reader.ReadUInt32();
            PPMax = reader.ReadInt32();
            PPMin = reader.ReadInt32();
            TireCompoundMaxFront = (TireType)reader.ReadInt32();
            TireCompoundMinFront = (TireType)reader.ReadInt32();
            TireCompoundMaxRear = (TireType)reader.ReadInt32();
            TireCompoundMinRear = (TireType)reader.ReadInt32();

            int carCategoryCount = reader.ReadInt32();
            for (int i = 0; i < carCategoryCount; i++)
                AllowedCategories.Add((CarCategoryRestriction)reader.ReadInt32());

            int carCount = reader.ReadInt32();
            for (int i = 0; i < carCount; i++)
            {
                reader.ReadInt32();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt32();
            }

            NeedLicense = reader.ReadBool4();
            PowerMax = reader.ReadInt32();
            PowerMin = reader.ReadInt32();
            if (regulationVersion < 101)
            {
                reader.ReadInt16();
                reader.ReadInt16();
            }

            WeightMax = reader.ReadInt32();
            WeightMin = reader.ReadInt32();
            CarLengthMax = reader.ReadInt32();
            CarLengthMin = reader.ReadInt32();
            DrivetrainNeeded = (DrivetrainBits)reader.ReadInt32();
            AspirationNeeded = (AspirationBits)reader.ReadInt32();
            YearMax = reader.ReadInt32();
            YearMin = reader.ReadInt32();

            ASpecLevelMax = reader.ReadInt32();
            ASpecLevelMin = reader.ReadInt32();
            BSpecLevelMax = reader.ReadInt32();
            BSpecLevelMin = reader.ReadInt32();
            BSpecDriverCountMax = reader.ReadInt32();
            BSpecDriverCountMin = reader.ReadInt32();

            int tunersCount = reader.ReadInt32();
            for (int i = 0; i < tunersCount; i++)
                reader.ReadInt32();

            int countriesCount = reader.ReadInt32();
            for (int i = 0; i < countriesCount; i++)
                reader.ReadInt32();

            reader.ReadString4(); // need_entitlement
            Tuning = reader.ReadBool4();

            if (regulationVersion >= 101)
                NOSNeeded = reader.ReadBool4();

            if (regulationVersion >= 102)
                KartPermitted = reader.ReadBool4();

            int banCars = reader.ReadInt32();
            for (int i = 0; i < banCars; i++)
            {
                reader.ReadInt32();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt32();
            }

            reader.ReadInt32(); // car_tag_id
            reader.ReadInt32(); // restrictor_limit
        }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_61_AB);
            bs.WriteUInt32(1_03); // Version

            bs.WriteInt32(PPMax);
            bs.WriteInt32(PPMin);

            bs.WriteInt32((int)TireCompoundMaxFront);
            bs.WriteInt32((int)TireCompoundMinFront);
            bs.WriteInt32((int)TireCompoundMaxRear);
            bs.WriteInt32((int)TireCompoundMinRear);

            bs.WriteInt32(AllowedCategories.Count);
            foreach (var cat in AllowedCategories)
                bs.WriteInt32((int)cat);

            bs.WriteInt32(AllowedVehicles.Count);
            foreach (var car in AllowedVehicles)
                car.Serialize(ref bs);

            bs.WriteBool4OrNull(NeedLicense);
            bs.WriteInt32(PowerMax);
            bs.WriteInt32(PowerMin);
            bs.WriteInt32(WeightMax);
            bs.WriteInt32(WeightMin);
            bs.WriteInt32(CarLengthMax);
            bs.WriteInt32(CarLengthMin);
            bs.WriteInt32(DrivetrainNeeded == 0 ? -1 : (int)DrivetrainNeeded);
            bs.WriteInt32(AspirationNeeded == 0 ? -1 : (int)AspirationNeeded);
            bs.WriteInt32(YearMax);
            bs.WriteInt32(YearMin);
            bs.WriteInt32(ASpecLevelMax);
            bs.WriteInt32(ASpecLevelMin);
            bs.WriteInt32(BSpecLevelMax);
            bs.WriteInt32(BSpecLevelMin);
            bs.WriteInt32(BSpecDriverCountMax);
            bs.WriteInt32(BSpecDriverCountMin);

            bs.WriteInt32(AllowedManufacturers.Count);
            foreach (var tuner in AllowedManufacturers)
                bs.WriteInt32(tuner.Code);

            bs.WriteInt32(AllowedCountries.Count);
            foreach (var country in AllowedCountries)
            {
                if (Enum.TryParse(country, out Country countryEnum))
                    bs.WriteInt32((int)countryEnum);
            }

            bs.WriteNullStringAligned4(string.Empty); // Entitlement
            bs.WriteBool4OrNull(Tuning);
            bs.WriteBool4OrNull(NOSNeeded);
            bs.WriteBool4OrNull(KartPermitted);

            bs.WriteInt32(RestrictedVehicles.Count);
            foreach (var banCar in RestrictedVehicles)
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
        None = 0,
        NA = 0x02,
        Turbo = 0x04,
        Supercharger = 0x08,
    }


    [Flags]
    public enum DrivetrainBits
    {
        None = 0,

        FR = 0x02,
        FF = 0x04,
        AWD = 0x08,
        MR = 0x10,
        RR = 0x20,

        All = FR | FF | AWD | MR | RR,
    }

    public enum Country
    {
        PDI,
        JP = 2,
        US,
        GB,
        DE,
        FR,
        IT,
        AU,
        KR,
        BE,
        NL,
        SE,
        ES,
        CA,
        AT,
    }
}
