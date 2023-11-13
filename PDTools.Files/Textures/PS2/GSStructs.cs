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
        public ushort TBP0_TextureBaseAddress;
        public byte TBW_TextureBufferWidth;

        /// <summary>
        /// Texture pixel storage format
        /// </summary>
        public SCE_GS_PSM PSM;

        /// <summary>
        /// Texture width - Actual size will be 2^w and 2^h
        /// </summary>
        public byte TW_TextureWidth;

        /// <summary>
        /// Texture height - Actual size will be 2^w and 2^h
        /// </summary>
        public byte TH_TextureHeight;

        /// <summary>
        /// Texture color component, 0 = RGB, 1 = RGBA
        /// </summary>
        public byte TCC_ColorComponent;

        /// <summary>
        /// Texture function
        /// </summary>
        public ulong TFX_TextureFunction;

        /// <summary>
        /// Base address of CLUT data (actual address will be cbp x 64)
        /// </summary>
        public ulong CBP_ClutBlockPointer;

        /// <summary>
        /// Format in which CLUT entries are saved
        /// </summary>
        public SCE_GS_PSM CPSM_ClutPartPixelFormatSetup;

        /// <summary>
        /// CLUT storage mode
        /// </summary>
        public ulong CSM_ClutStorageMode;

        /// <summary>
        /// CLUT entry offset - CSA = Offset / 16, In CSM2, CSA must be 0
        /// </summary>
        public ulong CSA_ClutEntryOffset;

        /// <summary>
        // 0 = Temporary buffer contents not changed
        // 1 = Load performed to CSA position of buffer
        // 2 = Load is performed to CSA position of buffer and CBP is copied to CBP0. (*2)
        // 3 = Load is performed to CSA position of buffer and CBP is copied to CBP1. (*2)
        // 4 = If CBP0 != CBP, load is performed and CBP is copied to CBP0. (*2)
        // 5 = If CBP1 != CBP, load is performed and CBP is copied to CBP1. (*2)
        /// </summary>
        public ulong CLD_ClutBufferLoadControl;

        public void Read(ref BitStream stream)
        {
            TBP0_TextureBaseAddress = (ushort)stream.ReadBits(14);
            TBW_TextureBufferWidth = (byte)stream.ReadBits(6);
            PSM = (SCE_GS_PSM)stream.ReadBits(6);
            TW_TextureWidth = (byte)stream.ReadBits(4);
            TH_TextureHeight = (byte)stream.ReadBits(4);
            TCC_ColorComponent = (byte)stream.ReadBits(1);
            TFX_TextureFunction = stream.ReadBits(2);
            CBP_ClutBlockPointer = stream.ReadBits(14);
            CPSM_ClutPartPixelFormatSetup = (SCE_GS_PSM)stream.ReadBits(4);
            CSM_ClutStorageMode = stream.ReadBits(1);
            CSA_ClutEntryOffset = stream.ReadBits(5);
            CLD_ClutBufferLoadControl = stream.ReadBits(3);
        }

        public void Write(ref BitStream stream)
        {
            stream.WriteBits(TBP0_TextureBaseAddress, 14);
            stream.WriteBits(TBW_TextureBufferWidth, 6);
            stream.WriteBits((ulong)PSM, 6);
            stream.WriteBits(TW_TextureWidth, 4);
            stream.WriteBits(TH_TextureHeight, 4);
            stream.WriteBits(TCC_ColorComponent, 1);
            stream.WriteBits(TFX_TextureFunction, 2);
            stream.WriteBits(CBP_ClutBlockPointer, 14);
            stream.WriteBits((byte)CPSM_ClutPartPixelFormatSetup, 4);
            stream.WriteBits(CSM_ClutStorageMode, 1);
            stream.WriteBits(CSA_ClutEntryOffset, 5);
            stream.WriteBits(CLD_ClutBufferLoadControl, 3);
        }
    };

    public class sceGsTex1
    {
        /// <summary>
        /// 0 = (LOD = (log2(1/|Q|)<<L+K), 1 = Fixed value (LOD = K)
        /// </summary>
        public ulong LCM_LightColorMatrix;
        public ulong pad01;

        /// <summary>
        /// 0-6
        /// </summary>
        public ulong MXL_MaximumMIPLevel;

        /// <summary>
        /// Filter when Texture is Expanded (LOD < 0) - Max LINEAR
        /// </summary>
        public SCE_GS_MAG MMAG = SCE_GS_MAG.SCE_GS_LINEAR;

        /// <summary>
        /// Filter when Texture is Reduced (LOD >= 0)
        /// </summary>
        public SCE_GS_MAG MMIN = SCE_GS_MAG.SCE_GS_LINEAR;
        public ulong MTBA_unknown;

        /// <summary>
        /// LOD Parameter Value L
        /// </summary>
        public ulong L;

        /// <summary>
        /// LOD Parameter Value K
        /// </summary>
        public ulong K;

        public void Read(ref BitStream stream)
        {
            LCM_LightColorMatrix = stream.ReadBits(1);
            pad01 = stream.ReadBits(1);
            MXL_MaximumMIPLevel = stream.ReadBits(3);
            MMAG = (SCE_GS_MAG)stream.ReadBits(1);
            MMIN = (SCE_GS_MAG)stream.ReadBits(3);
            MTBA_unknown = stream.ReadBits(1);
            stream.ReadBits(9);
            L = stream.ReadBits(2);
            stream.ReadBits(11);
            K = stream.ReadBits(12);
            stream.ReadBits(20);
        }

        public void Write(ref BitStream stream)
        {
            stream.WriteBits(LCM_LightColorMatrix, 1);
            stream.WriteBits(pad01, 1);
            stream.WriteBits(MXL_MaximumMIPLevel, 3);
            stream.WriteBits((ulong)MMAG, 1);
            stream.WriteBits((ulong)MMIN, 3);
            stream.WriteBits(MTBA_unknown, 1);
            stream.WriteBits(0, 9);
            stream.WriteBits(L, 2);
            stream.WriteBits(0, 11);
            stream.WriteBits(K, 12);
            stream.WriteBits(0, 20);
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

        public void Write(ref BitStream stream)
        {
            stream.WriteBits(TBP1, 14);
            stream.WriteBits(TBW1, 6);
            stream.WriteBits(TBP2, 14);
            stream.WriteBits(TBW2, 6);
            stream.WriteBits(TBP3, 14);
            stream.WriteBits(TBW3, 6);
            stream.WriteBits(0, 4);
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

        public void Write(ref BitStream stream)
        {
            stream.WriteBits(TBP4, 14);
            stream.WriteBits(TBW4, 6);
            stream.WriteBits(TBP5, 14);
            stream.WriteBits(TBW5, 6);
            stream.WriteBits(TBP6, 14);
            stream.WriteBits(TBW6, 6);
            stream.WriteBits(0, 4);
        }
    }

    public class sceGsClamp
    {
        /// <summary>
        /// Wrap Mode in Horizontal (S) Direction
        /// </summary>
        public SCE_GS_CLAMP_PARAMS WMS = SCE_GS_CLAMP_PARAMS.SCE_GS_CLAMP;

        /// <summary>
        /// Wrap Mode in Horizontal (T) Direction
        /// </summary>
        public SCE_GS_CLAMP_PARAMS WMT = SCE_GS_CLAMP_PARAMS.SCE_GS_CLAMP;

        /// <summary>
        /// Clamp U Direction - Lower Limit
        /// </summary>
        public ulong MINU;

        /// <summary>
        /// Clamp U Direction - Upper Limit
        /// </summary>
        public ulong MAXU;

        /// <summary>
        /// Clamp Y Direction - Lower Limit
        /// </summary>
        public ulong MINV;

        /// <summary>
        /// Clamp Y Direction - Upper Limit
        /// </summary>
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

        public void Write(ref BitStream stream)
        {
            stream.WriteBits((ulong)WMS, 2);
            stream.WriteBits((ulong)WMT, 2);
            stream.WriteBits(MINU, 10);
            stream.WriteBits(MAXU, 10);
            stream.WriteBits(MINV, 10);
            stream.WriteBits(MAXV, 10);
            stream.WriteBits(0, 20);
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
