using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using Syroot.BinaryData.Memory;
using System.Net.NetworkInformation;

namespace PDTools.Structures.PS2
{
    public class CarEquipments
    {
        public DbCode CarCode { get; set; }
        public DbCode TunedCarCode { get; set; }
        public DbCode Variation { get; set; }
        public byte VariationOrder { get; set; }
        public DbCode Brake { get; set; }
        public DbCode BrakeController { get; set; }
        public DbCode Chassis { get; set; }
        public DbCode Engine { get; set; }
        public DbCode Drivetrain { get; set; }
        public DbCode Gear { get; set; }
        public DbCode Suspension { get; set; }
        public DbCode LSD { get; set; }
        public DbCode FrontTire { get; set; }
        public DbCode RearTire { get; set; }
        public DbCode Steer { get; set; }
        public DbCode Lightweight { get; set; }
        public DbCode RacingModify { get; set; }
        public DbCode Portpolish { get; set; }
        public DbCode EngineBalance { get; set; }
        public DbCode Displacement { get; set; }
        public DbCode Computer { get; set; }
        public DbCode Natune { get; set; }
        public DbCode TurbineKit { get; set; }
        public DbCode Flywheel { get; set; }
        public DbCode Clutch { get; set; }
        public DbCode PropellerShaft { get; set; }
        public DbCode Muffler { get; set; }
        public DbCode Intercooler { get; set; }
        public DbCode ASCC { get; set; }
        public DbCode TCSC { get; set; }
        public DbCode Wheel { get; set; }
        public DbCode NOS { get; set; }
        public DbCode Wing { get; set; }
        public DbCode SuperCharger { get; set; }
        public ushort GearReverse { get; set; }
        public ushort GearRatio1 { get; set; }
        public ushort GearRatio2 { get; set; }
        public ushort GearRatio3 { get; set; }
        public ushort GearRatio4 { get; set; }
        public ushort GearRatio5 { get; set; }
        public ushort GearRatio6 { get; set; }
        public ushort GearRatio7 { get; set; }
        public ushort GearRatio8 { get; set; }
        public ushort GearRatio9 { get; set; }
        public ushort GearRatio10 { get; set; }
        public ushort GearRatio11 { get; set; }
        public ushort FinalGearRatio { get; set; }
        public ushort MaxSpeed { get; set; }
        public ushort LastFinalGearRatio { get; set; }
        public byte Param4WD { get; set; }
        public byte ABSLevelF { get; set; }
        public byte ABSLevelR { get; set; }
        public byte SettingClF { get; set; }
        public byte SettingClR { get; set; }
        public byte Turbo_Boost1 { get; set; }
        public byte Turbo_PeakRPM1 { get; set; }
        public byte Turbo_Response1 { get; set; }
        public byte Turbo_Boost2 { get; set; }
        public byte Turbo_PeakRPM2 { get; set; }
        public byte Turbo_Response2 { get; set; }
        public byte Susp_FrontCamber { get; set; }
        public byte Susp_RearCamber { get; set; }
        public byte field_138 { get; set; }
        public short Susp_RideHeightF { get; set; }
        public short Susp_RideHeightR { get; set; }
        public byte Susp_FrontToe { get; set; }
        public byte Susp_RearToe { get; set; }
        public byte Susp_FrontSpringRate { get; set; }
        public byte Susp_RearSpringRate { get; set; }
        public byte leverRatioF { get; set; }
        public byte leverRatioR { get; set; }
        public byte Susp_FrontDamperF1B { get; set; }
        public byte Susp_FrontDamperF2B { get; set; }
        public byte Susp_FrontDamperF1R { get; set; }
        public byte Susp_FrontDamperF2R { get; set; }
        public byte Susp_RearDamperF1B { get; set; }
        public byte Susp_RearDamperF2B { get; set; }
        public byte Susp_RearDamperF1R { get; set; }
        public byte Susp_RearDamperF2R { get; set; }
        public byte Susp_FrontStabilizer { get; set; }
        public byte Susp_RearStabilizer { get; set; }
        public byte LSDParam_FrontTorque { get; set; }
        public byte LSDParam_RearTorque { get; set; }
        public byte LSDParam_FrontAccel { get; set; }
        public byte LSDParam_RearAccel { get; set; }
        public byte LSDParam_FrontDecel { get; set; }
        public byte LSDParam_RearDecel { get; set; }
        public byte TCSC_Value { get; set; }
        public byte ASCC_VSC1 { get; set; }
        public byte ASCC_VSC2 { get; set; }
        public byte ASCC_VUC1 { get; set; }
        public byte ASCC_VUC2 { get; set; }
        public byte BallastWeight { get; set; }
        public byte BallastPosition { get; set; }
        public byte unk1 { get; set; }
        public byte unk2 { get; set; }
        public byte unk3 { get; set; }
        public byte unk4 { get; set; }
        public byte unk5 { get; set; }
        public byte WeightMultiplier  { get; set; }
        public byte PowerMultiplier { get; set; }
        public ushort UnkTunedCarValue { get; set; }
        public byte IsStrange { get; set; }
        public byte NOSTorqueVolume { get; set; }
        public byte TireSize_StiffnessCategory { get; set; }
        public byte byte163 { get; set; }
        public byte Gear_WeightRoller { get; set; }
        public byte Susp_FrontPreLoadLevel { get; set; }
        public byte Susp_RearPreLoadLevel { get; set; }
        public byte Susp_FrontSpringRateLevel { get; set; }
        public byte Susp_RearSpringRateLevel { get; set; }
        public byte byte169 { get; set; }
        public byte[] Unk_GT4OData { get; set; }

