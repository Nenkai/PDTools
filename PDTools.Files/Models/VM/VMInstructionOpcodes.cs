using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.VM
{
    public enum VMInstructionOpcode : byte
    {
        StackVariableEval = 0x10,
        PushIntConst = 0x11,
        RegisterEval = 0x12,
        StackVariablePush = 0x13,
        RegisterAssignPop = 0x14,

        StackAdvance = 0x20,
        GetStackAddressMaybe = 0x21,
        Unk0x22 = 0x22,
        Return = 0x23,
        JumpAbsolute = 0x24,
        JumpRelative = 0x25,
        JumpNotZero = 0x26,
        JumpZero = 0x27,
        Pop = 0x28,

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
        FloatModulus = 0x84,
        FloatLesserThan = 0x85,
        FloatLesserOrEqualTo = 0x86,
        FloatGreaterThen = 0x87,
        FloatGreaterOrEqualTo = 0x88,
        FloatEquals = 0x89,
        FloatNotEquals = 0x8A,
        FloatUnaryMinusOperator = 0x8B,
        ToInt = 0x8C,

        FloatRand = 0xA0,
        FloatSin = 0xA1,
        FloatCos = 0xA2,
        FloatTan = 0xA3,
        FloatSquare = 0xA4,
        FloatCosUnk = 0xA5,
        FloatSign = 0xA6, // Not in GT4, GT PSP

        FloatPow = 0xB0,
        FloatAtan2 = 0xB1,
        FloatUnk0xB2 = 0xB2,

        FloatMin = 0xB3, // Not in GT4, GT PSP
        FloatMax = 0xB4, // Not in GT4, GT PSP

        CallbackWithIntResult = 0xC0,
        CallbackWithFloatResult = 0xC1,
        Unk0xC2 = 0xC2, // Not in GT4, GT PSP
        Unk0xC3 = 0xC3, // Not in GT4, GT PSP
        Unk0xC4 = 0xC4, // Not in GT4, GT PSP
        Unk0xC5 = 0xC5, // Not in GT4, GT PSP
        Unk0xC6 = 0xC6, // Not in GT4, GT PSP
    }
}
