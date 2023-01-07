using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Textures
{
    /// <summary>
    /// Used in Gran Turismo 7
    /// </summary>
    public class PDITexture
    {
        public TextureSet3 TextureSet { get; set; } = new TextureSet3();

        public void FromFile(string filename)
        {
            using var fs = File.Open(filename, FileMode.Open);
            FromStream(fs);
        }

        public void FromStream(Stream stream)
        {
            using var bs = new BinaryStream(stream);
            uint magic = bs.ReadUInt32();

            if (magic != 0x30504449)
                throw new InvalidDataException("Not a PDI0 Texture");

            int type = bs.ReadInt32();
            long offsetToc = bs.ReadInt64();

            bs.Position = offsetToc;
            long extractColorNameOffset = bs.ReadInt64();
            long extractColorOffset = bs.ReadInt64();
            long extractColorSize = bs.ReadInt64();

            long textureSetNameOffset = bs.ReadInt64();
            long textureSetOffset = bs.ReadInt64();
            long textureSetSize = bs.ReadInt64();

            bs.Position = textureSetOffset;
            TextureSet.FromStream(bs, TextureSet3.TextureConsoleType.PS4);
        }
    }
}
