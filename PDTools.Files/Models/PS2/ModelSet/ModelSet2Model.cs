using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using PDTools.Files.Models.PS2.Commands;

namespace PDTools.Files.Models.PS2.ModelSet
{
    public class ModelSet2Model : ModelPS2Base
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

        public List<Vector3> Bounds { get; set; } = new();
        public float Unk { get; set; }
        public Vector3 Origin { get; set; }

        public int VMCtorBytecodeOffset { get; set; } = -1;
        public int VMUpdateBytecodeOffset { get; set; } = -1;
        public int VMUnkBytecodeOffset { get; set; } = -1;

        public void FromStream(BinaryStream bs, long mdlBasePos)
        {
            ModelSet2Model model = new ModelSet2Model();

            bs.Read1Byte();
            byte boundCount = bs.Read1Byte();
            bs.Position += 2;
            uint boundsOffset = bs.ReadUInt32();
            Unk = bs.ReadSingle();
            Origin = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
            uint commandsOffset = bs.ReadUInt32();

            bs.Position = mdlBasePos + boundsOffset;
            for (int i = 0; i < boundCount; i++)
            {
                Vector3 point = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
                Bounds.Add(point);
            }

            //setBound(new float[] { Bounds[5].X, Bounds[5].Y, Bounds[5].Z }, new float[] { Bounds[3].X, Bounds[3].Y, Bounds[3].Z });
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

        public void Write(BinaryStream bs)
        {
            bs.WriteByte(0);
            bs.WriteByte((byte)Bounds.Count); // Should be either 0 or 8
            bs.WriteSByte(-1);
            bs.WriteSByte(-1);
            bs.WriteInt32(0); // Skip bounds offset for now
            bs.WriteSingle(Unk);
            bs.WriteSingle(Origin.X); bs.WriteSingle(Origin.Y); bs.WriteSingle(Origin.Z);
            bs.WriteInt32(0); // Skip setup opcodes offset
            bs.WriteInt32(VMCtorBytecodeOffset);
            bs.WriteInt32(VMUpdateBytecodeOffset);
            bs.WriteInt32(VMUnkBytecodeOffset);
        }

        public void WriteBounds(BinaryStream bs)
        {
            for (int i = 0; i < Bounds.Count; i++)
            {
                bs.WriteSingle(Bounds[i].X); bs.WriteSingle(Bounds[i].Y); bs.WriteSingle(Bounds[i].Z);
            }
        }

        public void WriteCommands(BinaryStream bs)
        {
            foreach (var cmd in Commands)
            {
                bs.Write((byte)cmd.Opcode);
                cmd.Write(bs);
            }

            bs.WriteByte(0); // End
        }

        // RE'd - GT4O ModelSet2::Model::setBound - 0x2F6DA0
        private void setBound(float[] min, float[] max)
        {
            int x = 0;
            int y = 0;

            int i = 0;
            while (true)
            {
                for (int z = 0; z < 2; z++)
                {
                    var vec = Bounds[i];
                    vec.X = x != 0 ? min[0] : max[0];
                    vec.Y = y != 0 ? min[1] : max[1];
                    vec.Z = z != 0 ? min[2] : max[2];
                    i++;
                }

                y++;
                if (y >= 2)
                {
                    x++;
                    y = 0;
                    if (x >= 2)
                        break;
                }
            }

            float[] dist = new float[3];
            float total_UNUSED = 0.0f;
            float final = 0.0f;
            for (i = 0; i < 3; i++)
            {
                float center = (min[i] + max[i]) * 0.5f;
                dist[i] = center;
                final = (max[i] - center) * (max[i] - center);
                total_UNUSED += final;
            }

            // Not original, added for convenience
            Origin = new Vector3(dist);

            Unk = MathF.Sqrt(final);
        }

        public static uint GetSize()
        {
            return 0x28;
        }
    }
}
