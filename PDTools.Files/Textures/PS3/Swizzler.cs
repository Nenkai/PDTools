using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Textures.PS3;

public class Swizzler
{
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
