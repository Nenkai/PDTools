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
        // Some structures notes at the end of this file

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

    /*
     * struct BigNumber
       {
         _DWORD dword0;
         _DWORD RefCount;
         _DWORD Length_0x08;
         _DWORD Capacity_0x0C;
         _DWORD Sign;
         unsigned int *OperatingBuffer;
       };

       struct BufferWrapper or BigNumber wrapper?
       {
         int field_0;
         BigNumber *BigNum;
       };

       struct RSAContextMaybe
       {
         int field_0;
         BigNumber *BufRev1;
         BufferWrapper field_8;
         BufferWrapper AlsoHash1_field0x10;
         BufferWrapper BufWrap_0x18;
         BufferWrapper BufWrap_0x20;
         BufferWrapper field_28;
         int BitCounter;
       };

    - Functions names, all guessed from the hours wasted on this
    102FC68 - BigNumber::Delete(BigNumber *a1, char a2)
    102FCC0 - BigNumber::BigNumber(BigNumber *this, int valuePtr, int length)(
    102FDB8 - BigNumber::Copy(BigNumber *target, BigNumber *source)
    102FE48 - BigNumber::FromInt(BigNumber *a1, __int64 a2)
    102FED0 - BigNumber::CompareWithoutSign(BigNumber *a1, BigNumber *a2)
    102FF50 - BigNumber::CompareWithSign(BigNumber *a1, BigNumber *a2)
    102FFA0 - BigNumber::Equals(BigNumber *a1, BigNumber *a2)
    1030000 - BigNumber::NotEquals(BigNumber *a1, BigNumber *a2)
    1030060 - sub_1030060(BigNumber *a1, BigNumber *a2)
    10300C8 - BigNumber::ResizeBuffer(BigNumber *a1, unsigned __int64 sizeInt, __int64 clearBuffer)
    1030198 - BigNumber::InsertValue(BigNumber *a1, unsigned __int64 index, __int64 value)
    1030278 - BigNumber::CountBits(BigNumber *a1)
    1030300 - Delete??(__int64 a1, char a2)
    1030428 - CreateRSAContext(RSAContextMaybe *a1, BufferWrapper *hash1Holder)
    1030B98 - sub_1030B98(RSAContextMaybe *a1, BufferWrapper *a2, __int64 a3)
    1030FE8 - PerformRSAThingMaybe(BigNumber *a1, RSAContextMaybe *a2, __int64 a3, BufferWrapper *exponent)
    1031670 - RSAComputeMaybe(__int64 ret, __int64 hash2Holder, __int64 primeNumber, __int64 a4)
    10316D8 - sub_10316D8(BigNumber *ret, BigNumber *a2, BigNumber *a3, int totalBits)
    1031930 - BigNumber::Multiply(BufferWrapper *ret_1, BufferWrapper *a2, BufferWrapper *a3)
    1031AC0 - MutlplyTruncate(__int64 a1, int a2, BufferWrapper *a3, int a4) not certain but i think it's that
    1031C20 - BigNumber::RotateRight1(BigNumber *a1)
    1031C90 - BigNumber::RotateRightN(BigNumber *a1, __int64 a2)
    1031E18 - BigNumber::RotateLeft1(BigNumber *a1)
    1031E90 - BigNumber::RotateLeftN(BigNumber *a1, __int64 len)
    1032050 - DoMinusTargetA(BigNumber *left, BigNumber *right)
    1032120 - BigNumber::Subtract(BigNumber *a1, BigNumber *a2)
    1032210 - DoAdd(BigNumber *a1, BigNumber *a2)
    1032318 - BigNumber::Add(BigNumber *a1, BigNumber *a2)
    sub_10323F8(__int64 a1, int a2, int a3) ?? no xref
    1032760 - ExtGCD(BufferWrapper *output, BufferWrapper *a2, BufferWrapper *a3) this should be it
    1032FC8 - BigNumber::DivideMaybe(BigNumber *ret, BigNumber *a2, BigNumber *a3, __int64 a4)
    1033188 - BigNumber::Modulo(__int64 a1, unsigned int a2, BigNumber *a3)
    1033330 - BigNumber::DivRem(BufferWrapper *divResult, BufferWrapper *left, BufferWrapper *right, BufferWrapper *remResult)

    1033730 - SalsaSetup(_DWORD *a1, __int64 a2, unsigned __int64 a3)
    1033A80 - SalsaSetState(int a1, __int64 a2)
    1033AD0 - SalsaIncrement(int a1)
    1033B80 - SalsaHash(int a1, unsigned int a2)
    1033F78 - SalsaDecrypt_0(__int64 a1, int a2, int a3, unsigned __int64 a4)
    1034048 - SalsaDecrypt(__int64 a1, int a2, unsigned __int64 a3)

    1004900 - Sha512_Compute
    10038A0 - sha512_init_2
    1001318 - sha512_init
    10038D8 - sha512_perform
    1001468 - Sha512Hash

    1005C78 - CheckCRC
    1005C40 - DecryptKey
    1005BE8 - CRC32
    1005DF0 - StartDecrypt
    10049E8 - LoadImage
    1004748 - LoadCoreFile
    10011A8 - CreateBuffer
    10379D0 - InflatorBase::InflatorBase (decompresses)
    1004CB0 - PrepareCore
    1044840 - StartImage with ExecPS2
    */
}
