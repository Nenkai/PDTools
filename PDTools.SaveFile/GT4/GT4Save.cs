using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.InteropServices;

using PDTools.Crypto;
using PDTools.Utils;

namespace PDTools.SaveFile.GT4
{
    public class GT4Save
    {
        public const int ALIGNMENT = 0x10;

        public GT4GameType GameType { get; set; }
        public string GameDataName { get; set; }

        public GT4GameData GameData { get; set; } = new GT4GameData();
        public GarageFile GarageFile { get; set; } = new GarageFile();

        public static Dictionary<string, GT4GameType> GameDataRegionNames = new Dictionary<string, GT4GameType>()
        {
            { "BE" + "SCES-51719" + "GAMEDATA", GT4GameType.GT4_EU }, // GT4 EU
            { "BA" + "SCUS-97328" + "GAMEDATA", GT4GameType.GT4_US }, // GT4 US
            { "BI" + "SCPS-17001" + "GAMEDATA", GT4GameType.GT4_JP }, // GT4 JP
            { "BK" + "SCKA-30001" + "GAMEDATA", GT4GameType.GT4_KR }, // GT4 KR
            { "BA" + "SCUS-97436" + "GAMEDATA", GT4GameType.GT4O_US }, // GT4 Onilne Beta Test US
            { "BI" + "PAPX-90523" + "GAMEDATA", GT4GameType.GT4O_JP }, // GT4 Online Test Version JP
        };

        public static GT4Save Load(string directory)
        {
            string gameDataName = "";
            GT4GameType gameType = GT4GameType.Unknown;

            foreach (var saveFileName in GameDataRegionNames)
            {
                string saveFilePath = Path.Combine(directory, saveFileName.Key);
                if (File.Exists(saveFilePath))
                {
                    gameDataName = saveFileName.Key;
                    gameType = saveFileName.Value;
                    break;
                }
            }
            
            if (string.IsNullOrEmpty(gameDataName))
                throw new FileNotFoundException("Main save file is missing from directory.");

            GT4Save save = new GT4Save();
            save.GameType = gameType;
            save.GameDataName = gameDataName;
            save.GameData.LoadFile(save, Path.Combine(directory, gameDataName));

            string garageFilePath = Path.Combine(directory, "garage");
            if (!File.Exists(garageFilePath))
                throw new FileNotFoundException("Garage file is missing from directory.");

            save.GarageFile.Load(save, garageFilePath, save.GameData.Profile.Garage.UniqueID, save.GameData.UseOldRandomUpdateCrypto);

            return save;
        }

        public void SaveToDirectory(string directory)
        {
            string gameDataPath = Path.Combine(directory, GameDataName);
            GameData.SaveTo(this, gameDataPath);
            GarageFile.Save(Path.Combine(directory, "garage"));
        }

        public bool IsGT4Retail()
        {
            return GameType == GT4GameType.GT4_EU || GameType == GT4GameType.GT4_US || GameType == GT4GameType.GT4_JP || GameType == GT4GameType.GT4_KR;
        }

        public bool IsGT4Online()
        {
            return GameType == GT4GameType.GT4O_US || GameType == GT4GameType.GT4O_JP;
        }
    }

    public enum GT4GameType
    {
        Unknown,
        GT4_EU,
        GT4_US,
        GT4_JP,
        GT4_KR,
        GT4O_US,
        GT4O_JP,
    }
}
