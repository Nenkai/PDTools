using System;
using System.Collections.Generic;
using System.Text;

using System.Numerics;

namespace PDTools.Hashing
{
    // Not sure if this is finished
    class SHA512
    {
        private ulong[] _state = new ulong[8];
        private ulong usedspace;
        private ulong unk;
        private byte[] buffer = new byte[SHA512_BLOCK_LENGTH];

        public const int SHA512_BLOCK_LENGTH = 0x80;

        public void InitState() // 1001318 
        {
            _state[0] = 0x6A09E667F3BCC908L;
            _state[1] = 0xBB67AE8584CAA73BL;
            _state[2] = 0x3C6EF372FE94F82BL;
            _state[3] = 0xA54FF53A5F1D36F1L;
            _state[4] = 0x510E527FADE682D1L;
            _state[5] = 0x9B05688C2B3E6C1FL;
            _state[6] = 0x1F83D9ABFB41BD6BL;
            _state[7] = 0x5BE0CD19137E2179L;
        }

        public void Update(Span<byte> data)
        {
            int len = data.Length;

            usedspace += (8 * (ulong)data.Length);
            if (unk < usedspace)
            {

            }

            while (len >= SHA512_BLOCK_LENGTH)
            {

                len -= SHA512_BLOCK_LENGTH;
                data = data.Slice(SHA512_BLOCK_LENGTH);
            }

            if (len > 0)
            {

            }
        }

        public void Transform(Span<byte> data)
        {
            ulong a, b, c, d, e, f, g, h, s0, s1;
            UInt64 aa, bb, cc, dd, ee, ff, hh, gg;
            ulong T1;
            int j;

            /* initialize registers with the prev. intermediate value */
            a = _state[0];
            b = _state[1];
            c = _state[2];
            d = _state[3];
            e = _state[4];
            f = _state[5];
            g = _state[6];
            h = _state[7];

            j = 0;
            do
            {
                T1 = h + Sigma_1(e) + Ch(e, f, g) + _K[j] + data[j];
                ee = d + T1;
                aa = T1 + Sigma_0(a) + Maj(a, b, c);
                j++;
            }
            while (j < 16);
        }

        private static UInt64 Ch(UInt64 x, UInt64 y, UInt64 z)
        {
            return ((x & y) ^ ((x ^ 0xffffffffffffffff) & z));
        }

        private static UInt64 Maj(UInt64 x, UInt64 y, UInt64 z)
        {
            return ((x & y) ^ (x & z) ^ (y & z));
        }

        private static UInt64 Sigma_0(UInt64 x)
        {
            return (RotateRight(x, 28) ^ RotateRight(x, 34) ^ RotateRight(x, 39));
        }

        private static UInt64 Sigma_1(UInt64 x)
        {
            return (RotateRight(x, 14) ^ RotateRight(x, 18) ^ RotateRight(x, 41));
        }

        private static UInt64 sigma_0(UInt64 x)
        {
            return (RotateRight(x, 1) ^ RotateRight(x, 8) ^ (x >> 7));
        }

        private static UInt64 sigma_1(UInt64 x)
        {
            return (RotateRight(x, 19) ^ RotateRight(x, 61) ^ (x >> 6));
        }

        private static ulong RotateRight(ulong value, int offset)
        {
#if NETCOREAPP3_0_OR_GREATER
            return BitOperations.RotateRight(value, offset);
#else
            return (value >> offset) | (value << (64 - offset));
#endif
        }

        private readonly static UInt64[] _K = {
            0x428a2f98d728ae22, 0x7137449123ef65cd, 0xb5c0fbcfec4d3b2f, 0xe9b5dba58189dbbc,
            0x3956c25bf348b538, 0x59f111f1b605d019, 0x923f82a4af194f9b, 0xab1c5ed5da6d8118,
            0xd807aa98a3030242, 0x12835b0145706fbe, 0x243185be4ee4b28c, 0x550c7dc3d5ffb4e2,
            0x72be5d74f27b896f, 0x80deb1fe3b1696b1, 0x9bdc06a725c71235, 0xc19bf174cf692694,
            0xe49b69c19ef14ad2, 0xefbe4786384f25e3, 0x0fc19dc68b8cd5b5, 0x240ca1cc77ac9c65,
            0x2de92c6f592b0275, 0x4a7484aa6ea6e483, 0x5cb0a9dcbd41fbd4, 0x76f988da831153b5,
            0x983e5152ee66dfab, 0xa831c66d2db43210, 0xb00327c898fb213f, 0xbf597fc7beef0ee4,
            0xc6e00bf33da88fc2, 0xd5a79147930aa725, 0x06ca6351e003826f, 0x142929670a0e6e70,
            0x27b70a8546d22ffc, 0x2e1b21385c26c926, 0x4d2c6dfc5ac42aed, 0x53380d139d95b3df,
            0x650a73548baf63de, 0x766a0abb3c77b2a8, 0x81c2c92e47edaee6, 0x92722c851482353b,
            0xa2bfe8a14cf10364, 0xa81a664bbc423001, 0xc24b8b70d0f89791, 0xc76c51a30654be30,
            0xd192e819d6ef5218, 0xd69906245565a910, 0xf40e35855771202a, 0x106aa07032bbd1b8,
            0x19a4c116b8d2d0c8, 0x1e376c085141ab53, 0x2748774cdf8eeb99, 0x34b0bcb5e19b48a8,
            0x391c0cb3c5c95a63, 0x4ed8aa4ae3418acb, 0x5b9cca4f7763e373, 0x682e6ff3d6b2b8a3,
            0x748f82ee5defb2fc, 0x78a5636f43172f60, 0x84c87814a1f0ab72, 0x8cc702081a6439ec,
            0x90befffa23631e28, 0xa4506cebde82bde9, 0xbef9a3f7b2c67915, 0xc67178f2e372532b,
            0xca273eceea26619c, 0xd186b8c721c0c207, 0xeada7dd6cde0eb1e, 0xf57d4f7fee6ed178,
            0x06f067aa72176fba, 0x0a637dc5a2c898a6, 0x113f9804bef90dae, 0x1b710b35131c471b,
            0x28db77f523047d84, 0x32caab7b40c72493, 0x3c9ebe0a15c9bebc, 0x431d67c49c100d4c,
            0x4cc5d4becb3e42b6, 0x597f299cfc657e2a, 0x5fcb6fab3ad6faec, 0x6c44198c4a475817,
        };
    }
}