using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

/// <summary>
/// GT4 and above. Calls pglRotate using VM output registers.
/// </summary>
public class Cmd_VM_Branch : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.VM_Branch;

    public ushort OutRegisterIndex { get; set; }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        OutRegisterIndex = bs.ReadUInt16();
        byte numBranches = bs.Read1Byte();
        short[] offs = bs.ReadInt16s(numBranches);
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteUInt16(OutRegisterIndex);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pglRotate)}";
    }
}
