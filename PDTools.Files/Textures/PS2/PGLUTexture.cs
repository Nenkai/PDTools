using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

using PDTools.Utils;

namespace PDTools.Files.Textures.PS2
{
    /// <summary>
    /// Represents a PDI GL texture - thin wrapper over PS2 GS registers.
    /// </summary>
    public class PGLUtexture
    {
        public const int StructSize = 0x28;

        public sceGsTex0 tex0 { get; set; } = new();
        public sceGsTex1 tex1 { get; set; } = new();
        public sceGsMiptbp1 MipTable1 { get; set; } = new();
        public sceGsMiptbp2 MipTable2 { get; set; } = new();
        public sceGsClamp ClampSettings { get; set; } = new();

        public void Read(BinaryStream bs)
        {
            // Setup a bit stream for this, registers are bit packed and I can't be bothered doing bit shifting.
            byte[] registerData = bs.ReadBytes(StructSize);
            BitStream bitStream = new BitStream(BitStreamMode.Read, registerData, BitStreamSignificantBitOrder.MSB);

            tex0.Read(ref bitStream);
            tex1.Read(ref bitStream);
            MipTable1.Read(ref bitStream);
            MipTable2.Read(ref bitStream);
            ClampSettings.Read(ref bitStream);
        }

        public void Write(BinaryStream bs)
        {
            byte[] registerData = new byte[StructSize];

            BitStream bitStream = new BitStream(BitStreamMode.Write, registerData, BitStreamSignificantBitOrder.MSB);
            tex0.Write(ref bitStream);
            tex1.Write(ref bitStream);
            MipTable1.Write(ref bitStream);
            MipTable2.Write(ref bitStream);
            ClampSettings.Write(ref bitStream);

            bs.Write(registerData);
        }
    }
}
