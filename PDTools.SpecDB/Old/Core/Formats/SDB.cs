using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace SpecDBOld.Core.Formats
{
    /// <summary>
    /// String Database.
    /// </summary>
    public class SDB
    {
        private Endian _endian;
        public byte[] FileBuffer { get; set; }
        public const int HeaderSize = 0x10;

        public SDB(byte[] buffer, Endian endian)
        {
            FileBuffer = buffer;
            _endian = endian;
        }

        public string GetStringByID(int strID)
        {
            SpanReader sr = new SpanReader(FileBuffer, _endian);
            sr.Position = 4;

            int entryCount = sr.ReadInt32();
            if (strID < entryCount)
            {
                sr.Position = HeaderSize + (strID * sizeof(uint));
                int strOffset = sr.ReadInt32();

                sr.Position = HeaderSize + (entryCount * sizeof(uint));
                sr.Position += strOffset + 2; // Real implementation actually skips the short. No idea why.
                return sr.ReadString0();
            }

            return null;
        }
    }
}
