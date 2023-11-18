using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Textures.PS2
{
    public class TextureConfig
    {
        /// <summary>
        /// Pixel format. Defaults to <see cref="SCE_GS_PSM.SCE_GS_PSMT4"/>
        /// </summary>
        public SCE_GS_PSM Format { get; set; } = SCE_GS_PSM.SCE_GS_PSMT4;

        /// <summary>
        /// Wrap Mode S. Defaults to <see cref="SCE_GS_CLAMP_PARAMS.SCE_GS_REGION_CLAMP"/>
        /// </summary>
        public SCE_GS_CLAMP_PARAMS WrapModeS { get; set; } = SCE_GS_CLAMP_PARAMS.SCE_GS_REGION_CLAMP;

        /// <summary>
        /// Wrap Mode T. Defaults to <see cref="SCE_GS_CLAMP_PARAMS.SCE_GS_REGION_CLAMP"/>
        /// </summary>
        public SCE_GS_CLAMP_PARAMS WrapModeT { get; set; } = SCE_GS_CLAMP_PARAMS.SCE_GS_REGION_CLAMP;

        /// <summary>
        /// Whether this is for texture mapping. Textures dimensions will be resized to pow 2.
        /// </summary>
        public bool IsTextureMap { get; set; }

        /// <summary>
        /// Width to repeat texture
        /// </summary>
        public int RepeatWidth { get; set; }

        /// <summary>
        /// Height to repeat texture
        /// </summary>
        public int RepeatHeight { get; set; }
    }
}
