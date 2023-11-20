using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Textures.PS2;

namespace PDTools.Files.Models.PS2.ModelSet
{
    public abstract class ModelSetPS2Base
    {
        public abstract List<TextureSet1> GetTextureSetList();

        public abstract int AddShape(PGLUshape shape);

        public abstract int AddMaterial(PGLUmaterial material);

        public abstract int GetMaterialCount();
    }
}
