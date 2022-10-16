using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Utils;
namespace PDTools.Structures.PS3.MGameParameter
{
    public class OnlineRoomParameter
    {
        private static readonly byte[] qualityControlParameter = new byte[]
        {
            0x80, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x03, 0xE8, 0x07, 0xD0,
            0x00, 0x64, 0x00, 0x21, 0x01, 0x2C, 0x00, 0x21, 0x00, 0x21, 0x00, 0x21, 0x00, 0x21, 0x00, 0x1E,
            0x00, 0xC8, 0x00, 0x32, 0x00, 0x96, 0x00, 0x64, 0x00, 0x7D, 0x01, 0x2C, 0x00, 0x64, 0x01, 0xF4,
            0x00, 0x32, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
            0x01, 0x2C, 0x00, 0x64, 0x00, 0xC8, 0x00, 0x96, 0x00, 0x96, 0x00, 0xC8, 0x00, 0x64, 0x01, 0x2C,
            0x00, 0x4B, 0x01, 0xF4, 0x00, 0x32, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0xE8,
            0x00, 0x00, 0xAA, 0xAA
        };

        public string Comment { get; set; }
        public string Password { get; set; }
        public sbyte RoomType { get; set; }
        public sbyte ChatType { get; set; }
        public sbyte VoiceQuality { get; set; } = 4;
        public sbyte VoiceChatMode { get; set; } = 2;
        public sbyte Topology { get; set; }
        public sbyte RacerMax { get; set; } = 16;
        public sbyte RoomMax { get; set; } = 16;
        public short RaceCountdown { get; set; } = 30;
        public sbyte TrackDayMode { get; set; } = 1;
        public sbyte BattleMode { get; set; } = 1;
        public sbyte FreerunPenalty { get; set; }
        public bool FreerunCollision { get; set; } = true;
        public int Weather { get; set; }
        public int GameRegionCode { get; set; }
        public string LoungeOwnerID { get; set; }
        public sbyte Scope { get; set; }
        public short AlarmTime { get; set; } = -1;
        public sbyte RoomPolicy { get; set; }
        public uint GeneratedCourseHash { get; set; }
        public short AlarmTimeValue { get; set; } = -1;
        public byte ThemeColorIndex { get; set; }
        public short QualifierRaceType { get; set; } = 7;
        public short QualifierBegin { get; set; }
        public short QualifierPeriod { get; set; } = 300;
        public sbyte CourseSelectMethod { get; set; } = 1;
        public sbyte CarShuffleMethod { get; set; } = 1;
        public sbyte CarSelectMethod { get; set; } = 1;
        public sbyte CarFilterType { get; set; }
        public sbyte SeriesPointType { get; set; } = 2;
        public sbyte BoobyPoint { get; set; }
        public sbyte TrialStartType { get; set; } = 3;
        public ulong ClubID { get; set; }
        public sbyte MatchingWorldOffset { get; set; } = -1;
        public sbyte MatchingSpace { get; set; } = 1;
        public uint MatchingEventID32 { get; set; }
        public ulong ClubEventID { get; set; }
        public uint EventSettingVersion { get; set; }
        public uint EventSettingHash { get; set; }
        public int SceneryCourseCode { get; set; } = -1;

        public bool AutoGrantOwnership { get; set; } = true;
        public int NatRestriction { get; set; } = 1;
        public bool ExcludeBlockList { get; set; } = true;
        public bool VoiceChat { get; set; } = true;
        public bool IsAutocratic { get; set; } = true;

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_D8_45);
            bs.WriteUInt32(1_17); // Version

            bs.WriteNullStringAligned4(Comment);
            bs.WriteNullStringAligned4(Password);
            bs.WriteSByte(RoomType);
            bs.WriteSByte(ChatType);
            bs.WriteSByte(VoiceQuality);
            bs.WriteSByte(VoiceChatMode);
            bs.WriteSByte(Topology);
            bs.WriteSByte(RoomMax);
            bs.WriteSByte(RacerMax);
            bs.WriteInt16(RaceCountdown);

