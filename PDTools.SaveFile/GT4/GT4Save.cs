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
        public GT4GameData GameData { get; set; } = new GT4GameData();
        public GarageFile GarageFile { get; set; } = new GarageFile();

        public static GT4Save Load(string directory)
        {
            string saveFilePath = Path.Combine(directory, "BASCUS-97436GAMEDATA");
            if (!File.Exists(saveFilePath))
                throw new FileNotFoundException("Main save file is missing from directory.");

            GT4Save save = new GT4Save();
            save.GameData.LoadFile(saveFilePath);

            string garageFilePath = Path.Combine(directory, "garage");
            if (!File.Exists(saveFilePath))
                throw new FileNotFoundException("Garage file is missing from directory.");

            save.GarageFile.Load(garageFilePath, 0x240C8E8D); // TODO: Grab key from GameData file

            return save;
        }
    }
}
