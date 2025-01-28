using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Textures.PS4;

public enum GNFSurfaceFormat
{
    kSurfaceFormatInvalid = 0x00000000,
    kSurfaceFormat8 = 0x00000001,
    kSurfaceFormat16 = 0x00000002,
    kSurfaceFormat8_8 = 0x00000003,
    kSurfaceFormat32 = 0x00000004,
    kSurfaceFormat16_16 = 0x00000005,
    kSurfaceFormat10_11_11 = 0x00000006,
    kSurfaceFormat11_11_10 = 0x00000007,
    kSurfaceFormat10_10_10_2 = 0x00000008,
    kSurfaceFormat2_10_10_10 = 0x00000009,
    kSurfaceFormat8_8_8_8 = 0x0000000a,
    kSurfaceFormat32_32 = 0x0000000b,
    kSurfaceFormat16_16_16_16 = 0x0000000c,
    kSurfaceFormat32_32_32 = 0x0000000d,
    kSurfaceFormat32_32_32_32 = 0x0000000e,
    kSurfaceFormat5_6_5 = 0x00000010,
    kSurfaceFormat1_5_5_5 = 0x00000011,
    kSurfaceFormat5_5_5_1 = 0x00000012,
    kSurfaceFormat4_4_4_4 = 0x00000013,
    kSurfaceFormat8_24 = 0x00000014,
    kSurfaceFormat24_8 = 0x00000015,
    kSurfaceFormatX24_8_32 = 0x00000016,
    kSurfaceFormatGB_GR = 0x00000020,
    kSurfaceFormatBG_RG = 0x00000021,
    kSurfaceFormat5_9_9_9 = 0x00000022,
    kSurfaceFormatBc1 = 0x00000023,
    kSurfaceFormatBc2 = 0x00000024,
    kSurfaceFormatBc3 = 0x00000025,
    kSurfaceFormatBc4 = 0x00000026,
    kSurfaceFormatBc5 = 0x00000027,
    kSurfaceFormatBc6 = 0x00000028,
    kSurfaceFormatBc7 = 0x00000029,
    kSurfaceFormatFmask8_S2_F1 = 0x0000002C,
    kSurfaceFormatFmask8_S4_F1 = 0x0000002D,
    kSurfaceFormatFmask8_S8_F1 = 0x0000002E,
    kSurfaceFormatFmask8_S2_F2 = 0x0000002F,
    kSurfaceFormatFmask8_S4_F2 = 0x00000030,
    kSurfaceFormatFmask8_S4_F4 = 0x00000031,
    kSurfaceFormatFmask16_S16_F1 = 0x00000032,
    kSurfaceFormatFmask16_S8_F2 = 0x00000033,
    kSurfaceFormatFmask32_S16_F2 = 0x00000034,
    kSurfaceFormatFmask32_S8_F4 = 0x00000035,
    kSurfaceFormatFmask32_S8_F8 = 0x00000036,
    kSurfaceFormatFmask64_S16_F4 = 0x00000037,
    kSurfaceFormatFmask64_S16_F8 = 0x00000038,
    kSurfaceFormat4_4 = 0x00000039,
    kSurfaceFormat6_5_5 = 0x0000003A,
    kSurfaceFormat1 = 0x0000003B,
    kSurfaceFormat1Reversed = 0x0000003C
};

public enum GNFRenderTargetChannelType
{
    kRenderTargetChannelTypeUNorm = 0x00000000,
    kRenderTargetChannelTypeSNorm = 0x00000001,
    kRenderTargetChannelTypeUInt = 0x00000004,
    kRenderTargetChannelTypeSInt = 0x00000005,
    kRenderTargetChannelTypeSrgb = 0x00000006,
    kRenderTargetChannelTypeFloat = 0x00000007
};

public enum GNFTextureChannel
{
    kTextureChannelConstant0 = 0x00000000,
    kTextureChannelConstant1 = 0x00000001,
    kTextureChannelX = 0x00000004,
    kTextureChannelY = 0x00000005,
    kTextureChannelZ = 0x00000006,
    kTextureChannelW = 0x00000007
};


public enum GNFTileMode
{
    kTileModeDepth_2dThin_64 = 0x00000000,
    kTileModeDepth_2dThin_128 = 0x00000001,
    kTileModeDepth_2dThin_256 = 0x00000002,
    kTileModeDepth_2dThin_512 = 0x00000003,
    kTileModeDepth_2dThin_1K = 0x00000004,
    kTileModeDepth_1dThin = 0x00000005,
    kTileModeDepth_2dThinPrt_256 = 0x00000006,
    kTileModeDepth_2dThinPrt_1K = 0x00000007,
    kTileModeDisplay_LinearAligned = 0x00000008,
    kTileModeDisplay_1dThin = 0x00000009,
    kTileModeDisplay_2dThin = 0x0000000A,
    kTileModeDisplay_ThinPrt = 0x0000000B,
    kTileModeDisplay_2dThinPrt = 0x0000000C,
    kTileModeThin_1dThin = 0x0000000D,
    kTileModeThin_2dThin = 0x0000000E,
    kTileModeThin_3dThin = 0x0000000F,
    kTileModeThin_ThinPrt = 0x00000010,
    kTileModeThin_2dThinPrt = 0x00000011,
    kTileModeThin_3dThinPrt = 0x00000012,
    kTileModeThick_1dThick = 0x00000013,
    kTileModeThick_2dThick = 0x00000014,
    kTileModeThick_3dThick = 0x00000015,
    kTileModeThick_ThickPrt = 0x00000016,
    kTileModeThick_2dThickPrt = 0x00000017,
    kTileModeThick_3dThickPrt = 0x00000018,
    kTileModeThick_2dXThick = 0x00000019,
    kTileModeThick_3dXThick = 0x0000001A,
    kTileModeDisplay_LinearGeneral = 0x0000001F
};

public enum GNFTextureType
{
    kTextureType1d = 0x00000008,
    kTextureType2d = 0x00000009,
    kTextureType3d = 0x0000000A,
    kTextureTypeCubemap = 0x0000000B,
    kTextureType1dArray = 0x0000000C,
    kTextureType2dArray = 0x0000000D,
    kTextureType2dMsaa = 0x0000000E,
    kTextureType2dArrayMsaa = 0x0000000F
};