            /* WriteULongToBuffer(auStack80,
                   (ulonglong)param_1->autogrant_ownership | (ulonglong)param_1->nat_restriction << 0x1 |
                   (ulonglong)param_1->is_lan << 0x6 | (ulonglong)param_1->is_only_garage_car << 0x15 |
                   (ulonglong)param_1->is_saved_course << 0x17 |
                   (longlong)(int)((uint)param_1->fill_vacancy << 0x19) |
                   (longlong)(int)((uint)param_1->overwrite_user_region << 0x1b) |
                   (longlong)(int)((uint)param_1->use_custom_grid << 0x1d) |
                   (ulonglong)
                   ((uint)param_1->exclude_blocklist << 0x3 | (uint)param_1->voice_chat << 0x4 |
                    (uint)param_1->is_autocratic << 0x14 | (uint)param_1->is_only_rental_car << 0x16) |
                   (longlong)(int)((uint)param_1->is_automated << 0x18) |
                   (longlong)(int)((uint)param_1->overwrite_user_name << 0x1a) |
                   (longlong)(int)((uint)param_1->show_hidden_config << 0x1c) |
                   (longlong)(int)((uint)param_1->use_custom_countdown << 0x1e),param_3,param_4);
            */
            bs.WriteInt64(1048603);
            bs.WriteSByte(TrackDayMode);
            bs.WriteSByte(BattleMode);
            bs.WriteSByte(FreerunPenalty);
            bs.WriteBool(FreerunCollision);

            bs.WriteInt32(100); // QCP Size
            for (int i = 0; i < 100; i++)
                bs.WriteByte(qualityControlParameter[i]);

            for (int i = 0; i < 4; i++)
                bs.WriteInt16(0); // shuffle params

            for (int i = 0; i < 2; i++)
                bs.WriteInt32(0); // shuffle params 2

            bs.WriteInt32(Weather);
            bs.WriteInt32(GameRegionCode);
            bs.WriteNullStringAligned4(LoungeOwnerID);
            bs.WriteSByte(Scope);
            bs.WriteInt16(AlarmTime);
            bs.WriteSByte(RoomPolicy);
            bs.WriteUInt32(GeneratedCourseHash);
            bs.WriteInt16(AlarmTimeValue);
            bs.WriteByte(ThemeColorIndex);
            bs.WriteInt16(QualifierRaceType);
            bs.WriteInt16(QualifierBegin);
            bs.WriteInt16(QualifierPeriod);
            bs.WriteSByte(CourseSelectMethod);
            bs.WriteSByte(0); // Unk field_0x80

            bs.WriteByte(0); // online series size, skip
            bs.WriteSByte(CarShuffleMethod);
            bs.WriteSByte(CarSelectMethod);
            bs.WriteSByte(CarFilterType);

            // Following is intentional
            bs.WriteSByte(0);
            bs.WriteSByte(0);
            bs.WriteSByte(0);
            bs.WriteInt32(0);
            bs.WriteInt32(0);
            bs.WriteInt32(0);

            bs.WriteSByte(SeriesPointType);
            bs.WriteByte(16); // Unk size
            for (int i = 0; i < 16; i++)
                bs.WriteByte(0);

            bs.WriteSByte(BoobyPoint);
            bs.WriteSByte(TrialStartType);
            bs.WriteUInt64(ClubID);
            bs.WriteSByte(MatchingWorldOffset);
            bs.WriteSByte(MatchingSpace);
            bs.WriteUInt32(MatchingEventID32);
            bs.WriteUInt64(ClubEventID);
            bs.WriteUInt32(EventSettingVersion);
            bs.WriteUInt32(EventSettingHash);
            bs.WriteUInt64(0);
            bs.WriteInt32(SceneryCourseCode);
        }
    }
}
