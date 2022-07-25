using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using System.Buffers.Binary;
using System.Numerics;

namespace PDTools.Crypto
{
    /// <summary>
    /// Pure salsa20 implementation for direct decrypt/encrypt.
    /// </summary>
    public struct Salsa20
    {
        static readonly byte[] c_sigma = Encoding.ASCII.GetBytes("expand 32-byte k");
        static readonly byte[] c_tau = Encoding.ASCII.GetBytes("expand 16-byte k");

        public const int StateLength = 0x40;
        private uint[] m_state;

        public bool Initted { get; set; }

        public Salsa20(byte[] key, int keyLength)
        {
            if (keyLength > 32)
                keyLength = 32;

            m_state = new uint[0x10];

            // memcpy(vector, key, keyLength)
            var keyUints = MemoryMarshal.Cast<byte, uint>(key.AsSpan(0, keyLength));
            keyUints.CopyTo(m_state.AsSpan(1, keyLength / 4));

            byte[] constants = keyLength == 32 ? c_sigma : c_tau;
            int keyIndex = keyLength - 16;

            m_state[11] = ToUInt32(key, keyIndex + 0);
            m_state[12] = ToUInt32(key, keyIndex + 4);
            m_state[13] = ToUInt32(key, keyIndex + 8);
            m_state[14] = ToUInt32(key, keyIndex + 12);
            m_state[0] = ToUInt32(constants, 0);
            m_state[5] = ToUInt32(constants, 4);
            m_state[10] = ToUInt32(constants, 8);
            m_state[15] = ToUInt32(constants, 12);

            m_state[6] = 0;
            m_state[7] = 0;
            m_state[8] = 0;
            m_state[9] = 0;

            Initted = true;
        }

        public void SetIV(Span<byte> iv)
        {
            var ivUints = MemoryMarshal.Cast<byte, uint>(iv);
            m_state[6] = ivUints[0];
            m_state[7] = ivUints[1];
            m_state[8] = 0;
            m_state[9] = 0;
        }

        /// <summary>
        /// Decrypts bytes in one go.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        public void Decrypt(Span<byte> bytes, int length)
        {
            Span<byte> o = stackalloc byte[StateLength];
            for (int i = 0; i < StateLength; i++)
                o[i] = 0;

            int pos = 0;
            while (length > 0)
            {
                Hash(o);
                Increment();

                int blockSize = Math.Min(StateLength, length);
                for (int i = 0; i < blockSize; i++)
                    bytes[pos + i] ^= o[i];

                pos += StateLength;
                length -= StateLength;
            }

            if (length > 0)
            {
                Hash(o);
                Increment();

                // Remaining bytes
                for (int i = 0; i < length; i++)
                    bytes[pos + i] ^= o[i];
            }
        }

        /// <summary>
        /// Decrypts a buffer from an offset.
        /// </summary>
        /// <param name="bytes">Bytes input to decrypt.</param>
        /// <param name="length">Length to decrypt.</param>
        /// <param name="globalOffset">Global offset if its a file, to properly generate the continuing state.</param>
        /// <param name="offset">Offset of the buffer.</param>
        public void DecryptOffset(Span<byte> bytes, int length, long globalOffset, long offset)
        {
            Span<byte> o = stackalloc byte[StateLength];
            for (int i = 0; i < StateLength; i++)
                o[i] = 0;

            long pos = offset;

            Set(globalOffset / StateLength);
            Hash(o);

            long current = globalOffset & 0x3f;
            while (pos < offset + length)
            {
                bytes[(int)pos++] ^= o[(int)current++];
                if (current >= StateLength)
                {
                    Increment();
                    Hash(o);

                    current = 0;
                }
            }
        }

