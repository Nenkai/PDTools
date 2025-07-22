using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Utils;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh.Commands;

public class SmoothMeshTessellationCommandList
{
    public List<SmoothMeshTessellationCommandBase> Commands { get; } = [];

    public static SmoothMeshTessellationCommandList Read(SmoothMeshTessellationCommandContext ctx, ref BitStream bs)
    {
        var list = new SmoothMeshTessellationCommandList();

        int counter = 0;
        while (true)
        {
            byte opcode = (byte)bs.ReadBits(5);

            var cmd = SmoothMeshTessellationCommandBase.Create(opcode);
            cmd.Read(ctx, ref bs);
            list.Commands.Add(cmd);

            if (cmd is SmoothMeshTessellationCommand0)
                break;

            if (counter++ >= 10000)
                throw new InvalidDataException("Exceeded 10000 smooth mesh commands in list, data is likely bad");

        }

        return list;
    }
}
