using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Models.PS2.Commands;

namespace PDTools.Files.Models.PS2.ModelSet
{
    public class ModelSet2Model
    {
        /* There are three things that are done for a model set to be rendered - begin(), render(), end().
         * ---------------------------------
         * begin() sets parameters to use:
        
           void ModelSet2::begin(ModelSet2 *this, ModelSet2Instance *instance)
           {
             if ( *&a2 && a1->gap2C )
               ModelSet2::setColor(this, instance->field_18);
             if ( depth_test ) // set with ModelSet2::depthTest
               pglEnable(pgl_TEST_EnableDepthTest);
             else
               pglDisable(pgl_TEST_EnableDepthTest);
             pglEnable(pgl_TEST_EnableAlphaTest);
             pglDisable(pgl_TEST_EnableDestinationAlphaTest);
             pglEnable(pgl_CullMode);
             pglEnable(pgl_EnableRendering);
             pglBlendFunc(68);
             pglDepthBias(0.0);
             if ( depth_mask_ ) // set with ModelSet2::depthMaskEnable
               pglDepthMask(1);
             if ( use_color_mask_ ) // set with ModelSet2::useColorMask
               pglColorMask1ui(-1);
             pglDepthFunc(3);
             pglAlphaFunc1ub(6, 0x20u);
             pglDestinationAlphaFunc(5LL);
             pglAlphaFail(0);
             pglTexGenf(3uLL, facing_attenuation_);
             pglTexGenf(2uLL, facing_bias_);
             pglVariableColorScale(1.0, 1.0, 1.0, 1.0);
             pglVariableColorOffset(0.0, 0.0, 0.0, 0.0);
             pgluMatTable(a1->MatTable);
             pgluShapeCallback(pgluMaterialFunc);
           }
        
           ---------------------------------
           render() iterates through all the models, and their commands, and interprets them for rendering
           ---------------------------------
           end() resets everything:
        
           void ModelSet2::end()
           {
             pglDisable(pgl_TEST_EnableAlphaTest);
             pglDisable(pgl_TEST_EnableDestinationAlphaTest);
             if ( depth_mask_ )
               pglDepthMask(1);
             if ( use_color_mask_ )
               pglColorMask1ui(-1);
           }
        */

        public List<ModelSetupPS2Command> Commands { get; set; } = new();

        public void FromStream(BinaryStream bs, long mdlBasePos)
        {
            ModelSet2Model model = new ModelSet2Model();

            bs.Read1Byte();
            byte boundCount = bs.Read1Byte();
            bs.Position += 2;
            uint boundsOffset = bs.ReadUInt32();
            bs.Position += 0x10;
            uint commandsOffset = bs.ReadUInt32();

            bs.Position = mdlBasePos + commandsOffset;
            while (true)
            {
                ModelSetupPS2Opcode opcode = (ModelSetupPS2Opcode)bs.Read1Byte();
                if (opcode == ModelSetupPS2Opcode.End)
                    break;

                var cmd = ModelSetupPS2Command.GetByOpcode(opcode);

                cmd.Read(bs, 0);
                Commands.Add(cmd);
            }
        }

        public static uint GetSize()
        {
            return 0x28;
        }
    }
}
