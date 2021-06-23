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
        public static Dictionary<CarPartsType, int> PartsTableToPurchaseBit = new Dictionary<CarPartsType, int>
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

        public const int Version = 109;

        private byte unk;
        public MCarCondition Condition { get; set; } = new MCarCondition();

        private short unk1;
        private short unk2;
        private short unk3;
        private uint _empty_;
        private ushort _empty2_;
        private short unk4;
        public ushort DealerColor;

        public PDIDATETIME32 ObtainDate { get; set; }
        public short WinCount { get; set; }
        public int GarageID { get; set; }
        public short RideCount { get; set; }
        public byte PackSmall { get; set; }

        public MCarSettings Settings { get; set; } = new MCarSettings();

        public bool IsHavingParts(CarPartsType table, int partIndex)
        {
            int bit = PartsTableToPurchaseBit[table] + partIndex;
            return Settings.GetPurchasedPartFromBitIndex(bit);
        }

        public void SetOwnParts(CarPartsType table, int partIndex)
        {
            if (table == CarPartsType.FRONT_TIRE || table == CarPartsType.REAR_TIRE)
            {
                int ftBit = PartsTableToPurchaseBit[CarPartsType.FRONT_TIRE] + partIndex;
                if (ftBit != 0)
                    Settings.SetPurchasedPartFromBitIndex(ftBit);

                // Trick to also let it apply to rear tires
                table = CarPartsType.REAR_TIRE;
            }

            int bit = PartsTableToPurchaseBit[table] + partIndex;
            if (bit != 0)
                Settings.SetPurchasedPartFromBitIndex(bit);
        }

        // Custom
        public void RemovePurchasedParts(CarPartsType table, int partIndex)
        {
            if (table == CarPartsType.FRONT_TIRE || table == CarPartsType.REAR_TIRE)
            {
                int ftBit = PartsTableToPurchaseBit[CarPartsType.FRONT_TIRE] + partIndex;
                if (ftBit != 0)
                    Settings.RemovePurchasedPartFromBitIndex(ftBit);

                // Trick to also let it apply to rear tires
                table = CarPartsType.REAR_TIRE;
            }

            int bit = PartsTableToPurchaseBit[table] + partIndex;
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

        public static MCarParameter ImportFromBlob(string fileName)
        {
            var car = ImportFromBlob(File.ReadAllBytes(fileName));
            return car;
        }

        public static MCarParameter ImportFromBlob(ReadOnlySpan<byte> blob)
        {
            var car = new MCarParameter();

            BitStream reader = new BitStream(/*blob*/ Span<byte>.Empty); // FIX ME
            reader.BufferByteSize = blob.Length;

            int version = reader.ReadInt32();
            //if (version != Version)
              //  throw new InvalidDataException("File is not an expected MCarParameter blob.");

            if (version == 110)
                reader.ReadUInt32(); // Header Size
            reader.ReadUInt32();
            car.Condition.ParseCondition(ref reader, version);

            if (version == 110)
            {
                car.GarageID = reader.ReadInt32();
                car.RideCount = reader.ReadInt16();
                car.WinCount = reader.ReadInt16();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadInt16();
                car.PackSmall = reader.ReadByte();
                reader.ReadByte();
                reader.ReadInt32();
                reader.ReadInt16();
                reader.ReadByte();
                reader.ReadByte();

                car.Settings.PurchaseBits = new byte[0x40];
                reader.ReadIntoByteArray(0x40, car.Settings.PurchaseBits, BitStream.Byte_Bits);
            }
            else
            {
                uint obtainDate = reader.ReadUInt32();
                short winCount = reader.ReadInt16();
                reader.ReadInt16();

                if (version >= 109)
                {
                    reader.ReadInt16();
                    reader.ReadInt16();
                }
            }

            car.Settings.ParseSettings(ref reader, version);

            return car;
        }

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

            if (carParamVersion != 110)
            {
                reader.ReadInt16();
                WheelDirtFront = reader.ReadByte();
                WheelDirtRear = reader.ReadByte();

                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                Scratch = reader.ReadInt32(); // Scratch
                reader.ReadInt32();

                // Grouped
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();

                reader.ReadInt16();
                reader.ReadInt16();
            }
        }
    }

    public class MCarSettings
    {
        public short FrontWheelEx { get; set; }
        public short RearWheelEx { get; set; }
        private short WheelInchupRelated { get; set; }
        public int WheelSP { get; set; } = -1;
        public int CarCode { get; set; }
        public int GarageID { get; set; }
        public int FrontWheelCompound { get; set; } = -1;
        public int FrontTire { get; set; } = -1;
        public int RearTireCompound { get; set; } = -1;
        public int RearTire { get; set; } = -1;
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
        public int[] UnkTables = new int[3];

        public short GearReverse { get; set; }
        public short Gear1st { get; set; }
        public short Gear2nd { get; set; }
        public short Gear3rd { get; set; }
        public short Gear4th { get; set; }
        public short Gear5th { get; set; }
        public short Gear6th { get; set; }
        public short Gear7th { get; set; }
        public short Gear8th { get; set; }
        public short Gear9th { get; set; }
        public short Gear10th { get; set; }
        public short Gear11st { get; set; }
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
        public sbyte FrontToe { get; set; }
        public sbyte RearToe { get; set; }
        public short FrontSpringRate { get; set; }
        public short RearSpringRate { get; set; }
        public short LeverRatioF { get; set; }
        public short LevelRarioR { get; set; }
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
        private short gasCapacity;
        public short PowerLimiter { get; set; } = 1000;

        public int HornSoundID { get; set; }
        private byte wheel_color;
        public short BodyPaintID { get; set; } = -1;
        public short WheelPaintID { get; set; } = -1;
        public short BrakePaintID { get; set; } = -1;
        public short CustomRearWingPaintID { get; set; } = -1;
        public short FrontWheelWidth { get; set; }
        public short FrontWheelDiameter { get; set; }
        public short RearWheelWidth { get; set; }
        public short RearWheelDiameter { get; set; }
        public byte WheelInchup { get; set; }
        public byte DeckenPreface { get; set; }
        public byte DeckenNumber { get; set; }
        public byte DeckenType { get; set; }
        private byte[] unk7 = new byte[21];

        private byte unkCustomWing1;
        private byte unkCustomWing2;
        private byte unkCustomWing3;
        private byte customWingsStays;
        private byte unkCustomWing4;
        public short WingWidthOffset { get; set; }
        public short WingHeightOffset { get; set; }
        public short WingAngleOffset { get; set; }
        private int unk8;

        private byte unk9;
        private byte unk10;
        private byte unk11;

        public short CustomMeterData { get; set; }
        private short CustomMeterUnk { get; set; }
        public uint CustomMeterColor { get; set; }

        private short unkToeAngle1;
        private short unkToeAngle2;

        public byte[] PurchaseBits { get; set; } = new byte[0x20];

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

        public int TestA { get; set; }
        public int TestB { get; set; }
        public int TestC { get; set; }
        public int TestD { get; set; }
        public int TestE { get; set; }
        public int TestF { get; set; }

        public void ParseSettings(ref BitStream reader, int carParamVersion)
        {
            if (carParamVersion >= 110)
            {
                short partsVersion = reader.ReadInt16();
                short partsSize = reader.ReadInt16();
                long unkBits = reader.ReadInt64();
                reader.ReadInt64();
                reader.ReadInt32();
                reader.ReadByte(); // color
                reader.ReadInt16();
                reader.ReadInt16();
                TestA = reader.ReadInt16();
                TestB = reader.ReadInt16();
                TestC = reader.ReadInt32();
                TestD = reader.ReadInt16();
                TestE = reader.ReadInt16();
                TestF = reader.ReadInt32();
            }
            else
            {
                FrontWheelEx = reader.ReadInt16();
                RearWheelEx = reader.ReadInt16();
                WheelInchupRelated = reader.ReadInt16();
                WheelSP = reader.ReadInt32();
                CarCode = reader.ReadInt32();
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

            if (carParamVersion < 106 || carParamVersion >= 110)
                reader.ReadInt32();

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
            reader.ReadInt32();
            reader.ReadInt32();

            GearReverse = reader.ReadInt16();
            Gear1st = reader.ReadInt16();
            Gear2nd = reader.ReadInt16();
            Gear3rd = reader.ReadInt16();
            Gear4th = reader.ReadInt16();
            Gear5th = reader.ReadInt16();
            Gear6th = reader.ReadInt16();
            Gear7th = reader.ReadInt16();
            Gear8th = reader.ReadInt16();
            Gear9th = reader.ReadInt16();
            Gear10th = reader.ReadInt16();
            Gear11st = reader.ReadInt16();

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
            FrontToe = reader.ReadSByte();
            RearToe = reader.ReadSByte();
            FrontSpringRate = carParamVersion >= 110 ? reader.ReadByte() : reader.ReadInt16();
            RearSpringRate = carParamVersion >= 110 ? reader.ReadByte() : reader.ReadInt16();
            LeverRatioF = carParamVersion >= 110 ? reader.ReadByte() : reader.ReadInt16();
            LevelRarioR = carParamVersion >= 110 ? reader.ReadByte() : reader.ReadInt16();

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

            if (carParamVersion >= 110)
            {
                reader.ReadByte();
                reader.ReadByte();
            }

            NOSTorqueVolume = reader.ReadByte();
            GripMultiplier = reader.ReadByte();
            FrontBrakeBalanceLevel = reader.ReadByte();
            RearBrakeBalanceLevel = reader.ReadByte();
            ABSCorneringControlLevel = reader.ReadByte();
            int unk = carParamVersion >= 110 ? reader.ReadInt16() : reader.ReadByte();
            int unk2 = carParamVersion >= 110 ? reader.ReadInt16() : reader.ReadByte();
            gasCapacity = carParamVersion >= 110 ? reader.ReadByte() : reader.ReadInt16();
            PowerLimiter = reader.ReadInt16();
            HornSoundID = reader.ReadInt32();

            if (carParamVersion < 110)
            {
                wheel_color = reader.ReadByte();
                BodyPaintID = reader.ReadInt16();
                WheelPaintID = reader.ReadInt16();
                BrakePaintID = reader.ReadInt16();
                CustomRearWingPaintID = reader.ReadInt16();
                FrontWheelWidth = reader.ReadInt16();
                FrontWheelDiameter = reader.ReadInt16();
                RearWheelWidth = reader.ReadInt16();
                RearWheelDiameter = reader.ReadInt16();
                reader.ReadInt16();
                WheelInchup = reader.ReadByte();
                DeckenPreface = reader.ReadByte();
                DeckenNumber = reader.ReadByte();
                DeckenType = reader.ReadByte();
                reader.ReadIntoByteArray(21, new byte[21], BitStream.Byte_Bits);
                unkCustomWing1 = reader.ReadByte();
                unkCustomWing2 = reader.ReadByte();
                unkCustomWing3 = reader.ReadByte();
                customWingsStays = reader.ReadByte();
                unkCustomWing4 = reader.ReadByte();
                WingWidthOffset = reader.ReadInt16();
                WingHeightOffset = reader.ReadInt16();
                WingAngleOffset = reader.ReadInt16();
                unk8 = reader.ReadInt32();

                unk9 = reader.ReadByte();
                unk10 = reader.ReadByte();
                unk11 = reader.ReadByte();

                CustomMeterData = reader.ReadInt16();
                CustomMeterUnk = reader.ReadInt16();
                CustomMeterColor = reader.ReadUInt32();
                unkToeAngle1 = reader.ReadInt16();
                unkToeAngle2 = reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadIntoByteArray(0x20, PurchaseBits, BitStream.Byte_Bits);
            }
        }

        public void WriteSettings(BinaryStream bs)
        {
            bs.WriteInt16(FrontWheelEx);
            bs.WriteInt16(RearWheelEx);
            bs.WriteInt16(WheelInchupRelated);
            bs.WriteInt32(WheelSP);
            bs.WriteInt32(CarCode);
            bs.WriteInt32(GarageID);
            bs.WriteInt32(FrontWheelCompound);
            bs.WriteInt32(FrontTire);
            bs.WriteInt32(RearTireCompound);
            bs.WriteInt32(RearTire);
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
            bs.WriteInt32s(UnkTables);

            bs.WriteInt16(GearReverse);
            bs.WriteInt16(Gear1st);
            bs.WriteInt16(Gear2nd);
            bs.WriteInt16(Gear3rd);
            bs.WriteInt16(Gear4th);
            bs.WriteInt16(Gear5th);
            bs.WriteInt16(Gear6th);
            bs.WriteInt16(Gear7th);
            bs.WriteInt16(Gear8th);
            bs.WriteInt16(Gear9th);
            bs.WriteInt16(Gear10th);
            bs.WriteInt16(Gear11st);

            bs.WriteInt16(FinalGearRatio);
            bs.WriteByte(MaxSpeed_10);
            bs.WriteInt16(GearLastFinal);
            bs.WriteByte(Param4WD);
            bs.WriteByte(FrontABS);
            bs.WriteByte(RearABS);

            bs.WriteInt16(DownforceFront);
            bs.WriteInt16(DownforceRear);

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
            bs.WriteSByte(FrontToe);
            bs.WriteSByte(RearToe);
            bs.WriteInt16(FrontSpringRate);
            bs.WriteInt16(RearSpringRate);
            bs.WriteInt16(LeverRatioF);
            bs.WriteInt16(LevelRarioR);

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
            bs.WriteByte(NOSTorqueVolume);
            bs.WriteByte(GripMultiplier);
            bs.WriteByte(FrontBrakeBalanceLevel);
            bs.WriteByte(RearBrakeBalanceLevel);
            bs.WriteByte(ABSCorneringControlLevel);
            bs.WriteByte(unk5);
            bs.WriteInt16(unk6);
            bs.WriteInt16(gasCapacity);
            bs.WriteInt16(PowerLimiter);
            bs.WriteInt32(HornSoundID);
            bs.WriteByte(wheel_color);
            bs.WriteInt16(BodyPaintID);
            bs.WriteInt16(WheelPaintID);
            bs.WriteInt16(BrakePaintID);
            bs.WriteInt16(CustomRearWingPaintID);
            bs.WriteInt16(FrontWheelWidth);
            bs.WriteInt16(FrontWheelDiameter);
            bs.WriteInt16(RearWheelWidth);
            bs.WriteInt16(RearWheelDiameter);
            bs.WriteInt16(0);
            bs.WriteByte(WheelInchup);
            bs.WriteByte(DeckenPreface);
            bs.WriteByte(DeckenNumber);
            bs.WriteByte(DeckenType);
            bs.Write(unk7, 0, unk7.Length);
            bs.WriteByte(unkCustomWing1);
            bs.WriteByte(unkCustomWing2);
            bs.WriteByte(unkCustomWing3);
            bs.WriteByte(customWingsStays);
            bs.WriteByte(unkCustomWing4);
            bs.WriteInt16(WingWidthOffset);
            bs.WriteInt16(WingHeightOffset);
            bs.WriteInt16(WingAngleOffset);
            bs.WriteInt32(unk8);

            bs.WriteByte(unk9);
            bs.WriteByte(unk10);
            bs.WriteByte(unk11);

            bs.WriteInt16(CustomMeterData);
            bs.WriteInt16(CustomMeterUnk);
            bs.WriteUInt32(CustomMeterColor);
            bs.WriteInt16(unkToeAngle1);
            bs.WriteInt16(unkToeAngle2);
            bs.WriteInt16(0);
            bs.Write(PurchaseBits, 0, PurchaseBits.Length);
        }
    }

    public enum PurchaseFlagsA : ulong
    {
        Brake_Stock = 0x02,
        Racing_Brake_Calipers = 0x04,

        Suspension_Stock = 0x01_00,
        Suspension_RacingSoft = 0x02_00,
        Suspension_RacingHard = 0x04_00,
        Suspension_Rally = 0x08_00,
        Suspension_Custom = 0x10_00,

        Weight_Stage1 = 0x08_00_00,
        Weight_Stage2 = 0x10_00_00,
        Weight_Stage3 = 0x20_00_00,
        Weight_Stage4 = 0x40_00_00,
        Weight_Stage5 = 0x80_00_00,

        Transmission_Stock = 0x40_00_00_00,
        Transmission_FiveSpeed = 0x80_00_00_00,
        Transmission_SixSpeed = 0x01_00_00_00_00,
        Transmission_Custom = 0x02_00_00_00_00,

        Engine_Stock = 0x04_00_00_00_00,
        Engine_Stage1 = 0x08_00_00_00_00,
        Engine_Stage2 = 0x10_00_00_00_00,
        Engine_Stage3 = 0x20_00_00_00_00,
        Engine_Stage4 = 0x40_00_00_00_00,
        Engine_Stage5 = 0x80_00_00_00_00,

        Turbo_Stock = 0x01_00_00_00_00_00,
        Turbo_Low = 0x02_00_00_00_00_00,
        Turbo_Mid = 0x04_00_00_00_00_00,
        Turbo_High = 0x08_00_00_00_00_00,
        Turbo_Super = 0x10_00_00_00_00_00,
        Turbo_Ultra = 0x20_00_00_00_00_00,

        Computer_Sports = 0x08_00_00_00_00_00_00,

        Exhaust_Sports = 0x08_00_00_00_00_00_00_00,
        Exhaust_SemiRacing = 0x10_00_00_00_00_00_00_00,
        Exhaust_Racing = 0x20_00_00_00_00_00_00_00,

        Clutch_SinglePlate = 0x40_00_00_00_00_00_00_00,
        Clutch_TwinPlate = 0x80_00_00_00_00_00_00_00,
    }

    public enum PurchaseFlagsB : ulong
    {
        Clutch_TriplePlate = 0x01,
        Clutch_Unk = 0x02,

        PropellerShaft_Carbon = 0x80,

        LSD_Stock = 0x01_00,
        LSD_Custom = 0x02_00,
        LSD_Custom2 = 0x04_00,
        FrontTire_ComfortHard = 0x80_00,
        FrontTire_ComfortMedium = 0x01_00_00,
        FrontTire_ComfortSoft = 0x02_00_00,
        FrontTire_SportsHard = 0x04_00_00,
        FrontTire_SportsMedium = 0x08_00_00,
        FrontTire_SportsSoft = 0x10_00_00,
        FrontTire_SportsSuperSoft = 0x20_00_00,
        FrontTire_RacingHard = 0x40_00_00,
        FrontTire_RacingMedium = 0x80_00_00,
        FrontTire_RacingSoft = 0x01_00_00_00,
        FrontTire_RacingSuperSoft = 0x02_00_00_00,
        FrontTire_Intermediate = 0x04_00_00_00,
        FrontTire_HeavyWet = 0x08_00_00_00,
        FrontTire_Dirt = 0x10_00_00_00,
        FrontTire_Snow = 0x20_00_00_00,
        RearTire_ComfortHard = 0x40_00_00_00,
        RearTire_ComfortMedium = 0x80_00_00_00,
        RearTire_ComfortSoft = 0x01_00_00_00_00,
        RearTire_SportsHard = 0x02_00_00_00_00,
        RearTire_SportsMedium = 0x04_00_00_00_00,
        RearTire_SportsSoft = 0x08_00_00_00_00,
        RearTire_SportsSuperSoft = 0x10_00_00_00_00,
        RearTire_RacingHard = 0x20_00_00_00_00,
        RearTire_RacingMedium = 0x40_00_00_00_00,
        RearTire_RacingSoft = 0x80_00_00_00_00,
        RearTire_RacingSuperSoft = 0x01_00_00_00_00_00,
        RearTire_Intermediate = 0x02_00_00_00_00_00,
        RearTire_HeavyWet = 0x04_00_00_00_00_00,
        RearTire_Dirt = 0x08_00_00_00_00_00,
        RearTire_Snow = 0x10_00_00_00_00_00,

        Supercharger = 0x40_00_00_00_00_00,

        IntakeManifold_Tuning = 0x01_00_00_00_00_00_00,
        ExhaustManifold_Isometric = 0x02_00_00_00_00_00_00,
    }
}