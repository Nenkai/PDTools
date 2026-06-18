using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

/// <summary>
/// GT4 and above. Calls ModelSet2::setShapeTweenRatio with a direct float argument.
/// Opcode 52 (0x34).
/// </summary>
public class Cmd_ModelSet_setShapeTweenRatio : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.ModelSet_setShapeTweenRatio;

    public float Ratio { get; set; }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        Ratio = bs.ReadSingle();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteSingle(Ratio);
    }

    public override string ToString()
        => $"{nameof(Cmd_ModelSet_setShapeTweenRatio)}({Ratio:G})";
}
