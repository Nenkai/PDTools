using PDTools.Files.Models.VM.Instructions;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS3.PGLCommands
{
    // The command set differs a bit from the commands from PS2
    public abstract class ModelSetupCommand
    {
        public ModelSetupOpcode Opcode { get; private set; }

        public int Offset { get; set; }

        public abstract void Read(BinaryStream bs, int commandsBaseOffset);

        public abstract void Write(BinaryStream bs);

        public static ModelSetupCommand GetByOpcode(byte opcode)
        {
            ModelSetupCommand cmd = opcode switch
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
                16 => new Command_16_PGLInverse(),
                17 => new Command_17_PGLTranslate(),
                18 => new Command_18_PGLScale(),
                19 => new Command_19_PGLRotate(),
                20 => new Command_20_PGLRotateX(),
                21 => new Command_21_PGLRotateY(),
                22 => new Command_22_PGLRotateZ(),
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
                56 => new Command_56_PSP_Unk(),
                57 => new Command_57_PSP_Unk(),
                58 => new Command_58_PSP_Unk(),
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

            cmd.Opcode = (ModelSetupOpcode)opcode;
            return cmd;
        }
    }

    public enum ModelSetupOpcode : byte
    {
        Command_0_End = 0,
        Command_2_Unk = 2,
        Command_3_LoadMeshByteIndex = 3,
        Command_4_LoadMeshUShortIndex = 4,
        Command_5_Switch = 5,
        Command_6_Unk = 6,
        Command_7_Unk = 7,
        Command_8_Unk = 8,
        Command_9_JumpToByte = 9,
        Command_10_JumpToShort = 10,
        Command_11_PSP_Unk = 11,
        Command_13_PSP_Unk = 13,
        Command_14_Unk = 14,
        Command_15_Unk = 15,
        Command_16_PushMatrixMaybe = 16,
        Command_17_PushMatrix2Maybe = 17,
        Command_18_Unk = 18,
        Command_19_Unk = 19,
        Command_20_Unk = 20,
        Command_21_Unk = 21,
        Command_22_Unk = 22,
        Command_23_Unk = 23,
        Command_24_SetDepthTestEnabled = 24,
        Command_25_SetDepthTestDisabled = 25,
        Command_26_SetDepthFunc = 26,
        Command_27_SetAlphaTestEnabled = 27,
        Command_28_SetAlphaTestDisabled = 28,
        Command_29_SetAlphaFunc = 29,
        Command_34_SetColorMask = 34,
        Command_35_SetDepthMaskEnabled = 35,
        Command_36_SetDepthMaskDisabled = 36,
        Command_37_SetPolyOffsetScaleFactor = 37,
        Command_38_SetCullFaceDisable = 38,
        Command_39_SetCullFaceDisable = 39,
        Command_40_SetCullFaceBack = 40,
        Command_41_SetCullFaceFront = 41,
        Command_42_SetCullFaceSwitch = 42,
        Command_43_SetVMUnk = 43,
        Command_44_CallVM_Ptr2 = 44,
        Command_45_Unk = 45,
        Command_46_Unk = 46,
        Command_47_Unk = 47,
        Command_49_Unk = 49,
        Command_50_Unk = 50,
        Command_51_Unk = 51,
        Command_52_Unk = 52,
        Command_53_Unk = 53,
        Command_54_Unk = 54,
        Command_55_PSP_Unk = 55,
        Command_56_PSP_Unk = 56,
        Command_57_PSP_Unk = 57,
        Command_58_PSP_Unk = 58,
        Command_59_LoadMesh2_Byte = 59,
        Command_60_LoadMesh2_UShort = 60,
        Command_61_Semaphore_InvalidateL2 = 61,
        Command_62_Unk = 62,
        Command_65_Unk = 65,
        Command_66_Unk = 66,
        Command_67_Unk = 67,
        Command_68_Unk = 68,
        Command_69_Unk = 69,
        Command_70_Unk = 70,
        Command_71_Unk = 71,
        Command_72_Unk = 72,
        Command_74_LoadMultipleMeshes = 74,
        Command_75_LoadMultipleMeshes2 = 75,
        Command_77_Unk = 77,
    }
}
