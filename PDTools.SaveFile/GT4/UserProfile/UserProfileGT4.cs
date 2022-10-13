using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData;
using Syroot.BinaryData.Memory;
using PDTools.Utils;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class UserProfileGT4 : IGameSerializeBase
    {
        public const int MAX_USERNAME_LENGTH = 32;
        public string UserName { get; set; }

        public const int MAX_UNUSED_PASSWORD_LENGTH = 8;
        public string UnusedPassword { get; set; }

        public const int MAX_ENTRY_NAME_LENGTH = 16;
        public string LastEntryName { get; set; }

        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }
        public int Unk4 { get; set; }
        public int Unk5 { get; set; }
        public int Unk6 { get; set; }
        public int Unk7 { get; set; }
        public int Unk8 { get; set; }

        public byte[] Unk { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }

        public byte[] UnkData { get; set; }

        public Calendar Calendar { get; set; } = new Calendar();
        public GarageScratch Garage { get; set; } = new GarageScratch();
        public RaceRecord RaceRecords { get; set; } = new RaceRecord();
        public CourseRecordBase CourseRecords1 { get; set; } = new CourseRecordBase(128);
        public CourseRecordBase CourseRecords2 { get; set; } = new CourseRecordBase(4);
        public LicenseRecord LicenseRecords { get; set; } = new LicenseRecord();
        public Available AvailableCarsAndCourses { get; set; } = new Available();
        public Favorite FavoriteCourses { get; set; } = new Favorite();
        public Favorite FavoriteCars { get; set; } = new Favorite();
        public Present Presents { get; set; } = new Present();
        public ChampionshipContext CurrentChampionship { get; set; } = new ChampionshipContext();
        public UsedCar UsedCar { get; set; } = new UsedCar();

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteStringFix(UserName, MAX_USERNAME_LENGTH);
            sw.WriteStringFix(UnusedPassword, MAX_UNUSED_PASSWORD_LENGTH);
            sw.WriteStringFix(LastEntryName, MAX_ENTRY_NAME_LENGTH);

            sw.WriteInt32(Unk1);
            sw.WriteInt32(Unk2);
            sw.WriteInt32(Unk3);
            sw.WriteInt32(Unk4);
            sw.WriteInt32(Unk5);
            sw.WriteInt32(Unk6);
            sw.WriteInt32(Unk7);
            sw.WriteInt32(Unk8);

            sw.WriteBytes(Unk);
            sw.WriteStringFix(Account, 32);
            sw.WriteStringFix(Password, 32);

            sw.WriteBytes(UnkData);
            sw.Align(GT4Save.ALIGNMENT);

            Calendar.Pack(save, ref sw);
            Garage.Pack(save, ref sw);
            RaceRecords.Pack(save, ref sw);
            CourseRecords1.Pack(save, ref sw);
            CourseRecords2.Pack(save, ref sw);
            LicenseRecords.Pack(save, ref sw);
            AvailableCarsAndCourses.Pack(save, ref sw);
            FavoriteCourses.Pack(save, ref sw);
            FavoriteCars.Pack(save, ref sw);
            Presents.Pack(save, ref sw);
            CurrentChampionship.Pack(save, ref sw);
            UsedCar.Pack(save, ref sw);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            UserName = sr.ReadFixedString(MAX_USERNAME_LENGTH);
            UnusedPassword = sr.ReadFixedString(MAX_UNUSED_PASSWORD_LENGTH);
            LastEntryName = sr.ReadFixedString(MAX_ENTRY_NAME_LENGTH);

            Unk1 = sr.ReadInt32();
            Unk2 = sr.ReadInt32();
            Unk3 = sr.ReadInt32();
            Unk4 = sr.ReadInt32();
            Unk5 = sr.ReadInt32();
            Unk6 = sr.ReadInt32();
            Unk7 = sr.ReadInt32();
            Unk8 = sr.ReadInt32();

            Unk = sr.ReadBytes(36);
            Account = sr.ReadFixedString(32);
            Password = sr.ReadFixedString(32);

            if (save.IsGT4Retail())
                UnkData = sr.ReadBytes(0x15C);
            else
                UnkData = sr.ReadBytes(0x19C);
            sr.Align(GT4Save.ALIGNMENT);

            Calendar.Unpack(save, ref sr);
            Garage.Unpack(save, ref sr);
            RaceRecords.Unpack(save, ref sr);
            CourseRecords1.Unpack(save, ref sr);
            CourseRecords2.Unpack(save, ref sr);
            LicenseRecords.Unpack(save, ref sr);
            AvailableCarsAndCourses.Unpack(save, ref sr);
            FavoriteCourses.Unpack(save, ref sr);
            FavoriteCars.Unpack(save, ref sr);
            Presents.Unpack(save, ref sr);
            CurrentChampionship.Unpack(save, ref sr);
            UsedCar.Unpack(save, ref sr);
        }

        
    }

}
