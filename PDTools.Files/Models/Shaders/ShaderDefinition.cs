using PDTools.Files.Models.Shaders;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.Shaders;

public class ShaderDefinition
{
    public string Name { get; set; }
    public int UnkID { get; set; }
    public short ProgramID { get; set; }
    public short Unk0x24_Or_0x2C_EntryID { get; set; }

    public static ShaderDefinition FromStream(BinaryStream bs, long basePos = 0)
    {
        ShaderDefinition def = new ShaderDefinition();

        int nameOffset = bs.ReadInt32();
        def.UnkID = bs.ReadInt32();
        def.ProgramID = bs.ReadInt16();
        def.Unk0x24_Or_0x2C_EntryID = bs.ReadInt16();

        bs.Position = basePos + nameOffset;
        def.Name = bs.ReadString(StringCoding.ZeroTerminated);

        return def;
    }

    public override string ToString()
    {
        return $"{Name} (UnkID: {UnkID}, ProgramID: {ProgramID}, Unk0x24_0x2C: {Unk0x24_Or_0x2C_EntryID})";
    }
}
