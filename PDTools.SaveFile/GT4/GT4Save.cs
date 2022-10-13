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

        public GT4GameData GameData { get; set; } = new GT4GameData();
        public GarageFile GarageFile { get; set; } = new GarageFile();

        public static Dictionary<string, GT4GameType> GameDataRegionNames = new Dictionary<string, GT4GameType>()
        {
            {"BESCES-51719GAMEDATA", GT4GameType.GT4_EU }, // GT4 EU
            {"BASCUS-97436GAMEDATA", GT4GameType.GT4_US }, // GT4 US, GT4O US
        };

        public static GT4Save Load(string directory)
        {
            string gameDataFilePath = "";
            GT4GameType gameType = GT4GameType.Unknown;

            foreach (var saveFileName in GameDataRegionNames)
            {
                string saveFilePath = Path.Combine(directory, saveFileName.Key);
                if (File.Exists(saveFilePath))
                {
                    gameDataFilePath = saveFileName.Key;
                    gameType = saveFileName.Value;
                    break;
                }
            }
            
            if (string.IsNullOrEmpty(gameDataFilePath))
                throw new FileNotFoundException("Main save file is missing from directory.");

            GT4Save save = new GT4Save();
            save.GameType = gameType;
            save.GameData.LoadFile(save, Path.Combine(directory, gameDataFilePath));

            string garageFilePath = Path.Combine(directory, "garage");
            if (!File.Exists(garageFilePath))
                throw new FileNotFoundException("Garage file is missing from directory.");

            save.GarageFile.Load(garageFilePath, 0x240C8E8D); // TODO: Grab key from GameData file

            return save;
        }

        public bool IsGT4Retail()
        {
            return GameType == GT4GameType.GT4_EU || GameType == GT4GameType.GT4_US;
        }

        public bool IsGT4Online()
        {
            return GameType == GT4GameType.GT4O_US;
        }
    }

    public enum GT4GameType
    {
        Unknown,
        GT4_EU,
        GT4_US,
        GT4O_US
    }
}