        private void Hash(Span<byte> output)
        {
            Span<uint> state = stackalloc uint[0x10];
            m_state.AsSpan().CopyTo(state);

            for (int round = 20; round > 0; round -= 2)
            {
                state[4] ^= BitOperations.RotateLeft(Add(state[0], state[12]), 7);
                state[8] ^= BitOperations.RotateLeft(Add(state[4], state[0]), 9);
                state[12] ^= BitOperations.RotateLeft(Add(state[8], state[4]), 13);
                state[0] ^= BitOperations.RotateLeft(Add(state[12], state[8]), 18);
                state[9] ^= BitOperations.RotateLeft(Add(state[5], state[1]), 7);
                state[13] ^= BitOperations.RotateLeft(Add(state[9], state[5]), 9);
                state[1] ^= BitOperations.RotateLeft(Add(state[13], state[9]), 13);
                state[5] ^= BitOperations.RotateLeft(Add(state[1], state[13]), 18);
                state[14] ^= BitOperations.RotateLeft(Add(state[10], state[6]), 7);
                state[2] ^= BitOperations.RotateLeft(Add(state[14], state[10]), 9);
                state[6] ^= BitOperations.RotateLeft(Add(state[2], state[14]), 13);
                state[10] ^= BitOperations.RotateLeft(Add(state[6], state[2]), 18);
                state[3] ^= BitOperations.RotateLeft(Add(state[15], state[11]), 7);
                state[7] ^= BitOperations.RotateLeft(Add(state[3], state[15]), 9);
                state[11] ^= BitOperations.RotateLeft(Add(state[7], state[3]), 13);
                state[15] ^= BitOperations.RotateLeft(Add(state[11], state[7]), 18);
                state[1] ^= BitOperations.RotateLeft(Add(state[0], state[3]), 7);
                state[2] ^= BitOperations.RotateLeft(Add(state[1], state[0]), 9);
                state[3] ^= BitOperations.RotateLeft(Add(state[2], state[1]), 13);
                state[0] ^= BitOperations.RotateLeft(Add(state[3], state[2]), 18);
                state[6] ^= BitOperations.RotateLeft(Add(state[5], state[4]), 7);
                state[7] ^= BitOperations.RotateLeft(Add(state[6], state[5]), 9);
                state[4] ^= BitOperations.RotateLeft(Add(state[7], state[6]), 13);
                state[5] ^= BitOperations.RotateLeft(Add(state[4], state[7]), 18);
                state[11] ^= BitOperations.RotateLeft(Add(state[10], state[9]), 7);
                state[8] ^= BitOperations.RotateLeft(Add(state[11], state[10]), 9);
                state[9] ^= BitOperations.RotateLeft(Add(state[8], state[11]), 13);
                state[10] ^= BitOperations.RotateLeft(Add(state[9], state[8]), 18);
                state[12] ^= BitOperations.RotateLeft(Add(state[15], state[14]), 7);
                state[13] ^= BitOperations.RotateLeft(Add(state[12], state[15]), 9);
                state[14] ^= BitOperations.RotateLeft(Add(state[13], state[12]), 13);
                state[15] ^= BitOperations.RotateLeft(Add(state[14], state[13]), 18);
            }

            for (int index = 0; index < 16; index++)
                ToBytes(Add(state[index], m_state[index]), output, 4 * index);
        }


        private void Increment()
        {
            m_state[8]++;
            if (m_state[8] == 0)
                m_state[9]++;
        }

        /// <summary>
        /// Used to seek through the crypted buffer.
        /// </summary>
        /// <param name="offset"></param>
        public void Set(long offset)
        {
            m_state[8] = (uint)offset;
            m_state[9] = (uint)(offset >> 32);
        }

        private static uint Add(uint v, uint w)
        {
            return unchecked(v + w);
        }

        private static void ToBytes(uint input, Span<byte> output, int outputOffset)
        {
            unchecked
            {
                output[outputOffset] = (byte)input;
                output[outputOffset + 1] = (byte)(input >> 8);
                output[outputOffset + 2] = (byte)(input >> 16);
                output[outputOffset + 3] = (byte)(input >> 24);
            }
        }

        private static uint ToUInt32(byte[] input, int inputOffset)
            => unchecked((uint)(((input[inputOffset] | (input[inputOffset + 1] << 8)) | (input[inputOffset + 2] << 16)) | (input[inputOffset + 3] << 24)));
    }
}
