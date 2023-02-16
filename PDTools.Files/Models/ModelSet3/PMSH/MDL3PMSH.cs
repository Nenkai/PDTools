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
    public class MDL3PMSH
    {
        public List<MDL3PMSHFormat> Formats { get; set; } = new();


        public static MDL3PMSH FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3PMSH pmsh = new();

            if (bs.ReadInt32() != 0x504d5348)
                throw new Exception("Expected PMSH magic.");

            int unk = bs.ReadInt32();
            int relocSize = bs.ReadInt32();
            bs.ReadInt32(); // Reloc ptr
            short formatCount = bs.ReadInt16();
            short unkCount0x1C = bs.ReadInt16();
            short elementBitLayoutDefinitionCount = bs.ReadInt16();
            short structDeclarationCount = bs.ReadInt16();

            int formatsOffset = bs.ReadInt32();
            int unkOffset0x1C = bs.ReadInt32();
            int elementBitLayoutDefinitionsOffset = bs.ReadInt32();
            int structDeclarationsOffset = bs.ReadInt32();
            int unkOffset0x28 = bs.ReadInt32();
            bs.ReadInt32();
            int unkOffset0x30 = bs.ReadInt32();

            for (var i = 0; i < formatCount; i++)
            {
                bs.Position = mdlBasePos + formatsOffset + (i * MDL3PMSHFormat.GetSize());
                var format = MDL3PMSHFormat.FromStream(bs, mdlBasePos, mdl3VersionMajor);
                pmsh.Formats.Add(format);
            }

            return pmsh;
        }
    }
}
