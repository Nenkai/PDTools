using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.GT4ElfBuilderTool
{
    internal class RSAContext
    {
        // Offsets are based on GT4O US

        public BigInteger field_0x08_Gcd1;
        public BigInteger field_0x10_BigNumber1;
        public BigInteger field_0x18_Gcd2;
        public BigInteger field_0x20;
        public BigInteger field_0x28;

        private BigInteger hash2r;

        // Part 1 - 0x1030428 (Init RSA?)
        public void InitMaybe_0x1030428(byte[] number1, byte[] number2)
        {
            this.field_0x10_BigNumber1 = new BigInteger(number1, isUnsigned: true);
            this.hash2r = new BigInteger(number2, isUnsigned: true);

            BigInteger baseVal = new BigInteger(1);
            baseVal <<= (0x400);

            var subtracted = baseVal - this.field_0x10_BigNumber1;
            var gcd1 = Egcd(subtracted, this.field_0x10_BigNumber1).LeftFactor; // 1032760
            var gcd2 = -Egcd(this.field_0x10_BigNumber1, baseVal).LeftFactor; // 1032760

            this.field_0x08_Gcd1 = gcd1;
            this.field_0x18_Gcd2 = gcd2;
        }

        // Part 2 - 0x10316A8 (Perform RSA?)
        public BigInteger ComputeOrDecrypt_1030FE8(int exponent) // is it really exponent? not sure
        {
            BigInteger baseVal = new BigInteger(1);
            baseVal <<= (0x400);

          
            var multiplied = baseVal * this.hash2r;
            var modHash1And2 = multiplied % this.field_0x10_BigNumber1;

            var prime = new BigInteger(exponent);
            long numBits = prime.GetBitLength();

            var subtracted = baseVal - this.field_0x10_BigNumber1;

            var i = 0;
            while (true)
            {
                if (((prime >> i) & 1) != 0)
                {
                    sub_1030B98(ref subtracted, modHash1And2);
                }

                if (i == numBits)
                    break;

                i++;
                sub_1030B98(ref modHash1And2, modHash1And2);
            }

            var res = subtracted * this.field_0x08_Gcd1;
            var decrypted = res % this.field_0x10_BigNumber1;
            return decrypted;
        }

        void sub_1030B98(ref BigInteger unk, BigInteger unk2)
        {
            const int bit_size = 0x80 * 8; // Size depends on hash1? should be 0x400
            this.field_0x20 = truncate((unk * unk2), bit_size * 2);
            this.field_0x28 = truncate(this.field_0x20 * this.field_0x18_Gcd2, bit_size); /* ?? */
            var v3 = truncate(this.field_0x28 * this.field_0x10_BigNumber1, bit_size * 2);

            var v4 = v3 + this.field_0x20;
            v4 >>= 0x400;

            if (this.field_0x10_BigNumber1.CompareTo(v4) <= 0)
                v4 -= this.field_0x10_BigNumber1;

            unk = v4;
        }

        public static (BigInteger LeftFactor,
            BigInteger RightFactor,
            BigInteger Gcd) Egcd(BigInteger left, BigInteger right)
        {
            BigInteger leftFactor = 0;
            BigInteger rightFactor = 1;

            BigInteger u = 1;
            BigInteger v = 0;
            BigInteger gcd = 0;

            while (left != 0)
            {
                BigInteger q = right / left;
                BigInteger r = right % left;

                BigInteger m = leftFactor - u * q;
                BigInteger n = rightFactor - v * q;

                right = left;
                left = r;
                leftFactor = u;
                rightFactor = v;
                u = m;
                v = n;

                gcd = right;
            }

            // Added this, seems important
            // 06/10/2023 comment from later on - this might still break on i.e some GT4P region
            // Should it be "if (leftFactor < 0)" ?
            leftFactor = leftFactor + u;

            return (LeftFactor: leftFactor,
                    RightFactor: rightFactor,
                    Gcd: gcd);
        }


        public static BigInteger truncate(BigInteger big, int bit_size)
        {
            if (big.GetBitLength() <= bit_size)
                return big;

            var arr = big.ToByteArray(isUnsigned: big.Sign >= 0).AsSpan(0, bit_size / 8);
            return new BigInteger(arr, big.Sign >= 0);
        }
    }
}
