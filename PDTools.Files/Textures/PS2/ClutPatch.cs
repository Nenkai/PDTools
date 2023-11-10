using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

using PDTools.Utils;
using BCnEncoder.Shared;
using PDTools.Files.Models.PS2.CarModel1;

namespace PDTools.Files.Textures.PS2
{
    /// <summary>
    /// Represents a clut patch - palette patch for color switching (mostly for car color switching)
    /// </summary>
    public class ClutPatch
    {
        public List<TextureClutPatch> TexturesToPatch { get; set; } = new();

        public void Read(BinaryStream bs)
        {
            uint numTexturesToPatch = bs.ReadUInt32();
            for (int i = 0; i < numTexturesToPatch; i++)
            {
                TextureClutPatch texPatch = new TextureClutPatch();
                texPatch.Read(bs);
                TexturesToPatch.Add(texPatch);
            }
        }

        public void Write(BinaryStream bs)
        {
            bs.WriteUInt32((uint)TexturesToPatch.Count);
            for (int i = 0; i < TexturesToPatch.Count; i++)
                TexturesToPatch[i].Write(bs);
        }

        public override int GetHashCode()
        {
            int hashcode = 1430287;
            foreach (var textureToPatch in TexturesToPatch)
                hashcode += textureToPatch.GetHashCode();

            return hashcode;
        }
    }

    /// <summary>
    /// Patches tex0 register for a texture to change the clut
    /// </summary>
    public class TextureClutPatch
    {
        /// <summary>
        /// 5 bits
        /// </summary>
        public byte CSA_ClutEntryOffset { get; set; }

        /// <summary>
        /// 14 bits
        /// </summary>
        public ushort CBP_ClutBufferBasePointer { get; set; }
        
        /// <summary>
        /// 4 bits
        /// </summary>
        public SCE_GS_PSM Format { get; set; }

        /// <summary>
        /// 9 bits
        /// </summary>
        public ushort PGLUTextureIndex { get; set; }

        public void Read(BinaryStream bs)
        {
            uint bits = bs.ReadUInt32();
            CSA_ClutEntryOffset = (byte)(bits & 0b11111);
            CBP_ClutBufferBasePointer = (ushort)((bits >> 5) & 0b111111_11111111);
            Format = (SCE_GS_PSM)((bits >> 19) & 0b1111);
            PGLUTextureIndex = (ushort)(bits >> 23);
        }

        public void Write(BinaryStream bs)
        {
            uint bits = (uint)(CSA_ClutEntryOffset & 0b11111) |
                        (uint)(CBP_ClutBufferBasePointer & 0b111111_11111111) << 5 |
                        (uint)((byte)Format & 0b1111) << 19 |
                        (uint)(PGLUTextureIndex << 23);
            bs.WriteUInt32(bits);
            
        }

        public override int GetHashCode()
        {
            int hashcode = 1430287;
            hashcode = hashcode * 7302013 ^ CSA_ClutEntryOffset.GetHashCode();
            hashcode = hashcode * 7302013 ^ CBP_ClutBufferBasePointer.GetHashCode();
            hashcode = hashcode * 7302013 ^ Format.GetHashCode();
            hashcode = hashcode * 7302013 ^ PGLUTextureIndex.GetHashCode();

            return hashcode;
        }
    }
}
