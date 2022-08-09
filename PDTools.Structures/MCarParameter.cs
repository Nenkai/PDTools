using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using PDTools.Utils;

namespace PDTools.Structures
{
    public class MCarParameter
    {
        public int ParameterVersion { get; set; }

        public MCarCondition Condition { get; set; } = new MCarCondition();

        /// <summary>
        /// GT5 Only
        /// </summary>
        public int MainHeaderSize { get; set; } = 0x3C;

        /// <summary>
        /// GT5 Only
        /// </summary>
        public byte Target { get; set; }

        /// <summary>
        /// GT5 Only
        /// </summary>
        public byte TeamId { get; set; }

        /// <summary>
        /// GT5 Only
        /// </summary>
        public byte RaceClassId { get; set; }

        /// <summary>
        /// GT5 Only
        /// </summary>
        public byte special_gas_ratio_100 { get; set; }

        /// <summary>
        /// GT5 Only
        /// </summary>
        public byte special_gas_liter { get; set; }

        /// <summary>
        /// GT5 Only
        /// </summary>
        public byte nos_ratio_100 { get; set; }

        /// <summary>
        /// GT5 Only
        /// </summary>
        public byte nos_duration_sec { get; set; }

        /// <summary>
        /// GT6 Only
        /// </summary>
        public PDIDATETIME32 ObtainDate { get; set; }
        public short WinCount { get; set; }
        public int GarageID { get; set; }
        public short RideCount { get; set; }

        /// <summary>
        /// GT5 Only
        /// </summary>
        public byte PackSmall { get; set; }

        public short ObtainID { get; set; }
        public short PowerScratch { get; set; }
        public short PPScratch { get; set; }

        public MCarSettings Settings { get; set; } = new MCarSettings();

        public static MCarParameter ImportFromBlob(string fileName)
        {
            return ImportFromBlob(File.ReadAllBytes(fileName));
        }

        public static MCarParameter ImportFromBlob(Span<byte> blob)
        {
            BitStream reader = new BitStream(BitStreamMode.Read, blob); // FIX ME
            reader.BufferByteSize = blob.Length;

            return ImportFromBlob(ref reader);
        }

        public static MCarParameter ImportFromBlob(ref BitStream reader)
        {
            int baseOffset = reader.Position;

            var car = new MCarParameter();
            car.ParameterVersion = reader.ReadInt32();

            if (car.ParameterVersion >= 110) // GT5
            {
                car.MainHeaderSize = reader.ReadInt32(); // Header Size
                byte unk = reader.ReadByte();
                car.Target = reader.ReadByte();
                car.TeamId = reader.ReadByte();
                car.RaceClassId = reader.ReadByte();
            }
            else
            {
                reader.ReadBits(1);
                reader.ReadBits(6);
                reader.ReadBits(6);
                reader.ReadBits(12);
                reader.ReadBits(1);
                reader.ReadBits(5);
                reader.ReadBits(1);
            }

            car.Condition.ParseCondition(ref reader, car.ParameterVersion);

            if (car.ParameterVersion >= 110) // GT5
            {
                car.GarageID = reader.ReadInt32(); // Upper bit means rentacar
                car.RideCount = reader.ReadInt16();
                car.WinCount = reader.ReadInt16();
                car.special_gas_ratio_100 = reader.ReadByte();
                car.special_gas_liter = reader.ReadByte();
                car.nos_ratio_100 = reader.ReadByte();
                car.nos_duration_sec = reader.ReadByte();
                reader.ReadInt16();
                car.PackSmall = reader.ReadByte();
                reader.ReadBits(1);
                reader.ReadBits(5);
                reader.ReadBits(2);
                reader.ReadBits(6);
                reader.ReadBits(13);
                reader.ReadBits(13);
                byte decken_type = reader.ReadByte();
                byte decken_number = reader.ReadByte();

                reader.Position = baseOffset + car.MainHeaderSize;
            }
            else
            {
                car.ObtainDate = new PDIDATETIME32(reader.ReadUInt32());
                car.WinCount = reader.ReadInt16();
                reader.ReadInt16();

                if (car.ParameterVersion >= 109)
                {
                    reader.ReadInt16();
                    reader.ReadInt16();
                }

                reader.Position = baseOffset + 0x50;
            }

            if (car.ParameterVersion >= 110) // GT5
            {
                car.Settings.PurchaseBits = new byte[0x40];
                reader.ReadIntoByteArray(0x40, car.Settings.PurchaseBits, BitStream.Byte_Bits);
            }

            car.Settings.ParseSettings(ref reader, car.ParameterVersion);

            if (car.ParameterVersion >= 1_10)
            {
                car.ObtainID = reader.ReadInt16();
                car.PowerScratch = reader.ReadInt16();
                car.PPScratch = reader.ReadInt16();
                reader.ReadInt16(); // Unknown
            }
            else
            {
                car.Settings.PurchaseBits = new byte[0x20];
                reader.ReadIntoByteArray(0x20, car.Settings.PurchaseBits, BitStream.Byte_Bits);
            }

            reader.Align(0x10);
            return car;
        }

