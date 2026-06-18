using System.Numerics;
using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

/// <summary>
/// Calls pglCylinderMapHint with 3 floats. Opcode 49 (0x31).
/// </summary>
public class Cmd_pglCylinderMapHint : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglCylinderMapHint;

    public Vector3 Hint { get; set; }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        Hint = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteSingle(Hint.X);
        bs.WriteSingle(Hint.Y);
        bs.WriteSingle(Hint.Z);
    }

    public override string ToString()
        => $"{nameof(Cmd_pglCylinderMapHint)}({Hint.X:G}, {Hint.Y:G}, {Hint.Z:G})";
}
