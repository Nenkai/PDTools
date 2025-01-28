using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.GT4ElfBuilderTool;

internal class RSAContext
{
    // Offsets are based on GT4O US
    // Some structures notes at the end of this file

    // Maybe this is just a montgomery class?
    // https://web.archive.org/web/20240214014200/https://www.codeproject.com/Tips/791253/ModPow
    // https://web.archive.org/web/20151128235421/cs.ucsb.edu/~koc/docs/j34.pdf
    // https://web.archive.org/web/20161127114659/http://www.hackersdelight.org/hdcodetxt/mont64.c.txt
    // https://github.com/xfbs/montgomery/blob/main/montgomery.c

    public BigInteger Reciprocal;
    public BigInteger Modulus;
    public BigInteger Factor;
    //public BigInteger field_0x20; not needed
    //public BigInteger field_0x28; not needed

    // Other stuff, don't think that's part of the original struct
    private BigInteger EncHash;
    private BigInteger Mask;
    public int ReducerBits;
    private BigInteger Reducer;

    // Part 1 - 0x1030428 (Init RSA?)
    public void Init_0x1030428(byte[] modulus, byte[] encHash)
    {
        this.Modulus = new BigInteger(modulus, isUnsigned: true);
        this.EncHash = new BigInteger(encHash, isUnsigned: true);

        this.ReducerBits = modulus.Length * 8;
        this.Reducer = new BigInteger(1) << ReducerBits;
        this.Mask = this.Reducer - 1;

        this.Reciprocal = Egcd(Reducer % Modulus, Modulus).LeftFactor; // 1032760
        this.Factor = (Reducer * Reciprocal - 1) / Modulus;
    }

    // Part 2 - 0x10316A8 (Perform RSA?)
    public BigInteger MontModPow_1030FE8(int exponent) // is it really exponent? not sure
    {
        BigInteger one = new BigInteger(1) << 0x400;
        BigInteger u = exponent;

        var t = (one * this.EncHash) % this.Modulus;
        var s = one - this.Modulus;

        // Looks an awful lot like modPow here, refer to java source
        // https://developer.classpath.org/doc/java/math/BigInteger-source.html

        while (!u.IsZero)
        {
            if ((u & 1) == BigInteger.One)
                s = MontReduce_1030B98(s, t);

            u >>= 1;
            t = MontReduce_1030B98(t, t);
        }

        // montout
        var D = (s * this.Reciprocal) % this.Modulus;
        return D;
    }

    public BigInteger MontReduce_1030B98(BigInteger T, BigInteger B)
    {
        //this.field_0x20 = truncate((T * B), bit_size * 2);
        T = truncate((T * B), ReducerBits * 2); // & operator?
        var X = T;

        //this.field_0x28 = truncate(this.field_0x20 * this.field_0x18_Gcd2, bit_size); /* ?? */
        T = truncate(T * this.Factor, ReducerBits); // & operator?

        T = truncate(T * this.Modulus, ReducerBits * 2); // & operator?
        T += X;
        T >>= ReducerBits;

        if (this.Modulus.CompareTo(T) <= 0)
            T -= this.Modulus;

        return T;
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
        // 06/10/2023 comment from later on - this might still break on i.e some GT4P region, and GT4 EU (Preprod)
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
