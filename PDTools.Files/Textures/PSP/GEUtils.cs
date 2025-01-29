using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Textures.PSP;

public class GEUtils
{
    public static int BitsPerPixel(eSCE_GE_TPF pixelFormat)
    {
        switch (pixelFormat)
        {
            case eSCE_GE_TPF.SCE_GE_TPF_5650:
            case eSCE_GE_TPF.SCE_GE_TPF_5551:
            case eSCE_GE_TPF.SCE_GE_TPF_4444:
                return 16;
            case eSCE_GE_TPF.SCE_GE_TPF_8888:
                return 32;
            case eSCE_GE_TPF.SCE_GE_TPF_IDTEX4:
            case eSCE_GE_TPF.SCE_GE_TPF_DXT1:
                return 4;
            case eSCE_GE_TPF.SCE_GE_TPF_IDTEX8:
            case eSCE_GE_TPF.SCE_GE_TPF_DXT3:
            case eSCE_GE_TPF.SCE_GE_TPF_DXT5:
                return 8;
            default:
                return 0;
        }
    }
}