        public bool IsHavingParts(CarPartsType table, int partIndex)
        {
            Dictionary<CarPartsType, int> purchaseBitTable = ParameterVersion >= 110 ? PartsTableToPurchaseBitGT5 : PartsTableToPurchaseBitGT6;
            int bit = purchaseBitTable[table] + partIndex;
            return Settings.GetPurchasedPartFromBitIndex(bit);

        }

        public void SetOwnParts(CarPartsType table, int partIndex)
        {
            Dictionary<CarPartsType, int> purchaseBitTable = ParameterVersion >= 110 ? PartsTableToPurchaseBitGT5 : PartsTableToPurchaseBitGT6;
            if (table == CarPartsType.FRONT_TIRE || table == CarPartsType.REAR_TIRE)
            {
                int ftBit = purchaseBitTable[CarPartsType.FRONT_TIRE] + partIndex;
                if (ftBit != 0)
                    Settings.SetPurchasedPartFromBitIndex(ftBit);

                // Trick to also let it apply to rear tires
                table = CarPartsType.REAR_TIRE;
            }

            int bit = purchaseBitTable[table] + partIndex;
            if (bit != 0)
                Settings.SetPurchasedPartFromBitIndex(bit);
        }

        // Custom
        public void RemovePurchasedParts(CarPartsType table, int partIndex)
        {
            Dictionary<CarPartsType, int> purchaseBitTable = ParameterVersion >= 110 ? PartsTableToPurchaseBitGT5 : PartsTableToPurchaseBitGT6;
            if (table == CarPartsType.FRONT_TIRE || table == CarPartsType.REAR_TIRE)
            {
                int ftBit = purchaseBitTable[CarPartsType.FRONT_TIRE] + partIndex;
                if (ftBit != 0)
                    Settings.RemovePurchasedPartFromBitIndex(ftBit);

                // Trick to also let it apply to rear tires
                table = CarPartsType.REAR_TIRE;
            }

            int bit = purchaseBitTable[table] + partIndex;
            if (bit != 0)
                Settings.RemovePurchasedPartFromBitIndex(bit);
        }

        // Custom
        public void TogglePurchasedPart(CarPartsType table, int partIndex, bool purchased)
        {
            if (purchased)
                SetOwnParts(table, partIndex);
            else
                RemovePurchasedParts(table, partIndex);
        }

        public static Dictionary<CarPartsType, int> PartsTableToPurchaseBitGT6 = new Dictionary<CarPartsType, int>
        {
            { CarPartsType.BRAKE, 1 },
            { CarPartsType.BRAKE_CONTROLLER, 6 },
            { CarPartsType.SUSPENSION, 8 },
            { CarPartsType.ASCC, 14 },
            { CarPartsType.TCSC, 16 },
            { CarPartsType.LIGHT_WEIGHT, 18 },
            { CarPartsType.DRIVETRAIN, 27 },
            { CarPartsType.GEAR, 30 },
            { CarPartsType.ENGINE, 0 },
            { CarPartsType.NATUNE, 34 },
            { CarPartsType.TURBINEKIT, 40 },
            { CarPartsType.DISPLACEMENT, 46 },
            { CarPartsType.COMPUTER, 50 },
            { CarPartsType.INTERCOOLER, 53 },
            { CarPartsType.MUFFLER, 58 },
            { CarPartsType.CLUTCH, 62 },
            { CarPartsType.FLYWHEEL, 66 },
            { CarPartsType.PROPELLERSHAFT, 70 },
            { CarPartsType.LSD, 72 },
            { CarPartsType.FRONT_TIRE, 79 },
            { CarPartsType.REAR_TIRE, 94 },
            { CarPartsType.SUPERCHARGER, 109 },
            { CarPartsType.INTAKE_MANIFOLD, 111 },
            { CarPartsType.EXHAUST_MANIFOLD, 113 },
            { CarPartsType.CATALYST, 115 },
            { CarPartsType.AIR_CLEANER, 118 },
            { CarPartsType.BOOST_CONTROLLER, 121 },
            { CarPartsType.INDEP_THROTTLE, 123 },
            { CarPartsType.LIGHT_WEIGHT_WINDOW, 125 },
            { CarPartsType.BONNET, 127 },
            { CarPartsType.AERO, 130 },
            { CarPartsType.FLAT_FLOOR, 134 },
            { CarPartsType.FREEDOM, 136 },
            { CarPartsType.WING, 140 },
            { CarPartsType.STIFFNESS, 145 },
            { CarPartsType.NOS, 147 },
        };

