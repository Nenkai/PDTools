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
        public abstract VMInstructionOpcode Opcode { get; }

        public int Offset { get; set; }
        public abstract void Read(BinaryStream bs, int commandsBaseOffset);

        public abstract void Write(BinaryStream bs);

        public static VMInstruction GetByOpcode(VMInstructionOpcode opcode)
        {
            return opcode switch
            {
                VMInstructionOpcode.StackVariableEval => new VM0x10(),
                VMInstructionOpcode.PushIntConst => new VMIntConst(),
                VMInstructionOpcode.RegisterEval => new VMVariableEval(),
                VMInstructionOpcode.StackVariablePush => new VMVariablePush(),
                VMInstructionOpcode.RegisterAssignPop => new VMPopAssignToLocal(),

                VMInstructionOpcode.StackAdvance => new VMStackSeek(),
                VMInstructionOpcode.GetStackAddressMaybe => throw new NotImplementedException(),
                VMInstructionOpcode.Unk0x22 => throw new NotImplementedException(),
                VMInstructionOpcode.Return => new VMReturn(),
                VMInstructionOpcode.JumpAbsolute => throw new NotImplementedException(),
                VMInstructionOpcode.JumpRelative => new VMJump(),
                VMInstructionOpcode.JumpNotZero => new VMJumpIfFalse(),
                VMInstructionOpcode.JumpZero => throw new NotImplementedException(),
                VMInstructionOpcode.Pop => throw new NotImplementedException(),

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
                VMInstructionOpcode.FloatModulus => new VMModuloF(),
                VMInstructionOpcode.FloatLesserThan => new VMLesserThanF(),
                VMInstructionOpcode.FloatLesserOrEqualTo => new VMLesserEqualToF(),
                VMInstructionOpcode.FloatGreaterThen => new VMGreaterThanF(),
                VMInstructionOpcode.FloatGreaterOrEqualTo => new VMGreaterEqualToF(),
                VMInstructionOpcode.FloatEquals => new VMEqualsF(),
                VMInstructionOpcode.FloatNotEquals => new VMNotEqualsF(),
                VMInstructionOpcode.FloatUnaryMinusOperator => new VMUnaryMinusF(),
                VMInstructionOpcode.ToInt => new VMFloatToInt(),

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

                VMInstructionOpcode.CallbackWithIntResult => throw new NotImplementedException(),
                VMInstructionOpcode.CallbackWithFloatResult => throw new NotImplementedException(),
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
}
