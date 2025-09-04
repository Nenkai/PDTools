using Syroot.BinaryData.Core;

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PDTools.Utils;

/// <summary>
/// Bit stream reverse engineered & modified with extra features.
/// </summary>
[DebuggerDisplay("Position = {Position}")]
public ref struct BitStream
{
    public const int Byte_Bits = 8;
    public const int Short_Bits = 16;
    public const int Int_Bits = 32;
    public const int Long_Bits = 64;

    /// <summary>
    /// Current mode of the stream. (read or write)
    /// </summary>
    public BitStreamMode Mode { get; private set; }

    /// <summary>
    /// Bit order of the stream. This affects how values larger than a byte are read.
    /// For LE, you should be using MSB. For BE, LSB.
    /// </summary>
    public BitStreamSignificantBitOrder Order { get; set; }

    public Span<byte> SourceBuffer { get; set; }
    private Span<byte> _currentBuffer;

    // Note: we do not have an encoding field, we try to ensure not to hold any reference type into this ref struct

    /// <summary>
    /// If Writing: Bits written for the current byte
    /// If Reading: Bits left for the current byte
    /// </summary>
    public uint BitCounter { get; set; }

    public byte CurrentByte { get; set; }

    // Not original implementation
    public int BufferByteSize { get; set; }
    public bool IsEndOfStream { get; set; } // TODO: remove this property

    /// <summary>
    /// Current length of the stream.
    /// </summary>
    // Non original
    private int _length;
    public readonly int Length => _length;

    private bool _needFeedNextByte;
    private bool _needsFlush;

    // Used when the input buffer is provided (when writing).
    private bool _noExpand;

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
                // Wack...
                if (Mode == BitStreamMode.Read && BitCounter > 0 && (BitCounter != 8 && !_needFeedNextByte))
                    return (int)(current - start) - 1;
                else if (Mode == BitStreamMode.Write && _needFeedNextByte)
                    return (int)(current - start) + 1;
                else
                    return (int)(current - start);
            }
        }
        set
        {
            SeekToByte(value);
        }
    }

#if DEBUG
    private StreamWriter? _sw;