        public static Dictionary<CarPartsType, int> PartsTableToPurchaseBitGT5 = new Dictionary<CarPartsType, int>
        {
            { CarPartsType.BRAKE, 1 },
            { CarPartsType.BRAKE_CONTROLLER, 6 },
            { CarPartsType.SUSPENSION, 8 },
            { CarPartsType.ASCC, 14 },
            { CarPartsType.TCSC, 16 },
            { CarPartsType.LIGHT_WEIGHT, 18 },
            { CarPartsType.DRIVETRAIN, 27 },
            { CarPartsType.GEAR, 30 },
            { CarPartsType.ENGINE, 0 },
            { CarPartsType.NATUNE, 34 },
            { CarPartsType.TURBINEKIT, 40 },
            { CarPartsType.DISPLACEMENT, 46 },
            { CarPartsType.COMPUTER, 50 },
            { CarPartsType.INTERCOOLER, 53 },
            { CarPartsType.MUFFLER, 58 },
            { CarPartsType.CLUTCH, 62 },
            { CarPartsType.FLYWHEEL, 66 },
            { CarPartsType.PROPELLERSHAFT, 70 },
            { CarPartsType.LSD, 72 },
            { CarPartsType.FRONT_TIRE, 79 },
            { CarPartsType.REAR_TIRE, 124 }, // Different starting from here
            { CarPartsType.SUPERCHARGER, 169 },
            { CarPartsType.INTAKE_MANIFOLD, 171 },
            { CarPartsType.EXHAUST_MANIFOLD, 173 },
            { CarPartsType.CATALYST, 175 },
            { CarPartsType.AIR_CLEANER, 177 },
            { CarPartsType.BOOST_CONTROLLER, 179 },
            { CarPartsType.INDEP_THROTTLE, 181 },
            { CarPartsType.LIGHT_WEIGHT_WINDOW, 183 },
            { CarPartsType.BONNET, 185 },
            { CarPartsType.AERO, 188 },
            { CarPartsType.FLAT_FLOOR, 192 },
            { CarPartsType.FREEDOM, 196 },
            { CarPartsType.WING, 198 },
            { CarPartsType.STIFFNESS, 202 },
        };
    }

    public class MCarCondition
    {
        public uint Odometer { get; set; }
        public int EngineLife { get; set; }
        public float OilLife { get; set; }
        public uint BodyLife { get; set; }
        public byte Dirtiness100 { get; set; }
        public byte RainX { get; set; }
        public byte BodyCoating { get; set; }
        private byte unk2;
        public int Everlasting { get; set; }
        public byte WheelDirtFront { get; set; }
        public byte WheelDirtRear { get; set; }
        public int Scratch { get; set; }

        public void ParseCondition(ref BitStream reader, int carParamVersion)
        {
            Odometer = reader.ReadUInt32();
            EngineLife = reader.ReadInt32();
            OilLife = reader.ReadSingle();
            BodyLife = reader.ReadUInt32();
            Dirtiness100 = reader.ReadByte();
            RainX = reader.ReadByte();
            BodyCoating = reader.ReadByte();
            unk2 = reader.ReadByte();

            reader.ReadBits(1); //   *(ulonglong *)&(param_1->Meta).dirtiness = (uVar3 & 0x1) << 0x1f | *(ulonglong*)&(param_1->Meta).dirtiness & 0xffffffff7fffffff;
            reader.ReadBits(1); //   *(ulonglong*)&(param_1->Meta).dirtiness = (uVar3 & 0x1) << 0x1e | *(ulonglong*)&(param_1->Meta).dirtiness & 0xffffffffbfffffff;
            reader.ReadBits(30); //   *(ulonglong *)&(param_1->Meta).dirtiness = (uVar3 & 0x1) << 0x1e | *(ulonglong*)&(param_1->Meta).dirtiness & 0xffffffffbfffffff;

            if (carParamVersion >= 110)
                return; // GT5 Ends here

            reader.ReadInt16(); // 5 bits, 6 bits, 5 bits
            WheelDirtFront = reader.ReadByte();
            WheelDirtRear = reader.ReadByte();

            // All unknown
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt32();
            Scratch = reader.ReadInt32(); // Scratch
            reader.ReadInt32(); // Scratch related

            // Grouped
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();

            reader.ReadInt16();
            reader.ReadInt16();
        }
    }

    public class MCarSettings
    {
        public uint PartsVersion { get; set; }

        public short FrontWheelEx { get; set; }
        public short RearWheelEx { get; set; }
        private short WheelInchupRelated { get; set; }
        public int WheelSP { get; set; } = -1;
        public long NormalCarCode { get; set; } = -1;

        /// <summary>
        /// GT5 Only
        /// </summary>
        public long TunedCarCode { get; set; } = -1;

        /// <summary>
        /// GT5 Only
        /// </summary>
        public byte ColorIndex { get; set; }

        /// <summary>
        /// GT5 Only
        /// </summary>
        public int WheelFront { get; set; }

        /// <summary>
        /// GT5 Only
        /// </summary>
        public int WheelRear { get; set; }

        public int GarageID { get; set; }
        public int FrontWheelCompound { get; set; } = -1;
        public long FrontTire { get; set; } = -1;
        public int RearTireCompound { get; set; } = -1;
        public long RearTire { get; set; } = -1;
        public int Brake { get; set; } = -1;
        public int Brakecontroller { get; set; } = -1;
        public int Chassis { get; set; } = -1;
        public int Engine { get; set; } = -1;
        public int DriveTrain { get; set; } = -1;
        public int Gear { get; set; } = -1;
        public int Suspension { get; set; } = -1;
        public int LSD { get; set; } = -1;
        public int Steer { get; set; } = -1;
        public int Lightweight { get; set; } = -1;
        public int Racingmodify { get; set; } = -1;
        public int Displacement { get; set; } = -1;
        public int Computer { get; set; } = -1;
        public int Natune { get; set; } = -1;
        public int TurbineKit { get; set; } = -1;
        public int Flywheel { get; set; } = -1;
        public int Clutch { get; set; } = -1;
        public int PropellerShaft { get; set; } = -1;
        public int Muffler { get; set; } = -1;
        public int Intercooler { get; set; } = -1;
        public int ASCC { get; set; } = -1;
        public int TCSC { get; set; } = -1;
        public int Supercharger { get; set; } = -1;
        public int IntakeManifold { get; set; } = -1;
        public int ExhaustManifold { get; set; } = -1;
        public int Catalyst { get; set; } = -1;
        public int AirCleaner { get; set; } = -1;
        public int NOS { get; set; } = -1;
        public int WindowReduction { get; set; } = -1;
        public int CarbonBonnet { get; set; } = -1;
        public int BodyKit { get; set; } = -1;
        private int FlatFloors = -1;
        public int Aero { get; set; } = -1;
        public int Wing { get; set; } = -1;

