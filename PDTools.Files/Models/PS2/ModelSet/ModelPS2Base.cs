using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Models.PS2.Commands;
using PDTools.Files.Textures.PS2;

namespace PDTools.Files.Models.PS2.ModelSet
{
    public abstract class ModelPS2Base
    {
        /// <summary>
        /// Commands interpreted on every tick to figure how and what shape should be rendered.
        /// </summary>
        public List<ModelSetupPS2Command> Commands { get; set; } = new();
    }
}
