using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.Utils;

public class ByteBufferComparer : IComparer<byte[]>
{
    private static readonly ByteBufferComparer _default = new ByteBufferComparer();
    public static ByteBufferComparer Default => _default;

    public int Compare(byte[]? value1, byte[]? value2)
    {
        byte[] v1 = value1 ?? [];
        byte[] v2 = value2 ?? [];

        if (v1.Length < v2.Length)
            return -1;
        else if (v1.Length > v2.Length)
            return 1;

        int min = v1.Length > v2.Length ? v2.Length : v1.Length;
        for (int i = 0; i < min; i++)
        {
            if (v1[i] < v2[i])
                return -1;
            else if (v1[i] > v2[i])
                return 1;
        }

        return 0;
    }
}