        /* Important notes (from FUN_0049e0e4 in GT5 EU 2.11 EBOOT):
         * GENERIC_ITEMS hardcodes 'ItemType' to a car parameter field
         * Subtract 50 from type
         * 0 = NOS Field
         * 2 & 11 = Tire (second field, second long)
         * 3 = NOS/Nitro Kit Field, but last part value
         * 4 = Carbon Bonnet
         * 5 = AERO_01
         * 6 = AERO_02
         * 7 = WING
         * 8 = LIGHT_WEIGHT_WINDOW
         * 9 = STIFFNESS
         * 12 = AERO_03
         * default: tire stuff?
         * */

        /// <summary>
        /// GT5 Only - Will be hardcoded directly to type 3 (53) in GENERIC_ITEMS
        /// </summary>
        public int RigidityImprovement { get; private set; }

        /// <summary>
        /// GT5 Only
        /// </summary>
        public int NitroKit { get; private set; }

        /// <summary>
        /// Reverse then 1 to 11
        /// </summary>
        public short[] Gears { get; set; } = new short[12];
        public short FinalGearRatio { get; set; }
        public byte MaxSpeed_10 { get; set; }
        public short GearLastFinal { get; set; }
        public byte Param4WD { get; set; }
        public byte FrontABS { get; set; }
        public byte RearABS { get; set; }
        public short DownforceFront { get; set; }
        public short DownforceRear { get; set; }
        public byte turbo_Boost1 { get; set; }
        public byte turbo_peakRpm1 { get; set; }
        public byte turbo_response1 { get; set; }
        public byte turbo_Boost2 { get; set; }
        public byte turbo_peakRpm2 { get; set; }
        public byte turbo_response2 { get; set; }

        public byte FrontCamber { get; set; }
        public byte RearCamber { get; set; }
        public short FrontRideHeight { get; set; }
        public short RearRideHeight { get; set; }
        public short FrontToe { get; set; }
        public short RearToe { get; set; }
        public short FrontSpringRate { get; set; }
        public short RearSpringRate { get; set; }
        public short LeverRatioF { get; set; }
        public short LevelRatioR { get; set; }
        public byte FrontDamperF1B { get; set; }
        public byte FrontDamperF2B { get; set; }
        public byte FrontDamperF1R { get; set; }
        public byte FrontDamperF2R { get; set; }
        public byte RearDamperF1B { get; set; }
        public byte RearDamperF2B { get; set; }
        public byte RearDamperF1R { get; set; }
        public byte RearDamperF2R { get; set; }
        public byte FrontStabilizer { get; set; }
        public byte RearStabilizer { get; set; }
        public byte FrontLSDParam { get; set; }
        public byte RearLSDParam { get; set; }
        public byte FrontLSDParam2 { get; set; }
        public byte RearLSDParam2 { get; set; }
        public byte FrontLSDParam3 { get; set; }
        public byte RearLSDParam3 { get; set; }
        public byte TCSC_UserValueDF { get; set; }
        public byte ASCC_VSCParamLevel { get; set; }
        public byte ASCC_VSCParam1DF { get; set; }
        public byte ASCC_VUCParamLevel { get; set; }
        public byte ASCC_VUCParam11DF { get; set; }
        public byte BallastWeight { get; set; }
        public sbyte BallastPosition { get; set; }
        public byte SteerLimit { get; set; }
        private short unk3;
        public short WeightModifyRatio { get; set; } = 100;
        public short PowerModifyRatio { get; set; }
        public byte NOSTorqueVolume { get; set; }
        public byte GripMultiplier { get; set; }
        public byte FrontBrakeBalanceLevel { get; set; }
        public byte RearBrakeBalanceLevel { get; set; } = 5;
        public byte ABSCorneringControlLevel { get; set; } = 1;
        private byte unk5;
        private short unk6;
        private short GasCapacity;
        public short PowerLimiter { get; set; } = 1000;

        public int HornSoundID { get; set; }
        private byte wheel_color;
        public short BodyPaintID { get; set; } = -1;
        public short WheelPaintID { get; set; } = -1;
        public short BrakeCaliperPaintID { get; set; } = -1;
        public short CustomRearWingPaintID { get; set; } = -1;
        public short FrontWheelWidth { get; set; }
        public short FrontWheelDiameter { get; set; }
        public short RearWheelWidth { get; set; }
        public short RearWheelDiameter { get; set; }
        public byte WheelInchup { get; set; }
        public byte DeckenPreface { get; set; }
        public byte DeckenNumber { get; set; }
        public byte DeckenType { get; set; }

