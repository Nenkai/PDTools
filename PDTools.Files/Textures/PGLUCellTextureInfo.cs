﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Textures
{
    public class PGLUCellTextureInfo : PGLUTextureInfo
    {
        public uint Head0 { get; set; }
        public uint Offset { get; set; }
        public byte MipmapLevelLast { get; set; }
        public CELL_GCM_TEXTURE_FORMAT FormatBits { get; set; }

        /// <summary>
        /// 1 = 1D
        /// 2 = 2D
        /// </summary>
        public CELL_GCM_TEXTURE_DIMENSION Dimension { get; set; } = CELL_GCM_TEXTURE_DIMENSION.CELL_GCM_TEXTURE_DIMENSION_2;

        public CELL_GCM_TEXTURE_BORDER Border { get; set; } = CELL_GCM_TEXTURE_BORDER.CELL_GCM_TEXTURE_BORDER_COLOR;
        public CELL_GCM_BOOL CubeMap { get; set; } = CELL_GCM_BOOL.CELL_GCM_FALSE;

        /// <summary>
        /// 0 = local memory
        /// 1 = main memory
        /// </summary>
        public CELL_GCM_LOCATION Location { get; set; } = CELL_GCM_LOCATION.CELL_GCM_LOCATION_LOCAL;

        public CELL_GCM_TEXTURE_ZFUNC ZFunc { get; set; } = CELL_GCM_TEXTURE_ZFUNC.CELL_GCM_TEXTURE_ZFUNC_NEVER;
        public byte Gamma { get; set; } = 0;
        public CELL_GCM_TEXTURE_WRAP WrapR { get; set; } = CELL_GCM_TEXTURE_WRAP.CELL_GCM_TEXTURE_CLAMP_TO_EDGE;
        public CELL_GCM_TEXTURE_UNSIGNED_REMAP UnsignedRemap { get; set; } = CELL_GCM_TEXTURE_UNSIGNED_REMAP.CELL_GCM_TEXTURE_UNSIGNED_REMAP_NORMAL;
        public CELL_GCM_TEXTURE_WRAP WrapT { get; set; } = CELL_GCM_TEXTURE_WRAP.CELL_GCM_TEXTURE_CLAMP_TO_EDGE;
        public byte AnisoBias { get; set; } = 0;
        public CELL_GCM_TEXTURE_WRAP WrapS { get; set; } = CELL_GCM_TEXTURE_WRAP.CELL_GCM_TEXTURE_CLAMP_TO_EDGE;
        public CELL_GCM_BOOL VertexTextureSamplerEnable { get; set; } = CELL_GCM_BOOL.CELL_GCM_TRUE;

        /// <summary>
        /// min LOD of texture reduction filter
        /// 12-bit unsigned fixed point value from 0 to 12
        /// </summary>
        public short LODMin { get; set; } = 0;

        /// <summary>
        /// max LOD of texture reduction filter
        /// 12-bit unsigned fixed point value from 0 to 12
        /// </summary>
        public short LODMax { get; set; } = 3840;

        public CELL_GCM_TEXTURE_MAX_ANISO MaxAniso { get; set; } = CELL_GCM_TEXTURE_MAX_ANISO.CELL_GCM_TEXTURE_MAX_ANISO_1;
        public CELL_GCM_BOOL AlphaKill { get; set; } = CELL_GCM_BOOL.CELL_GCM_FALSE;

        public CELL_GCM_TEXTURE_REMAP_ORDER RemapOrder { get; set; } = CELL_GCM_TEXTURE_REMAP_ORDER.CELL_GCM_TEXTURE_REMAP_ORDER_XYXY;
        public CELL_GCM_TEXTURE_REMAP_OUT OutB { get; set; } = CELL_GCM_TEXTURE_REMAP_OUT.CELL_GCM_TEXTURE_REMAP_REMAP;
        public CELL_GCM_TEXTURE_REMAP_OUT OutG { get; set; } = CELL_GCM_TEXTURE_REMAP_OUT.CELL_GCM_TEXTURE_REMAP_REMAP;
        public CELL_GCM_TEXTURE_REMAP_OUT OutR { get; set; } = CELL_GCM_TEXTURE_REMAP_OUT.CELL_GCM_TEXTURE_REMAP_REMAP;
        public CELL_GCM_TEXTURE_REMAP_OUT OutA { get; set; } = CELL_GCM_TEXTURE_REMAP_OUT.CELL_GCM_TEXTURE_REMAP_REMAP;
        public CELL_GCM_TEXTURE_REMAP_FROM InB { get; set; } = CELL_GCM_TEXTURE_REMAP_FROM.CELL_GCM_TEXTURE_REMAP_FROM_B;
        public CELL_GCM_TEXTURE_REMAP_FROM InG { get; set; } = CELL_GCM_TEXTURE_REMAP_FROM.CELL_GCM_TEXTURE_REMAP_FROM_G;
        public CELL_GCM_TEXTURE_REMAP_FROM InR { get; set; } = CELL_GCM_TEXTURE_REMAP_FROM.CELL_GCM_TEXTURE_REMAP_FROM_R;
        public CELL_GCM_TEXTURE_REMAP_FROM InA { get; set; } = CELL_GCM_TEXTURE_REMAP_FROM.CELL_GCM_TEXTURE_REMAP_FROM_A;

        /// <summary>
        /// 1 bit per color, to hold CELL_GCM_BOOL on whether they are handled as complement of 2
        /// </summary>
        public byte SignedRGBA { get; set; }

        public CELL_GCM_TEXTURE_MAG Mag { get; set; } = CELL_GCM_TEXTURE_MAG.CELL_GCM_TEXTURE_LINEAR_MAG;
        public CELL_GCM_TEXTURE_MIN Min { get; set; } = CELL_GCM_TEXTURE_MIN.CELL_GCM_TEXTURE_LINEAR;
        public CELL_GCM_TEXTURE_CONVOLUTION Convultion { get; set; } = CELL_GCM_TEXTURE_CONVOLUTION.CELL_GCM_TEXTURE_CONVOLUTION_QUINCUNX;
        public byte LODBias { get; set; }
        public int BorderColor { get; set; } = 0;

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public short Depth { get; set; } = 1;
        public int Pitch { get; set; }

        public uint ImageId { get; set; }
        public string Name { get; set; }

        public override void Write(BinaryStream bs)
        {
            bs.WriteInt32(6656); // head0
            bs.WriteInt32(0); // offset (runtime)

            // CELL_GCM_METHOD_DATA_TEXTURE_BORDER_FORMAT
            int bits = 0;
            bits |= (byte)(((byte)MipmapLevelLast & 0b_11111111) << 16);
            bits |= (byte)(((byte)FormatBits & 0b_11111111) << 8);
            bits |= (byte)(((byte)Dimension & 0b_1111) << 4);
            bits |= (byte)(((byte)Border & 1) << 3);
            bits |= (byte)(((byte)CubeMap & 1) << 2);
            bits |= (byte)(((byte)Location + 1) & 0b_11);
            bs.WriteInt32(bits);

            // CELL_GCM_METHOD_DATA_TEXTURE_ADDRESS
            bits = 0;
            bits |= (((byte)ZFunc & 0b_1111) << 28);
            bits |= ((Gamma & 0b_1111_1111) << 20);
            bits |= (((byte)WrapR & 0b_1111) << 16);
            bits |= (((byte)UnsignedRemap << 0b1111) << 12);
            bits |= (((byte)WrapT & 0b_1111) << 8);
            bits |= ((AnisoBias & 0b_1111) << 4);
            bits |= ((byte)WrapS & 0b_1111);
            bs.WriteInt32(bits);

            // CELL_GCM_METHOD_DATA_TEXTURE_CONTROL0_ALPHA_KILL
            bits = 0;
            bits |= (((byte)VertexTextureSamplerEnable & 1) << 31);
            bits |= ((LODMin & 0b_1111_11111111) << 19);
            bits |= ((LODMax & 0b_1111_11111111) << 7);
            bits |= (((byte)MaxAniso << 4) & 0b_111);
            bits |= (((byte)AlphaKill << 2) & 1);
            bs.WriteInt32(bits);

            // CELL_GCM_METHOD_DATA_TEXTURE_CONTROL1
            bits = 0;
            bits |= (((byte)RemapOrder & 1) << 16);
            bits |= (((byte)OutB & 0b_11) << 14);
            bits |= (((byte)OutG & 0b_11) << 12);
            bits |= (((byte)OutR & 0b_11) << 10);
            bits |= (((byte)OutA & 0b_11) << 8);
            bits |= (((byte)InB & 0b_11) << 6);
            bits |= (((byte)InG & 0b_11) << 4);
            bits |= (((byte)InR & 0b_11) << 2);
            bits |= (((byte)InA & 0b_11));
            bs.WriteInt32(bits);

            // CELL_GCM_METHOD_DATA_TEXTURE_FILTER_SIGNED
            bits = 0;
            bits |= ((SignedRGBA & 0b1_1111) << 27);
            bits |= (((byte)Mag & 0b_111) << 24);
            bits |= (((byte)Min & 0b_1111_1111) << 16);
            bits |= (((byte)Convultion & 0b_111) << 13);
            bits |= (LODBias & 0b_1111_1111_1111);
            bs.WriteInt32(bits);

            // CELL_GCM_METHOD_DATA_TEXTURE_IMAGE_RECT
            bits = 0;
            bits |= ((Width & 0b11111111_11111111) << 16);
            bits |= (Height & 0b11111111_11111111);
            bs.WriteInt32(bits);

            // CELL_GCM_METHOD_DATA_TEXTURE_BORDER_COLOR
            bs.WriteInt32(BorderColor);

            // CELL_GCM_METHOD_DATA_TEXTURE_CONTROL2
            bs.WriteInt32(6208); // TODO

            // CELL_GCM_METHOD_DATA_TEXTURE_CONTROL3
            int bits5 = 0;
            bits5 |= (int)((Depth & 0x1111_1111_1111) << 20);
            bits5 |= (Pitch & 0b1111_1111_1111_1111_1111);
            bs.WriteInt32(bits5); // head1 fixme

            bs.WriteInt32(0); // Reserved.. or not?
            bs.WriteInt32(0); // Same
            bs.WriteInt32(0);
            bs.WriteUInt32(ImageId); // Image Id
            bs.WriteInt32(0);
            bs.WriteInt32(0); // Img name offset to write later if exists
        }

        public override void Read(BinaryStream bs)
        {
            Head0 = bs.ReadUInt32();
            Offset = bs.ReadUInt32();

            uint bits = bs.ReadUInt32();
            MipmapLevelLast = (byte)((bits >> 16) & 0b11111111);
            FormatBits = (CELL_GCM_TEXTURE_FORMAT)((bits >> 8) & 0b11111111);
            Dimension = (CELL_GCM_TEXTURE_DIMENSION)((bits >> 4) & 0b1111);
            Border = (CELL_GCM_TEXTURE_BORDER)((bits >> 3) & 1);
            CubeMap = (CELL_GCM_BOOL)((bits >> 2) & 1);
            Location = (CELL_GCM_LOCATION)((bits & 0b11) - 1);

            bits = bs.ReadUInt32();
            ZFunc = (CELL_GCM_TEXTURE_ZFUNC)((bits >> 28) & 0b11111);
            Gamma = (byte)((bits >> 20) & 0b1111_1111);
            WrapR = (CELL_GCM_TEXTURE_WRAP)((bits >> 16) & 0b1111);
            UnsignedRemap = (CELL_GCM_TEXTURE_UNSIGNED_REMAP)((bits >> 12) & 0b1111);
            WrapT = (CELL_GCM_TEXTURE_WRAP)((bits >> 8) & 0b1111);
            AnisoBias = (byte)((bits >> 4) & 0b1111);
            WrapS = (CELL_GCM_TEXTURE_WRAP)(bits & 0b1111);

            bits = bs.ReadUInt32();
            VertexTextureSamplerEnable = (CELL_GCM_BOOL)((bits >> 31) & 1);
            LODMin = (short)((bits >> 19) & 0b_1111_1111_1111);
            LODMax = (short)((bits >> 7) & 0b_1111_1111_1111);
            MaxAniso = (CELL_GCM_TEXTURE_MAX_ANISO)((bits >> 4) & 0b_111);
            AlphaKill = (CELL_GCM_BOOL)((bits >> 2) & 1); // Some padding before and after

            bits = bs.ReadUInt32();
            RemapOrder = (CELL_GCM_TEXTURE_REMAP_ORDER)((bits >> 16) & 1);
            OutB = (CELL_GCM_TEXTURE_REMAP_OUT)((bits >> 14) & 0b11);
            OutG = (CELL_GCM_TEXTURE_REMAP_OUT)((bits >> 12) & 0b11);
            OutR = (CELL_GCM_TEXTURE_REMAP_OUT)((bits >> 10) & 0b11);
            OutA = (CELL_GCM_TEXTURE_REMAP_OUT)((bits >> 8) & 0b11);
            InB = (CELL_GCM_TEXTURE_REMAP_FROM)((bits >> 6) & 0b11);
            InG = (CELL_GCM_TEXTURE_REMAP_FROM)((bits >> 4) & 0b11);
            InR = (CELL_GCM_TEXTURE_REMAP_FROM)((bits >> 2) & 0b11);
            InA = (CELL_GCM_TEXTURE_REMAP_FROM)((bits) & 0b11);

            bits = bs.ReadUInt32();
            SignedRGBA = (byte)((bits >> 27) & 0b1_1111);
            Mag = (CELL_GCM_TEXTURE_MAG)((bits >> 24) & 0b_111);
            Min = (CELL_GCM_TEXTURE_MIN)((bits >> 16) & 0b_11111111);
            Convultion = (CELL_GCM_TEXTURE_CONVOLUTION)((bits >> 13) & 0b_111);
            LODBias = (byte)(bits & 0b_1111_1111_1111);

            bits = bs.ReadUInt32();
            Width = (ushort)(bits >> 16);
            Height = (ushort)(bits & 0b11111111_11111111);

            BorderColor = bs.ReadInt32();
            bs.ReadInt32(); // CELL_GCM_METHOD_DATA_TEXTURE_CONTROL2 TODO

            bits = bs.ReadUInt32();
            Depth = (short)((bits >> 20) & 0x1111_1111_1111);
            Pitch = (int)((bits & 0b1111_11111111_11111111));

            bs.ReadUInt32();
            bs.ReadUInt32();
            bs.ReadUInt32();
            ImageId = bs.ReadUInt32();
            bs.ReadUInt32();
            uint imageNameOffset = bs.ReadUInt32();
            bs.Position = imageNameOffset;
            Name = bs.ReadString(StringCoding.ZeroTerminated);
        }
    }
}