        public void CopyTo(CarEquipments dest)
        {
            dest.CarCode = new DbCode(CarCode.Code, CarCode.TableId);
            dest.TunedCarCode = new DbCode(TunedCarCode.Code, TunedCarCode.TableId);
            dest.Variation = new DbCode(Variation.Code, Variation.TableId);
            dest.VariationOrder = VariationOrder;
            dest.Brake = new DbCode(Brake.Code, Brake.TableId);
            dest.BrakeController = new DbCode(BrakeController.Code, BrakeController.TableId);
            dest.Chassis = new DbCode(Chassis.Code, Chassis.TableId);
            dest.Engine = new DbCode(Engine.Code, Engine.TableId);
            dest.Drivetrain = new DbCode(Drivetrain.Code, Drivetrain.TableId);
            dest.Gear = new DbCode(Gear.Code, Gear.TableId);
            dest.Suspension = new DbCode(Suspension.Code, Suspension.TableId);
            dest.LSD = new DbCode(LSD.Code, LSD.TableId);
            dest.FrontTire = new DbCode(FrontTire.Code, FrontTire.TableId);
            dest.RearTire = new DbCode(RearTire.Code, RearTire.TableId);
            dest.Steer = new DbCode(Steer.Code, Steer.TableId);
            dest.Lightweight = new DbCode(Lightweight.Code, Lightweight.TableId);
            dest.RacingModify = new DbCode(RacingModify.Code, RacingModify.TableId);
            dest.Portpolish = new DbCode(Portpolish.Code, Portpolish.TableId);
            dest.EngineBalance = new DbCode(EngineBalance.Code, EngineBalance.TableId);
            dest.Displacement = new DbCode(Displacement.Code, Displacement.TableId);
            dest.Computer = new DbCode(Computer.Code, Computer.TableId);
            dest.Natune = new DbCode(Natune.Code, Natune.TableId);
            dest.TurbineKit = new DbCode(TurbineKit.Code, TurbineKit.TableId);
            dest.Flywheel = new DbCode(Flywheel.Code, Flywheel.TableId);
            dest.Clutch = new DbCode(Clutch.Code, Clutch.TableId);
            dest.PropellerShaft = new DbCode(PropellerShaft.Code, PropellerShaft.TableId);
            dest.Muffler = new DbCode(Muffler.Code, Muffler.TableId);
            dest.Intercooler = new DbCode(Intercooler.Code, Intercooler.TableId);
            dest.ASCC = new DbCode(ASCC.Code, ASCC.TableId);
            dest.TCSC = new DbCode(TCSC.Code, TCSC.TableId);
            dest.Wheel = new DbCode(Wheel.Code, Wheel.TableId);
            dest.NOS = new DbCode(NOS.Code, NOS.TableId);
            dest.Wing = new DbCode(Wing.Code, Wing.TableId);
            dest.SuperCharger = new DbCode(SuperCharger.Code, SuperCharger.TableId);
            dest.GearReverse = GearReverse;
            dest.GearRatio1 = GearRatio1;
            dest.GearRatio2 = GearRatio2;
            dest.GearRatio3 = GearRatio3;
            dest.GearRatio4 = GearRatio4;
            dest.GearRatio5 = GearRatio5;
            dest.GearRatio6 = GearRatio6;
            dest.GearRatio7 = GearRatio7;
            dest.GearRatio8 = GearRatio8;
            dest.GearRatio9 = GearRatio9;
            dest.GearRatio10 = GearRatio10;
            dest.GearRatio11 = GearRatio11;
            dest.FinalGearRatio = FinalGearRatio;
            dest.MaxSpeed = MaxSpeed;
            dest.LastFinalGearRatio = LastFinalGearRatio;
            dest.Param4WD = Param4WD;
            dest.ABSLevelF = ABSLevelF;
            dest.ABSLevelR = ABSLevelR;
            dest.SettingClF = SettingClF;
            dest.SettingClR = SettingClR;
            dest.Turbo_Boost1 = Turbo_Boost1;
            dest.Turbo_PeakRPM1 = Turbo_PeakRPM1;
            dest.Turbo_Response1 = Turbo_Response1;
            dest.Turbo_Boost2 = Turbo_Boost2;
            dest.Turbo_PeakRPM2 = Turbo_PeakRPM2;
            dest.Turbo_Response2 = Turbo_Response2;
            dest.Susp_FrontCamber = Susp_FrontCamber;
            dest.Susp_RearCamber = Susp_RearCamber;
            dest.field_138 = field_138;
            dest.Susp_RideHeightF = Susp_RideHeightF;
            dest.Susp_RideHeightR = Susp_RideHeightR;
            dest.Susp_FrontToe = Susp_FrontToe;
            dest.Susp_RearToe = Susp_RearToe;
            dest.Susp_FrontSpringRate = Susp_FrontSpringRate;
            dest.Susp_RearSpringRate = Susp_RearSpringRate;
            dest.leverRatioF = leverRatioF;
            dest.leverRatioR = leverRatioR;
            dest.Susp_FrontDamperF1B = Susp_FrontDamperF1B;
            dest.Susp_FrontDamperF2B = Susp_FrontDamperF2B;
            dest.Susp_FrontDamperF1R = Susp_FrontDamperF1R;
            dest.Susp_FrontDamperF2R = Susp_FrontDamperF2R;
            dest.Susp_RearDamperF1B = Susp_RearDamperF1B;
            dest.Susp_RearDamperF2B = Susp_RearDamperF2B;
            dest.Susp_RearDamperF1R = Susp_RearDamperF1R;
            dest.Susp_RearDamperF2R = Susp_RearDamperF2R;
            dest.Susp_FrontStabilizer = Susp_FrontStabilizer;
            dest.Susp_RearStabilizer = Susp_RearStabilizer;
            dest.LSDParam_FrontTorque = LSDParam_FrontTorque;
            dest.LSDParam_RearTorque = LSDParam_RearTorque;
            dest.LSDParam_FrontAccel = LSDParam_FrontAccel;
            dest.LSDParam_RearAccel = LSDParam_RearAccel;
            dest.LSDParam_FrontDecel = LSDParam_FrontDecel;
            dest.LSDParam_RearDecel = LSDParam_RearDecel;
            dest.TCSC_Value = TCSC_Value;
            dest.ASCC_VSC1 = ASCC_VSC1;
            dest.ASCC_VSC2 = ASCC_VSC2;
            dest.ASCC_VUC1 = ASCC_VUC1;
            dest.ASCC_VUC2 = ASCC_VUC2;
            dest.BallastWeight = BallastWeight;
            dest.BallastPosition = BallastPosition;
            dest.unk1 = unk1;
            dest.unk2 = unk2;
            dest.unk3 = unk3;
            dest.unk4 = unk4;
            dest.unk5 = unk5;
            dest.WeightMultiplier = WeightMultiplier;
            dest.PowerMultiplier = PowerMultiplier;
            dest.UnkTunedCarValue = UnkTunedCarValue;
            dest.IsStrange = IsStrange;
            dest.NOSTorqueVolume = NOSTorqueVolume;
            dest.TireSize_StiffnessCategory = TireSize_StiffnessCategory;
            dest.byte163 = byte163;
            dest.Gear_WeightRoller = Gear_WeightRoller;
            dest.Susp_FrontPreLoadLevel = Susp_FrontPreLoadLevel;
            dest.Susp_RearPreLoadLevel = Susp_RearPreLoadLevel;
            dest.Susp_FrontSpringRateLevel = Susp_FrontSpringRateLevel;
            dest.Susp_RearSpringRateLevel = Susp_RearSpringRateLevel;
            dest.byte169 = byte169;

            if (Unk_GT4OData != null)
            {
                dest.Unk_GT4OData = new byte[0x18];
                Array.Copy(Unk_GT4OData, dest.Unk_GT4OData, Unk_GT4OData.Length);
            }
        }

