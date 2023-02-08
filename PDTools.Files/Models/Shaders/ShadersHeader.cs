using PDTools.Files.Models.Shaders;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.Shaders
{
    public class ShadersHeader
    {
        public List<ShaderDefinition> Definitions { get; set; } = new();
        public List<ShadersProgram_0x20> Programs0x20 { get; set; } = new();
        public List<ShadersProgram_0x2C> Programs0x2C { get; set; } = new();
        public List<Shaders_0x3C> _0x3C { get; set; } = new();
        public List<ShadersProgram_0x40> Programs0x40 { get; set; } = new();

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

            shaders.ReadShaderDefinitions(bs, shaderDefinitionsOffset, shaderDefinitionCount, basePos);
            shaders.ReadShaderPrograms(bs, shaderProgramsOffset, shaderProgramCount, basePos);
            shaders.ReadUnkPrograms0x2C(bs, offset_0x2c, count_0x24_2c, basePos);
            shaders.ReadUnk0x3C(bs, offset_0x3c, count_0x3c, basePos);
            shaders.ReadUnkPrograms0x40(bs, offset_0x40, count_0x40, basePos);

            return shaders;
        }

        private void ReadShaderPrograms(BinaryStream bs, int offset, int count, long basePos)
        {
            for (var i = 0; i < count; i++)
            {
                bs.Position = basePos + offset + (i * 0x28);
                var prog = ShadersProgram_0x20.FromStream(bs, basePos);
                Programs0x20.Add(prog);
            }
        }

        private void ReadShaderDefinitions(BinaryStream bs, int offset, int count, long basePos)
        {
            for (var i = 0; i < count; i++)
            {
                bs.Position = basePos + offset + (i * 0x0C);
                var def = ShaderDefinition.FromStream(bs, basePos);
                Definitions.Add(def);
            }
        }

        private void ReadUnkPrograms0x2C(BinaryStream bs, int offset, int count, long basePos)
        {
            for (var i = 0; i < count; i++)
            {
                bs.Position = basePos + offset + (i * 0x48);
                var prog = ShadersProgram_0x2C.FromStream(bs, basePos);
                Programs0x2C.Add(prog);
            }
        }

        private void ReadUnk0x3C(BinaryStream bs, int offset, int count, long basePos)
        {
            for (var i = 0; i < count; i++)
            {
                bs.Position = basePos + offset + (i * 0x04);
                int entryOffset = bs.ReadInt32();

                bs.Position = basePos + entryOffset;
                var entry = Shaders_0x3C.FromStream(bs, basePos);
                _0x3C.Add(entry);
            }
        }

        private void ReadUnkPrograms0x40(BinaryStream bs, int offset, int count, long basePos)
        {
            for (var i = 0; i < count; i++)
            {
                bs.Position = basePos + offset + (i * 0x20);
                var prog = ShadersProgram_0x40.FromStream(bs, basePos);
                Programs0x40.Add(prog);
            }
        }
    }
}
