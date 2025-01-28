using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;
using System.Numerics;

namespace PDTools.Utils;

public static class MiscUtils
{
    /// <summary>
    /// Aligns a value to the specified alignment
    /// </summary>
    /// <param name="x">Value</param>
    /// <param name="alignment">Alignment</param>
    /// <returns></returns>
    public static uint AlignValue(uint x, uint alignment)
    {
        uint mask = ~(alignment - 1);
        return (x + (alignment - 1)) & mask;
    }

    public static int MeasureBytesTakenByBits(double bitCount)
        => (int)Math.Round(bitCount / 8, MidpointRounding.AwayFromZero);

    // https://stackoverflow.com/a/4975942
    private static readonly string[] sizeSuf = ["B", "KB", "MB", "GB", "TB", "PB", "EB"]; //Longs run out around EB
    public static string BytesToString(long byteCount)
    {
        if (byteCount == 0)
            return "0" + sizeSuf[0];
        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return (Math.Sign(byteCount) * num).ToString() + sizeSuf[place];
    }

    // https://stackoverflow.com/a/9995303
    public static byte[] StringToByteArray(string hex)
    {
        if (hex.Length % 2 == 1)
            throw new Exception("The binary key cannot have an odd number of digits");

        byte[] arr = new byte[hex.Length >> 1];

        for (int i = 0; i < hex.Length >> 1; ++i)
        {
            arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
        }

        return arr;
    }

    public static int GetHexVal(char hex)
    {
        int val = (int)hex;
        //For uppercase A-F letters:
        //return val - (val < 58 ? 48 : 55);
        //For lowercase a-f letters:
        //return val - (val < 58 ? 48 : 87);
        //Or the two combined, but a bit slower:
        return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
    }

    /// <summary>
    /// Gets the highest bit index for a value
    /// </summary>
    /// <param name="value">Value to get the highest bit for</param>
    /// <returns></returns>
    public static int GetHighestBitIndex(int value)
    {
#if NETCOREAPP3_0_OR_GREATER
        return (32 - BitOperations.LeadingZeroCount((uint)value));
#else
        return (32 - LeadingZerosSoftware(value));
#endif
    }

    /// <summary>
    /// Gets the max value for the specified bit count
    /// </summary>
    /// <param name="bits">Number of bits</param>
    /// <returns></returns>
    public static uint GetMaxForBitCount(int bits)
    {
        if (bits == 0)
            return 0;

        return (uint)Math.Pow(2, bits);
    }

    /// <summary>
    /// Gets the max signed value for the specified bit count
    /// </summary>
    /// <param name="bits">Number of bits</param>
    /// <returns></returns>
    public static uint GetMaxSignedForBitCount(int bits)
    {
        if (bits == 0)
            return 0;

        return (uint)(GetMaxForBitCount(bits) / 2) - 1;
    }

    /// <summary>
    /// Packs a float into signed bits
    /// </summary>
    /// <param name="value">Value to pack</param>
    /// <param name="packedValue">Packed value bits</param>
    /// <param name="bits">Bits taken by value</param>
    public static void PackFloat(float value, out int packedValue, out int bits)
    {
        float rounded = (float)Math.Round(value);

        int absDistance = (int)Math.Abs(rounded);
        bits = MiscUtils.GetHighestBitIndex(absDistance);

        if (absDistance != 0 && absDistance > MiscUtils.GetMaxSignedForBitCount(bits))
            bits++;

        if (value < 0)
            packedValue = (int)(MiscUtils.GetMaxForBitCount(bits) + rounded);
        else
            packedValue = absDistance;
    }

    /// <summary>
    /// Packs a float to a specific amount of bits
    /// </summary>
    /// <param name="value"></param>
    /// <param name="bits"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static int PackFloatToBitRange(float value, int bits)
    {
        float rounded = (float)Math.Round(value);

        int max = (int)MiscUtils.GetMaxForBitCount(bits);
        if (rounded > max)
            throw new Exception($"Value too large to pack to {bits} bits");

        if (value < 0)
            return max + (int)rounded;
        else
            return (int)rounded;
    }

    /// <summary>
    /// Gets the highest bit index of a packed float
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int GetHighestBitIndexOfPackedFloat(float value)
    {
        float rounded = (float)Math.Round(value);

        int absDistance = Math.Abs((int)rounded);
        int bits = MiscUtils.GetHighestBitIndex(absDistance);

        if (absDistance != 0 && absDistance > MiscUtils.GetMaxSignedForBitCount(bits))
            bits++;

        return bits;
    }

    private static int LeadingZerosSoftware(int x)
    {
        const int numIntBits = sizeof(int) * 8; //compile time constant
                                                //do the smearing
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        //count the ones
        x -= x >> 1 & 0x55555555;
        x = (x >> 2 & 0x33333333) + (x & 0x33333333);
        x = (x >> 4) + x & 0x0f0f0f0f;
        x += x >> 8;
        x += x >> 16;
        return numIntBits - (x & 0x0000003f); //subtract # of 1s from 32
    }
}
