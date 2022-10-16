using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;

using PDTools.Utils;

namespace PDTools.Structures.PS3.MGameParameter
{
    public class EventConstraints
    {
        public bool NeedsPopulating { get; set; } = true;

        public bool ABSConstrained { get; set; }
        private bool? _absEnabled;
        public bool? ABSEnabled
        {
            get
            {
                if (ABSConstrained)
                    return _absEnabled ?? false;
                return null;
            }
            set
            {
                _absEnabled = value;
                ABSConstrained = _absEnabled.HasValue;
            }
        }

        public bool ActiveSteeringConstrained { get; set; }
        private bool? _activeSteeringEnabled;
        public bool? ActiveSteeringEnabled
        {
            get
            {
                if (ActiveSteeringConstrained)
                    return _activeSteeringEnabled ?? false;
                return null;
            }
            set
            {
                _activeSteeringEnabled = value;
                ActiveSteeringConstrained = _activeSteeringEnabled.HasValue;
            }
        }

        public bool ASMConstrained { get; set; }
        private bool? _asmEnabled;
        public bool? ASMEnabled
        {
            get
            {
                if (ASMConstrained)
                    return _asmEnabled ?? false;
                return null;
            }
            set
            {
                _asmEnabled = value;
                ASMConstrained = _asmEnabled.HasValue;
            }
        }

        public bool SkidRecoveryForceConstrained { get; set; }
        private bool? _skidRecoveryForceDisabled;
        public bool? SkidRecoveryForceDisabled
        {
            get
            {
                if (SkidRecoveryForceConstrained)
                    return _skidRecoveryForceDisabled ?? false;
                return null;
            }
            set
            {
                _skidRecoveryForceDisabled = value;
                SkidRecoveryForceConstrained = _skidRecoveryForceDisabled.HasValue;
            }
        }

        public int DriftType { get; set; }
        public int InCarView { get; set; }

        /// <summary>
        /// Tire used for ai pitting
        /// </summary>
        public TireType EnemyTire { get; set; } = TireType.NONE_SPECIFIED;

        public bool DrivingLineConstrained { get; set; }
        private bool? _drivingLineEnabled;
        public bool? DrivingLineEnabled
        {
            get
            {
                if (DrivingLineConstrained)
                    return _drivingLineEnabled ?? false;
                return null;
            }
            set
            {
                _drivingLineEnabled = value;
                DrivingLineConstrained = _drivingLineEnabled.HasValue;
            }
        }

        public TireType FrontTireLimit { get; set; } = TireType.NONE_SPECIFIED;
        public TireType RearTireLimit { get; set; } = TireType.NONE_SPECIFIED;
        public TireType NeededFrontTire { get; set; } = TireType.NONE_SPECIFIED;
        public TireType NeededRearTire { get; set; } = TireType.NONE_SPECIFIED;
        public TireType SuggestedRearTire { get; set; } = TireType.NONE_SPECIFIED;
        public TireType SuggestedFrontTire { get; set; } = TireType.NONE_SPECIFIED;

        public bool TCSConstrained { get; set; }
        private bool? _tcsEnabled;
        public bool? TCSEnabled
        {
            get
            {
                if (TCSConstrained)
                    return _tcsEnabled ?? false;
                return null;
            }
            set
            {
                _tcsEnabled = value;
                TCSConstrained = _tcsEnabled.HasValue;
            }
        }

        public bool TransmissionConstrained { get; set; }
        private bool? _transmissionEnabled;
        public bool? TransmissionEnabled
        {
            get
            {
                if (TransmissionConstrained)
                    return _transmissionEnabled ?? false;
                return null;
            }
            set
            {
                _transmissionEnabled = value;
                TransmissionConstrained = _transmissionEnabled.HasValue;
            }
        }

        public bool TuningConstrained { get; set; }
        private bool? _tuningEnabled;
        public bool? TuningEnabled
        {
            get
            {
                if (TuningConstrained)
                    return _tuningEnabled ?? false;
                return null;
            }
            set
            {
                _tuningEnabled = value;
                TuningConstrained = _tuningEnabled.HasValue;
            }
        }

