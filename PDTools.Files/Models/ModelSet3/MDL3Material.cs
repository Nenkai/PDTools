using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3
{
    public class MDL3Material
    {
        public string Name { get; set; }
        public short UnkIndexForParent0x0CMap { get; set; }
        public short UnkIndexForParent0x10Map { get; set; }
        public ushort Flags { get; set; }

        public Dictionary<string, uint> Entries { get; set; } = new();
        public static MDL3Material FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3Material entry = new();
            uint nameOffset = bs.ReadUInt32();
            entry.UnkIndexForParent0x0CMap = bs.ReadInt16();
            entry.UnkIndexForParent0x10Map = bs.ReadInt16();
            entry.Flags = bs.ReadUInt16();

            ushort keyCount = bs.ReadUInt16();
            uint keyOffset = bs.ReadUInt32();

            bs.Position = mdlBasePos + nameOffset;
            entry.Name = bs.ReadString(StringCoding.ZeroTerminated);

            for (int i = 0; i < keyCount; i++)
            {
                bs.Position = mdlBasePos + keyOffset + i * 0x08;
                uint entryNameOffset = bs.ReadUInt32();
                uint pgluTextureID = bs.ReadUInt32();

                bs.Position = mdlBasePos + entryNameOffset;
                entry.Entries.Add(bs.ReadString(StringCoding.ZeroTerminated), pgluTextureID);
            }

            return entry;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
