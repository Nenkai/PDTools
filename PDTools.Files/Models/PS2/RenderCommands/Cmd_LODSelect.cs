using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

/// <summary>
/// Whether to render this model based on bbox, seeks past commands if not
/// </summary>
public class Cmd_LODSelect : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.LODSelect;

    public Vector3 Unk { get; set; }
    public float Unk2 { get; set; }
    public List<List<ModelSetupPS2Command>> CommandsPerLOD { get; set; } = new();

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        Unk = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        Unk2 = bs.ReadSingle();
        byte lodCount = bs.Read1Byte();

        long tableOffset = bs.Position;
        ushort[] jumpOffsets = bs.ReadUInt16s(lodCount);

        for (int i = 0; i < lodCount; i++)
        {
            List<ModelSetupPS2Command> lodCommands = new List<ModelSetupPS2Command>();
            long startOffset = tableOffset + i * sizeof(ushort);
            long nextStartOffset = startOffset + sizeof(ushort);

            bs.Position = startOffset + jumpOffsets[i];
            long endOffset = nextStartOffset + (i == jumpOffsets.Length - 1 ? bs.Length : jumpOffsets[i + 1]);

            while (bs.Position < endOffset)
            {
                ModelSetupPS2Opcode opcode = (ModelSetupPS2Opcode)bs.Read1Byte();
                if (opcode == ModelSetupPS2Opcode.End || opcode == ModelSetupPS2Opcode.pglEnableRendering)
                {
                    bs.Position -= 1;
                    break;
                }

                var cmd = GetByOpcode(opcode);
                cmd.Read(bs, 0);

                if (opcode != ModelSetupPS2Opcode.Jump_Byte && opcode != ModelSetupPS2Opcode.Jump_UShort)
                    lodCommands.Add(cmd);
            }

            CommandsPerLOD.Add(lodCommands);
        }
    }

    /// <summary>
    /// Writes the lod select command. This will take care of jumps.
    /// </summary>
    /// <param name="bs"></param>
    public override void Write(BinaryStream bs)
    {
        bs.WriteSingle(Unk.X); bs.WriteSingle(Unk.Y); bs.WriteSingle(Unk.Z);
        bs.WriteSingle(Unk2);
        bs.WriteByte((byte)CommandsPerLOD.Count);

        long tableOffset = bs.Position;
        long dataOffset = bs.Position + sizeof(ushort) * CommandsPerLOD.Count;

        List<long> jumpOffsets = new List<long>();

        for (int i = 0; i < CommandsPerLOD.Count; i++)
        {
            long tableEntryOffset = tableOffset + sizeof(ushort) * i;
            bs.Position = tableEntryOffset;
            bs.WriteUInt16((ushort)(dataOffset - tableEntryOffset));

            bs.Position = dataOffset;

            foreach (var cmd in CommandsPerLOD[i])
            {
                bs.WriteByte((byte)cmd.Opcode);
                cmd.Write(bs);
            }

            if (i != CommandsPerLOD.Count - 1)
            {
                bs.WriteByte((byte)ModelSetupPS2Opcode.Jump_UShort);
                jumpOffsets.Add(bs.Position);
                bs.WriteInt16(0);
            }

            dataOffset = bs.Position;
        }

        for (int i = 0; i < jumpOffsets.Count; i++)
        {
            bs.Position = jumpOffsets[i];
            bs.WriteUInt16((ushort)(dataOffset - jumpOffsets[i]));
        }

        bs.Position = dataOffset;
    }

    public void SetNumberOfLODs(int num)
    {
        CommandsPerLOD.Clear();
        for (int i = 0; i < num; i++)
            CommandsPerLOD.Add(new List<ModelSetupPS2Command>());
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_LODSelect)}";
    }
}
