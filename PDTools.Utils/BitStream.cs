using System;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace PDTools.Utils
{
    [DebuggerDisplay("Position = {BytePosition}")]
    public ref struct BitStream
    {
        public const int Byte_Bits = 8;
        public const int Short_Bits = 16;
        public const int Int_Bits = 32;
        public const int Long_Bits = 64;

        public Span<byte> SourceBuffer { get; set; }

        private Span<byte> _currentBuffer { get; set; }
        public uint RemainingByteBits { get; set; }
        public byte CurrentByte { get; set; }

        // Not original implementation
        public int BufferByteSize { get; set; }
        public bool IsEndOfStream { get; set; }

        public bool UnkBool { get; set; }

#if DEBUG
        private StreamWriter _sw;
#endif

        /// <summary>
        /// Creates a new bit stream based on an existing buffer.
        /// </summary>
        /// <param name="buffer"></param>
        public BitStream(Span<byte> buffer)
        {
            RemainingByteBits = 0;
            CurrentByte = 0;

            SourceBuffer = buffer;
            _currentBuffer = buffer;

            BufferByteSize = 0;

            UnkBool = false;

            IsEndOfStream = false;
#if DEBUG
            _sw = null;
#endif
        }

        /// <summary>
        /// Creates a new bit stream with a new buffer.
        /// </summary>
        public BitStream(int capacity = 1024)
        {
            RemainingByteBits = 0;
            CurrentByte = 0;

            SourceBuffer = new byte[capacity];
            _currentBuffer = SourceBuffer;

            BufferByteSize = 0;

            UnkBool = false;

            IsEndOfStream = false;

#if DEBUG
            _sw = null;
#endif
        }

        // Non Original
        public unsafe int BytePosition
        {
            get
            {
                if (_currentBuffer.Length == 0)
                    return SourceBuffer.Length;

                fixed (byte* current = _currentBuffer,
                       start = SourceBuffer)
                {
                    return (int)(current - start);
                }
            }
        }

        // Non Original
        public void SeekToBit(int bitOffset)
        {

        }

        // Non original
        public void SeekToByte(int byteOffset)
        {
            if (byteOffset > SourceBuffer.Length)
                EnsureCapacity((SourceBuffer.Length - byteOffset) * Byte_Bits);

            _currentBuffer = SourceBuffer.Slice(byteOffset);
            CurrentByte = _currentBuffer[0];
            RemainingByteBits = 0;
        }

        public void SeekToByteFromCurrentPosition(int byteOffset)
        {
            if (byteOffset + BytePosition >= SourceBuffer.Length)
                EnsureCapacity(byteOffset * Byte_Bits);

            _currentBuffer = _currentBuffer.Slice(byteOffset);
            CurrentByte = _currentBuffer[0];
            RemainingByteBits = 0;
        }

        public unsafe bool CanRead(int bitCountToRead)
        {
            if (IsEndOfStream)
                return false;

            fixed (byte* start = _currentBuffer,
                   end = SourceBuffer.Slice(BufferByteSize - 1))
            {
                long bitsLeft = ((end + 1) - start) * Byte_Bits;
                bitsLeft += RemainingByteBits;

                IsEndOfStream = bitsLeft < bitCountToRead;
                return !IsEndOfStream;
            }
        }

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

            var alignOffset = (-BytePosition % alignment + alignment) % alignment;
            SeekToByte(BytePosition + alignOffset);
            return str;
        }

        // Original Impl
        public ulong ReadBits(int bitCount)
        {
            ulong totalBitCountToRead = (ulong)bitCount;
            var buf = _currentBuffer;

            ulong result = 0;
            ulong currentByte = CurrentByte;
            ulong bitsLeftForThisByte = RemainingByteBits;

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
                    result = result << (int)(bitsRead & 0x7f) | currentByte >> (int)(Byte_Bits - bitsRead & 0x7f);
                    currentByte = currentByte << (int)(bitsRead & 0x7f) & 0xff;
                } while (totalBitCountToRead != 0);
            }

            RemainingByteBits = (uint)bitsLeftForThisByte;
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
        public void ReadByteArrayPrefixed(byte[] dest)
        {
            uint len = this.ReadUInt32();
            dest = new byte[len];
            this.ReadIntoByteArray(dest.Length, dest, Byte_Bits);
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

        private void EnsureCapacity(long bitCount)
        {
            bitCount += Byte_Bits; // Since we may slice to the next byte, better to be safe

            uint bitsLeftThisByte = Byte_Bits - RemainingByteBits;
            long totalFreeBits = (_currentBuffer.Length * Byte_Bits) + bitsLeftThisByte;

            if (bitCount >= totalFreeBits)
            {
                int cPos = BytePosition < 0 ? SourceBuffer.Length : BytePosition;

                // Expand our buffer by twice the size everytime
                var newBuffer = new byte[SourceBuffer.Length * 2];

                // Copy our buffer over the larger one
                SourceBuffer.CopyTo(newBuffer.AsSpan());
                SourceBuffer = newBuffer;

                // Ensure that the current representation of the rest of our stream is updated
                _currentBuffer = SourceBuffer.Slice(cPos);
            }
        }

        // Original Impl (except capacity ensuring)
        public void WriteBits(ulong value, ulong bitCountToWrite)
        {
            EnsureCapacity((long)bitCountToWrite);

            var buf = _currentBuffer;
            ulong currentByte = CurrentByte;
            ulong operatingByte = currentByte;
            ulong bitsWritenForThisByte = RemainingByteBits;
            ulong bitsWriten = RemainingByteBits;

            if (bitCountToWrite != 0)
            {
                ulong val = value << (int)(Long_Bits - bitCountToWrite & 0x7f);
                do
                {
                    ulong bitsToAdd = val >> (int)(bitsWriten + 0x38 & 0x7f);
                    ulong v3 = Byte_Bits - bitsWriten;

                    if ((int)bitCountToWrite < (int)v3)
                        v3 = bitCountToWrite;

                    bitsWriten += v3;
                    bitCountToWrite -= v3;
                    val <<= (int)(v3 & 0x7f);

                    operatingByte |= bitsToAdd;
                    if (bitsWriten == Byte_Bits)
                    {
                        buf[0] = (byte)operatingByte;
                        buf = buf.Slice(1);
                        operatingByte = 0;
                        bitsWriten = 0;
                    }

                    currentByte = operatingByte;
                    bitsWritenForThisByte = bitsWriten;
                } while (bitCountToWrite != 0);
            }

            if (RemainingByteBits != 0)
                buf[0] = (byte)currentByte;

            RemainingByteBits = (uint)bitsWritenForThisByte;
            _currentBuffer = buf;
            CurrentByte = (byte)currentByte;

#if DEBUG
            _sw?.WriteLine($"[{BytePosition} - {RemainingByteBits}/8] Wrote {value} ({bitCountToWrite} bits)");
#endif
        }

        public void WriteNullStringAligned4(string value)
        {
            // Require to seek to the next round byte incase we're currently in the middle of a byte's bits
            if (RemainingByteBits != 0)
                SeekToByteFromCurrentPosition(1);

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
            SeekToByteFromCurrentPosition(totalSize);
        }

        public void WriteByteData(Span<byte> data, bool withPrefixLength = true)
        {
            EnsureCapacity(((withPrefixLength ? 4 : 0) + data.Length) * Byte_Bits);

            if (withPrefixLength)
                WriteInt32(data.Length);

            data.CopyTo(_currentBuffer);
            SeekToByteFromCurrentPosition(data.Length);
        }

        public void WriteUInt64(ulong value)
            => WriteBits(value, Long_Bits);

        public void WriteInt64(uint value)
            => WriteBits(value, Long_Bits);

        public void WriteUInt32(uint value)
            => WriteBits(value, Int_Bits);

        public void WriteInt32(int value)
            => WriteBits((ulong)value, Int_Bits);

        public void WriteInt32OrNull(int? value)
        {
            if (value == null || value == -1)
                WriteBits(unchecked((ulong)-1), Int_Bits);
            else
                WriteBits((ulong)value, Int_Bits);
        }

        public unsafe void WriteSingle(float value)
            => WriteBits((ulong)*(int*)&value, Int_Bits);

        public unsafe void WriteDouble(double value)
            => WriteBits((ulong)value, Long_Bits);

        public void WriteBool4OrNull(bool? value)
        {
            if (value is null)
                WriteInt32(-1);
            else
                WriteBool4(value.Value);
        }

        public void WriteInt16(short value)
            => WriteBits((ulong)value, Short_Bits);

        public void WriteUInt16(uint value)
            => WriteBits(value, Short_Bits);

        public void WriteByte(byte value)
            => WriteBits(value, Byte_Bits);

        public void WriteBool(bool value)
            => WriteBits((ulong)(value ? 1 : 0), Byte_Bits);

        public void WriteBool2(bool value)
            => WriteBits((ulong)(value ? 1 : 0), Short_Bits);

        public void WriteBool4(bool value)
            => WriteBits((ulong)(value ? 1 : 0), Int_Bits);

        public void WriteSByte(sbyte value)
            => WriteBits((ulong)value, Byte_Bits);

        public void WriteBoolBit(bool value)
            => WriteBits((ulong)(value ? 1 : 0), 1);

        public Span<byte> GetBuffer()
            => SourceBuffer.Slice(0, BytePosition);

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

        public void EndWriteLog(string path)
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
}