        public bool SuggestedGearConstrained { get; set; }
        private bool? _suggestedGearEnabled;
        public bool? SuggestedGearEnabled
        {
            get
            {
                if (SuggestedGearConstrained)
                    return _suggestedGearEnabled ?? false;
                return null;
            }
            set
            {
                _suggestedGearEnabled = value;
                SuggestedGearConstrained = _suggestedGearEnabled.HasValue;
            }
        }

        public bool PowerLimitConstrained { get; set; }
        private float? _powerLimit;
        public float? PowerLimit
        {
            get => PowerLimitConstrained ? _powerLimit : null;
            set
            {
                _powerLimit = value;
                PowerLimitConstrained = _powerLimit.HasValue;
            }
        }

        public void ReadFromCache(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5E5F33D && magic != 0xE6E6F33D)
                throw new System.IO.InvalidDataException($"Constraint magic did not match - Got {magic.ToString("X8")}, expected 0xE6E6F33D");

            uint contraintVersion = reader.ReadUInt32();
            TransmissionEnabled = reader.ReadBool4();
            DrivingLineEnabled = reader.ReadBool4();
            ASMEnabled = reader.ReadBool4();
            TCSEnabled = reader.ReadBool4();
            reader.ReadBool4();
            ABSEnabled = reader.ReadBool4();
            FrontTireLimit = (TireType)reader.ReadInt32();
            NeededFrontTire = (TireType)reader.ReadInt32();
            SuggestedFrontTire = (TireType)reader.ReadInt32();
            RearTireLimit = (TireType)reader.ReadInt32();
            NeededRearTire = (TireType)reader.ReadInt32();
            SuggestedRearTire = (TireType)reader.ReadInt32();
            SkidRecoveryForceDisabled = reader.ReadBool4();
            ActiveSteeringEnabled = reader.ReadBool4();
            int cars = reader.ReadInt32();
            for (int i = 0; i < cars; i++)
            {
                // Car Thin
                reader.ReadInt32();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt32();
            }
            reader.ReadInt32(); // drift_type
            SuggestedGearEnabled = reader.ReadBool4();
            reader.ReadInt32(); // in_car_view
            EnemyTire = (TireType)reader.ReadInt32(); // enemy_tire

