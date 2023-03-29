using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using PDTools.Utils;

namespace PDTools.Files.Textures.PS2
{
    public class sceGsTex0
    {
        public ulong TBP0_TextureBaseAddress;
        public ulong TBW_TextureBufferWidth;
        public SCE_GS_PSM PSM;
        public byte TW_TextureWidth;
        public byte TH_TextureHeight;
        public ulong TCC_ColorComponent;
        public ulong TFX_TextureFunction;
        public ulong CBP_BaseClutData;
        public ulong CPSM_ClutPartPixelFormatSetup;
        public ulong CSM_ClutStorageMode;
        public ulong CSA_ClutEntryOffset;
        public ulong CLD_ClutBufferLoadControl;

        public void Read(ref BitStream stream)
        {
            TBP0_TextureBaseAddress = stream.ReadBits(14);
            TBW_TextureBufferWidth = stream.ReadBits(6);
            PSM = (SCE_GS_PSM)stream.ReadBits(6);
            TW_TextureWidth = (byte)stream.ReadBits(4);
            TH_TextureHeight = (byte)stream.ReadBits(4);
            TCC_ColorComponent = stream.ReadBits(1);
            TFX_TextureFunction = stream.ReadBits(2);
            CBP_BaseClutData = stream.ReadBits(14);
            CPSM_ClutPartPixelFormatSetup = stream.ReadBits(4);
            CSM_ClutStorageMode = stream.ReadBits(1);
            CSA_ClutEntryOffset = stream.ReadBits(5);
            CLD_ClutBufferLoadControl = stream.ReadBits(3);
        }
    };

    public class sceGsTex1
    {
        public ulong LCM_LightColorMatrix;
        public ulong pad01;
        public ulong MXL_unknown;
        public SCE_GS_MAG MMAG;
        public SCE_GS_MAG MMIN;
        public ulong MTBA_unknown;
        public ulong L;
        public ulong K;

        public void Read(ref BitStream stream)
        {
            LCM_LightColorMatrix = stream.ReadBits(1);
            pad01 = stream.ReadBits(1);
            MXL_unknown = stream.ReadBits(3);
            MMAG = (SCE_GS_MAG)stream.ReadBits(1);
            MMIN = (SCE_GS_MAG)stream.ReadBits(3);
            MTBA_unknown = stream.ReadBits(1);
            stream.ReadBits(9);
            L = stream.ReadBits(2);
            stream.ReadBits(11);
            K = stream.ReadBits(12);
            stream.ReadBits(20);
        }
    };

    public class sceGsMiptbp1
    {
        public ulong TBP1;
        public ulong TBW1;
        public ulong TBP2;
        public ulong TBW2;
        public ulong TBP3;
        public ulong TBW3;

        public void Read(ref BitStream stream)
        {
            TBP1 = stream.ReadBits(14);
            TBW1 = stream.ReadBits(6);
            TBP2 = stream.ReadBits(14);
            TBW2 = stream.ReadBits(6);
            TBP3 = stream.ReadBits(14);
            TBW3 = stream.ReadBits(6);
            stream.ReadBits(4);
        }
    };

    public class sceGsMiptbp2
    {
        public ulong TBP4;
        public ulong TBW4;
        public ulong TBP5;
        public ulong TBW5;
        public ulong TBP6;
        public ulong TBW6;

        public void Read(ref BitStream stream)
        {
            TBP4 = stream.ReadBits(14);
            TBW4 = stream.ReadBits(6);
            TBP5 = stream.ReadBits(14);
            TBW5 = stream.ReadBits(6);
            TBP6 = stream.ReadBits(14);
            TBW6 = stream.ReadBits(6);
            stream.ReadBits(4);
        }
    }

    public class sceGsClamp
    {
        public SCE_GS_CLAMP_PARAMS WMS;
        public SCE_GS_CLAMP_PARAMS WMT;
        public ulong MINU;
        public ulong MAXU;
        public ulong MINV;
        public ulong MAXV;

        public void Read(ref BitStream stream)
        {
            WMS = (SCE_GS_CLAMP_PARAMS)stream.ReadBits(2);
            WMT = (SCE_GS_CLAMP_PARAMS)stream.ReadBits(2);
            MINU = stream.ReadBits(10);
            MAXU = stream.ReadBits(10);
            MINV = stream.ReadBits(10);
            MAXV = stream.ReadBits(10);
            stream.ReadBits(20);
        }
    }

    public enum SCE_GS_MAG : byte
    {
        SCE_GS_NEAREST = 0,
        SCE_GS_LINEAR = 1,
        SCE_GS_NEAREST_MIPMAP_NEAREST = 2,
        SCE_GS_NEAREST_MIPMAP_LINEAR = 3,
        SCE_GS_LINEAR_MIPMAP_NEAREST = 4,
        SCE_GS_LINEAR_MIPMAP_LINEAR = 5,
    };

    public enum SCE_GS_PSM : byte
    {
        SCE_GS_PSMCT32 = 0, // RGBA32, uses 32-bit per pixel.
        SCE_GS_PSMCT24 = 1, // RGB24, uses 24-bit per pixel with the upper 8 bit unused.
        SCE_GS_PSMCT16 = 2, // RGBA16 unsigned, pack two pixels in 32-bit in little endian order.
        SCE_GS_PSMCT16S = 10, // RGBA16 signed, pack two pixels in 32-bit in little endian order.
        SCE_GS_PSMT8 = 19, // 8-bit indexed, packing 4 pixels per 32-bit.
        SCE_GS_PSMT4 = 20, // 4-bit indexed, packing 8 pixels per 32-bit.
        SCE_GS_PSMT8H = 27, // 8-bit indexed, but the upper 24-bit are unused.
        SCE_GS_PSMT4HL = 36, // 4-bit indexed, but the upper 24-bit are unused.
        SCE_GS_PSMT4HH = 44,
        SCE_GS_PSMZ32 = 48,  // 32-bit Z buffer
        SCE_GS_PSMZ24 = 49, // 24-bit Z buffer with the upper 8-bit unused
        SCE_GS_PSMZ16 = 50, // 16-bit unsigned Z buffer, pack two pixels in 32-bit in little endian order.
        SCE_GS_PSMZ16S = 58, // 16-bit signed Z buffer, pack two pixels in 32-bit in little endian order.
    }

    public enum SCE_GS_CLAMP_PARAMS : byte
    {
        SCE_GS_REPEAT = 0,
        SCE_GS_CLAMP = 1,
        SCE_GS_REGION_CLAMP = 2,
        SCE_GS_REGION_REPEAT = 3,
    };
}
