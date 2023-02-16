using PDTools.Files.Models.ModelSet3.Meshes;
using PDTools.Utils;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.ModelSet3.PMSH
{
    public class MDL3PMSHFormatData
    {
        public static MDL3PMSHFormatData FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3PMSHFormatData data = new();

            int vertsOffset = bs.ReadInt32();
            int unkOffset_0x04 = bs.ReadInt32();
            int unkOffset_0x08 = bs.ReadInt32();
            int unkOffset_0x0c = bs.ReadInt32();
            int vertColorsOffset = bs.ReadInt32();
            int unkOffset_0x14 = bs.ReadInt32();
            int unkOffset_0x18 = bs.ReadInt32();

            // TODO read rest
            bs.Position = vertsOffset;

            BitStream bits = new BitStream(BitStreamMode.Read, bs.ReadBytes(18));
            ulong b = bits.ReadBits(13);
            ulong b2 = bits.ReadBits(13);
            ulong b3 = bits.ReadBits(14);
            ulong b4 = bits.ReadBits(32);

            ulong c = bits.ReadBits(13);
            ulong c2 = bits.ReadBits(13);
            ulong c3 = bits.ReadBits(14);
            ulong c4 = bits.ReadBits(32);
            return data;
        }

        public static int GetSize()
        {
            return 0x30;
        }
    }
}