        public short WingWidthOffset { get; set; }
        public short WingHeightOffset { get; set; }
        public short WingAngleOffset { get; set; }

        public short CustomMeterData { get; set; }
        public uint CustomMeterColor { get; set; }

        public byte[] PurchaseBits { get; set; }
        public int VariationID { get; private set; }

        public bool GetPurchasedPartFromBitIndex(int bitIndex)
        {
            byte byteLocation = PurchaseBits[bitIndex / 8];
            return ((byteLocation >> (bitIndex % 8)) & 1) == 1;
        }

        public void SetPurchasedPartFromBitIndex(int purchaseBitIndex)
        {
            ref byte byteLocation = ref PurchaseBits[purchaseBitIndex / 8];
            int bitIndex = purchaseBitIndex % 8;
            byteLocation = (byte)((1 << bitIndex) | byteLocation);
        }

        public void RemovePurchasedPartFromBitIndex(int purchaseBitIndex)
        {
            ref byte byteLocation = ref PurchaseBits[purchaseBitIndex / 8];
            int bitIndex = purchaseBitIndex % 8;
            byteLocation &= (byte)~(1 << bitIndex);
        }

        public void ParseSettings(ref BitStream reader, int carParamVersion)
        {
            if (carParamVersion >= 110)
            {
                PartsVersion = (uint)reader.ReadInt16();
                reader.ReadInt16(); // Parts size
                NormalCarCode = reader.ReadInt64();
                TunedCarCode = reader.ReadInt64();
                ColorIndex = reader.ReadByte();
                WheelFront = reader.ReadInt32();
                WheelRear = reader.ReadInt32();
                FrontTire = reader.ReadInt64();
                RearTire = reader.ReadInt64();
            }
            else
            {
                PartsVersion = reader.ReadUInt32();
                FrontWheelEx = reader.ReadInt16();
                RearWheelEx = reader.ReadInt16();
                WheelInchupRelated = (short)reader.ReadInt32();
                WheelSP = reader.ReadInt32();
                NormalCarCode = reader.ReadInt32();
                GarageID = reader.ReadInt32();
                FrontWheelCompound = reader.ReadInt32();
                FrontTire = reader.ReadInt32();
                RearTireCompound = reader.ReadInt32();
                RearTire = reader.ReadInt32();
            }

            Brake = reader.ReadInt32();
            Brakecontroller = reader.ReadInt32();
            Chassis = reader.ReadInt32();
            Engine = reader.ReadInt32();
            DriveTrain = reader.ReadInt32();
            Gear = reader.ReadInt32();
            Suspension = reader.ReadInt32();
            LSD = reader.ReadInt32();
            Steer = reader.ReadInt32();
            Lightweight = reader.ReadInt32();
            Racingmodify = reader.ReadInt32();
            Displacement = reader.ReadInt32();
            Computer = reader.ReadInt32();
            Natune = reader.ReadInt32();
            TurbineKit = reader.ReadInt32();
            Flywheel = reader.ReadInt32();
            Clutch = reader.ReadInt32();
            PropellerShaft = reader.ReadInt32();
            Muffler = reader.ReadInt32();
            Intercooler = reader.ReadInt32();
            ASCC = reader.ReadInt32();
            TCSC = reader.ReadInt32();

            if ((carParamVersion >= 110 && PartsVersion >= 0x103) // GT5
                || PartsVersion < 106) // GT6
            {
                VariationID = reader.ReadInt32();
            }

            Supercharger = reader.ReadInt32();
            IntakeManifold = reader.ReadInt32();
            ExhaustManifold = reader.ReadInt32();
            Catalyst = reader.ReadInt32();
            AirCleaner = reader.ReadInt32();
            NOS = reader.ReadInt32();
            WindowReduction = reader.ReadInt32();
            CarbonBonnet = reader.ReadInt32();
            BodyKit = reader.ReadInt32();
            FlatFloors = reader.ReadInt32();
            Aero = reader.ReadInt32();
            Wing = reader.ReadInt32();
            reader.ReadInt32();
            RigidityImprovement = reader.ReadInt32();
            NitroKit = reader.ReadInt32();

            reader.ReadIntoShortArray(12, Gears, 0x10);
            FinalGearRatio = reader.ReadInt16();
            MaxSpeed_10 = reader.ReadByte();
            GearLastFinal = reader.ReadInt16();

            Param4WD = reader.ReadByte();
            FrontABS = reader.ReadByte();
            RearABS = reader.ReadByte();
            DownforceFront = carParamVersion >= 110 ? reader.ReadByte() : reader.ReadInt16();
            DownforceRear = carParamVersion >= 110 ? reader.ReadByte() : reader.ReadInt16();

            turbo_Boost1 = reader.ReadByte();
            turbo_peakRpm1 = reader.ReadByte();
            turbo_response1 = reader.ReadByte();
            turbo_Boost2 = reader.ReadByte();
            turbo_peakRpm2 = reader.ReadByte();
            turbo_response2 = reader.ReadByte();

            FrontCamber = reader.ReadByte();
            RearCamber = reader.ReadByte();
            FrontRideHeight = reader.ReadInt16();
            RearRideHeight = reader.ReadInt16();
            FrontToe = (short)reader.ReadSByte();
            RearToe = (short)reader.ReadSByte();
            FrontSpringRate = PartsVersion >= 110 ? reader.ReadByte() : reader.ReadInt16();
            RearSpringRate = PartsVersion >= 110 ? reader.ReadByte() : reader.ReadInt16();
            LeverRatioF = PartsVersion >= 110 ? reader.ReadByte() : reader.ReadInt16();
            LevelRatioR = PartsVersion >= 110 ? reader.ReadByte() : reader.ReadInt16();

            FrontDamperF1B = reader.ReadByte();
            FrontDamperF2B = reader.ReadByte();
            FrontDamperF1R = reader.ReadByte();
            FrontDamperF2R = reader.ReadByte();
            RearDamperF1B = reader.ReadByte();
            RearDamperF2B = reader.ReadByte();
            RearDamperF1R = reader.ReadByte();
            RearDamperF2R = reader.ReadByte();

            FrontStabilizer = reader.ReadByte();
            RearStabilizer = reader.ReadByte();
            FrontLSDParam = reader.ReadByte();
            RearLSDParam = reader.ReadByte();
            FrontLSDParam2 = reader.ReadByte();
            RearLSDParam2 = reader.ReadByte();
            FrontLSDParam3 = reader.ReadByte();
            RearLSDParam3 = reader.ReadByte();
            TCSC_UserValueDF = reader.ReadByte();
            ASCC_VSCParamLevel = reader.ReadByte();
            ASCC_VSCParam1DF = reader.ReadByte();
            ASCC_VUCParamLevel = reader.ReadByte();
            ASCC_VUCParam11DF = reader.ReadByte();
            BallastWeight = reader.ReadByte();
            BallastPosition = reader.ReadSByte();
            SteerLimit = reader.ReadByte();
            unk3 = reader.ReadInt16();
            WeightModifyRatio = reader.ReadInt16();
            PowerModifyRatio = reader.ReadInt16();

            if (carParamVersion >= 1_10)
            {
                reader.ReadByte();
                reader.ReadByte();
            }

            NOSTorqueVolume = reader.ReadByte();
            GripMultiplier = reader.ReadByte();
            FrontBrakeBalanceLevel = reader.ReadByte();
            RearBrakeBalanceLevel = reader.ReadByte();
            ABSCorneringControlLevel = reader.ReadByte();
            int unk = carParamVersion >= 1_10 ? reader.ReadInt16() : reader.ReadByte();
            reader.ReadInt16();
            GasCapacity = carParamVersion >= 1_10 ? reader.ReadByte() : reader.ReadInt16();

            if (carParamVersion >= 1_10 && PartsVersion >= 0x104) // GT5
                PowerLimiter = reader.ReadInt16();
            else if (carParamVersion < 1_09) // GT6
                PowerLimiter = reader.ReadInt16();

            HornSoundID = reader.ReadInt32();

            if (carParamVersion >= 1_10)
            {
                // GT5 ends here
                reader.Align(0x10);
                return;
            }

            wheel_color = reader.ReadByte();
            BodyPaintID = reader.ReadInt16();
            WheelPaintID = reader.ReadInt16();
            BrakeCaliperPaintID = reader.ReadInt16();

            if (PartsVersion >= 1_17)
                CustomRearWingPaintID = reader.ReadInt16();

            FrontWheelWidth = reader.ReadInt16();
            FrontWheelDiameter = reader.ReadInt16();
            RearWheelWidth = reader.ReadInt16();
            RearWheelDiameter = reader.ReadInt16();
            WheelInchup = reader.ReadByte();
            DeckenPreface = reader.ReadByte();

            if (PartsVersion < 1_16)
            {
                DeckenNumber = reader.ReadByte();
                DeckenType = reader.ReadByte();
                reader.ReadByte(); // window_sticker_custom_type
                reader.ReadInt64();
                reader.ReadInt64(); // Decken custom ID
                reader.ReadByte(); // Wing customized
            }
            else
            {
                DeckenNumber = (byte)reader.ReadBits(2);
                DeckenType = (byte)reader.ReadBits(2);
                reader.ReadBits(2); // window_sticker_custom_type
                reader.ReadInt64();
                reader.ReadInt64(); // Decken custom ID
                reader.ReadBoolBit(); // Wing customized
            }

            reader.ReadByte(); // Wing Flap Type
            reader.ReadByte(); // Wing End Plate Type;
            reader.ReadByte(); // Wing Stay Type
            reader.ReadByte(); // Wing Mount Type
            WingWidthOffset = reader.ReadInt16();
            WingHeightOffset = reader.ReadInt16();
            WingAngleOffset = reader.ReadInt16();
            reader.ReadInt16();
            reader.ReadInt16();

            // Wing bits
            reader.ReadBits(3);
            reader.ReadBoolBit();
            reader.ReadBits(4);
            reader.ReadBits(4);
            reader.ReadBits(2);
            reader.ReadBits(2);
            reader.ReadBits(3);
            reader.ReadBits(2);
            reader.ReadBoolBit();
            reader.ReadBits(2);

            // Custom Meter stuff
            if (PartsVersion == 1_15)
            {
                reader.ReadByte();
                reader.ReadByte();

                reader.ReadInt16(); // Extra Meter Count
                reader.ReadInt16();
                reader.ReadInt16();

                // Backlight Color ARGB
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
            }
            else
            {
                reader.ReadBits(2);
                reader.ReadBits(2);

                reader.ReadBits(10); // Extra Meter Count
                reader.ReadBits(10);
                reader.ReadBits(10);

                // Backlight Color ARGB
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
            }

            if (PartsVersion >= 1_18)
            {
                FrontToe = (sbyte)reader.ReadInt16();
                RearToe = (sbyte)reader.ReadInt16();
            }
        }

