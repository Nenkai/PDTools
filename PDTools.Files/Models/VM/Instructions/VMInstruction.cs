using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.VM.Instructions
{
    public abstract class VMInstruction
    {
        public int Offset { get; set; }
        public abstract void Read(BinaryStream bs, int commandsBaseOffset);

        public abstract void Write(BinaryStream bs);

        public static VMInstruction GetByOpcode(VMInstructionOpcode opcode)
        {
            return opcode switch
            {
                VMInstructionOpcode.StackPushFromStackIndex0x10 => new VM0x10(),
                VMInstructionOpcode.StackPushConstInt0x11 => new VMIntConst(),
                VMInstructionOpcode.StackPushValueFromStorageIdx0x12 => new VMVariableEval(),
                VMInstructionOpcode.StackAssignPop0x13 => new VMVariablePush(),
                VMInstructionOpcode.StackAssignToStoragePop0x14 => new VMPopAssignToLocal(),

                VMInstructionOpcode.JumpToByte => new VMStackSeek(),
                VMInstructionOpcode.JumpUnk0x21 => throw new NotImplementedException(),
                VMInstructionOpcode.JumpToShort => throw new NotImplementedException(),
                VMInstructionOpcode.Return => new VMReturn(),
                VMInstructionOpcode.JumpUnk0x24 => throw new NotImplementedException(),
                VMInstructionOpcode.JumpUnk0x25 => new VMJump(),
                VMInstructionOpcode.JumpIfFalse0x26 => new VMJumpIfFalse(),
                VMInstructionOpcode.JumpUnk0x27 => throw new NotImplementedException(),
                VMInstructionOpcode.JumpUnk0x28 => throw new NotImplementedException(),

                VMInstructionOpcode.Add => new VMAdd(),
                VMInstructionOpcode.Subtract => new VMSubtract(),
                VMInstructionOpcode.Multiply => new VMMultiply(),
                VMInstructionOpcode.Divide => new VMDivide(),
                VMInstructionOpcode.Modulo => new VMModulo(),
                VMInstructionOpcode.LesserThan => new VMLesserThan(),
                VMInstructionOpcode.LesserOrEqualTo => new VMLesserEqualTo(),
                VMInstructionOpcode.GreaterThen => new VMGreaterThan(),
                VMInstructionOpcode.GreaterOrEqualTo => new VMGreaterEqualTo(),
                VMInstructionOpcode.Equals => new VMEquals(),
                VMInstructionOpcode.NotEqual => new VMNotEquals(),
                VMInstructionOpcode.UnaryMinusOperator => new VMUnaryMinus(),
                VMInstructionOpcode.ToFloat => new VMIntToFloat(),

                VMInstructionOpcode.BinaryAndOperator => new VMBinaryAnd(),
                VMInstructionOpcode.BinaryOrOperator => new VMBinaryOr(),
                VMInstructionOpcode.BinaryXorOperator => new VMBinaryXor(),
                VMInstructionOpcode.BinaryLeftShiftOperator => new VMBinaryLeftShift(),
                VMInstructionOpcode.BinaryRightShiftOperator => new VMBinaryRightShift(),
                VMInstructionOpcode.UnaryBitwiseNotOperator => new VMUnaryBitwiseNot(),

                VMInstructionOpcode.FloatAddition => new VMAddF(),
                VMInstructionOpcode.FloatSubtract => new VMSubtractF(),
                VMInstructionOpcode.FloatMultiply => new VMMultiplyF(),
                VMInstructionOpcode.FloatDivide => new VMDivideF(),
                VMInstructionOpcode.FloatMod => new VMModuloF(),
                VMInstructionOpcode.FloatLesserThan => new VMLesserThan(),
                VMInstructionOpcode.FloatLesserOrEqualTo => new VMLesserEqualToF(),
                VMInstructionOpcode.FloatGreaterThen => new VMGreaterThanF(),
                VMInstructionOpcode.FloatGreaterOrEqualTo => new VMGreaterEqualToF(),
                VMInstructionOpcode.FloatEquals => new VMEqualsF(),
                VMInstructionOpcode.FloatNotEquals => new VMNotEqualsF(),
                VMInstructionOpcode.FloatUnaryMinusOperator => new VMUnaryMinusF(),
                VMInstructionOpcode.FloatToInt => new VMFloatToInt(),

                VMInstructionOpcode.FloatRand => new VMRandF(),
                VMInstructionOpcode.FloatSin => new VMSinF(),
                VMInstructionOpcode.FloatCos => new VMCosF(),
                VMInstructionOpcode.FloatTan => new VMTanF(),
                VMInstructionOpcode.FloatSquare => new VMSqrtF(),
                VMInstructionOpcode.FloatCosUnk => throw new NotImplementedException(),
                VMInstructionOpcode.FloatSign => new VMSignF(), // Not in GT PSP

                VMInstructionOpcode.FloatPow => new VMPowF(),
                VMInstructionOpcode.FloatAtan2 => new VMAtan2F(),
                VMInstructionOpcode.FloatUnk0xB2 => throw new NotImplementedException(),

                VMInstructionOpcode.FloatMin => new VMMinF(),
                VMInstructionOpcode.FloatMax => new VMMaxF(),

                VMInstructionOpcode.UnkCall0xC0 => throw new NotImplementedException(),
                VMInstructionOpcode.UnkCall0xC1 => throw new NotImplementedException(),
                VMInstructionOpcode.Unk0xC2 => new VM0xC2(),
                VMInstructionOpcode.Unk0xC3 => throw new NotImplementedException(),
                VMInstructionOpcode.Unk0xC4 => throw new NotImplementedException(),
                VMInstructionOpcode.Unk0xC5 => throw new NotImplementedException(),
                VMInstructionOpcode.Unk0xC6 => throw new NotImplementedException(),
                _ => throw new InvalidOperationException("Not implemented at all"),
            };
        }

        public virtual string Disassemble(Dictionary<short, VMHostMethodEntry> values)
        {
            return ToString();
        }
    }

    public enum VMInstructionOpcode : byte
    {
        StackPushFromStackIndex0x10 = 0x10,
        StackPushConstInt0x11 = 0x11,
        StackPushValueFromStorageIdx0x12 = 0x12,
        StackAssignPop0x13 = 0x13,
        StackAssignToStoragePop0x14 = 0x14,

        JumpToByte = 0x20,
        JumpUnk0x21 = 0x21,
        JumpToShort = 0x22,
        Return = 0x23,
        JumpUnk0x24 = 0x24,
        JumpUnk0x25 = 0x25,
        JumpIfFalse0x26 = 0x26,
        JumpUnk0x27 = 0x27,
        JumpUnk0x28 = 0x28,

        Add = 0x30,
        Subtract = 0x31,
        Multiply = 0x32,
        Divide = 0x33,
        Modulo = 0x34,
        LesserThan = 0x35,
        LesserOrEqualTo = 0x36,
        GreaterThen = 0x37,
        GreaterOrEqualTo = 0x38,
        Equals = 0x39,
        NotEqual = 0x3A,
        UnaryMinusOperator = 0x3B,
        ToFloat = 0x3C,

        BinaryAndOperator = 0x40,
        BinaryOrOperator = 0x41,
        BinaryXorOperator = 0x42,
        BinaryLeftShiftOperator = 0x44,
        BinaryRightShiftOperator = 0x45,
        UnaryBitwiseNotOperator = 0x46,

        FloatAddition = 0x80,
        FloatSubtract = 0x81,
        FloatMultiply = 0x82,
        FloatDivide = 0x83,
        FloatMod = 0x84,
        FloatLesserThan = 0x85,
        FloatLesserOrEqualTo = 0x86,
        FloatGreaterThen = 0x87,
        FloatGreaterOrEqualTo = 0x88,
        FloatEquals = 0x89,
        FloatNotEquals = 0x8A,
        FloatUnaryMinusOperator = 0x8B,
        FloatToInt = 0x8C,

        FloatRand = 0xA0,
        FloatSin = 0xA1,
        FloatCos = 0xA2,
        FloatTan = 0xA3,
        FloatSquare = 0xA4,
        FloatCosUnk = 0xA5,
        FloatSign = 0xA6, // Not in GT PSP

        FloatPow = 0xB0,
        FloatAtan2 = 0xB1,
        FloatUnk0xB2 = 0xB2,

        FloatMin = 0xB3, // Not in GT PSP
        FloatMax = 0xB4, // Not in GT PSP

        UnkCall0xC0 = 0xC0,
        UnkCall0xC1 = 0xC1,
        Unk0xC2 = 0xC2, // Not in GT PSP
        Unk0xC3 = 0xC3, // Not in GT PSP
        Unk0xC4 = 0xC4, // Not in GT PSP
        Unk0xC5 = 0xC5, // Not in GT PSP
        Unk0xC6 = 0xC6, // Not in GT PSP
    };
}
