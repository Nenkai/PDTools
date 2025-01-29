using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Textures.PS4;

public class Swizzler
{
    public static void DoSwizzle(Span<byte> input, Span<byte> output, int width, int height, int blockSize)
    {
        var heightTexels = height / 4;
        var heightTexelsAligned = (heightTexels + 7) / 8;
        int widthTexels = width / 4;
        var widthTexelsAligned = (widthTexels + 7) / 8;
        var dataIndex = 0;

        for (int y = 0; y < heightTexelsAligned; y++)
        {
            for (int x = 0; x < widthTexelsAligned; x++)
            {
                for (int t = 0; t < 64; t++)
                {
                    int pixelIndex = Swizzler.MortonReorder(t, 8, 8);
                    int cPixel = pixelIndex / 8;
                    int remPixel = pixelIndex % 8;
                    var yOffset = y * 8 + cPixel;
                    var xOffset = x * 8 + remPixel;

                    if (xOffset < widthTexels && yOffset < heightTexels)
                    {
                        var destPixelIndex = yOffset * widthTexels + xOffset;
                        int destIndex = blockSize * destPixelIndex;

                        // Memcpy(input + dataIndex, output + destIndex, size)
                        input.Slice(dataIndex, blockSize).CopyTo(output.Slice(destIndex, blockSize));
                    }

                    dataIndex += blockSize;
                }
            }
        }
    }

    public static int MortonReorder(int i, int width, int height)
    {
        int x = 1;
        int y = 1;

        int w = width;
        int h = height;

        int index = 0;
        int index2 = 0;

        while (w > 1 || h > 1)
        {
            if (w > 1)
            {
                index += x * (i & 1);
                i >>= 1;
                x *= 2;
                w >>= 1;
            }
            if (h > 1)
            {
                index2 += y * (i & 1);
                i >>= 1;
                y *= 2;
                h >>= 1;
            }
        }

        return index2 * width + index;
    }
}
