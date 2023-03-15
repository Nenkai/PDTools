using PDTools.Files.Models.VM.Instructions;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public abstract class ModelCommand
    {
        public byte Opcode { get; private set; }

        public int Offset { get; set; }

        public abstract void Read(BinaryStream bs, int commandsBaseOffset);

        public abstract void Write(BinaryStream bs);

        public static ModelCommand GetByOpcode(byte opcode)
        {
            ModelCommand cmd = opcode switch
            {
                0 => new Command_0_End(),
                2 => new Command_2_Unk(),
                3 => new Command_3_LoadMeshByteIndex(),
                4 => new Command_4_LoadMeshUShortIndex(),
                5 => new Command_5_Switch(),
                6 => new Command_6_Unk(),
                7 => new Command_7_Unk(),
                8 => new Command_8_Unk(),
                9 => new Command_9_JumpToByte(),
                10 => new Command_10_JumpToShort(),
                11 => new Command_11_PSP_Unk(),
                13 => new Command_13_PSP_Unk(),
                14 => new Command_14_Unk(),
                15 => new Command_15_Unk(),
                16 => new Command_16_PushMatrixMaybe(),
                17 => new Command_17_PushMatrix2Maybe(),
                18 => new Command_18_Unk(),
                19 => new Command_19_Unk(),
                20 => new Command_20_Unk(),
                21 => new Command_21_Unk(),
                22 => new Command_22_Unk(),
                23 => new Command_23_Unk(),
                24 => new Command_24_SetDepthTestEnabled(),
                25 => new Command_25_SetDepthTestDisabled(),
                26 => new Command_26_SetDepthFunc(),
                27 => new Command_27_SetAlphaTestEnabled(),
                28 => new Command_28_SetAlphaTestDisabled(),
                29 => new Command_29_SetAlphaFunc(),
                34 => new Command_34_SetColorMask(),
                35 => new Command_35_SetDepthMaskEnabled(),
                36 => new Command_36_SetDepthMaskDisabled(),
                37 => new Command_37_SetPolyOffsetScaleFactor(),
                38 => new Command_38_SetCullFaceDisable(),
                39 => new Command_39_SetCullFaceDisable(),
                40 => new Command_40_SetCullFaceBack(),
                41 => new Command_41_SetCullFaceFront(),
                42 => new Command_42_SetCullFaceSwitch(),
                43 => new Command_43_SetVMUnk(),
                44 => new Command_44_CallVM_Ptr2(),
                45 => new Command_45_Unk(),
                46 => new Command_46_Unk(),
                47 => new Command_47_Unk(),
                49 => new Command_49_Unk(),
                50 => new Command_50_Unk(),
                51 => new Command_51_Unk(),
                52 => new Command_52_Unk(),
                53 => new Command_53_Unk(),
                54 => new Command_54_Unk(),
                55 => new Command_55_PSP_Unk(),
                56 => new Command_55_PSP_Unk(),
                57 => new Command_55_PSP_Unk(),
                58 => new Command_55_PSP_Unk(),
                59 => new Command_59_LoadMesh2_Byte(),
                60 => new Command_60_LoadMesh2_UShort(),
                61 => new Command_61_Semaphore_InvalidateL2(),
                62 => new Command_62_Unk(),
                65 => new Command_65_Unk(),
                66 => new Command_66_Unk(),
                67 => new Command_67_Unk(),
                68 => new Command_68_Unk(),
                69 => new Command_69_Unk(),
                70 => new Command_70_Unk(),
                71 => new Command_71_Unk(),
                72 => new Command_72_Unk(),
                74 => new Command_74_LoadMultipleMeshes(),
                75 => new Command_75_LoadMultipleMeshes2(),
                77 => new Command_77_Unk(),
                _ => throw new Exception($"Unexpected opcode {opcode}")
            };

            cmd.Opcode = opcode;
            return cmd;
        }
    }
}
