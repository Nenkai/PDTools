using PDTools.Files.Textures;
using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using PDTools.Utils;
using System.Diagnostics;

namespace PDTools.Files.Textures.PS2
{
    public class TextureSet1
    {
        public List<PGLUtexture> pGLUtextures { get; set; } = new List<PGLUtexture>();

        public void FromStream(byte[] data)
        {
            BitStream bs = new BitStream(BitStreamMode.Read, data, BitStreamSignificantBitOrder.MSB);
            string magic = bs.ReadStringRaw(4);
            if (magic != "Tex1")
                throw new InvalidDataException("Expected Tex1 magic");


            int relocPtr = bs.ReadInt32();
            int empty = bs.ReadInt32();
            int fileSize = bs.ReadInt32();

            short runtimeBufferSizeUnk = bs.ReadInt16();
            short bufferSizeToAllocate = bs.ReadInt16();
            short pgluTextureCOunt = bs.ReadInt16();
            short bufferInfoCount = bs.ReadInt16();
            int pgluTextureMapOffset = bs.ReadInt32();
            int bufferInfoMapOffset = bs.ReadInt32();

            bs.Position = pgluTextureMapOffset;

            for (var i = 0; i < pgluTextureCOunt; i++)
            {
                PGLUtexture tex = new PGLUtexture();
                tex.Read(ref bs);
            }
        }
    }

    public class PGLUtexture
    {
        public sceGsTex0 tex0 { get; set; } = new();
        public sceGsTex1 tex1 { get; set; } = new();
        public sceGsMiptbp1 MipTable1 { get; set; } = new();
        public sceGsMiptbp2 MipTable2 { get; set; } = new();
        public sceGsClamp ClampSettings { get; set; } = new();

        public void Read(ref BitStream stream)
        {
            tex0.Read(ref stream);
            tex1.Read(ref stream);
            MipTable1.Read(ref stream);
            MipTable2.Read(ref stream);
            ClampSettings.Read(ref stream);
        }
    }
}
