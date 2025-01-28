using System;
using System.Text;
using System.Diagnostics;
using System.IO;

using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;

namespace PDTools.Utils
{
    /// <summary>
    /// Bit stream reverse engineered & modified with extra features. Big Endian only.
    /// </summary>
    [DebuggerDisplay("Position = {Position}")]
    public ref struct BitStream
    {
        public const int Byte_Bits = 8;
        public const int Short_Bits = 16;
        public const int Int_Bits = 32;
        public const int Long_Bits = 64;

        public BitStreamMode Mode { get; }

        /// <summary>
        /// Bit order of the stream. This affects how values larger than a byte are read.
        /// For LE, you should be using MSB. For BE, LSB.
        /// </summary>
        public BitStreamSignificantBitOrder Order { get; set; }

        public Span<byte> SourceBuffer { get; set; }

        private Span<byte> _currentBuffer;

        /// <summary>
        /// If Writing: Bits written for the current byte
        /// If Reading: Bits left for the current byte
        /// </summary>
        public uint BitCounter { get; set; }

        public byte CurrentByte { get; set; }

        // Not original implementation
        public int BufferByteSize { get; set; }
        public bool IsEndOfStream { get; set; }

        private bool _needsFlush;

        // Used when the input buffer is provided (when writing).
        private readonly bool _noExpand;

        /// <summary>
        /// Returns the byte position of the CURRENT bit. Make sure to align to the next byte if you want to use this for offsets!
        /// </summary>
        // Non Original
        public unsafe int Position
        {
            get
            {
                if (_currentBuffer.Length == 0)
                    return SourceBuffer.Length;

                fixed (byte* current = _currentBuffer,
                       start = SourceBuffer)
                {
                    if (Mode == BitStreamMode.Read && BitCounter > 0)
                        return (int)(current - start) - 1;
                    else
                        return (int)(current - start);
                }
            }
            set
            {
                SeekToByte(value);
            }
        }

        /// <summary>
        /// Current length of the stream.
        /// </summary>
        // Non original
        private int _length;
        public int Length => _length;

#if DEBUG
        private StreamWriter _sw;
#endif
        // TODO: Enum to mark stream as write or read only

        /// <summary>
        /// Creates a new bit stream based on an existing buffer.
        /// </summary>
        /// <param name="buffer"></param>
        public BitStream(BitStreamMode mode, Span<byte> buffer, BitStreamSignificantBitOrder order = BitStreamSignificantBitOrder.LSB)
        {
            Mode = mode;
            Order = order;

            BitCounter = 0;
            CurrentByte = 0;

            SourceBuffer = buffer;
            _currentBuffer = buffer;

            BufferByteSize = 0;

            IsEndOfStream = false;
            _length = buffer.Length;

            _needsFlush = false;
            _noExpand = mode == BitStreamMode.Write; // Buffer is provided, do not expand it. Throw.
#if DEBUG
            _sw = null;
#endif
        }


        /// <summary>
        /// Creates a new bit stream with a new buffer.
        /// </summary>
        public BitStream(BitStreamMode mode, int capacity = 1024, BitStreamSignificantBitOrder endian = BitStreamSignificantBitOrder.LSB)
        {
            Mode = mode;
            Order = endian;

            BitCounter = 0;
            CurrentByte = 0;

            SourceBuffer = new byte[capacity];
            _currentBuffer = SourceBuffer;

            BufferByteSize = 0;

            IsEndOfStream = false;
            _length = 1;
            _needsFlush = false;
            _noExpand = false;
#if DEBUG
            _sw = null;
#endif
        }

        public static int GetSizeOfVariablePrefixString(string str)
            => GetSizeOfVarInt(str.Length) + str.Length;

        /// <summary>
        /// For GTPSP Volumes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetSizeOfVariablePrefixStringAlt(string str)
            => GetSizeOfVarIntAlt((uint)str.Length) + str.Length;

        /// <summary>
        /// For GTPSP Volumes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetSizeOfVarIntAlt(uint value)
        {
            Span<byte> output = stackalloc byte[0x05];
            int outputSize = 0;

            while (value > 0x7F)
            {
                output[outputSize] = (byte)(value & 0x7F);
                if (outputSize != 0)
                    output[outputSize] |= 0x80;

                value >>= 7;
                value--;
                outputSize++;
            }
            output[outputSize++] = (byte)(value & 0x7F);
            if (outputSize > 1)
                output[outputSize - 1] |= 0x80;

            return outputSize;
        }

        public static int GetSizeOfVarInt(int val)
        {
            if (val < 0x80)
                return 1;
            else if (val < 0x4000)
                return 2;
            else if (val < 0x200000)
                return 3;
            else if (val < 0x10000000)
                return 4;
            else
                return 5;
        }


        /// <summary>
        /// Returns a span of the stream. Note that this will return the working span, and <see cref="GetSpanToCurrentPosition"/> should be used instead when getting buffers.
        /// </summary>
        /// <returns></returns>
        public Span<byte> GetSpan()
        { 
            return SourceBuffer.Slice(0, _length);
        }

        public Span<byte> GetSpan(int start, int length)
        {
            return SourceBuffer.Slice(start, Math.Min(length, _length));
        }

        /// <summary>
        /// Returns a span of the stream starting from the current position.
        /// </summary>
        /// <returns></returns>
        public Span<byte> GetSpanFromCurrentPosition()
            => SourceBuffer.Slice(Position);

        /// <summary>
        /// Returns a span of the stream from the start to the current position.
        /// </summary>
        /// <returns></returns>
        public Span<byte> GetSpanToCurrentPosition()
        {
            return SourceBuffer.Slice(0, Position);
        }

        /// <summary>
        /// Seeks to a specific bit within the whole stream.
        /// </summary>
        /// <param name="bitOffset">Bit offset.</param>
        // Non original
        public void SeekToBit(long bitOffset)
        {
            int bytePos = (int)(bitOffset / Byte_Bits);
            uint bitPos = (uint)bitOffset % Byte_Bits;

            if (Mode == BitStreamMode.Read && bytePos > Length)
                throw new ArgumentOutOfRangeException("Position is beyond end of stream.");

            // Update stream length if needed
            if (bytePos > _length)
                _length = bytePos;

            if (bytePos > SourceBuffer.Length)
                EnsureCapacity((SourceBuffer.Length - bytePos) * Byte_Bits + Byte_Bits);

            _currentBuffer = SourceBuffer.Slice(bytePos);
            if (_currentBuffer.IsEmpty)
                CurrentByte = 0;
            else
                CurrentByte = _currentBuffer[0];

            BitCounter = bitPos;

        }

        /// <summary>
        /// Seeks to a specific byte within the whole stream.
        /// </summary>
        /// <param name="byteOffset"></param>
        public void SeekToByte(int byteOffset, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            if (seekOrigin == SeekOrigin.Begin)
            {
                if (Mode == BitStreamMode.Read && byteOffset > Length)
                    throw new ArgumentOutOfRangeException("Position is beyond end of stream.");

                // Flush current byte
                AlignToNextByte();

                if (!_noExpand && byteOffset >= _length)
                    _length = byteOffset;

                if (_length > SourceBuffer.Length)
                    EnsureCapacity(_length * Byte_Bits);

                _currentBuffer = SourceBuffer.Slice(byteOffset);
            }
            else if (seekOrigin == SeekOrigin.Current)
            {
                int newPos = byteOffset + Position;
                if (Mode == BitStreamMode.Read && newPos > Length)
                    throw new ArgumentOutOfRangeException("Position is beyond end of stream.");

                // Flush current byte
                AlignToNextByte();

                if (!_noExpand && newPos >= _length)
                    _length = newPos + 1;

                if (_length >= SourceBuffer.Length)
                    EnsureCapacity(_length * Byte_Bits);

                _currentBuffer = _currentBuffer.Slice(byteOffset);
            }
            else
            {
                throw new NotImplementedException("Unimplemented seek origin type");
            }

            if (_currentBuffer.IsEmpty)
                CurrentByte = 0;
            else
                CurrentByte = _currentBuffer[0];

            BitCounter = 0;
        }

        /// <summary>
        /// Seeks the stream to the next new byte.
        /// </summary>
        public void AlignToNextByte()
        {
            if (BitCounter > 0 && BitCounter != 8)
            {
                if (_needsFlush)
                {
                    SourceBuffer[Position] = CurrentByte;
                    _needsFlush = false;
                }

                if (Position >= SourceBuffer.Length)
                    EnsureCapacity(Position + Byte_Bits);

                CurrentByte = _currentBuffer[0];
                _currentBuffer = _currentBuffer.Slice(1);

                if (Mode == BitStreamMode.Read)
                {
                    BitCounter = Byte_Bits;
                }
                else
                {
                    BitCounter = 0;
                }
            }
        }

        public void Align(int alignment)
        {
            AlignToNextByte();
            var newPos = (-Position % alignment + alignment) % alignment;
            Position += newPos;
        }

        public unsafe bool CanRead(int bitCountToRead)
        {
            if (IsEndOfStream)
                return false;

            fixed (byte* start = _currentBuffer,
                   end = SourceBuffer.Slice(BufferByteSize - 1))
            {
                long bitsLeft = ((end + 1) - start) * Byte_Bits;
                bitsLeft += BitCounter;

                IsEndOfStream = bitsLeft < bitCountToRead;
                return !IsEndOfStream;
            }
        }

        private void EnsureCapacity(long bitCount)
        {
            uint bitsLeftThisByte = Byte_Bits - BitCounter;
            long totalFreeBits = ((_currentBuffer.Length - 1) * Byte_Bits) + bitsLeftThisByte;

            if (_noExpand && totalFreeBits < bitCount)
                throw new ArgumentOutOfRangeException("Position is beyond end of stream.");

            if (bitCount > totalFreeBits)
            {
                int cPos = Position < 0 ? SourceBuffer.Length : Position;

                // Expand our buffer by twice the size everytime - if possible
                int newCapacity;
                if (SourceBuffer.Length * 2 < cPos + (bitCount / 8))
                    newCapacity = (int)((bitCount / 8) * 2);
                else
                    newCapacity = SourceBuffer.Length * 2;

                var newBuffer = new byte[newCapacity];

                // Copy our buffer over the larger one
                SourceBuffer.CopyTo(newBuffer.AsSpan());
                SourceBuffer = newBuffer;

                // Ensure that the current representation of the rest of our stream is updated
                _currentBuffer = SourceBuffer.Slice(cPos);
            }
        }

        public Span<byte> GetBuffer()
        {
            return SourceBuffer.Slice(0, _length);
        }

        /* ********************************************
         * *                                          *
         * *         Reading Stream Methods           *
         * *                                          *
         * ******************************************** */


        public ulong ReadUInt64()
            => ReadBits(Long_Bits);

        public long ReadInt64()
            => (long)ReadBits(Long_Bits);

        public uint ReadUInt32()
            => (uint)ReadBits(Int_Bits);

        public int ReadInt32()
            => (int)ReadBits(Int_Bits);

        public short ReadInt16()
            => (short)ReadBits(Short_Bits);

        public ushort ReadUInt16()
            => (ushort)ReadBits(Short_Bits);

        public byte ReadByte()
            => (byte)ReadBits(Byte_Bits);

        public bool ReadBoolBit()
            => ReadBits(1) == 1;

        public bool ReadBool()
            => ReadByte() == 1;

        public bool ReadBool4()
            => ReadInt32() == 1;

        public sbyte ReadSByte()
            => (sbyte)ReadBits(Byte_Bits);

        public unsafe float ReadSingle()
        {
            int val = ReadInt32();
            if (val != -1)
                return *(float*)&val;
            return -1;
        }

        public ulong ReadVarInt()
        {
            ulong value = ReadByte();
            ulong mask = 0x80;

            while ((value & mask) != 0)
            {
                value = ((value - mask) << 8) | (ReadByte());
                mask <<= 7;
            }

            return value;
        }

        /// <summary>
        /// For GTPSP Volumes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ulong ReadVarIntAlt()
        {
            byte currentByte = ReadByte();
            const int _7bits = 0b_111_1111;

            int value = (int)(currentByte & _7bits);
            while (((currentByte >> 7) & 1) > 0)
            {
                value++;
                value <<= 7;
                currentByte = ReadByte();
                value += currentByte & _7bits;
            }
            return (ulong)value;
        }

        public string ReadVarPrefixString()
        {
            int strLen = (int)ReadVarInt();
            if (strLen < 0)
                throw new Exception($"Attempted to read string length of {strLen}.");

            byte[] chars = new byte[strLen];
            ReadIntoByteArray(strLen, chars, Byte_Bits);
            return Encoding.ASCII.GetString(chars);
        }

        /// <summary>
        /// For GTPSP Volumes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ReadVarPrefixStringAlt()
        {
            int strLen = (int)ReadVarIntAlt();
            if (strLen < 0)
                throw new Exception($"Attempted to read string length of {strLen}.");

            byte[] chars = new byte[strLen];
            ReadIntoByteArray(strLen, chars, Byte_Bits);
            return Encoding.ASCII.GetString(chars);
        }

        public string ReadStringRaw(int length)
        {
            byte[] chars = new byte[length];
            ReadIntoByteArray(length, chars, Byte_Bits);
            return Encoding.ASCII.GetString(chars);
        }

        public string ReadNullTerminatedString()
        {
            List<byte> bytes = new List<byte>();

            byte b = ReadByte();
            while ((char)b != '\0')
            {
                bytes.Add(b);
                b = ReadByte();
            }

            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        public string ReadString4()
        {
            int strLen = ReadInt32();
            byte[] chars = new byte[strLen];
            ReadIntoByteArray(strLen, chars, Byte_Bits);
            return Encoding.ASCII.GetString(chars);
        }

        public string ReadString4Aligned()
        {
            int strLen = ReadInt32();
            byte[] chars = new byte[strLen];
            ReadIntoByteArray(strLen, chars, Byte_Bits);
            string str = Encoding.UTF8.GetString(chars).TrimEnd('\0');

            // Align
            const int alignment = 0x04;

            var alignOffset = (-Position % alignment + alignment) % alignment;
            SeekToByte(Position + alignOffset);
            return str;
        }

        // Original Impl
        public ulong ReadBits(int bitCount)
        {
            ulong totalBitCountToRead = (ulong)bitCount;
            var buf = _currentBuffer;

            ulong result = 0;
            ulong currentByte = CurrentByte;
            ulong bitsLeftForThisByte = BitCounter;

            int currentBitOffset = 0;

            if (bitCount != 0x0)
            {
                do
                {
                    // Are we starting a new byte?
                    if (bitsLeftForThisByte == 0)
                    {
                        // This is our working byte
                        currentByte = buf[0];
                        bitsLeftForThisByte = Byte_Bits;

                        // Buffer gets advanced by one while we work on our current byte
                        buf = buf.Slice(1);
                    }

                    ulong bitsRead = bitsLeftForThisByte;
                    if (totalBitCountToRead < bitsLeftForThisByte)
                        bitsRead = totalBitCountToRead;

                    totalBitCountToRead -= bitsRead;
                    bitsLeftForThisByte -= bitsRead;

                    int nBitsToClear = (int)(Byte_Bits - bitsRead);
                    if (Order == BitStreamSignificantBitOrder.MSB) // Non original, but for utility
                    {
                        // Could be better
                        ulong bitsToAdd = (ulong)((byte)(currentByte << (byte)nBitsToClear) >> nBitsToClear);  // Clear unneeded bits
                        result = (bitsToAdd << currentBitOffset) | result;

                        currentByte >>= (int)bitsRead;
                        currentBitOffset += (int)bitsRead;
                    }
                    else if (Order == BitStreamSignificantBitOrder.LSB)
                    {
                        result = result << (byte)bitsRead | currentByte >> nBitsToClear;
                        currentByte <<= (int)bitsRead;
                    }

                } while (totalBitCountToRead != 0);
            }

            BitCounter = (uint)bitsLeftForThisByte;
            _currentBuffer = buf;
            CurrentByte = (byte)currentByte;
            return result;
        }

        // Original Impl - GT6 EU 1.22 - FUN_0df2bbc
        public ulong ReadBitsSafe(int bitCount)
            => CanRead(bitCount) ? ReadBits(bitCount) : 0;

        /// <summary>
        /// Reads an array.
        /// </summary>
        /// <param name="arraySize">Size of the array.</param>
        /// <param name="dest">Bytes destination.</param>
        /// <param name="elemBitSize">Bits to read per element. Note that each element will still go within each byte in the destination.</param>
        // The original function (GT6 1.22 EU - FUN_00d42b94 takes an extra argument to go through two paths,
        // seemingly does the exact same so probably compiler macro)
        // Original Impl
        public void ReadIntoByteArray(int arraySize, byte[] dest, int elemBitSize /*, bool debugMaybe */)
        {
            if (dest.Length != 0 && arraySize != 0) // if (dest != destEndPos)
            {
                for (int i = 0; i < arraySize; i++)
                {
                    byte value = 0;
                    if (/* this->field_0x14 == false || */ false) // !CanRead(elemBitSize)) Ignore for time being
                    {
                        // this->field_0x14 = false;
                    }
                    else
                    {
                        value = (byte)ReadBits(elemBitSize);
                    }

                    dest[i] = value;
                }
            }
        }

        /// <summary>
        /// Reads an array based on a prefixed length of 4.
        /// </summary>
        /// <param name="arraySize">Size of the array.</param>
        /// <param name="dest">Bytes destination.</param>
        /// <param name="elemBitSize"></param>
        // Non original impl
        public byte[] ReadByteArrayPrefixed()
        {
            uint len = this.ReadUInt32();
            byte[] dest = new byte[len];
            this.ReadIntoByteArray(dest.Length, dest, Byte_Bits);
            return dest;
        }

        /// <summary>
        /// Reads an array.
        /// </summary>
        /// <param name="arraySize">Size of the array.</param>
        /// <param name="dest">Bytes destination.</param>
        /// <param name="elemBitSize">Bits to read per element. Note that each element will still go within each byte in the destination.</param>
        public void ReadIntoByteArray(int arraySize, sbyte[] dest, int elemBitSize /*, bool debugMaybe */)
        {
            if (dest.Length != 0 && arraySize != 0) // if (dest != destEndPos)
            {
                for (int i = 0; i < arraySize; i++)
                {
                    sbyte value = 0;
                    if (/* this->field_0x14 == false || */ false) // !CanRead(elemBitSize)) Ignore for time being
                    {
                        // this->field_0x14 = false;
                    }
                    else
                    {
                        value = (sbyte)ReadBits(elemBitSize);
                    }

                    dest[i] = value;
                }
            }
        }

        /// <summary>
        /// Reads an array.
        /// </summary>
        /// <param name="arraySize">Size of the array.</param>
        /// <param name="dest">Bytes destination.</param>
        /// <param name="elemBitSize">Bits to read per element. Note that each element will still go within each byte in the destination.</param>
        // The original function (GT6 1.22 EU - FUN_00e2de14 takes an extra argument to go through two paths,
        // seemingly does the exact same so probably compiler macro)
        // Original Impl
        public void ReadIntoShortArray(int arraySize, short[] dest, int elemBitSize /*, bool debugMaybe */)
        {
            if (dest.Length != 0 && arraySize != 0) // if (dest != destEndPos)
            {
                for (int i = 0; i < arraySize; i++)
                {
                    short value = 0;
                    if (/* this->field_0x14 == false || */ false) // !CanRead(elemBitSize)) Ignore for time being
                    {
                        // this->field_0x14 = false;
                    }
                    else
                    {
                        value = (short)ReadBits(elemBitSize);
                    }

                    dest[i] = value;
                }
            }
        }

        /* ********************************************
         * *                                          *
         * *         Writing Stream Methods           *
         * *                                          *
         * ******************************************** */

        // Original Impl (except capacity ensuring, MSB/LSB & length)
        /// <summary>
        /// Writes bits to the stream.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bitCount"></param>
        /// <exception cref="IOException"></exception>
        public void WriteBits(ulong value, ulong bitCount)
        {
            if (Mode == BitStreamMode.Read)
                throw new IOException("Stream is read only.");

            EnsureCapacity((long)bitCount);

            if (Order == BitStreamSignificantBitOrder.MSB)
            {
                while (bitCount != 0)
                {
                    ulong nBitsToWriteThisByte = bitCount;
                    if (nBitsToWriteThisByte > Byte_Bits - BitCounter)
                        nBitsToWriteThisByte = Byte_Bits - BitCounter;

                    CurrentByte |= (byte)(value << (int)BitCounter);
                    BitCounter += (uint)nBitsToWriteThisByte;
                    bitCount -= nBitsToWriteThisByte;

                    value >>= (int)nBitsToWriteThisByte;

                    if (BitCounter == Byte_Bits)
                    {
                        _currentBuffer[0] = CurrentByte;
                        _currentBuffer = _currentBuffer.Slice(1);
                        CurrentByte = 0;
                        BitCounter = 0;
                    }
                }
            }
            else if (Order == BitStreamSignificantBitOrder.LSB)
            {
                ulong val = value << (Long_Bits - (byte)bitCount);
                while (bitCount != 0)
                {
                    ulong nBitsToWriteThisByte = bitCount;
                    if (nBitsToWriteThisByte > Byte_Bits - BitCounter)
                        nBitsToWriteThisByte = Byte_Bits - BitCounter;

                    CurrentByte |= (byte)(val >> ((int)BitCounter + 56));
                    BitCounter += (uint)nBitsToWriteThisByte;
                    bitCount -= nBitsToWriteThisByte;

                    val <<= (int)nBitsToWriteThisByte;

                    if (BitCounter == Byte_Bits)
                    {
                        _currentBuffer[0] = CurrentByte;
                        _currentBuffer = _currentBuffer.Slice(1);
                        CurrentByte = 0;
                        BitCounter = 0;
                    }
                }
            }

            if (BitCounter != 0)
                _currentBuffer[0] = CurrentByte;

            if (Position >= _length)
            {
                if (BitCounter == 0)
                    _length = Position;
                else
                    _length = Position + 1;
            }

            _needsFlush = BitCounter > 0;
#if DEBUG
            _sw?.WriteLine($"[{Position} - {BitCounter}/8] Wrote {value} ({bitCount} bits)");
#endif
        }

        public void WriteNullStringAligned4(string value)
        {
            // Require to seek to the next round byte incase we're currently in the middle of a byte's bits
            if (BitCounter != 0)
                SeekToByte(1, SeekOrigin.Current);

            if (string.IsNullOrEmpty(value))
            {
                WriteInt32(0);
                return;
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            var bytesSize = bytes.Length;
            EnsureCapacity(((bytesSize + 1) + 4) * Byte_Bits); // Account for string bytes length + null termination + pad

            int nullTerminatedSize = bytesSize + 1;
            WriteInt32(nullTerminatedSize);

            bytes.AsSpan().CopyTo(_currentBuffer);

            int pad = nullTerminatedSize % 4;
            if (nullTerminatedSize % 4 != 0)
                pad = 4;

            int totalSize = pad + (nullTerminatedSize & 0xffffffc);
            SeekToByte(totalSize, SeekOrigin.Current);
        }

        public void WriteVarInt(int val)
        {
            Span<byte> buffer = Array.Empty<byte>();

            if (val <= 0x7F)
            {
                WriteByte((byte)val);
                return;
            }
            else if (val <= 0x3FFF)
            {
                Span<byte> tempBuf = BitConverter.GetBytes(val).AsSpan();
                tempBuf.Reverse();
                buffer = tempBuf.Slice(2, 2);
            }
            else if (val <= 0x1FFFFF)
            {
                Span<byte> tempBuf = BitConverter.GetBytes(val).AsSpan();
                tempBuf.Reverse();
                buffer = tempBuf.Slice(1, 3);
            }
            else if (val <= 0xFFFFFFF)
            {
                buffer = BitConverter.GetBytes(val).AsSpan();
                buffer.Reverse();
            }
            else if (val <= 0xFFFFFFFF)
            {
                buffer = BitConverter.GetBytes(val);
                buffer.Reverse();
                buffer = new byte[] { 0, buffer[0], buffer[1], buffer[2], buffer[3] };
            }

            uint mask = 0x80;
            for (int i = 1; i < buffer.Length; i++)
            {
                buffer[0] += (byte)mask;
                mask >>= 1;
            }

            WriteByteData(buffer, false);
        }

        /// <summary>
        /// For GTPSP Volumes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void WriteVarIntAlt(uint value)
        {
            // If you pass by please make this better
            // My brain blew out
            Span<byte> output = stackalloc byte[0x05];
            int outputSize = 0;

            while (value > 0x7F)
            {
                output[outputSize] = (byte)(value & 0x7F);
                if (outputSize != 0)
                    output[outputSize] |= 0x80;

                value >>= 7;
                value--;
                outputSize++;
            }
            output[outputSize++] = (byte)(value & 0x7F);
            if (outputSize > 1)
                output[outputSize - 1] |= 0x80;

            for (int i = outputSize - 1; i >= 0; i--)
                WriteByte(output[i]);
        }

        public void WriteVarPrefixString(string str)
        {
            WriteVarInt(str.Length);
            if (!string.IsNullOrEmpty(str))
                WriteByteData(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// For GTPSP Volumes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void WriteVarPrefixStringAlt(string str)
        {
            WriteVarIntAlt((uint)str.Length);
            if (!string.IsNullOrEmpty(str))
                WriteByteData(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// Writes a buffer to the bit stream. This WILL align to the nearest byte.
        /// </summary>
        public void WriteByteData(Span<byte> data, bool withPrefixLength = false)
        {
            EnsureCapacity(((withPrefixLength ? 4 : 0) + data.Length) * Byte_Bits);
            AlignToNextByte();

            if (withPrefixLength)
                WriteInt32(data.Length);

            data.CopyTo(_currentBuffer);
            SeekToByte(data.Length, SeekOrigin.Current);
        }

        /// <summary>
        /// Writes a ulong to the stream. This will write 64 bits and will not align to the nearest byte.
        /// </summary>
        public void WriteUInt64(ulong value)
            => WriteBits(value, Long_Bits);

        /// <summary>
        /// Writes a long to the stream. This will write 64 bits and will not align to the nearest byte.
        /// </summary>
        public void WriteInt64(long value)
            => WriteBits((ulong)value, Long_Bits);

        /// <summary>
        /// Writes a uint to the stream. This will write 32 bits and will not align to the nearest byte.
        /// </summary>
        public void WriteUInt32(uint value)
            => WriteBits(value, Int_Bits);

        /// <summary>
        /// Writes a int to the stream. This will write 64 bits and will not align to the nearest byte.
        /// </summary>
        public void WriteInt32(int value)
            => WriteBits((ulong)value, Int_Bits);

        /// <summary>
        /// Writes a int or -1 to the stream if null. This will write 32 bits and will not align to the nearest byte.
        /// </summary>
        public void WriteInt32OrNull(int? value)
        {
            if (value == null || value == -1)
                WriteBits(unchecked((ulong)-1), Int_Bits);
            else
                WriteBits((ulong)value, Int_Bits);
        }

        /// <summary>
        /// Writes a single/float to the stream. This will write 32 bits and will not align to the nearest byte.
        /// </summary>
        public unsafe void WriteSingle(float value)
            => WriteBits((ulong)*(int*)&value, Int_Bits);

        /// <summary>
        /// Writes a double to the stream. This will write 4 bits and will not align to the nearest byte.
        /// </summary>
        public unsafe void WriteDouble(double value)
            => WriteBits((ulong)value, Long_Bits);

        public void WriteBool4OrNull(bool? value)
        {
            if (value is null)
                WriteInt32(-1);
            else
                WriteBool4(value.Value);
        }

        /// <summary>
        /// Writes a short to the stream. This will write 16 bits and will not align to the nearest byte.
        /// </summary>
        public void WriteInt16(short value)
            => WriteBits((ulong)value, Short_Bits);

        /// <summary>
        /// Writes a ushort to the stream. This will write 16 bits and will not align to the nearest byte.
        /// </summary>
        public void WriteUInt16(uint value)
            => WriteBits(value, Short_Bits);

        /// <summary>
        /// Writes a byte to the stream. This will write 8 bits and will not align to the nearest byte.
        /// </summary>
        /// <param name="value"></param>
        public void WriteByte(byte value)
            => WriteBits(value, Byte_Bits);

        /// <summary>
        /// Writes a bool to the stream. This will write 8 bit and will not align to the nearest byte.
        /// </summary>
        public void WriteBool(bool value)
            => WriteBits((ulong)(value ? 1 : 0), Byte_Bits);

        /// <summary>
        /// Writes a bool to the stream. This will write 16 bits and will not align to the nearest byte.
        /// </summary>
        public void WriteBool2(bool value)
            => WriteBits((ulong)(value ? 1 : 0), Short_Bits);

        /// <summary>
        /// Writes a bool to the stream. This will write 32 bits and will not align to the nearest byte.
        /// </summary>
        public void WriteBool4(bool value)
            => WriteBits((ulong)(value ? 1 : 0), Int_Bits);

        /// <summary>
        /// Writes a sbyte to the stream. This will write 8 bits and will not align to the nearest byte.
        /// </summary>
        public void WriteSByte(sbyte value)
            => WriteBits((ulong)value, Byte_Bits);

        public void WriteBoolBit(bool value)
            => WriteBits((ulong)(value ? 1 : 0), 1);


        #region Debug Tools

        /// <summary>
        /// Logs bits written into the specified file. DEBUG only.
        /// </summary>
        /// <param name="path"></param>
        public void StartWriteLog(string path)
        {
#if DEBUG
            _sw = new StreamWriter(path);
#endif
        }

        public void EndWriteLog()
        {
#if DEBUG
            if (_sw != null)
            {
                _sw.Flush();
                _sw.Dispose();
            }
#endif
        }
        #endregion

        public static void Test()
        {
            byte[] test = new byte[] { 0, 1, 2, 3, 4 };
            BitStream testRead = new BitStream(BitStreamMode.Read, test);

            testRead.ReadBits(1);
            Debug.Assert(testRead.Position == 0);

            testRead.ReadBits(7);
            Debug.Assert(testRead.Position == 1);

            testRead.ReadBits(1);
            Debug.Assert(testRead.Position == 1);

            testRead.AlignToNextByte();
            Debug.Assert(testRead.Position == 2);

            testRead.Position = 3;
            Debug.Assert(testRead.Position == 3);

            var val = testRead.ReadBits(8);
            Debug.Assert(val == 3);

            BitStream testWrite = new BitStream(BitStreamMode.Write, new byte[5]);
            testWrite.WriteBits(1, 1);
            Debug.Assert(testWrite.Position == 0);

            testWrite.WriteBits(1, 1);
            Debug.Assert(testWrite.BitCounter == 2);

            testWrite.AlignToNextByte();
            Debug.Assert(testWrite.Position == 1);

            try
            {
                testWrite.Position = 5;
                testWrite.WriteBits(1, 1);
            }
            catch (Exception ex)
            {
                Debug.Assert(ex is IOException iOException && iOException.Message.Contains("Out of range"));
            }

            testWrite.Position = 0;
            testWrite.WriteByteData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
            Debug.Assert(testWrite.GetBuffer().ToArray().All(e => e == 0xFF));
            Debug.Assert(testWrite.GetSpanToCurrentPosition().Length == 5);

            int offset = 1024;
            BitStream testDynamicStream = new BitStream(BitStreamMode.Write);
            testDynamicStream.SeekToByte(offset);
            testDynamicStream.SeekToBit(offset * Byte_Bits);
            Debug.Assert(testDynamicStream.Position == offset);

            testDynamicStream.WriteBoolBit(true);
            Debug.Assert(testDynamicStream.Length == offset + 1);

            Span<byte> span = testDynamicStream.GetSpanFromCurrentPosition();
            Debug.Assert(span.Length == offset); // Source buffer should have been doubled, therefore we get a span of the same length.
        }
    }

    public enum BitStreamSignificantBitOrder
    {
        /// <summary>
        /// Least significant bit (default, for big endian)
        /// </summary>
        LSB,

        /// <summary>
        /// Most significant bit (for little endian)
        /// </summary>
        MSB,
    }

    public enum BitStreamMode
    {
        Read,
        Write,
    }
}