#endif

    /// <summary>
    /// Inits a bit stream in Write mode, Order = <see cref="BitStreamSignificantBitOrder.LSB"/>, with a new buffer and pre-determined initial capacity.
    /// </summary>
    public BitStream()
    {
        InitWriteMode(1024, BitStreamSignificantBitOrder.LSB);
    }

    /// <summary>
    /// Creates a new bit stream based on an existing buffer.
    /// </summary>
    /// <param name="mode">Read or Write mode.</param>
    /// <param name="buffer">Buffer to read from or write to.</param>
    /// <param name="order">Bit order. Defaults to LSB, which is suitable for big-endian streams.</param>
    public BitStream(BitStreamMode mode, Span<byte> buffer, BitStreamSignificantBitOrder order = BitStreamSignificantBitOrder.LSB)
    {
        Mode = mode;
        Order = order;

        if (mode == BitStreamMode.Read)
            BitCounter = 8;
        else
            BitCounter = 0;

        CurrentByte = 0;

        SourceBuffer = buffer;
        _currentBuffer = buffer;

        BufferByteSize = 0;

        IsEndOfStream = false;
        _length = buffer.Length;

        _needsFlush = false;
        _noExpand = mode == BitStreamMode.Write; // Buffer is provided, do not expand it. Throw.
        _needFeedNextByte = mode == BitStreamMode.Read;
#if DEBUG
        _sw = null;
#endif
    }


    /// <summary>
    /// Creates a new bit stream in write mode with a new buffer.
    /// </summary>
    public BitStream(int capacity = 1024, BitStreamSignificantBitOrder endian = BitStreamSignificantBitOrder.LSB)
    {
        InitWriteMode(capacity, endian);
    }

    private void InitWriteMode(int capacity, BitStreamSignificantBitOrder endian)
    {
        Mode = BitStreamMode.Write;
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

    /// <summary>
    /// Gets the total bit position of the stream.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public long GetBitPosition()
    {
        if (Mode == BitStreamMode.Read)
            return (Position * Byte_Bits) + (8 - BitCounter);
        else if (Mode == BitStreamMode.Write)
            return (Position * Byte_Bits) + BitCounter;
        else
            throw new ArgumentException("Unexpected stream mode");
    }

    /// <summary>
    /// Gets the size of a string prefixed by a variable length integer.
    /// </summary>
    /// <param name="str">String to get the size of.</param>
    /// <param name="encoding">Encoding. Defaults to UTF-8.</param>
    /// <returns></returns>
    public static uint GetSizeOfVariablePrefixString(string str, Encoding? encoding = default)
    {
        uint byteCount = (uint)(encoding == default ? Encoding.UTF8.GetByteCount(str) : encoding.GetByteCount(str));
        return GetSizeOfVarInt(byteCount) + byteCount;
    }

    /// <summary>
    /// Gets the size of a string prefixed by a variable length integer (GTPSP Volumes version)
    /// </summary>
    /// <param name="str">String to get the size of.</param>
    /// <param name="encoding">Encoding. Defaults to UTF-8.</param>
    /// <returns></returns>
    public static uint GetSizeOfVariablePrefixStringAlt(string str, Encoding? encoding = default)
    {
        uint byteCount = (uint)(encoding == default ? Encoding.UTF8.GetByteCount(str) : encoding.GetByteCount(str));
        return GetSizeOfVarIntAlt(byteCount) + byteCount;
    }

    /// <summary>
    /// For GTPSP Volumes
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static uint GetSizeOfVarIntAlt(uint value)
    {
        Span<byte> output = stackalloc byte[0x05];
        uint outputSize = 0;

        while (value > 0x7F)
        {
            output[(int)outputSize] = (byte)(value & 0x7F);
            if (outputSize != 0)
                output[(int)outputSize] |= 0x80;

            value >>= 7;
            value--;
            outputSize++;
        }
        output[(int)outputSize++] = (byte)(value & 0x7F);
        if (outputSize > 1)
            output[(int)outputSize - 1] |= 0x80;

        return outputSize;
    }

    public static uint GetSizeOfVarInt(uint val)
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
    public readonly Span<byte> GetSpan() => SourceBuffer.Slice(0, _length);

    public readonly Span<byte> GetSpan(int start, int length) => SourceBuffer.Slice(start, Math.Min(length, _length));

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
            throw new IndexOutOfRangeException("Bit position is beyond end of stream.");

        SeekToByte(bytePos);

        if (Mode == BitStreamMode.Read)
        {
            BitCounter = Byte_Bits - bitPos;
        }
        else if (Mode == BitStreamMode.Write)
        {
            BitCounter = bitPos;
        }

        if (bitPos == 0)
        {
            _needFeedNextByte = true;
        }
        else
        {
            _needFeedNextByte = false;
            _currentBuffer = SourceBuffer[(bytePos + 1)..];

            if (Order == BitStreamSignificantBitOrder.MSB)
                CurrentByte >>= (int)bitPos;
            else if (Order == BitStreamSignificantBitOrder.LSB)
                CurrentByte <<= (int)bitPos;


        }

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
                throw new IndexOutOfRangeException("Position is beyond end of stream.");

            // Flush current byte
            AlignToNextByte();

            if (!_noExpand && byteOffset >= _length)
                _length = byteOffset;

            if (_length > SourceBuffer.Length)
                EnsureCapacity(_length * Byte_Bits);

            _currentBuffer = SourceBuffer[byteOffset..];
        }
        else if (seekOrigin == SeekOrigin.Current)
        {
            int newPos = byteOffset + Position;
            if (Mode == BitStreamMode.Read && newPos > Length)
                throw new IndexOutOfRangeException("Position is beyond end of stream.");

            // Flush current byte
            AlignToNextByte();

            if (!_noExpand && newPos >= _length)
                _length = newPos + 1;

            if (_length >= SourceBuffer.Length)
                EnsureCapacity(_length * Byte_Bits);

            _currentBuffer = _currentBuffer[byteOffset..];
        }
        else
        {
            throw new NotImplementedException("Unimplemented seek origin type");
        }

        if (_currentBuffer.IsEmpty)
            CurrentByte = 0;
        else
            CurrentByte = _currentBuffer[0];

        if (Mode == BitStreamMode.Read)
            BitCounter = Byte_Bits;
        else
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

            if (Mode == BitStreamMode.Read)
            {
                BitCounter = Byte_Bits;
                _needFeedNextByte = true;
            }
            else
            {
                BitCounter = 0;
                _currentBuffer[0] = CurrentByte;
                _currentBuffer = _currentBuffer.Slice(1);

                if (Position < _length)
                    CurrentByte = _currentBuffer[0];
                else
                    CurrentByte = 0;
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
            throw new IndexOutOfRangeException("Position is beyond end of stream.");

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

    public readonly Span<byte> GetBuffer() => SourceBuffer.Slice(0, _length);

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

    /// <summary>
    /// Reads a string prefixed by a variable length integer.
    /// </summary>
    /// <param name="encoding">Encoding. Defaults to UTF-8.</param>
    /// <returns></returns>
    public string ReadVarPrefixString(Encoding? encoding = default)
    {
        int strLen = (int)ReadVarInt();
        if (strLen < 0)
            throw new Exception($"Attempted to read string length of {strLen}.");

        byte[] chars = new byte[strLen];
        ReadIntoByteArray(strLen, chars, Byte_Bits);
        return encoding is not null ? encoding.GetString(chars) : Encoding.UTF8.GetString(chars);
    }

    /// <summary>
    /// Reads a string prefixed by a variable length integer. (GTPSP Volumes version)
    /// </summary>
    /// <param name="encoding">Encoding. Defaults to UTF-8.</param>
    /// <returns></returns>
    public string ReadVarPrefixStringAlt(Encoding? encoding = default)
    {
        int strLen = (int)ReadVarIntAlt();
        if (strLen < 0)
            throw new Exception($"Attempted to read string length of {strLen}.");

        byte[] bytes = new byte[strLen];
        ReadIntoByteArray(strLen, bytes, Byte_Bits);
        return encoding is not null ? encoding.GetString(bytes) : Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Reads a raw string by the specified byte count.
    /// </summary>
    /// <param name="byteCount">Byte count.</param>
    /// <param name="encoding">Encoding. Defaults to UTF-8.</param>
    /// <returns></returns>
    public string ReadStringRaw(int byteCount, Encoding? encoding = default)
    {
        byte[] bytes = new byte[byteCount];
        ReadIntoByteArray(byteCount, bytes, Byte_Bits);
        return encoding is not null ? encoding.GetString(bytes) : Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Reads a null-terminated string.
    /// </summary>
    /// <param name="encoding">Encoding. Defaults to UTF-8.</param>
    /// <returns></returns>
    public string ReadNullTerminatedString(Encoding? encoding = default)
    {
        List<byte> bytes = [];

        byte b = ReadByte();
        while ((char)b != '\0')
        {
            bytes.Add(b);
            b = ReadByte();
        }

        return (encoding is not null ? encoding : Encoding.UTF8).GetString(bytes.ToArray());
    }

    /// <summary>
    /// Reads a string prefixed by a 4-byte integer (length).
    /// </summary>
    /// <param name="encoding">Encoding. Defaults to UTF-8.</param>
    /// <returns></returns>
    public string ReadString4(Encoding? encoding = default)
    {
        int strLen = ReadInt32();
        byte[] chars = new byte[strLen];
        ReadIntoByteArray(strLen, chars, Byte_Bits);
        return (encoding is not null ? encoding : Encoding.UTF8).GetString(chars);
    }

    /// <summary>
    /// Reads a string prefixed by a 4-byte integer (length), and aligned with the specified alignment.
    /// </summary>
    /// <param name="align">Alignment.</param>
    /// <param name="encoding">Encoding. Defaults to UTF-8.</param>
    /// <returns></returns>
    public string ReadString4Aligned(int align, Encoding? encoding = default)
    {
        int strLen = ReadInt32();
        byte[] chars = new byte[strLen];
        ReadIntoByteArray(strLen, chars, Byte_Bits);
        string str = (encoding is not null ? encoding : Encoding.UTF8).GetString(chars).TrimEnd('\0');

        // Align
        var alignOffset = (-Position % align + align) % align;
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
                if (_needFeedNextByte || bitsLeftForThisByte == 0)
                {
                    // This is our working byte
                    currentByte = buf[0];
                    bitsLeftForThisByte = Byte_Bits;

                    // Buffer gets advanced by one while we work on our current byte
                    buf = buf[1..];
                    _needFeedNextByte = false;
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

        if (BitCounter == 0)
        {
            BitCounter = Byte_Bits;
            _needFeedNextByte = true;
        }

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
    public void ReadIntoByteArray(int arraySize, Span<byte> dest, int elemBitSize /*, bool debugMaybe */)
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
    public void ReadIntoByteArray(int arraySize, Span<sbyte> dest, int elemBitSize /*, bool debugMaybe */)
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
    public void ReadIntoShortArray(int arraySize, Span<short> dest, int elemBitSize /*, bool debugMaybe */)
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

        if (Position > _length)
            _length = Position;
        else if (Position >= _length && BitCounter > 0)
            _length = Position + 1;

        _needsFlush = BitCounter > 0;
        _needFeedNextByte = false;
#if DEBUG
        _sw?.WriteLine($"[{Position} - {BitCounter}/8] Wrote {value} ({bitCount} bits)");
#endif
    }

    public void WriteNullStringAligned4(string? value, Encoding? encoding = default)
    {
        // Require to seek to the next round byte incase we're currently in the middle of a byte's bits
        if (BitCounter != 0)
            SeekToByte(1, SeekOrigin.Current);

        if (string.IsNullOrEmpty(value))
        {
            WriteInt32(0);
            return;
        }

        var bytes = (encoding is not null ? encoding : Encoding.UTF8).GetBytes(value);
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

    public void WriteVarInt(uint val)
    {
        Span<byte> buffer = [];

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

    /// <summary>
    /// Writes a string prefixed by a variable length integer.
    /// </summary>
    /// <param name="str">String to write.</param>
    /// <param name="encoding">Encoding. Defaults to UTF-8.</param>
    public void WriteVarPrefixString(string str, Encoding? encoding = default)
    {
        WriteVarInt((uint)str.Length);
        if (!string.IsNullOrEmpty(str))
            WriteByteData((encoding is not null ? encoding : Encoding.UTF8).GetBytes(str));
    }

    /// <summary>
    /// Writes a string prefixed by a variable length integer. (GTPSP Volumes version)
    /// </summary>
    /// <param name="str">String to write.</param>
    /// <param name="encoding">Encoding. Defaults to UTF-8.</param>
    public void WriteVarPrefixStringAlt(string str, Encoding? encoding = default)
    {
        WriteVarIntAlt((uint)str.Length);
        if (!string.IsNullOrEmpty(str))
            WriteByteData((encoding is not null ? encoding : Encoding.UTF8).GetBytes(str));
    }

    /// <summary>
    /// Writes a buffer to the bit stream. This WILL align to the nearest byte.
    /// </summary>
    public void WriteByteData(ReadOnlySpan<byte> data, bool withPrefixLength = false)
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