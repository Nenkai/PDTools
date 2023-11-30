using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;

using PDTools.Files.Models.PS2.ModelSet;

namespace PDTools.Files.Models.VM
{
    internal class VMContext
    {
        public RegisterVal[][] Registers = new RegisterVal[3][];
        private ModelSet2 set; // TODO remove this

        // GT4O 0x2F8608
        public VMContext(ModelSet2.Instance instance)
        {
            set = instance.ModelSet;
            Registers[0] = instance.OutputRegisters;
            Registers[1] = instance.Unk2;
            Registers[2] = instance.HostMethodRegisters;
        }

        // RE'd - GT4O 0x2F8628
        public void callVM(Span<byte> bytecode, int bytecodeOffset, Func<int, int, int> cb)
        {
            int instCounter = 0;
            Span<byte> codePtr = bytecode[bytecodeOffset..];

            Span<int> stack = stackalloc int[1 + 0x100];
            Span<int> stackFrame = stack;

            int stackPtr = 1;

            while (true)
            {
                if (instCounter > 10000)
                    break;

                VMInstructionOpcode opcode = (VMInstructionOpcode)codePtr[0];
                codePtr = codePtr[1..];

                switch (opcode)
                {
                    // 0x1x - Register/Stack management
                    case VMInstructionOpcode.StackVariableEval: // Push value from stack index
                        {
                            stackPtr++;

                            short stackIndex = BinaryPrimitives.ReadInt16LittleEndian(codePtr);
                            DebugPrint($"{opcode} - push value from stack index {stackIndex}, stack index now {stackPtr}");

                            stack[stackPtr] = stackFrame[stackIndex];

                            codePtr = codePtr[2..];
                        }
                        break;

                    case VMInstructionOpcode.PushIntConst: // Int const
                        stackPtr++;
                        stack[stackPtr] = BinaryPrimitives.ReadInt32LittleEndian(codePtr);
                        DebugPrint($"{opcode} - stack index:{stackPtr}, value:{stack[stackPtr]} ({BitConverter.Int32BitsToSingle(stack[stackPtr])}f)");
                        codePtr = codePtr[4..];
                        break;

                    case VMInstructionOpcode.RegisterEval: // Push from storage
                        {
                            stackPtr++;
                            ushort bits = BinaryPrimitives.ReadUInt16LittleEndian(codePtr);

                            byte registerType = (byte)(bits >> 14);
                            ushort registerIndex = (ushort)(bits & 0x3FFF);
                            if (Registers[registerType] != null)
                            {
                                if (registerType == 0)
                                {
                                    RegisterInfo info = set.OutRegisterInfos.Find(e => e.RegisterIndex == registerIndex);
                                    DebugPrint($"{opcode} - push stack index: {stackPtr} from OutRegister {registerIndex} ({info.Name})");
                                }
                                else if (registerType == 2)
                                {
                                    RegisterInfo info = set.HostMethodInfos.Find(e => e.RegisterIndex == registerIndex);
                                    Registers[registerType][registerIndex].Value = BitConverter.SingleToInt32Bits(0);
                                    DebugPrint($"{opcode} - push stack index: {stackPtr} from HostMethod {registerIndex} ({info.Name})");
                                }
                                else
                                    throw new NotSupportedException();

                                stack[stackPtr] = Registers[registerType][registerIndex].Value;
                            }
                            else
                                stack[stackPtr] = 0;

                            codePtr = codePtr[2..];
                        }
                        break;

                    case VMInstructionOpcode.StackVariablePush: // Pop & set value at stack index
                        {
                            short targetStackIndex = BinaryPrimitives.ReadInt16LittleEndian(codePtr);
                            int lastValue = stack[stackPtr];

                            DebugPrint($"{opcode} - set stack index {targetStackIndex} value to last value of stack (index {stackPtr}), stack index now {stackPtr - 1}");
                            stackPtr--;
                            stackFrame[targetStackIndex] = lastValue;

                            codePtr = codePtr[2..];
                        }
                        break;

                    case VMInstructionOpcode.RegisterAssignPop: // Assign pop?
                        {
                            short bits = BinaryPrimitives.ReadInt16LittleEndian(codePtr);
                            byte registerType = (byte)(bits >> 14);
                            ushort registerIndex = (ushort)(bits & 0x3FFF);

                            int lastValue = stack[stackPtr];
                            stackPtr--;

                            if (Registers[registerType] != null)
                            {
                                if (registerType == 0)
                                {
                                    RegisterInfo info = set.OutRegisterInfos.Find(e => e.RegisterIndex == registerIndex);
                                    DebugPrint($"{opcode} - OutRegister {registerIndex} ({info.Name}) {lastValue} (from stack index: {stackPtr + 1}), stack index now {stackPtr}");
                                }
                                else
                                    throw new NotSupportedException();

                                Registers[registerType][registerIndex].Value = lastValue;
                            }

                            codePtr = codePtr[2..];
                        }
                        break;

                    // 0x2x - Branch/jump logic (& pop for some reason)
                    case VMInstructionOpcode.StackAdvance: // Stack advance - for array declarations maybe?
                        DebugPrint($"{opcode} Advance stack by {codePtr[0]}, {stackPtr} -> {stackPtr + codePtr[0]}, likely making space to {codePtr[0]} variable(s)");

                        stackPtr += codePtr[0];
                        codePtr = codePtr[1..];
                        break;

                    case VMInstructionOpcode.GetStackAddressMaybe: // Debug get stack address?
                        //stack[stackPtr + 1] = stackFrame; ?
                        stackPtr += 2;
                        break;

                    case VMInstructionOpcode.Unk0x22: // ????
                        {
                            byte stackIndex = codePtr[0];
                            codePtr = codePtr[1..];
                            ushort jumpAddr = BinaryPrimitives.ReadUInt16LittleEndian(codePtr);
                            codePtr = codePtr[2..];

                            Span<int> v29 = stack[(stackPtr - stackIndex)..];
                            //v29[0] = codePtr;
                            //*(_QWORD *)&stackFrame = (int)(v29 - 1);
                            codePtr = bytecode[jumpAddr..];
                        }
                        break;

                    case VMInstructionOpcode.Return: // Return?
                        {
                            DebugPrint($"{opcode}");
                            if (stackFrame == stack) // start
                                return; // Done

                            int lastValue = stack[stackPtr];

                            /*
                            stackPtr = stackFrame - 1;
                            stackFrame[-codePtr[0] - 1] = lastValue;
                            codePtr = (unsigned __int8 *)stackFrame[1]
                            *(_QWORD *)&v7_frame = *v7_frame;
                            */
                            break;
                        }
                        break;

                    case VMInstructionOpcode.JumpAbsolute:
                        {
                            DebugPrint($"{opcode}");
                            ushort jumpOffset = BinaryPrimitives.ReadUInt16LittleEndian(codePtr);
                            codePtr = bytecode[jumpOffset..];
                        }
                        break;

                    case VMInstructionOpcode.JumpRelative:
                        {
                            DebugPrint($"{opcode}");
                            ushort relativeJumpOffset = BinaryPrimitives.ReadUInt16LittleEndian(codePtr);
                            codePtr = codePtr[2..];
                            codePtr = codePtr[relativeJumpOffset..];
                        }
                        break;

                    case VMInstructionOpcode.JumpNotZero: 
                        {
                            DebugPrint($"{opcode}");

                            ushort relativeJumpOffset = BinaryPrimitives.ReadUInt16LittleEndian(codePtr);
                            codePtr = codePtr[2..];

                            int lastValue = stack[stackPtr];
                            stackPtr--;

                            if (lastValue != 0)
                                codePtr = codePtr[relativeJumpOffset..];
                        }
                        break;
                    case VMInstructionOpcode.JumpZero:
                        {
                            DebugPrint($"{opcode}");

                            ushort relativeJumpOffset = BinaryPrimitives.ReadUInt16LittleEndian(codePtr);
                            codePtr = codePtr[2..];

                            int lastValue = stack[stackPtr];
                            stackPtr--;

                            if (lastValue != 0)
                                codePtr = codePtr[relativeJumpOffset..];
                        }
                        break;
                    case VMInstructionOpcode.Pop:
                        {
                            DebugPrint($"{opcode}");
                            stackPtr--;
                        }
                        break;

                    // 0x30 - Common integer operations
                    case VMInstructionOpcode.Add:
                        stackPtr--;
                        DebugPrint($"{opcode}, stack index now {stackPtr}");

                        stack[stackPtr] += stack[stackPtr + 1];
                        break;
                    case VMInstructionOpcode.Subtract:
                        stackPtr--;
                        DebugPrint($"{opcode}, stack index now {stackPtr}");

                        stack[stackPtr] -= stack[stackPtr + 1];
                        break;
                    case VMInstructionOpcode.Multiply:
                        stackPtr--;
                        DebugPrint($"{opcode}, stack index now {stackPtr}");

                        stack[stackPtr] *= stack[stackPtr + 1];
                        break;
                    case VMInstructionOpcode.Divide:
                        stackPtr--;
                        DebugPrint($"{opcode}, stack index now {stackPtr}");

                        if (stack[stackPtr] == 0) // Avoid division by zero
                            throw new DivideByZeroException(); //  _break()

                        stack[stackPtr] /= stack[stackPtr + 1];
                        break;
                    case VMInstructionOpcode.Modulo:
                        stackPtr--;
                        DebugPrint($"{opcode}, stack index now {stackPtr}");

                        if (stack[stackPtr + 1] == 0) // Avoid division by zero
                            throw new DivideByZeroException(); //  _break()

                        stack[stackPtr] %= stack[stackPtr + 1];
                        break;
                    case VMInstructionOpcode.LesserThan:
                        {
                            int last = stack[stackPtr];
                            int prev = stack[stackPtr - 1];
                            stackPtr--;

                            DebugPrint($"{opcode} - {prev} < {last}");
                            stack[stackPtr] = prev < last ? 1 : 0;
                        }
                        break;
                    case VMInstructionOpcode.LesserOrEqualTo:
                        {
                            int last = stack[stackPtr];
                            int prev = stack[stackPtr - 1];
                            stackPtr--;

                            DebugPrint($"{opcode} - {prev} <= {last}");
                            stack[stackPtr] = prev <= last ? 1 : 0;
                        }
                        break;
                    case VMInstructionOpcode.GreaterThen:
                        {
                            int last = stack[stackPtr];
                            int prev = stack[stackPtr - 1];
                            stackPtr--;

                            DebugPrint($"{opcode} - {prev} > {last}");
                            stack[stackPtr] = prev > last ? 1 : 0;
                        }
                        break;
                    case VMInstructionOpcode.GreaterOrEqualTo:
                        {
                            int last = stack[stackPtr];
                            int prev = stack[stackPtr - 1];
                            stackPtr--;

                            DebugPrint($"{opcode} - {prev} >= {last}");
                            stack[stackPtr] = prev >= last ? 1 : 0;
                        }
                        break;
                    case VMInstructionOpcode.Equals:
                        {
                            int last = stack[stackPtr];
                            int prev = stack[stackPtr - 1];
                            stackPtr--;

                            DebugPrint($"{opcode} - {prev} == {last}");
                            stack[stackPtr] = prev == last ? 1 : 0;
                        }
                        break;
                    case VMInstructionOpcode.NotEqual:
                        {
                            int last = stack[stackPtr];
                            int prev = stack[stackPtr - 1];
                            stackPtr--;

                            DebugPrint($"{opcode} - {prev} != {last}");
                            stack[stackPtr] = prev != last ? 1 : 0;
                        }
                        break;
                    case VMInstructionOpcode.UnaryMinusOperator:
                        {
                            DebugPrint($"{opcode}");
                            stack[stackPtr] = -stack[stackPtr];
                        }
                        break;
                    case VMInstructionOpcode.ToFloat: // lesser than
                        {
                            DebugPrint($"{opcode}");
                            stack[stackPtr] = BitConverter.SingleToInt32Bits((float)stack[stackPtr]);
                        }
                        break;
                    // 0x4x - Binary operators
                    case VMInstructionOpcode.BinaryAndOperator:
                        stackPtr--;
                        DebugPrint($"{opcode}, stack index now {stackPtr}");

                        stack[stackPtr] = stack[stackPtr] & stack[stackPtr + 1];
                        break;
                    case VMInstructionOpcode.BinaryOrOperator:
                        stackPtr--;
                        DebugPrint($"{opcode}, stack index now {stackPtr}");

                        stack[stackPtr] = stack[stackPtr] | stack[stackPtr + 1];
                        break;
                    case VMInstructionOpcode.BinaryXorOperator:
                        stackPtr--;
                        DebugPrint($"{opcode}, stack index now {stackPtr}");

                        stack[stackPtr] = stack[stackPtr] ^ stack[stackPtr + 1];
                        break;
                    case VMInstructionOpcode.BinaryLeftShiftOperator:
                        DebugPrint($"{opcode}");
                        stack[stackPtr] = stack[stackPtr] << 1;
                        break;
                    case VMInstructionOpcode.BinaryRightShiftOperator:
                        DebugPrint($"{opcode}");
                        stack[stackPtr] = stack[stackPtr] >> 1;
                        break;
                    case VMInstructionOpcode.UnaryBitwiseNotOperator:
                        DebugPrint($"{opcode}");
                        stack[stackPtr] = ~stack[stackPtr];
                        break;

                    // 0x8x - Floats
                    case VMInstructionOpcode.FloatAddition:
                        {
                            float last = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            float prev = BitConverter.Int32BitsToSingle(stack[stackPtr - 1]);
                            stackPtr--;

                            DebugPrint($"{opcode} - {prev} + {last}, stack index now {stackPtr}");
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(prev + last);
                        }
                        break;
                    case VMInstructionOpcode.FloatSubtract:
                        {
                            float last = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            float prev = BitConverter.Int32BitsToSingle(stack[stackPtr - 1]);
                            stackPtr--;

                            DebugPrint($"{opcode} - {prev} - {last}, stack index now {stackPtr}");
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(prev - last);
                        }
                        break;
                    case VMInstructionOpcode.FloatMultiply:
                        {
                            float last = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            float prev = BitConverter.Int32BitsToSingle(stack[stackPtr - 1]);
                            stackPtr--;

                            DebugPrint($"{opcode} - {prev} - {last}, stack index now {stackPtr}");
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(prev * last);
                        }
                        break;
                    case VMInstructionOpcode.FloatDivide:
                        {
                            float last = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            float prev = BitConverter.Int32BitsToSingle(stack[stackPtr - 1]);
                            stackPtr--;

                            DebugPrint($"{opcode} - {prev} / {last}, stack index now {stackPtr}");
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(prev / last);
                        }
                        break;
                    case VMInstructionOpcode.FloatModulus:
                        {
                            float last = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            float prev = BitConverter.Int32BitsToSingle(stack[stackPtr - 1]);
                            stackPtr--;

                            DebugPrint($"{opcode} - {prev} % {last}, stack index now {stackPtr}");
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(prev % last);
                        }
                        break;
                    case VMInstructionOpcode.FloatLesserThan:
                        {
                            float last = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            float prev = BitConverter.Int32BitsToSingle(stack[stackPtr - 1]);
                            stackPtr--;

                            DebugPrint($"{opcode}, stack index now {stackPtr}");
                            stack[stackPtr] = (prev > last) ? 1 : 0;
                        }
                        break;
                    case VMInstructionOpcode.FloatLesserOrEqualTo:
                        {
                            float last = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            float prev = BitConverter.Int32BitsToSingle(stack[stackPtr - 1]);
                            stackPtr--;

                            DebugPrint($"{opcode}, stack index now {stackPtr}");
                            stack[stackPtr] = (prev >= last) ? 1 : 0;
                        }
                        break;
                    case VMInstructionOpcode.FloatGreaterThen:
                        {
                            float last = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            float prev = BitConverter.Int32BitsToSingle(stack[stackPtr - 1]);
                            stackPtr--;

                            DebugPrint($"{opcode}, stack index now {stackPtr}");
                            stack[stackPtr] = (last < prev) ? 1 : 0;
                        }
                        break;
                    case VMInstructionOpcode.FloatGreaterOrEqualTo:
                        {
                            float last = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            float prev = BitConverter.Int32BitsToSingle(stack[stackPtr - 1]);
                            stackPtr--;

                            DebugPrint($"{opcode}, stack index now {stackPtr}");
                            stack[stackPtr] = (last <= prev) ? 1 : 0;
                        }
                        break;
                    case VMInstructionOpcode.FloatEquals:
                        {
                            float last = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            float prev = BitConverter.Int32BitsToSingle(stack[stackPtr - 1]);
                            stackPtr--;

                            DebugPrint($"{opcode}, stack index now {stackPtr}");
                            stack[stackPtr] = (prev == last) ? 1 : 0;
                        }
                        break;
                    case VMInstructionOpcode.FloatNotEquals:
                        {
                            float last = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            float prev = BitConverter.Int32BitsToSingle(stack[stackPtr - 1]);
                            stackPtr--;

                            DebugPrint($"{opcode}, stack index now {stackPtr}");
                            stack[stackPtr] = (prev != last) ? 1 : 0;
                        }
                        break;
                    case VMInstructionOpcode.FloatUnaryMinusOperator:
                        {
                            DebugPrint($"{opcode}");
                            float value = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(-value);
                        }
                        break;
                    case VMInstructionOpcode.ToInt:
                        DebugPrint($"{opcode}");
                        stack[stackPtr] = BitConverter.SingleToInt32Bits(stack[stackPtr]);
                        break;

                    // 0xA0 - Float math
                    case VMInstructionOpcode.FloatRand:
                        DebugPrint($"{opcode}");
                        stack[stackPtr] = BitConverter.SingleToInt32Bits(
                            (BitConverter.Int32BitsToSingle(stack[stackPtr]) / (float)int.MaxValue) * (float)Random.Shared.Next()
                        );
                        break;
                    case VMInstructionOpcode.FloatSin:
                        {
                            DebugPrint($"{opcode}");
                            float value = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(MathF.Sin(value));
                        }
                        break;
                    case VMInstructionOpcode.FloatCos:
                        {
                            DebugPrint($"{opcode}");
                            float value = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(MathF.Cos(value));
                        }
                        break;
                    case VMInstructionOpcode.FloatTan:
                        {
                            DebugPrint($"{opcode}");
                            float value = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(MathF.Tan(value));
                        }
                        break;
                    case VMInstructionOpcode.FloatSquare:
                        {
                            DebugPrint($"{opcode}");
                            float value = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(MathF.Sqrt(value));
                        }
                        break;
                    case VMInstructionOpcode.FloatCosUnk:
                        {
                            DebugPrint($"{opcode}");
                            float value = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(0.5f - (MathF.Cos(value) * 0.5f));
                        }
                        break;

                    // 0xBx - More float math
                    case VMInstructionOpcode.FloatPow:
                        {
                            float last = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            float prev = BitConverter.Int32BitsToSingle(stack[stackPtr - 1]);
                            stackPtr--;

                            DebugPrint($"{opcode}, stack index now {stackPtr}");
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(MathF.Pow(prev, last));
                        }
                        break;
                    case VMInstructionOpcode.FloatAtan2:
                        {
                            float last = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            float prev = BitConverter.Int32BitsToSingle(stack[stackPtr - 1]);
                            stackPtr--;

                            DebugPrint($"{opcode}, stack index now {stackPtr}");
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(MathF.Atan2(prev, last));
                        }
                        break;
                    case VMInstructionOpcode.FloatUnk0xB2:
                        {
                            float last = BitConverter.Int32BitsToSingle(stack[stackPtr]);
                            float prev = BitConverter.Int32BitsToSingle(stack[stackPtr - 1]);
                            stackPtr--;

                            stackPtr--;
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(
                                (float)(prev - (float)((float)(int)(float)(prev * (float)(1.0 / last)) * last)) * (float)(1.0 / last)
                            );

                            DebugPrint($"{opcode}, stack index now {stackPtr}");
                            stack[stackPtr] = BitConverter.SingleToInt32Bits(MathF.Atan2(prev, last));
                        }
                        break;

                    case VMInstructionOpcode.CallbackWithIntResult:
                        {
                            stackPtr++;
                            ushort param = BinaryPrimitives.ReadUInt16LittleEndian(codePtr);
                            ushort regBits = BinaryPrimitives.ReadUInt16LittleEndian(codePtr);

                            DebugPrint($"{opcode} - param: {param}");

                            if (cb != null)
                            {
                                byte registerType = (byte)(regBits >> 14);
                                ushort registerIndex = (ushort)(regBits & 0x3FFF);

                                int value = Registers[registerType][registerIndex].Value;
                                stack[stackPtr] = cb(param, value);
                            }
                            else
                            {
                                stack[stackPtr] = 0;
                            }
                        }
                        break;
                    case VMInstructionOpcode.CallbackWithFloatResult:
                        {
                            stackPtr++;
                            ushort param = BinaryPrimitives.ReadUInt16LittleEndian(codePtr);
                            ushort regBits = BinaryPrimitives.ReadUInt16LittleEndian(codePtr);

                            DebugPrint($"{opcode} - param: {param}");

                            if (cb != null)
                            {
                                byte registerType = (byte)(regBits >> 14);
                                ushort registerIndex = (ushort)(regBits & 0x3FFF);

                                int value = Registers[registerType][registerIndex].Value;
                                stack[stackPtr] = cb(param, value);
                            }
                            else
                            {
                                stack[stackPtr] = 0;
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException($"Not implemented vm instruction {opcode}");
                }
            }
        }

        private void DebugPrint(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }
    }
}
