using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

/// <summary>
/// Calls pglTranslate, same as glTranslate. Multiplies the current matrix by the specified translation vector
/// </summary>
public class Cmd_pglTranslate : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglTranslate;

    /// <summary>
    /// Translation vector.
    /// </summary>
    public Vector3 Vector { get; set; }

    /// <summary>Absolute stream byte offset of the translation floats (recorded on read so
    /// callers can scale the placement in place without re-serialising).</summary>
    public long VectorStreamOffset { get; set; }

    public Cmd_pglTranslate()
    {

    }

    public Cmd_pglTranslate(float x, float y, float z)
    {
        Vector = new Vector3(x, y, z);
    }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        VectorStreamOffset = bs.Position;
        Vector = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteSingle(Vector.X); bs.WriteSingle(Vector.Y); bs.WriteSingle(Vector.Z);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_pglTranslate)}";
    }
}
