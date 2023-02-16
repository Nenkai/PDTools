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
    public class MDL3PMSHFormat
    {
        public short StructDeclarationID { get; set; }
        public short ElementBitLayoutDefinitionID { get; set; }

        public MDL3PMSHFormatData Data { get; set; }

        public static MDL3PMSHFormat FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3PMSHFormat format = new();

            format.StructDeclarationID = bs.ReadInt16();
            format.ElementBitLayoutDefinitionID = bs.ReadInt16();
            bs.ReadInt16();

            byte countOfUnk = bs.Read1Byte();
            bs.ReadByte(); // 1
            int unkOffset = bs.ReadInt32();
            float unk = bs.ReadSingle();
            int dataTocOffset = bs.ReadInt32();
            bs.Position += 0x18;

            // TODO read rest
            bs.Position = mdlBasePos + dataTocOffset;
            format.Data = MDL3PMSHFormatData.FromStream(bs, mdlBasePos, mdl3VersionMajor);

            return format;
        }

        public static int GetSize()
        {
            return 0x30;
        }
    }
}
