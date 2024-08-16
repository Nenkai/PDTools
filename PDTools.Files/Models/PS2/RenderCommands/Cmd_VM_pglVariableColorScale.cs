using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands;

/// <summary>
/// GT4 and above. Calls pglVariableColorScale
/// </summary>
public class Cmd_VM_pglVariableColorScale : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.VM_pglVariableColorScale;

    public ushort Reg1 { get; set; }
    public ushort Reg2 { get; set; }
    public ushort Reg3 { get; set; }
    public ushort Reg4 { get; set; }

    public Cmd_VM_pglVariableColorScale()
    {

    }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        Reg1 = bs.ReadUInt16();
        Reg2 = bs.ReadUInt16();
        Reg3 = bs.ReadUInt16();
        Reg4 = bs.ReadUInt16();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteUInt16(Reg1);
        bs.WriteUInt16(Reg2);
        bs.WriteUInt16(Reg3);
        bs.WriteUInt16(Reg4);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_VM_pglVariableColorScale)}";
    }
}
