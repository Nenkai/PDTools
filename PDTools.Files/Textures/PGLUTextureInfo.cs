using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Textures
{
    public abstract class PGLUTextureInfo
    {
        public abstract void Write(BinaryStream bs);

        public abstract void Read(BinaryStream bs);
    }
}
