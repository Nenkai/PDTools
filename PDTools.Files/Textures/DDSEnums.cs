using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Textures
{
    /// <summary>
    /// DDS Header Flags
    /// </summary>
    [Flags]
    public enum DDSHeaderFlags : uint
    {
        TEXTURE = 0x00001007,  // DDSDCAPS | DDSDHEIGHT | DDSDWIDTH | DDSDPIXELFORMAT 
        MIPMAP = 0x00020000,  // DDSDMIPMAPCOUNT
        VOLUME = 0x00800000,  // DDSDDEPTH
        PITCH = 0x00000008,  // DDSDPITCH
        LINEARSIZE = 0x00080000,  // DDSDLINEARSIZE
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/direct3ddds/dds-pixelformat
    /// </summary>
    [Flags]
    public enum DDSPixelFormatFlags
    {
        DDPF_ALPHAPIXELS = 0x01,
        DDPF_ALPHA = 0x02,
        DDPF_FOURCC = 0x04,
        DDPF_RGB = 0x40,
        DDPF_YUV = 0x200,
        DDPF_LUMINANCE = 0x20000
    }
}