        public void WriteSettings(ref BitStream bs, int carParamVersion)
        {
            int baseOffset = bs.Position;
            if (carParamVersion >= 110)
            {
                bs.WriteInt16((short)PartsVersion);
                bs.WriteInt32(0x140); // Parts Size
                bs.WriteInt64(NormalCarCode);
                bs.WriteInt64(TunedCarCode);
                bs.WriteByte(ColorIndex);
                bs.WriteInt32(WheelFront);
                bs.WriteInt32(WheelRear);
                bs.WriteInt64(FrontTire);
                bs.WriteInt64(RearTire);
            }
            else
            {
                bs.WriteUInt32(PartsVersion);
                bs.WriteInt16(FrontWheelEx);
                bs.WriteInt16(RearWheelEx);
                bs.WriteInt16(WheelInchupRelated);
                bs.WriteInt32(WheelSP);
                bs.WriteInt32((int)NormalCarCode);
                bs.WriteInt32(GarageID);
                bs.WriteInt32(FrontWheelCompound);
                bs.WriteInt32((int)FrontTire);
                bs.WriteInt32(RearTireCompound);
                bs.WriteInt32((int)RearTire);
            }

            bs.WriteInt32(Brake);
            bs.WriteInt32(Brakecontroller);
            bs.WriteInt32(Chassis);
            bs.WriteInt32(Engine);
            bs.WriteInt32(DriveTrain);
            bs.WriteInt32(Gear);
            bs.WriteInt32(Suspension);
            bs.WriteInt32(LSD);
            bs.WriteInt32(Steer);
            bs.WriteInt32(Lightweight);
            bs.WriteInt32(Racingmodify);
            bs.WriteInt32(Displacement);
            bs.WriteInt32(Computer);
            bs.WriteInt32(Natune);
            bs.WriteInt32(TurbineKit);
            bs.WriteInt32(Flywheel);
            bs.WriteInt32(Clutch);
            bs.WriteInt32(PropellerShaft);
            bs.WriteInt32(Muffler);
            bs.WriteInt32(Intercooler);
            bs.WriteInt32(ASCC);
            bs.WriteInt32(TCSC);

            if ((carParamVersion >= 110 && PartsVersion >= 0x103)
                || PartsVersion < 106)
            {
                bs.WriteInt32((int)VariationID); 
            }

            bs.WriteInt32(Supercharger);
            bs.WriteInt32(IntakeManifold);
            bs.WriteInt32(ExhaustManifold);
            bs.WriteInt32(Catalyst);
            bs.WriteInt32(AirCleaner);
            bs.WriteInt32(NOS);
            bs.WriteInt32(WindowReduction);
            bs.WriteInt32(CarbonBonnet);
            bs.WriteInt32(BodyKit);
            bs.WriteInt32(FlatFloors);
            bs.WriteInt32(Aero);
            bs.WriteInt32(Wing);
            bs.WriteInt32(-1);
            bs.WriteInt32(RigidityImprovement);
            bs.WriteInt32(NitroKit);

            for (var i = 0; i < Gears.Length; i++)
                bs.WriteInt16(Gears[i]);

            bs.WriteInt16(FinalGearRatio);
            bs.WriteByte(MaxSpeed_10);
            bs.WriteInt16(GearLastFinal);
            bs.WriteByte(Param4WD);
            bs.WriteByte(FrontABS);
            bs.WriteByte(RearABS);

            if (carParamVersion >= 110)
            {
                bs.WriteByte((byte)DownforceFront);
                bs.WriteByte((byte)DownforceRear);
            }
            else
            {
                bs.WriteInt16(DownforceFront);
                bs.WriteInt16(DownforceRear);
            }

            bs.WriteByte(turbo_Boost1);
            bs.WriteByte(turbo_peakRpm1);
            bs.WriteByte(turbo_response1);
            bs.WriteByte(turbo_Boost2);
            bs.WriteByte(turbo_peakRpm2);
            bs.WriteByte(turbo_response2);

            bs.WriteByte(FrontCamber);
            bs.WriteByte(RearCamber);

            bs.WriteInt16(FrontRideHeight);
            bs.WriteInt16(RearRideHeight);
            bs.WriteSByte((sbyte)FrontToe);
            bs.WriteSByte((sbyte)RearToe);

            if (carParamVersion >= 110)
            {
                bs.WriteByte((byte)FrontSpringRate);
                bs.WriteByte((byte)RearSpringRate);
                bs.WriteByte((byte)LeverRatioF);
                bs.WriteByte((byte)LevelRatioR);
            }
            else
            {
                bs.WriteInt16(FrontSpringRate);
                bs.WriteInt16(RearSpringRate);
                bs.WriteInt16(LeverRatioF);
                bs.WriteInt16(LevelRatioR);
            }

            bs.WriteByte(FrontDamperF1B);
            bs.WriteByte(FrontDamperF2B);
            bs.WriteByte(FrontDamperF1R);
            bs.WriteByte(FrontDamperF2R);
            bs.WriteByte(RearDamperF1B);
            bs.WriteByte(RearDamperF2B);
            bs.WriteByte(RearDamperF1R);
            bs.WriteByte(RearDamperF2R);

            bs.WriteByte(FrontStabilizer);
            bs.WriteByte(RearStabilizer);
            bs.WriteByte(FrontLSDParam);
            bs.WriteByte(RearLSDParam);
            bs.WriteByte(FrontLSDParam2);
            bs.WriteByte(RearLSDParam2);
            bs.WriteByte(FrontLSDParam3);
            bs.WriteByte(RearLSDParam3);
            bs.WriteByte(TCSC_UserValueDF);
            bs.WriteByte(ASCC_VSCParamLevel);
            bs.WriteByte(ASCC_VSCParam1DF);
            bs.WriteByte(ASCC_VUCParamLevel);
            bs.WriteByte(ASCC_VUCParam11DF);
            bs.WriteByte(BallastWeight);
            bs.WriteSByte(BallastPosition);
            bs.WriteByte(SteerLimit);
            bs.WriteInt16(unk3);
            bs.WriteInt16(WeightModifyRatio);
            bs.WriteInt16(PowerModifyRatio);

            if (carParamVersion >= 1_10)
            {
                bs.WriteByte(1);
                bs.WriteByte(0);
            }

            bs.WriteByte(NOSTorqueVolume);
            bs.WriteByte(GripMultiplier);
            bs.WriteByte(FrontBrakeBalanceLevel);
            bs.WriteByte(RearBrakeBalanceLevel);
            bs.WriteByte(ABSCorneringControlLevel);

            if (carParamVersion >= 1_10)
                bs.WriteInt16(0);
            else
                bs.WriteByte(0);
            bs.WriteInt16(0);

            if (carParamVersion >= 1_10)
                bs.WriteByte((byte)GasCapacity);
            else
                bs.WriteInt16(GasCapacity);

            if (carParamVersion >= 1_10 && PartsVersion >= 0x104)
                bs.WriteInt16(PowerLimiter);
            else if (carParamVersion < 1_09)
                bs.WriteInt16(PowerLimiter);

            bs.WriteInt32(HornSoundID);

            if (carParamVersion >= 1_10)
            {
                bs.Position = baseOffset + 0x140;
                return;
            }


            bs.WriteByte(wheel_color);
            bs.WriteInt16(BodyPaintID);
            bs.WriteInt16(WheelPaintID);
            bs.WriteInt16(BrakeCaliperPaintID);

            if (PartsVersion >= 1_17)
                bs.WriteInt16(CustomRearWingPaintID);

            bs.WriteInt16(FrontWheelWidth);
            bs.WriteInt16(FrontWheelDiameter);
            bs.WriteInt16(RearWheelWidth);
            bs.WriteInt16(RearWheelDiameter);
            bs.WriteByte(WheelInchup);
            bs.WriteByte(DeckenPreface);

            if (PartsVersion < 1_16)
            {
                bs.WriteByte(DeckenNumber);
                bs.WriteByte(DeckenType);
                bs.WriteByte(0);
                bs.WriteInt64(0);
                bs.WriteInt64(0);
                bs.WriteByte(0);
            }
            else
            {
                bs.WriteBits(DeckenNumber, 2);
                bs.WriteBits(DeckenType, 2);
                bs.WriteBits(0, 2);
                bs.WriteInt64(0);
                bs.WriteInt64(0);
                bs.WriteBoolBit(false);
            }

            bs.WriteByte(0); // Wing Flap Type
            bs.WriteByte(0); // Wing End Plate Type
            bs.WriteByte(0); // Wing Stay Type
            bs.WriteByte(0); // Wing Mount Type

            // TODO
            bs.WriteBits(0, 3);
            bs.WriteBits(0, 1);
            bs.WriteBits(0, 4);
            bs.WriteBits(0, 4);
            bs.WriteBits(0, 2);
            bs.WriteBits(0, 2);
            bs.WriteBits(0, 3);
            bs.WriteBits(0, 2);
            bs.WriteBits(0, 1);
            bs.WriteBits(0, 2);

            if (PartsVersion == 1_15)
            {
                bs.WriteByte(0);
                bs.WriteByte(0);

                bs.WriteInt16(0);
                bs.WriteInt16(0);
                bs.WriteInt16(0);

                bs.WriteInt16(0);
                bs.WriteInt16(0);
                bs.WriteInt16(0);
                bs.WriteInt16(0);
            }
            else
            {
                bs.WriteBits(0, 2);
                bs.WriteBits(0, 2);

                bs.WriteBits(0, 10);
                bs.WriteBits(0, 10);
                bs.WriteBits(0, 10);

                bs.WriteByte(0);
                bs.WriteByte(0);
                bs.WriteByte(0);
                bs.WriteByte(0);
            }

            if (PartsVersion >= 1_18)
            {
                bs.WriteInt16(FrontToe);
                bs.WriteInt16(RearToe);
            }

            bs.Align(0x10);
        }
    }
}