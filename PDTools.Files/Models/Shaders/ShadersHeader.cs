using PDTools.Files.Models.Shaders;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models
{
    public class ShadersHeader
    {
        public List<ShadersProgram0x40> Programs0x40 { get; set; } = new();

        public static ShadersHeader FromStream(BinaryStream bs, long basePos = 0)
        {
            string magic = bs.ReadString(4);
            if (magic == "SHDS")
                bs.ByteConverter = ByteConverter.Big;
            else
                throw new InvalidDataException("Not a valid SHDS header.");

            /* HEADER - 0xE4 */
            ShadersHeader shaders = new();

            bs.ReadInt32();
            bs.ReadInt32(); // Reloc ptr
            bs.ReadInt32(); // Empty
            int offset0x10 = bs.ReadInt32(); // Offset 0x10;
            bs.ReadInt16(); // Empty
            short shaderDefinitionCount = bs.ReadInt16(); // Shader definition count
            short shaderProgramCount = bs.ReadInt16(); // Shader program count
            short count_0x24_2c = bs.ReadInt16(); // Unk count 0x24_2c
            int shaderDefinitionsOffset = bs.ReadInt32(); // Shader def offset 0x1c
            int shaderProgramsOffset = bs.ReadInt32(); // Shader programs offset 0x20
            int offset_0x24 = bs.ReadInt32();
            bs.ReadInt32(); // Runtime offset
            int offset_0x2c = bs.ReadInt32();
            short count_0x38 = bs.ReadInt16();
            short count_0x3c = bs.ReadInt16();
            short count_0x40 = bs.ReadInt16();
            bs.ReadInt16(); // pad

            int offset_0x38 = bs.ReadInt32();
            int offset_0x3c = bs.ReadInt32();
            int offset_0x40 = bs.ReadInt32();

            shaders.ReadUnkPrograms0x40(bs, offset_0x40, count_0x40, basePos);

            return shaders;
        }

        private void ReadUnkPrograms0x40(BinaryStream bs, int offset, int count, long basePos)
        {
            for (var i = 0; i < count; i++)
            {
                bs.Position = basePos + offset + (i * 0x20);
                var prog = ShadersProgram0x40.FromStream(bs, basePos);
                Programs0x40.Add(prog);
            }
        }
    }
}
