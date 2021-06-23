using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools;
using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class PenaltyParameter
    {
        public bool Enable { get; set; } = true;
        public byte CarCrashInterval { get; set; } = 100;
        public short ScoreSumThreshold { get; set; } = 500;
        public byte CondType { get; set; }
        public byte RatioDiffMin { get; set; }
        public short ScoreDiffMin { get; set; }
        public byte CarImpactThreshold { get; set; }
        public byte CarImpactThreshold2 { get; set; } = 30;
        public byte VelocityDirAngle0 { get; set; } = 20;
        public byte VelocityDirAngle1 { get; set; } = 60;
        public short VelocityDirScore0 { get; set; }
        public short VelocityDirScore1 { get; set; } = 100;
        public byte SteeringAngle0 { get; set; } = 5;
        public byte SteeringAngle1 { get; set; } = 20;
        public short SpeedScore0 { get; set; }
        public short SpeedScore1 { get; set; }
        public byte Speed0 { get; set; } = 15;
        public byte Speed1 { get; set; } = 120;
        public short unk0;
        public short unk1 = 100;
        public byte BackwardAngle { get; set; }
        public byte BackwardMoveRatio { get; set; } = 10;
        public byte WallImpactThreshold { get; set; } = 30;
        public byte WallAlongTimer { get; set; } = 20;
        public byte WallAlongCounter { get; set; } = 5;
        public byte PunishSpeedLimit { get; set; } = 50;
        public byte PunishImpactThreshold0 { get; set; } = 120;
        public byte PunishImpactThreshold1 { get; set; } = 50;

        public UnkPenaltyData[] UnkPenaltyDatas = new UnkPenaltyData[]
        {
            new UnkPenaltyData(10,5,0,0,0,0,0,0),
            new UnkPenaltyData(10,10,5,0,5,5,5,0),
            new UnkPenaltyData(10,10,8,5,5,5,5,0),
            new UnkPenaltyData(10,10,8,5,5,10,10,0),
            new UnkPenaltyData(0,0,0,0,0,0,5,0),
            new UnkPenaltyData(0,0,0,0,0,5,0,0),
            new UnkPenaltyData(0,5,0,0,0,5,5,0),
            new UnkPenaltyData(0,10,5,0,0,5,10,5),
        };

        public bool PunishCollision { get; set; }
        public byte CollisionRecoverDelay { get; set; } = 50;
        public short ShortcutRadius { get; set; } = 250;
        public byte ShortcutMinSpeed { get; set; }
        public bool FreeCrashedbyAutodrive { get; set; } = true;
        public byte FreeRatioByAutodrive { get; set; } = 30;
        public bool PitPenalty { get; set; }
        public byte SideSpeed0 { get; set; }
        public byte SideSpeed1 { get; set; }
        public short SideSpeedScore0 { get; set; }
        public short SideSpeedScore1 { get; set; }
        public byte ShortcutCancelTime1 { get; set; }
        public byte ShortcutCancelTime0 { get; set; }
        public short CollisionOffScore0 { get; set; }
        public short CollisionOffScore1 { get; set; }
        public byte CollisionOffScoreType { get; set; }
        public byte field_0x79;
        public byte FreeLessRatio { get; set; }
        public byte CancelSteeringAngleDiff { get; set; }
        public short CollisionOffDispScore0 { get; set; }
        public byte ShortcutCancelInJamSpeed { get; set; }
        public byte field_0x7f;
        public byte SteeringScoreRatioMin { get; set; }
        public byte SteeringScoreRatioMax { get; set; }
        public byte WallImpactThreshold0 { get; set; } = 3;
        public byte ShortcutRatio { get; set; }
        public byte SideSpeed0Steering { get; set; }
        public byte SideSpeed1Steering { get; set; }
        public byte SideSpeedSteeringScore0 { get; set; }
        public byte SideSpeedSteeringScore1 { get; set; }
        public byte ShortcutType { get; set; } = 1;
        public byte PenaSpeedRatio1 { get; set; } = 80;
        public byte PenaSpeedRatio2 { get; set; } = 120;
        public byte PenaSpeedRatio3 { get; set; } = 150;

        public void ReadPenaltyParameter(ref BitStream reader)
        {
            Enable = reader.ReadBool(); // enable
            CarCrashInterval = reader.ReadByte(); // car_crash_interval
            ScoreSumThreshold = reader.ReadInt16(); // score_sum_threshold
            CondType = reader.ReadByte(); // cond_type
            RatioDiffMin = reader.ReadByte(); // ratio_diff_min
            ScoreDiffMin = reader.ReadInt16(); // score_diff_min
            CarImpactThreshold = reader.ReadByte(); // car_impact_threshold
            CarImpactThreshold2 = reader.ReadByte(); // car_impact_threshold2
            VelocityDirAngle0 = reader.ReadByte(); // velocity_dir_angle0
            VelocityDirAngle1 = reader.ReadByte(); // velocity_dir_angle1
            VelocityDirScore0 = reader.ReadInt16(); // velocity_dir_score0
            VelocityDirScore1 = reader.ReadInt16(); // velocity_dir_score1
            SteeringAngle0 = reader.ReadByte(); // steering_angle0
            SteeringAngle1 = reader.ReadByte(); // steering_angle1
            SpeedScore0 = reader.ReadInt16(); // speed_score0
            SpeedScore1 = reader.ReadInt16(); // speed_score1
            Speed0 = reader.ReadByte(); // speed0
            Speed1 = reader.ReadByte(); // speed1
            reader.ReadInt16(); // unk field_0x18
            reader.ReadInt16(); // unk field_0x1a
            BackwardAngle = reader.ReadByte(); // backward_angle
            BackwardMoveRatio = reader.ReadByte(); // backward_move_ratio
            WallImpactThreshold = reader.ReadByte(); // wall_impact_threshold
            WallAlongTimer = reader.ReadByte(); // wall_along_timer
            WallAlongCounter = reader.ReadByte(); // wall_along_counter
            PunishSpeedLimit = reader.ReadByte(); // punish_speed_limit
            PunishImpactThreshold0 = reader.ReadByte(); // punish_impact_threshold0
            PunishImpactThreshold1 = reader.ReadByte(); // punish_impact_threshold1

            // Needs research
            for (int i = 0; i < 8; i++)
            {
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
            }

            PunishCollision = reader.ReadBool(); // punish_collision
            CollisionRecoverDelay = reader.ReadByte(); // collision_recover_delay
            ShortcutRadius = reader.ReadInt16(); // shortcut_radius
            ShortcutMinSpeed = reader.ReadByte(); // shortcut_min_speed
            FreeCrashedbyAutodrive = reader.ReadBool(); // free_crashed_by_autodrive
            FreeRatioByAutodrive = reader.ReadByte(); // free_ratio_by_autodrive
            PitPenalty = reader.ReadBool(); // pit_penalty
            SideSpeed0 = reader.ReadByte(); // side_speed0
            SideSpeed1 = reader.ReadByte(); // side_speed1
            SideSpeedScore0 = reader.ReadInt16(); // side_speed_score0
            SideSpeedScore1 = reader.ReadInt16(); // side_speed_score1
            ShortcutCancelTime1 = reader.ReadByte(); // shortcut_cancel_time1
            ShortcutCancelTime0 = reader.ReadByte(); // shortcut_cancel_time0
            CollisionOffScore0 = reader.ReadInt16(); // collision_off_score0
            CollisionOffScore1 = reader.ReadInt16(); // collision_off_score1
            CollisionOffScoreType = reader.ReadByte(); // collision_off_score_type
            reader.ReadByte(); // unk field_0x79
            FreeLessRatio = reader.ReadByte(); // free_less_ratio
            CancelSteeringAngleDiff = reader.ReadByte(); // cancel_steering_angle_diff
            CollisionOffDispScore0 = reader.ReadInt16(); // collision_off_disp_score0
            ShortcutCancelInJamSpeed = reader.ReadByte(); // shortcut_cancel_in_jam_speed_ratio
            reader.ReadByte(); // unk field_0x7f
            SteeringScoreRatioMin = reader.ReadByte(); // steering_score_ratio_min
            SteeringScoreRatioMax = reader.ReadByte(); // steering_score_ratio_max
            WallImpactThreshold = reader.ReadByte(); // wall_impact_threshold0
            ShortcutRatio = reader.ReadByte(); // shortcut_ratio
            SideSpeed0Steering = reader.ReadByte(); // side_speed0_steering
            SideSpeed1Steering = reader.ReadByte(); // side_speed1_steering
            SideSpeedSteeringScore0 = reader.ReadByte(); // side_speed_steering_score0
            SideSpeedSteeringScore1 = reader.ReadByte(); // side_speed_steering_score1
            ShortcutType = reader.ReadByte(); // shortcut_type
            PenaSpeedRatio1 = reader.ReadByte(); // pena_speed_ratio1
            PenaSpeedRatio2 = reader.ReadByte(); // pena_speed_ratio2
            PenaSpeedRatio3 = reader.ReadByte(); // pena_speed_ratio3
        }

        public void WritePenaltyParameter(ref BitStream bs)
        {
            bs.WriteBool(Enable);
            bs.WriteByte(CarCrashInterval);
            bs.WriteInt16(ScoreSumThreshold);
            bs.WriteByte(CondType);
            bs.WriteByte(RatioDiffMin);
            bs.WriteInt16(ScoreDiffMin);
            bs.WriteByte(CarImpactThreshold);
            bs.WriteByte(CarImpactThreshold2);
            bs.WriteByte(VelocityDirAngle0);
            bs.WriteByte(VelocityDirAngle1);
            bs.WriteInt16(VelocityDirScore0);
            bs.WriteInt16(VelocityDirScore1);
            bs.WriteByte(SteeringAngle0);
            bs.WriteByte(SteeringAngle1);
            bs.WriteInt16(SpeedScore0);
            bs.WriteInt16(SpeedScore1);
            bs.WriteByte(Speed0);
            bs.WriteByte(Speed1);
            bs.WriteInt16(unk0);
            bs.WriteInt16(unk1);
            bs.WriteByte(BackwardAngle);
            bs.WriteByte(BackwardMoveRatio);
            bs.WriteByte(WallImpactThreshold);
            bs.WriteByte(WallAlongTimer);
            bs.WriteByte(WallAlongCounter);
            bs.WriteByte(PunishSpeedLimit);
            bs.WriteByte(PunishImpactThreshold0);
            bs.WriteByte(PunishImpactThreshold1);

            for (int i = 0; i < 8; i++)
            {
                bs.WriteByte(UnkPenaltyDatas[i].unk1);
                bs.WriteByte(UnkPenaltyDatas[i].unk2);
                bs.WriteByte(UnkPenaltyDatas[i].unk3);
                bs.WriteByte(UnkPenaltyDatas[i].unk4);
                bs.WriteByte(UnkPenaltyDatas[i].unk5);
                bs.WriteByte(UnkPenaltyDatas[i].unk6);
                bs.WriteByte(UnkPenaltyDatas[i].unk7);
                bs.WriteByte(UnkPenaltyDatas[i].unk8);
            }

            bs.WriteBool(PunishCollision);
            bs.WriteByte(CollisionRecoverDelay);
            bs.WriteInt16(ShortcutRadius);
            bs.WriteByte(ShortcutMinSpeed);
            bs.WriteBool(FreeCrashedbyAutodrive);
            bs.WriteByte(FreeRatioByAutodrive);
            bs.WriteBool(PitPenalty);
            bs.WriteByte(SideSpeed0);
            bs.WriteByte(SideSpeed1);
            bs.WriteInt16(SideSpeedScore0);
            bs.WriteInt16(SideSpeedScore1);
            bs.WriteByte(ShortcutCancelTime0);
            bs.WriteByte(ShortcutCancelTime1);
            bs.WriteInt16(CollisionOffScore0);
            bs.WriteInt16(CollisionOffScore1);
            bs.WriteByte(CollisionOffScoreType);
            bs.WriteByte(0);
            bs.WriteByte(FreeLessRatio);
            bs.WriteByte(CancelSteeringAngleDiff);
            bs.WriteInt16(CollisionOffDispScore0);
            bs.WriteByte(ShortcutCancelInJamSpeed);
            bs.WriteByte(0);
            bs.WriteByte(SteeringScoreRatioMin);
            bs.WriteByte(SteeringScoreRatioMax);
            bs.WriteByte(WallImpactThreshold0);
            bs.WriteByte(ShortcutRatio);
            bs.WriteByte(SideSpeed0Steering);
            bs.WriteByte(SideSpeed1Steering);
            bs.WriteByte(SideSpeedSteeringScore0);
            bs.WriteByte(SideSpeedSteeringScore1);
            bs.WriteByte(ShortcutType);
            bs.WriteByte(PenaSpeedRatio1);
            bs.WriteByte(PenaSpeedRatio2);
            bs.WriteByte(PenaSpeedRatio3);
        }
        public class UnkPenaltyData
        {
            public byte unk1;
            public byte unk2;
            public byte unk3;
            public byte unk4;
            public byte unk5;
            public byte unk6;
            public byte unk7;
            public byte unk8;

            public UnkPenaltyData(byte unk1, byte unk2, byte unk3, byte unk4, 
                byte unk5, byte unk6, byte unk7, byte unk8)
            {
                this.unk1 = unk1;
                this.unk2 = unk2;
                this.unk3 = unk3;
                this.unk4 = unk4;
                this.unk5 = unk5;
                this.unk6 = unk6;
                this.unk7 = unk7;
                this.unk8 = unk8;
            }
        }
    }
}
