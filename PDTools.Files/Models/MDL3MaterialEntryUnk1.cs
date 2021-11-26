using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models
{
    public class MDL3MaterialEntryUnk1
    {
        public short UnkIndexForParent0x0CMap { get; set; }

        public ushort Flags { get; set; }

        public Dictionary<string, uint> Entries { get; set; }  = new();
        public static MDL3MaterialEntryUnk1 FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3MaterialEntryUnk1 entry = new();
            uint offset_0x00 = bs.ReadUInt32();
            entry.UnkIndexForParent0x0CMap = bs.ReadInt16();
            bs.ReadInt16(); // Unk
            entry.Flags = bs.ReadUInt16();

            ushort keyCount = bs.ReadUInt16();
            uint keyOffset = bs.ReadUInt32();

            for (int i = 0; i < keyCount; i++)
            {
                bs.Position = mdlBasePos + keyOffset + (i * 0x08);
                uint nameOffset = bs.ReadUInt32();
                uint imageParamIndex = bs.ReadUInt32();

                bs.Position = mdlBasePos + nameOffset;
                entry.Entries.Add(bs.ReadString(StringCoding.ZeroTerminated), imageParamIndex);
            }

            return entry;
        }
    }
}
