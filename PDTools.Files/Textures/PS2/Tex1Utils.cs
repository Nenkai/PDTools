using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Textures.PS2.GSPixelFormats;

namespace PDTools.Files.Textures.PS2;

public static class Tex1Utils
{
    public static int GetDataSize(int width, int height, SCE_GS_PSM format)
    {
        int bpp = Tex1Utils.GetBitsPerPixel(format);
        int bytes = (int)Math.Round((double)(width * height * ((double)bpp / 8)), MidpointRounding.AwayFromZero);
        return bytes;
    }

    public static int GetBitsPerPixel(SCE_GS_PSM psm)
    {
        return psm switch
        {
            SCE_GS_PSM.SCE_GS_PSMCT32 => 32,
            SCE_GS_PSM.SCE_GS_PSMCT24 => 32, // RGB24, uses 24-bit per pixel with the upper 8 bit unused.
            SCE_GS_PSM.SCE_GS_PSMCT16 => 16, // RGBA16 unsigned, pack two pixels in 32-bit in little endian order.
            SCE_GS_PSM.SCE_GS_PSMCT16S => 16, // RGBA16 signed, pack two pixels in 32-bit in little endian order.
            SCE_GS_PSM.SCE_GS_PSMT8 => 8, // 8-bit indexed, packing 4 pixels per 32-bit.
            SCE_GS_PSM.SCE_GS_PSMT4 => 4, // 4-bit indexed, packing 8 pixels per 32-bit.
            SCE_GS_PSM.SCE_GS_PSMT8H => 4, // 8-bit indexed, but the upper 24-bit are unused.
            SCE_GS_PSM.SCE_GS_PSMT4HL => 4, // 4-bit indexed, but the upper 24-bit are unused.
            SCE_GS_PSM.SCE_GS_PSMT4HH => 4,
            SCE_GS_PSM.SCE_GS_PSMZ32 => 32,  // 32-bit Z buffer
            SCE_GS_PSM.SCE_GS_PSMZ24 => 32, // 24-bit Z buffer with the upper 8-bit unused
            SCE_GS_PSM.SCE_GS_PSMZ16 => 16, // 16-bit unsigned Z buffer, pack two pixels in 32-bit in little endian order.
            SCE_GS_PSM.SCE_GS_PSMZ16S => 16, // 16-bit signed Z buffer, pack two pixels in 32-bit in little endian order.
            _ => throw new InvalidOperationException($"Invalid pixel surface type '{psm}'")
        };
    }

    public static List<(int Width, int Height)> CalculateSwizzledTransferSizes(int dataSize)
    {
        /* This mimics GT4's swizzling
         * where it creates one transfer of 64x<height aligned to pages>
         * Then 64x32, 32x32, 32x16, etc. */
        List<(int Width, int Height)> transfers = [];

        if (dataSize >= 0x2000) // 64x32, size of one page
        {
            int numPages = dataSize / 0x2000;
            dataSize %= 0x2000;

            int transferHeight = GSPixelFormat.PSM_CT32.PageWidth;
            int transferWidth = GSPixelFormat.PSM_CT32.PageHeight * numPages;
            transfers.Add((transferHeight, transferWidth));
        }

        int size = 0x1000;
        int transferW = 32;
        int transferH = 32;
        while (dataSize > 0)
        {
            if (dataSize >= size)
            {
                dataSize %= size;
                transfers.Add((transferW, transferH));
            }

            if (transferH == transferW / 2)
            {
                transferW >>= 1;
                transferH = transferW;
            }
            else
            {
                transferH >>= 1;
            }

            size >>= 1;
        }

        return transfers;
    }

    public static double Normalize(double val, double valmin, double valmax, double min, double max)
    {
        return (((val - valmin) / (valmax - valmin)) * (max - min)) + min;
    }
}
