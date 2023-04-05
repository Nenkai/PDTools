using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData;
using Syroot.BinaryData.Memory;
using PDTools.Utils;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class UserProfileGT4 : IGameSerializeBase<UserProfileGT4>
    {
        public const int MAX_USERNAME_LENGTH = 32;
        public string UserName { get; set; }

        public const int MAX_PASSWORD_LENGTH = 8;
        public string Password { get; set; }

        public const int MAX_ENTRY_NAME_LENGTH = 16;
        public string LastEntryName { get; set; }

        public long Score { get; set; }
        public long TotalPrizeMoney { get; set; }
        public int TotalPrizeCars { get; set; }
        public int[] RankCounts { get; set; } = new int[7];
        public long TotalASpecDistanceMeters { get; set; }
        public long TotalBSpecDistanceMeters { get; set; }
        public bool WithdrawnGT3 { get; set; }
        public bool WithdrawnGT4P { get; set; }
        public int MetType { get; set; }

        public byte[] BSpecData { get; set; }

        public byte[] UnkGT4OData { get; set; } = new byte[0x40];
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

        public void CopyTo(UserProfileGT4 dest)
        {
            dest.UserName = UserName;
            dest.Password = Password;
            dest.LastEntryName = LastEntryName;

            dest.Score = Score;
            dest.TotalPrizeMoney = TotalPrizeMoney;
            dest.TotalPrizeCars = TotalPrizeCars;
            Array.Copy(RankCounts, dest.RankCounts, RankCounts.Length);
            dest.TotalASpecDistanceMeters = TotalASpecDistanceMeters;
            dest.TotalBSpecDistanceMeters = TotalBSpecDistanceMeters;
            dest.WithdrawnGT3 = WithdrawnGT3;
            dest.WithdrawnGT4P = WithdrawnGT4P;
            dest.MetType = MetType;

            dest.BSpecData = new byte[BSpecData.Length];
            Array.Copy(BSpecData, dest.BSpecData, BSpecData.Length);
            Array.Copy(UnkGT4OData, dest.UnkGT4OData, UnkGT4OData.Length);

            Calendar.CopyTo(dest.Calendar);
            Garage.CopyTo(dest.Garage);
            RaceRecords.CopyTo(dest.RaceRecords);
            CourseRecords1.CopyTo(dest.CourseRecords1);
            CourseRecords2.CopyTo(dest.CourseRecords2);
            LicenseRecords.CopyTo(dest.LicenseRecords);
            AvailableCarsAndCourses.CopyTo(dest.AvailableCarsAndCourses);
            FavoriteCourses.CopyTo(dest.FavoriteCourses);
            FavoriteCars.CopyTo(dest.FavoriteCars);
            Presents.CopyTo(dest.Presents);
            CurrentChampionship.CopyTo(dest.CurrentChampionship);
            UsedCar.CopyTo(dest.UsedCar);
        }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteStringFix(UserName, MAX_USERNAME_LENGTH);
            sw.WriteStringFix(Password, MAX_PASSWORD_LENGTH);
            sw.WriteStringFix(LastEntryName, MAX_ENTRY_NAME_LENGTH);
            sw.WriteInt64(Score);
            sw.WriteInt64(TotalPrizeMoney);
            sw.WriteInt32(TotalPrizeCars);

            for (var i = 0; i < RankCounts.Length; i++)
                sw.WriteInt32(RankCounts[i]);

            sw.WriteInt64(TotalASpecDistanceMeters);
            sw.WriteInt64(TotalBSpecDistanceMeters);
            sw.WriteBoolean4(WithdrawnGT3);
            sw.WriteBoolean4(WithdrawnGT4P);
            sw.WriteInt32(MetType);
            sw.Position += 0x10;

            if (GT4Save.IsGT4Online(save.Type))
                sw.WriteBytes(UnkGT4OData);

            sw.WriteBytes(BSpecData);
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
            Password = sr.ReadFixedString(MAX_PASSWORD_LENGTH);
            LastEntryName = sr.ReadFixedString(MAX_ENTRY_NAME_LENGTH);
            Score = sr.ReadInt64();
            TotalPrizeMoney = sr.ReadInt64();
            TotalPrizeCars = sr.ReadInt32();

            for (var i = 0; i < 7; i++)
                RankCounts[i] = sr.ReadInt32();

            TotalASpecDistanceMeters = sr.ReadInt64();
            TotalBSpecDistanceMeters = sr.ReadInt64();
            WithdrawnGT3 = sr.ReadBoolean4();
            WithdrawnGT4P = sr.ReadBoolean4();
            MetType = sr.ReadInt32();
            sr.Position += 0x10;

            if (GT4Save.IsGT4Online(save.Type))
                UnkGT4OData = sr.ReadBytes(0x40);

            BSpecData = sr.ReadBytes((2 * (sizeof(long) * 8)) + (2 * (sizeof(long)) * 8) + (2 * (sizeof(long)) * 8));
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