        public void Unpack(ref SpanReader sr, bool gt4o = false)
        {
            CarCode = DbCode.Unpack(ref sr);
            TunedCarCode = DbCode.Unpack(ref sr);
            Variation = DbCode.Unpack(ref sr);
            VariationOrder = sr.ReadByte();
            sr.Position += 7;
            Brake = DbCode.Unpack(ref sr);
            BrakeController = DbCode.Unpack(ref sr);
            Chassis = DbCode.Unpack(ref sr);
            Engine = DbCode.Unpack(ref sr);
            Drivetrain = DbCode.Unpack(ref sr);
            Gear = DbCode.Unpack(ref sr);
            Suspension = DbCode.Unpack(ref sr);
            LSD = DbCode.Unpack(ref sr);
            FrontTire = DbCode.Unpack(ref sr);
            RearTire = DbCode.Unpack(ref sr);
            Steer = DbCode.Unpack(ref sr);
            Lightweight = DbCode.Unpack(ref sr);
            RacingModify = DbCode.Unpack(ref sr);
            Portpolish = DbCode.Unpack(ref sr);
            EngineBalance = DbCode.Unpack(ref sr);
            Displacement = DbCode.Unpack(ref sr);
            Computer = DbCode.Unpack(ref sr);
            Natune = DbCode.Unpack(ref sr);
            TurbineKit = DbCode.Unpack(ref sr);
            Flywheel = DbCode.Unpack(ref sr);
            Clutch = DbCode.Unpack(ref sr);
            PropellerShaft = DbCode.Unpack(ref sr);
            Muffler = DbCode.Unpack(ref sr);
            Intercooler = DbCode.Unpack(ref sr);
            ASCC = DbCode.Unpack(ref sr);
            TCSC = DbCode.Unpack(ref sr);
            Wheel = DbCode.Unpack(ref sr);
            NOS = DbCode.Unpack(ref sr);
            Wing = DbCode.Unpack(ref sr);
            SuperCharger = DbCode.Unpack(ref sr);
            GearReverse = sr.ReadUInt16();
            GearRatio1 = sr.ReadUInt16();
            GearRatio2 = sr.ReadUInt16();
            GearRatio3 = sr.ReadUInt16();
            GearRatio4 = sr.ReadUInt16();
            GearRatio5 = sr.ReadUInt16();
            GearRatio6 = sr.ReadUInt16();
            GearRatio7 = sr.ReadUInt16();
            GearRatio8 = sr.ReadUInt16();
            GearRatio9 = sr.ReadUInt16();
            GearRatio10 = sr.ReadUInt16();
            GearRatio11 = sr.ReadUInt16();
            FinalGearRatio = sr.ReadUInt16();
            MaxSpeed = sr.ReadUInt16();
            LastFinalGearRatio = sr.ReadUInt16();
            Param4WD = sr.ReadByte();
            ABSLevelF = sr.ReadByte();
            ABSLevelR = sr.ReadByte();
            SettingClF = sr.ReadByte();
            SettingClR = sr.ReadByte();
            Turbo_Boost1 = sr.ReadByte();
            Turbo_PeakRPM1 = sr.ReadByte();
            Turbo_Response1 = sr.ReadByte();
            Turbo_Boost2 = sr.ReadByte();
            Turbo_PeakRPM2 = sr.ReadByte();
            Turbo_Response2 = sr.ReadByte();
            Susp_FrontCamber = sr.ReadByte();
            Susp_RearCamber = sr.ReadByte();
            field_138 = sr.ReadByte();
            Susp_RideHeightF = sr.ReadInt16();
            Susp_RideHeightR = sr.ReadInt16();
            Susp_FrontToe = sr.ReadByte();
            Susp_RearToe = sr.ReadByte();
            Susp_FrontSpringRate = sr.ReadByte();
            Susp_RearSpringRate = sr.ReadByte();
            leverRatioF = sr.ReadByte();
            leverRatioR = sr.ReadByte();
            Susp_FrontDamperF1B = sr.ReadByte();
            Susp_FrontDamperF2B = sr.ReadByte();
            Susp_FrontDamperF1R = sr.ReadByte();
            Susp_FrontDamperF2R = sr.ReadByte();
            Susp_RearDamperF1B = sr.ReadByte();
            Susp_RearDamperF2B = sr.ReadByte();
            Susp_RearDamperF1R = sr.ReadByte();
            Susp_RearDamperF2R = sr.ReadByte();
            Susp_FrontStabilizer = sr.ReadByte();
            Susp_RearStabilizer = sr.ReadByte();
            LSDParam_FrontTorque = sr.ReadByte();
            LSDParam_RearTorque = sr.ReadByte();
            LSDParam_FrontAccel = sr.ReadByte();
            LSDParam_RearAccel = sr.ReadByte();
            LSDParam_FrontDecel = sr.ReadByte();
            LSDParam_RearDecel = sr.ReadByte();
            TCSC_Value = sr.ReadByte();
            ASCC_VSC1 = sr.ReadByte();
            ASCC_VSC2 = sr.ReadByte();
            ASCC_VUC1 = sr.ReadByte();
            ASCC_VUC2 = sr.ReadByte();
            BallastWeight = sr.ReadByte();
            BallastPosition = sr.ReadByte();
            unk1 = sr.ReadByte();
            unk2 = sr.ReadByte();
            unk3 = sr.ReadByte();
            unk4 = sr.ReadByte();
            unk5 = sr.ReadByte();
            WeightMultiplier = sr.ReadByte();
            PowerMultiplier = sr.ReadByte();
            UnkTunedCarValue = sr.ReadUInt16();
            IsStrange = sr.ReadByte();
            NOSTorqueVolume = sr.ReadByte();
            TireSize_StiffnessCategory = sr.ReadByte();
            byte163 = sr.ReadByte();
            Gear_WeightRoller = sr.ReadByte();
            Susp_FrontPreLoadLevel = sr.ReadByte();
            Susp_RearPreLoadLevel = sr.ReadByte();
            Susp_FrontSpringRateLevel = sr.ReadByte();
            Susp_RearSpringRateLevel = sr.ReadByte();
            byte169 = sr.ReadByte();

            sr.Position += 8; // Empty bytes

            if (gt4o)
                Unk_GT4OData = sr.ReadBytes(0x18);
        }

