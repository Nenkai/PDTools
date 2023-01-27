using Syroot.BinaryData;

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Textures
{
    public class DdsHeader
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int PitchOrLinearSize { get; set; }
        public int LastMipmapLevel { get; set; }

        public DDSPixelFormatFlags FormatFlags { get; set; }
        public string FourCCName { get; set; }

        public int RGBBitCount { get; set; }
        public uint RBitMask { get; set; }
        public uint GBitMask { get; set; }
        public uint BBitMask { get; set; }
        public uint ABitMask { get; set; }

        public DDS_DXGI_FORMAT DxgiFormat { get; set; }

        public byte[] ImageData { get; set; }

        public void Write(Stream outStream)
        {
            var bs = new BinaryStream(outStream);

            bs.WriteString("DDS ", StringCoding.Raw);
            bs.WriteInt32(124);    // dwSize (Struct Size)
            bs.WriteUInt32((uint)(DDSHeaderFlags.TEXTURE)); // dwFlags
            bs.WriteInt32(Height); // dwHeight

            // Dirty fix, some TXS3's in GTHD have 1920 as width, but its actually 2048. Stride is correct, so use it instead.
            bs.WriteInt32(Width);
            bs.WriteInt32(PitchOrLinearSize);
            bs.WriteInt32(0);    // Depth
            bs.WriteInt32(LastMipmapLevel);
            bs.WriteBytes(new byte[44]); // reserved
            bs.WriteInt32(32); // DDSPixelFormat Header starts here - Struct Size

            
            bs.WriteUInt32((uint)(FormatFlags));           // Format Flags
            bs.WriteString(FourCCName, StringCoding.Raw); // FourCC
            bs.WriteInt32(RGBBitCount);         // RGBBitCount

            bs.WriteUInt32(RBitMask);  // RBitMask 
            bs.WriteUInt32(GBitMask);  // GBitMask
            bs.WriteUInt32(BBitMask);  // BBitMask
            bs.WriteUInt32(ABitMask);  // ABitMask

            bs.WriteInt32(0x1000); // dwCaps, 0x1000 = required
            bs.WriteBytes(new byte[16]); // dwCaps1-4

            if (FourCCName == "DX10")
            {
                // DDS_HEADER_DXT10
                bs.WriteInt32((int)DxgiFormat);
                bs.WriteInt32(3);  // DDS_DIMENSION_TEXTURE2D
                bs.BaseStream.Seek(4, SeekOrigin.Current);  // miscFlag
                bs.WriteInt32(1); // arraySize
                bs.WriteInt32(0); // miscFlags2
            }

            bs.Write(ImageData);
        }
    }
}
