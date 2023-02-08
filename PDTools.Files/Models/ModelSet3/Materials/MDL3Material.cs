using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Materials
{
    public class MDL3Material
    {
        public string Name { get; set; }
        public short MaterialDataID { get; set; }
        public short CellGcmParamsID { get; set; }
        public ushort Flags { get; set; }

        public Dictionary<string, uint> ImageEntries { get; set; } = new();
        public static MDL3Material FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3Material entry = new();
            uint nameOffset = bs.ReadUInt32();
            entry.MaterialDataID = bs.ReadInt16();
            entry.CellGcmParamsID = bs.ReadInt16();
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
                entry.ImageEntries.Add(bs.ReadString(StringCoding.ZeroTerminated), pgluTextureID);
            }

            return entry;
        }

        public static int GetSize()
        {
            return 0x34;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
