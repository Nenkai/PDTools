using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.RenderCommands;

/// <summary>
/// Calls a model callback - different per type of model
/// </summary>
public class Cmd_CallModelCallback : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.CallModelCallback;

    public List<ModelSetupPS2Command> Default = [];
    public List<List<ModelSetupPS2Command>> CommandsPerBranch = [];

    public ModelCallbackParameter Parameter { get; set; }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        Parameter = (ModelCallbackParameter)bs.ReadUInt16();

        ushort branchCount = bs.Read1Byte();

        long tableOffset = bs.Position;
        ushort[] jumpOffsets = bs.ReadUInt16s(branchCount); // Last is always skip?

        if (branchCount > 0)
        {
            // Default
            long endOfAllOffset = 0;
            while (bs.Position < tableOffset + jumpOffsets[0])
            {
                ModelSetupPS2Opcode opcode = (ModelSetupPS2Opcode)bs.Read1Byte();
                if (opcode == ModelSetupPS2Opcode.End)
                    break;

                var cmd = GetByOpcode(opcode);
                cmd.Read(bs, 0);

                if (opcode == ModelSetupPS2Opcode.Jump_UShort || opcode == ModelSetupPS2Opcode.Jump_Byte)
                {
                    if (opcode == ModelSetupPS2Opcode.Jump_UShort)
                        endOfAllOffset = bs.Position - 2 + (cmd as Cmd_JumpUShort).JumpOffset;
                    else if (opcode == ModelSetupPS2Opcode.Jump_Byte)
                        endOfAllOffset = bs.Position - 1 + (cmd as Cmd_JumpByte).JumpOffset;
                    break;
                }

                Default.Add(cmd);
            }

            // Commands per returned parameter
            for (int i = 0; i < branchCount; i++)
            {
                List<ModelSetupPS2Command> thisBranchCmds = new List<ModelSetupPS2Command>();
                long startOffset = tableOffset + i * sizeof(ushort);
                long nextStartOffset = startOffset + sizeof(ushort);

                bs.Position = startOffset + jumpOffsets[i];

                // Attempt to use default jump offset as delimiter for last branch
                long endOffset = i == branchCount - 1 ? endOfAllOffset : nextStartOffset + jumpOffsets[i + 1];

                while (bs.Position < endOffset)
                {
                    ModelSetupPS2Opcode opcode = (ModelSetupPS2Opcode)bs.Read1Byte();
                    if (opcode == ModelSetupPS2Opcode.End)
                        break;

                    var cmd = GetByOpcode(opcode);
                    cmd.Read(bs, 0);

                    // New jump = likely branch end
                    if (opcode == ModelSetupPS2Opcode.Jump_UShort || opcode == ModelSetupPS2Opcode.Jump_Byte)
                        break;

                    thisBranchCmds.Add(cmd);
                }

                CommandsPerBranch.Add(thisBranchCmds);
            }
        }
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteUInt16((ushort)Parameter);
        bs.WriteByte((byte)CommandsPerBranch.Count);

        long tableOffset = bs.Position;
        long dataOffset = bs.Position + sizeof(ushort) * CommandsPerBranch.Count;
        bs.Position = dataOffset;

        foreach (var cmd in Default)
        {
            bs.WriteByte((byte)cmd.Opcode);
            cmd.Write(bs);
        }

        bs.WriteByte((byte)ModelSetupPS2Opcode.Jump_UShort);
        long skipAllJumpOffset = bs.Position;
        bs.WriteInt16(0);

        dataOffset = bs.Position;

        List<long> jumpOffsets = [];

        for (int i = 0; i < CommandsPerBranch.Count; i++)
        {
            long off = tableOffset + sizeof(ushort) * i;
            bs.Position = off;
            bs.WriteUInt16((ushort)(dataOffset - off));
            bs.Position = dataOffset;

            foreach (var cmd in CommandsPerBranch[i])
            {
                bs.WriteByte((byte)cmd.Opcode);
                cmd.Write(bs);
            }

            if (i != CommandsPerBranch.Count - 1)
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

        bs.Position = skipAllJumpOffset;
        bs.WriteUInt16((ushort)(dataOffset - skipAllJumpOffset));

        bs.Position = dataOffset;
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_CallModelCallback)}";
    }
}

// Handlers:
// - 0021FD10 (GT3 EU)
// - 002F3AC0 (GT4O US)
public enum ModelCallbackParameter : ushort
{
    /// <summary>
    /// Supported by >=GT3
    /// </summary>
    IsTailLampActive = 0,

    /// <summary>
    /// Supported by >=GT3
    /// </summary>
    TweenShapeSpeedRandom_1 = 1,

    /// <summary>
    /// Supported by >=GT3
    /// </summary>
    TweenShapeSpeedRandom_2 = 2,

    /// <summary>
    /// Supported by >=GT3
    /// </summary>
    TweenShapeSpeedRandom_3 = 3,

    /// <summary>
    /// Supported by >=GT3
    /// </summary>
    TweenShapeSpeedRandom_4 = 4,

    /// <summary>
    /// Supported by >=GT3
    /// </summary>
    SetSteering = 5,

    /// <summary>
    /// Supported by >=GT4<br/>
    /// <code>ModelSet2::setShapeTweenRatio(*(float *)&a1->CarModel->ActiveWing);</code>
    /// </summary>
    SetActiveWingShapeTweenRatio = 6,

    /// <summary>
    /// Supported by >=GT4
    /// <code>result = a1->CarModel->TimeZone;</code>
    /// </summary>
    GetTimeZone = 7,

    /// <summary>
    /// Supported by >=GT4 <br/>
    /// <code>pglRotateZ(a1->CarModel->field_64 * 57.29578)</code>
    /// </summary>
    RotateZ = 8,

    /// <summary>
    /// Supported by >=GT4 <br/>
    /// <code>pglMultMatrix((__int64 *)&a1->CarModel->field_5A0)</code>
    /// </summary>
    Unk9 = 9,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkTire11 = 11,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkTire12 = 12,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkTire13_ = 13,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkTire14 = 14,

    // Group of 4
    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    RenderTire_1 = 15,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    RenderTire_2 = 16,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    RenderTire_3 = 17,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    RenderTire_4 = 18,

    // Group from 20-30 - GT4 commands involving a matrix

    // Group of 4
    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    RenderWheel_1 = 36,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    RenderWheel_2 = 37,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    RenderWheel_3 = 38,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    RenderWheel_4 = 39,

    // Group of 4
    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkShapeTweenRatio42 = 42,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkShapeTweenRatio43 = 43,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkShapeTweenRatio44 = 44,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkShapeTweenRatio45 = 45,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkWheelRelated46 = 46,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkWingRelated46 = 47,

    // Group of 4
    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkBikeRelated48 = 48,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkBikeRelated49 = 49,

    // Group of 2
    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkShapeTweenRatio52 = 52,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkShapeTweenRatio53 = 53,

    // Group of 2
    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkShapeTweenRatio56 = 56,


    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkShapeTweenRatio57 = 57,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkShapeTweenRatio60 = 60,

    // Group of 4
    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkShapeTweenRatio62 = 62,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkShapeTweenRatio63 = 63,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkShapeTweenRatio64 = 64,

    /// <summary>
    /// Supported by >=GT4
    /// </summary>
    UnkShapeTweenRatio65 = 65,

    /// <summary>
    /// Supported by >=GT4
    /// <code>result = a1->CarModel->field_90</code>
    /// </summary>
    Unk66 = 66,
}