        public void Pack(ref SpanWriter sw, bool gt4o = false)
        {
            CarCode.Pack(ref sw);
            TunedCarCode.Pack(ref sw);
            Variation.Pack(ref sw);
            sw.WriteByte(VariationOrder);
            sw.Position += 7;
            Brake.Pack(ref sw);
            BrakeController.Pack(ref sw);
            Chassis.Pack(ref sw);
            Engine.Pack(ref sw);
            Drivetrain.Pack(ref sw);
            Gear.Pack(ref sw);
            Suspension.Pack(ref sw);
            LSD.Pack(ref sw);
            FrontTire.Pack(ref sw);
            RearTire.Pack(ref sw);
            Steer.Pack(ref sw);
            Lightweight.Pack(ref sw);
            RacingModify.Pack(ref sw);
            Portpolish.Pack(ref sw);
            EngineBalance.Pack(ref sw);
            Displacement.Pack(ref sw);
            Computer.Pack(ref sw);
            Natune.Pack(ref sw);
            TurbineKit.Pack(ref sw);
            Flywheel.Pack(ref sw);
            Clutch.Pack(ref sw);
            PropellerShaft.Pack(ref sw);
            Muffler.Pack(ref sw);
            Intercooler.Pack(ref sw);
            ASCC.Pack(ref sw);
            TCSC.Pack(ref sw);
            Wheel.Pack(ref sw);
            NOS.Pack(ref sw);
            Wing.Pack(ref sw);
            SuperCharger.Pack(ref sw);
            sw.WriteUInt16(GearReverse);
            sw.WriteUInt16(GearRatio1);
            sw.WriteUInt16(GearRatio2);
            sw.WriteUInt16(GearRatio3);
            sw.WriteUInt16(GearRatio4);
            sw.WriteUInt16(GearRatio5);
            sw.WriteUInt16(GearRatio6);
            sw.WriteUInt16(GearRatio7);
            sw.WriteUInt16(GearRatio8);
            sw.WriteUInt16(GearRatio9);
            sw.WriteUInt16(GearRatio10);
            sw.WriteUInt16(GearRatio11);
            sw.WriteUInt16(FinalGearRatio);
            sw.WriteUInt16(MaxSpeed);
            sw.WriteUInt16(LastFinalGearRatio);
            sw.WriteByte(Param4WD);
            sw.WriteByte(ABSLevelF);
            sw.WriteByte(ABSLevelR);
            sw.WriteByte(SettingClF);
            sw.WriteByte(SettingClR);
            sw.WriteByte(Turbo_Boost1);
            sw.WriteByte(Turbo_PeakRPM1);
            sw.WriteByte(Turbo_Response1);
            sw.WriteByte(Turbo_Boost2);
            sw.WriteByte(Turbo_PeakRPM2);
            sw.WriteByte(Turbo_Response2);
            sw.WriteByte(Susp_FrontCamber);
            sw.WriteByte(Susp_RearCamber);
            sw.WriteByte(field_138);
            sw.WriteInt16(Susp_RideHeightF);
            sw.WriteInt16(Susp_RideHeightR);
            sw.WriteByte(Susp_FrontToe);
            sw.WriteByte(Susp_RearToe);
            sw.WriteByte(Susp_FrontSpringRate);
            sw.WriteByte(Susp_RearSpringRate);
            sw.WriteByte(leverRatioF);
            sw.WriteByte(leverRatioR);
            sw.WriteByte(Susp_FrontDamperF1B);
            sw.WriteByte(Susp_FrontDamperF2B);
            sw.WriteByte(Susp_FrontDamperF1R);
            sw.WriteByte(Susp_FrontDamperF2R);
            sw.WriteByte(Susp_RearDamperF1B);
            sw.WriteByte(Susp_RearDamperF2B);
            sw.WriteByte(Susp_RearDamperF1R);
            sw.WriteByte(Susp_RearDamperF2R);
            sw.WriteByte(Susp_FrontStabilizer);
            sw.WriteByte(Susp_RearStabilizer);
            sw.WriteByte(LSDParam_FrontTorque);
            sw.WriteByte(LSDParam_RearTorque);
            sw.WriteByte(LSDParam_FrontAccel);
            sw.WriteByte(LSDParam_RearAccel);
            sw.WriteByte(LSDParam_FrontDecel);
            sw.WriteByte(LSDParam_RearDecel);
            sw.WriteByte(TCSC_Value);
            sw.WriteByte(ASCC_VSC1);
            sw.WriteByte(ASCC_VSC2);
            sw.WriteByte(ASCC_VUC1);
            sw.WriteByte(ASCC_VUC2);
            sw.WriteByte(BallastWeight);
            sw.WriteByte(BallastPosition);
            sw.WriteByte(unk1);
            sw.WriteByte(unk2);
            sw.WriteByte(unk3);
            sw.WriteByte(unk4);
            sw.WriteByte(unk5);
            sw.WriteByte(WeightMultiplier);
            sw.WriteByte(PowerMultiplier);
            sw.WriteUInt16(UnkTunedCarValue);
            sw.WriteByte(IsStrange);
            sw.WriteByte(NOSTorqueVolume);
            sw.WriteByte(TireSize_StiffnessCategory);
            sw.WriteByte(byte163);
            sw.WriteByte(Gear_WeightRoller);
            sw.WriteByte(Susp_FrontPreLoadLevel);
            sw.WriteByte(Susp_RearPreLoadLevel);
            sw.WriteByte(Susp_FrontSpringRateLevel);
            sw.WriteByte(Susp_RearSpringRateLevel);
            sw.WriteByte(byte169);

            sw.Position += 8;

            if (gt4o)
                sw.WriteBytes(Unk_GT4OData);
        }
    }
}
