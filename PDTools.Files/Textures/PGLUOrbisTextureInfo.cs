using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Textures
{
    public class PGLUOrbisTextureInfo : PGLUTextureInfo
    {
        public GNFSurfaceFormat SurfaceFormat { get; set; }
        public GNFRenderTargetChannelType ChannelType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int SamplerModulationFormat { get; set; }
        public GNFTextureChannel ChannelOrderX { get; set; }
        public GNFTextureChannel ChannelOrderY { get; set; }
        public GNFTextureChannel ChannelOrderZ { get; set; }
        public GNFTextureChannel ChannelOrderW { get; set; }
        public int BaseMipLevel { get; set; }
        public int LastMipLevel { get; set; }
        public GNFTileMode TileMode { get; set; }
        public bool IsPaddedToPow2 { get; set; }
        public GNFTextureType TextureType { get; set; }

        public int Depth { get; set; }
        public int Pitch { get; set; }
        public int ImageSize { get; set; }

        public override void Read(BinaryStream bs)
        {
            bs.ReadInt32(); // Nothing
            int bits = bs.ReadInt32();
            SurfaceFormat = (GNFSurfaceFormat)((bits >> 20) & 0b111111);
            ChannelType = (GNFRenderTargetChannelType)((bits >> 26) & 0b1111);
            // 2 bits empty


            bits = bs.ReadInt32();
            Width = (bits & 0b111111_11111111) + 1; // 14 bits
            Height = ((bits >> 14) & 0b111111_11111111) + 1; // 14 bits
            SamplerModulationFormat = (bits >> 28) & 0b1111; // 4 bits

            bits = bs.ReadInt32();
            ChannelOrderX = (GNFTextureChannel)((bits) & 0b111);
            ChannelOrderY = (GNFTextureChannel)((bits >> 3) & 0b111);
            ChannelOrderZ = (GNFTextureChannel)((bits >> 6) & 0b111);
            ChannelOrderW = (GNFTextureChannel)((bits >> 9) & 0b111);
            BaseMipLevel = ((bits >> 12) & 0b1111);
            LastMipLevel = ((bits >> 16) & 0b1111);
            TileMode = (GNFTileMode)((bits >> 20) & 0b11111);
            IsPaddedToPow2 = (bits >> 25 & 1) == 1;
            TextureType = (GNFTextureType)((bits >> 28) & 0b1111);

            bits = bs.ReadInt32();
            Depth = bits & 0b11111_11111111;
            Pitch = ((bits >> 13) & 0b111111_11111111) + 1;
            bs.Position += 8;

            ImageSize = bs.ReadInt32();
        }

        public override void Write(BinaryStream bs)
        {
            throw new NotImplementedException();
        }
    }
}
