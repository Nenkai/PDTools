using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

using System.IO;

namespace PDTools.Files.Models.PS3.ModelSet3
{
    public class MDL3ModelVMUnk_0x04
    {
        public string Name { get; set; }
        public short StackStorage2Index { get; set; }
        public List<MDL3ModelVMUnk_0x04_Data> Data { get; set; } = new();

        public static MDL3ModelVMUnk_0x04 FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3ModelVMUnk_0x04 unk = new();

            int nameOffset = bs.ReadInt32();
            unk.StackStorage2Index = bs.ReadInt16();
            int materialRefCount = bs.ReadInt16();

            int dataOffset = bs.ReadInt32();
            float emptyUntilRuntime = bs.ReadInt32();

            bs.Position = mdlBasePos + nameOffset;
            unk.Name = bs.ReadString(StringCoding.ZeroTerminated);

            for (var i = 0; i < materialRefCount; i++)
            {
                bs.Position = dataOffset + materialRefCount * 0x10;
                // short material data index
                // short material data 0x0c index
                // 0xb8 indices

                var data = MDL3ModelVMUnk_0x04_Data.FromStream(bs, mdlBasePos, mdl3VersionMajor);
                unk.Data.Add(data);
            }

            return unk;
        }

        public static int GetSize()
        {
            return 0x10;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
