using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;

using PDTools.Crypto;
using PDTools.Utils;
using PDTools.Enums.PS2;

namespace PDTools.SaveFile.GT4
{
    public class GT4Save
    {
        public const int ALIGNMENT = 0x10;

        public GT4SaveType Type { get; set; }
        public string GameDataName { get; set; }

        public GT4GameData GameData { get; set; } = new GT4GameData();
        public GarageFile GarageFile { get; set; } = new GarageFile();

        public static Dictionary<string, GT4SaveType> GameDataRegionNames = new Dictionary<string, GT4SaveType>()
        {
            { "BE" + "SCES-51719" + "GAMEDATA", GT4SaveType.GT4_EU }, // GT4 EU
            { "BA" + "SCUS-97328" + "GAMEDATA", GT4SaveType.GT4_US }, // GT4 US
            { "BI" + "SCPS-17001" + "GAMEDATA", GT4SaveType.GT4_JP }, // GT4 JP
            { "BK" + "SCKA-30001" + "GAMEDATA", GT4SaveType.GT4_KR }, // GT4 KR
            { "BA" + "SCUS-97436" + "GAMEDATA", GT4SaveType.GT4O_US }, // GT4 Onilne Beta Test US
            { "BI" + "PAPX-90523" + "GAMEDATA", GT4SaveType.GT4O_JP }, // GT4 Online Test Version JP
        };

        public static GT4Save Load(string directory)
        {
            string gameDataName = DetectGameTypeFromSaveDirectory(directory);

            if (string.IsNullOrEmpty(gameDataName))
                throw new FileNotFoundException("Main save file is missing from directory.");

            GT4Save save = new GT4Save();
            save.Type = GameDataRegionNames[gameDataName];
            save.GameDataName = gameDataName;
            save.GameData.LoadFile(save, Path.Combine(directory, gameDataName));

            string garageFilePath = Path.Combine(directory, "garage");
            if (!File.Exists(garageFilePath))
                throw new FileNotFoundException("Garage file is missing from directory.");

            save.GarageFile.Load(save, garageFilePath, save.GameData.Profile.Garage.UniqueID, save.GameData.UseOldRandomUpdateCrypto);

            return save;
        }

        public static string DetectGameTypeFromSaveDirectory(string directory)
        {
            string gameDataName = "";

            foreach (var saveFileName in GameDataRegionNames)
            {
                string saveFilePath = Path.Combine(directory, saveFileName.Key);
                if (File.Exists(saveFilePath))
                {
                    gameDataName = saveFileName.Key;
                    break;
                }
            }

            return gameDataName;
        }

        public void SaveToDirectory(string directory)
        {
            string gameDataPath = Path.Combine(directory, GameDataName);
            GameData.SaveTo(this, gameDataPath);
            GarageFile.Save(Path.Combine(directory, "garage"));
        }

        public void CopyTo(GT4Save dest)
        {
            dest.Type = Type;
            dest.GameDataName = GameDataName;

            GameData.CopyTo(dest.GameData);
            GarageFile.CopyTo(dest.GarageFile);
        }

        public void ConvertToType(GT4SaveType type)
        {
            bool clearAllCarData = GT4Save.IsGT4Online(Type) != GT4Save.IsGT4Online(type);
            Type = type;
            GameDataName = GT4Save.GameDataRegionNames.FirstOrDefault(x => x.Value == type).Key;

            // Clear garage data since we can't decrypt it reliably
            if (clearAllCarData)
                GameData.Profile.Garage.ClearAllCarData();

            GarageFile.IdentifyGarageCarSize(type);

            // Adjust GameZone & language
            switch (type)
            {
                case GT4SaveType.Unknown:
                    GameData.Option.GameZone.GameZoneType = GameZoneType.JP;
                    GameData.Option.language = Locale.JP;
                    break;
                case GT4SaveType.GT4_EU:
                    GameData.Option.GameZone.GameZoneType = GameZoneType.EU;
                    GameData.Option.language = Locale.GB;
                    break;
                case GT4SaveType.GT4_US:
                    GameData.Option.GameZone.GameZoneType = GameZoneType.US;
                    GameData.Option.language = Locale.US;
                    break;
                case GT4SaveType.GT4_JP:
                    GameData.Option.GameZone.GameZoneType = GameZoneType.JP;
                    GameData.Option.language = Locale.JP;
                    break;
                case GT4SaveType.GT4_KR:
                    GameData.Option.GameZone.GameZoneType = GameZoneType.KR;
                    GameData.Option.language = Locale.KR;
                    break;
                case GT4SaveType.GT4O_US:
                    GameData.Option.GameZone.GameZoneType = GameZoneType.US;
                    GameData.Option.language = Locale.US;

                    GameData.Option.unk_udpdata = new byte[0x10];
                    GameData.Option.entrance_addr = "gt4-pubeta.muis.pdonline.scea.com";
                    GameData.Option.entrance_port = 10071;

                    GameData.Option.display_license_bestline = 1;

                    for (var i = 0; i < 3; i++)
                        GameData.Profile.Garage.CurrentCar.Sheets[i].Unk_GT4OData = new byte[0x18];

                    break;
                case GT4SaveType.GT4O_JP:
                    GameData.Option.GameZone.GameZoneType = GameZoneType.JP;
                    GameData.Option.language = Locale.JP;

                    GameData.Option.unk_udpdata = new byte[0x10];
                    GameData.Option.entrance_addr = "gt4online-muis.scej-online.jp";
                    GameData.Option.entrance_port = 10071;

                    GameData.Option.display_license_bestline = 1;

                    for (var i = 0; i < 3; i++)
                        GameData.Profile.Garage.CurrentCar.Sheets[i].Unk_GT4OData = new byte[0x18];
                    break;
                default:
                    throw new NotImplementedException("Not implemented");
            }

            for (var i = 0; i < GarageFile.Cars.Count; i++)
                GarageFile.Cars[i] = new byte[GarageFile.GarageCarSizeAligned];

            GameData.UseOldRandomUpdateCrypto = GT4Save.IsGT4Retail(type);
            GarageFile.UseOldRandomUpdateCrypto = GameData.UseOldRandomUpdateCrypto;
        }

        public static bool IsGT4Retail(GT4SaveType saveType)
        {
            return saveType == GT4SaveType.GT4_EU || saveType == GT4SaveType.GT4_US || saveType == GT4SaveType.GT4_JP || saveType == GT4SaveType.GT4_KR;
        }

        public static bool IsGT4Online(GT4SaveType saveType)
        {
            return saveType == GT4SaveType.GT4O_US || saveType == GT4SaveType.GT4O_JP;
        }
    }

    public enum GT4SaveType
    {
        Unknown,

        [Description("GT4 Europe PAL (SCES-51719)")]
        GT4_EU,

        [Description("GT4 US NTSC-U (SCUS-97328)")]
        GT4_US,

        [Description("GT4 Japan NTSC-J (SCPS-17001)")]
        GT4_JP,

        [Description("GT4 Korea NTSC-J (SCKA-30001)")]
        GT4_KR,

        [Description("GT4 Online US (SCUS-97436)")]
        GT4O_US,

        [Description("GT4 Online Japan (PAPX-90523)")]
        GT4O_JP,
    }
}