            if (contraintVersion >= 101)
                PowerLimit = reader.ReadSingle();
        }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_F3_3D);
            bs.WriteUInt32(1_01);
            bs.WriteBool4OrNull(TransmissionEnabled);
            bs.WriteBool4OrNull(DrivingLineEnabled);
            bs.WriteBool4OrNull(ASMEnabled);
            bs.WriteBool4OrNull(TCSEnabled);
            bs.WriteInt32(-1); // Unk - Unused
            bs.WriteBool4OrNull(ABSEnabled);
            bs.WriteInt32((int)FrontTireLimit);
            bs.WriteInt32((int)NeededFrontTire);
            bs.WriteInt32((int)SuggestedFrontTire);
            bs.WriteInt32((int)RearTireLimit);
            bs.WriteInt32((int)NeededRearTire);
            bs.WriteInt32((int)SuggestedRearTire);
            bs.WriteBool4OrNull(SkidRecoveryForceDisabled);
            bs.WriteBool4OrNull(ActiveSteeringEnabled);

            bs.WriteInt32(0); // Cars

            bs.WriteInt32(DriftType); // drift_type;
            bs.WriteBool4OrNull(SuggestedGearEnabled);
            bs.WriteInt32(-1); // in_car_view
            bs.WriteInt32((int)EnemyTire);
            bs.WriteInt32(PowerLimit is null ? -1 : (int)(PowerLimit.Value * 10));

        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("constraint");
            xml.WriteElementBoolOrNull("abs", ABSEnabled);
            xml.WriteElementBoolOrNull("active_steering", ActiveSteeringEnabled);
            xml.WriteElementBoolOrNull("asm", ASMEnabled);
            xml.WriteElementInt("drift_type", -1);
            xml.WriteElementBoolOrNull("driving_line", DrivingLineEnabled);
            xml.WriteElementEnumInt("enemy_tire", EnemyTire);
            xml.WriteElementEnumInt("limit_tire_f", FrontTireLimit);
            xml.WriteElementEnumInt("limit_tire_r", RearTireLimit);
            xml.WriteElementEnumInt("need_tire_f", NeededFrontTire);
            xml.WriteElementEnumInt("need_tire_r", NeededRearTire);

            if (PowerLimit.HasValue)
                xml.WriteElementFloat("restrictor_limit", (float)Math.Floor((double)(PowerLimit * 10f)));

            xml.WriteElementBoolOrNull("simulation", SkidRecoveryForceDisabled);
            xml.WriteElementEnumInt("suggest_tire_f", SuggestedFrontTire);
            xml.WriteElementEnumInt("suggest_tire_r", SuggestedRearTire);
            xml.WriteElementBoolIfSet("suggested_gear", SuggestedGearEnabled);
            xml.WriteElementBoolOrNull("tcs", TCSEnabled);
            xml.WriteElementBoolOrNull("transmission", TransmissionEnabled);
            xml.WriteElementBoolIfSet("tuning", TuningEnabled);
            xml.WriteEndElement();
        }

        public void ParseRaceConstraints(XmlNode node)
        {
            foreach (XmlNode constraintNode in node.ChildNodes)
            {
                switch (constraintNode.Name)
                {
                    case "abs":
                        ABSEnabled = constraintNode.ReadValueBoolNull();
                        break;
                    case "active_steering":
                        ActiveSteeringEnabled = constraintNode.ReadValueBoolNull();
                        break;
                    case "asm":
                        ASMEnabled = constraintNode.ReadValueBoolNull();
                        break;
                    case "drift_type":
                        DriftType = constraintNode.ReadValueInt();
                        break;
                    case "driving_line":
                        DrivingLineEnabled = constraintNode.ReadValueBoolNull();
                        break;
                    case "enemy_tire":
                        EnemyTire = constraintNode.ReadValueEnum<TireType>();
                        break;
                    case "limit_tire_f":
                        FrontTireLimit = constraintNode.ReadValueEnum<TireType>();
                        break;
                    case "limit_tire_r":
                        RearTireLimit = constraintNode.ReadValueEnum<TireType>();
                        break;
                    case "need_tire_f":
                        NeededFrontTire = constraintNode.ReadValueEnum<TireType>();
                        break;
                    case "need_tire_r":
                        NeededRearTire = constraintNode.ReadValueEnum<TireType>();
                        break;
                    case "restrictor_limit":
                        PowerLimit = constraintNode.ReadValueInt() / 10f;
                        break;
                    case "simulation":
                        SkidRecoveryForceDisabled = constraintNode.ReadValueBoolNull();
                        break;
                    case "suggest_tire_f":
                        SuggestedFrontTire = constraintNode.ReadValueEnum<TireType>();
                        break;
                    case "suggest_tire_r":
                        SuggestedRearTire = constraintNode.ReadValueEnum<TireType>();
                        break;
                    case "tcs":
                        TCSEnabled = constraintNode.ReadValueBoolNull();
                        break;
                    case "transmission":
                        TransmissionEnabled = constraintNode.ReadValueBoolNull();
                        break;
                    case "tuning":
                        TuningEnabled = constraintNode.ReadValueBoolNull();
                        break;

                }
            }
        }
    }

    public enum TireType
    {
        [Description("No restrictions")]
        NONE_SPECIFIED = -1,

        [Description("Comfort - Hard")]
        COMFORT_HARD,

        [Description("Comfort - Medium")]
        COMFORT_MEDIUM,

        [Description("Comfort - Soft")]
        COMFORT_SOFT,

        [Description("Sports - Hard")]
        SPORTS_HARD,

        [Description("Sports - Medium")]
        SPORTS_MEDIUM,

        [Description("Sports - Soft")]
        SPORTS_SOFT,

        [Description("Sports - Super Soft")]
        SPORTS_SUPER_SOFT,

        [Description("Racing - Hard")]
        RACING_HARD,

        [Description("Racing - Medium")]
        RACING_MEDIUM,

        [Description("Racing - Soft")]
        RACING_SOFT,

        [Description("Racing - Super Soft")]
        RACING_SUPER_SOFT,

        [Description("Racing - Rain Intermediate")]
        RAIN_INTERMEDIATE,

        [Description("Racing - Heavy Wet")]
        RAIN_HEAVY_WET,

        [Description("Dirt Tyres")]
        DIRT,

        [Description("Spiked Snow Tyres")]
        SPIKED_SNOW,
    }
}
