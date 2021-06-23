using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

using Syroot.BinaryData;
using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace SpecDBOld.Core
{
    public class StringDatabase
    {
        public Endian Endian { get; set; }
        public int Version { get; set; }
        public ObservableCollection<string> Strings { get; set; } = new ObservableCollection<string>();
        public static StringDatabase LoadFromFile(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Open))
            using (var bs = new BinaryStream(file))
            {
                if (bs.ReadString(4) != "GTST")
                    throw new InvalidDataException($"Invalid SDB file '{fileName}'. Magic does not match.");

                var strDb = new StringDatabase();
                bs.Position = 0x08;
                strDb.Version = bs.ReadInt32();
                strDb.Endian = strDb.Version == 1 ? Endian.Little : Endian.Big;
                if (strDb.Endian == Endian.Big)
                    bs.ByteConverter = ByteConverter.Big;

                bs.Position = 0x04;
                int entryCount = bs.ReadInt32();
                
                bs.Position = 0x10;
                for (int i = 0; i < entryCount; i++)
                {
                    int strOffset = bs.ReadInt32();
                    using (var seek = bs.TemporarySeek(0x10 + (entryCount * sizeof(uint) + strOffset), SeekOrigin.Begin))
                    {
                        seek.Stream.Position += 2;
                        strDb.Strings.Add(seek.Stream.ReadString(StringCoding.ZeroTerminated));
                    }
                }

                return strDb;
            }
        }

        public int GetOrCreate(string str)
        {
            int index = Strings.IndexOf(str);
            if (index == -1)
            {
                Strings.Add(str);
                index = Strings.Count - 1;
            }
            return index;
        }
    }
}
