using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using Syroot.BinaryData.Memory;

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
        public short GearReverse { get; set; }
        public short GearRatio1 { get; set; }
        public short GearRatio2 { get; set; }
        public short GearRatio3 { get; set; }
        public short GearRatio4 { get; set; }
        public short GearRatio5 { get; set; }
        public short GearRatio6 { get; set; }
        public short GearRatio7 { get; set; }
        public short GearRatio8 { get; set; }
        public short GearRatio9 { get; set; }
        public short GearRatio10 { get; set; }
        public short GearRatio11 { get; set; }
        public short FinalGearRatio { get; set; }
        public short MaxSpeed { get; set; }
        public short LastFinalGearRatio { get; set; }
        public byte Param4WD { get; set; }
        public byte FrontABS { get; set; }
        public byte RearABS { get; set; }
        public byte SettingClF { get; set; }
        public byte SettingClR { get; set; }
        public byte Boost1 { get; set; }
        public byte PeakRPM1 { get; set; }
        public byte Response1 { get; set; }
        public byte Boost2 { get; set; }
        public byte PeakRPM2 { get; set; }
        public byte Response2 { get; set; }
        public byte field_136 { get; set; }
        public byte field_137 { get; set; }
        public byte field_138 { get; set; }
        public short Susp_RideHeightF { get; set; }
        public short Susp_RideHeightR { get; set; }
        public byte FrontToe { get; set; }
        public byte RearToe { get; set; }
        public byte FrontSpringRate { get; set; }
        public byte RearSpringRate { get; set; }
        public byte leverRatioF { get; set; }
        public byte leverRatioR { get; set; }
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
        public byte FrontLSDparam { get; set; }
        public byte FrontLSDparam2 { get; set; }
        public byte FrontLSDparam3 { get; set; }
        public byte RearLSDparam { get; set; }
        public byte RearLSDparam2 { get; set; }
        public byte RearLSDpara3 { get; set; }
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
        public byte unk6  { get; set; }
        public byte field_15D { get; set; }
        public short word15E { get; set; }
        public byte byte160 { get; set; }
        public byte byte161 { get; set; }
        public byte byte162 { get; set; }
        public byte byte163 { get; set; }
        public byte byte164 { get; set; }
        public byte byte165 { get; set; }
        public byte byte166 { get; set; }
        public byte unk7 { get; set; }
        public byte unk8 { get; set; }
        public byte byte169 { get; set; }
        public byte unk9 { get; set; }
        public byte byte16B { get; set; }
        public byte byte16C { get; set; }
        public byte byte16D { get; set; }
        public byte byte16E { get; set; }
        public byte field_16F { get; set; }
        public byte field_170 { get; set; }
        public byte field_171 { get; set; }


        public void Unpack(ref SpanReader sr)
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
            GearReverse = sr.ReadInt16();
            GearRatio1 = sr.ReadInt16();
            GearRatio2 = sr.ReadInt16();
            GearRatio3 = sr.ReadInt16();
            GearRatio4 = sr.ReadInt16();
            GearRatio5 = sr.ReadInt16();
            GearRatio6 = sr.ReadInt16();
            GearRatio7 = sr.ReadInt16();
            GearRatio8 = sr.ReadInt16();
            GearRatio9 = sr.ReadInt16();
            GearRatio10 = sr.ReadInt16();
            GearRatio11 = sr.ReadInt16();
            FinalGearRatio = sr.ReadInt16();
            MaxSpeed = sr.ReadInt16();
            LastFinalGearRatio = sr.ReadInt16();
            Param4WD = sr.ReadByte();
            FrontABS = sr.ReadByte();
            RearABS = sr.ReadByte();
            SettingClF = sr.ReadByte();
            SettingClR = sr.ReadByte();
            Boost1 = sr.ReadByte();
            PeakRPM1 = sr.ReadByte();
            Response1 = sr.ReadByte();
            Boost2 = sr.ReadByte();
            PeakRPM2 = sr.ReadByte();
            Response2 = sr.ReadByte();
            field_136 = sr.ReadByte();
            field_137 = sr.ReadByte();
            field_138 = sr.ReadByte();
            Susp_RideHeightF = sr.ReadInt16();
            Susp_RideHeightR = sr.ReadInt16();
            FrontToe = sr.ReadByte();
            RearToe = sr.ReadByte();
            FrontSpringRate = sr.ReadByte();
            RearSpringRate = sr.ReadByte();
            leverRatioF = sr.ReadByte();
            leverRatioR = sr.ReadByte();
            FrontDamperF1B = sr.ReadByte();
            FrontDamperF2B = sr.ReadByte();
            FrontDamperF1R = sr.ReadByte();
            FrontDamperF2R = sr.ReadByte();
            RearDamperF1B = sr.ReadByte();
            RearDamperF2B = sr.ReadByte();
            RearDamperF1R = sr.ReadByte();
            RearDamperF2R = sr.ReadByte();
            FrontStabilizer = sr.ReadByte();
            RearStabilizer = sr.ReadByte();
            FrontLSDparam = sr.ReadByte();
            FrontLSDparam2 = sr.ReadByte();
            FrontLSDparam3 = sr.ReadByte();
            RearLSDparam = sr.ReadByte();
            RearLSDparam2 = sr.ReadByte();
            RearLSDpara3 = sr.ReadByte();
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
            unk6 = sr.ReadByte();
            field_15D = sr.ReadByte();
            word15E = sr.ReadInt16();
            byte160 = sr.ReadByte();
            byte161 = sr.ReadByte();
            byte162 = sr.ReadByte();
            byte163 = sr.ReadByte();
            byte164 = sr.ReadByte();
            byte165 = sr.ReadByte();
            byte166 = sr.ReadByte();
            unk7 = sr.ReadByte();
            unk8 = sr.ReadByte();
            byte169 = sr.ReadByte();
            unk9 = sr.ReadByte();
            byte16B = sr.ReadByte();
            byte16C = sr.ReadByte();
            byte16D = sr.ReadByte();
            byte16E = sr.ReadByte();
            field_16F = sr.ReadByte();
            field_170 = sr.ReadByte();
            field_171 = sr.ReadByte();
        }

        public void Pack(ref SpanWriter sw)
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
            sw.WriteInt16(GearReverse);
            sw.WriteInt16(GearRatio1);
            sw.WriteInt16(GearRatio2);
            sw.WriteInt16(GearRatio3);
            sw.WriteInt16(GearRatio4);
            sw.WriteInt16(GearRatio5);
            sw.WriteInt16(GearRatio6);
            sw.WriteInt16(GearRatio7);
            sw.WriteInt16(GearRatio8);
            sw.WriteInt16(GearRatio9);
            sw.WriteInt16(GearRatio10);
            sw.WriteInt16(GearRatio11);
            sw.WriteInt16(FinalGearRatio);
            sw.WriteInt16(MaxSpeed);
            sw.WriteInt16(LastFinalGearRatio);
            sw.WriteByte(Param4WD);
            sw.WriteByte(FrontABS);
            sw.WriteByte(RearABS);
            sw.WriteByte(SettingClF);
            sw.WriteByte(SettingClR);
            sw.WriteByte(Boost1);
            sw.WriteByte(PeakRPM1);
            sw.WriteByte(Response1);
            sw.WriteByte(Boost2);
            sw.WriteByte(PeakRPM2);
            sw.WriteByte(Response2);
            sw.WriteByte(field_136);
            sw.WriteByte(field_137);
            sw.WriteByte(field_138);
            sw.WriteInt16(Susp_RideHeightF);
            sw.WriteInt16(Susp_RideHeightR);
            sw.WriteByte(FrontToe);
            sw.WriteByte(RearToe);
            sw.WriteByte(FrontSpringRate);
            sw.WriteByte(RearSpringRate);
            sw.WriteByte(leverRatioF);
            sw.WriteByte(leverRatioR);
            sw.WriteByte(FrontDamperF1B);
            sw.WriteByte(FrontDamperF2B);
            sw.WriteByte(FrontDamperF1R);
            sw.WriteByte(FrontDamperF2R);
            sw.WriteByte(RearDamperF1B);
            sw.WriteByte(RearDamperF2B);
            sw.WriteByte(RearDamperF1R);
            sw.WriteByte(RearDamperF2R);
            sw.WriteByte(FrontStabilizer);
            sw.WriteByte(RearStabilizer);
            sw.WriteByte(FrontLSDparam);
            sw.WriteByte(FrontLSDparam2);
            sw.WriteByte(FrontLSDparam3);
            sw.WriteByte(RearLSDparam);
            sw.WriteByte(RearLSDparam2);
            sw.WriteByte(RearLSDpara3);
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
            sw.WriteByte(unk6);
            sw.WriteByte(field_15D);
            sw.WriteInt16(word15E);
            sw.WriteByte(byte160);
            sw.WriteByte(byte161);
            sw.WriteByte(byte162);
            sw.WriteByte(byte163);
            sw.WriteByte(byte164);
            sw.WriteByte(byte165);
            sw.WriteByte(byte166);
            sw.WriteByte(unk7);
            sw.WriteByte(unk8);
            sw.WriteByte(byte169);
            sw.WriteByte(unk9);
            sw.WriteByte(byte16B);
            sw.WriteByte(byte16C);
            sw.WriteByte(byte16D);
            sw.WriteByte(byte16E);
            sw.WriteByte(field_16F);
            sw.WriteByte(field_170);
            sw.WriteByte(field_171);
        }
    }
}
