using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3;

public class MDL3ModelKeyComparer : IComparer<MDL3ModelKey>
{
    private static readonly MDL3ModelKeyComparer _default = new MDL3ModelKeyComparer();
    public static MDL3ModelKeyComparer Default => _default;

    public int Compare(MDL3ModelKey value1, MDL3ModelKey value2)
    {
        MDL3ModelKey v1 = value1;
        MDL3ModelKey v2 = value2;

        int min = v1.Name.Length > v2.Name.Length ? v2.Name.Length : v1.Name.Length;
        for (int i = 0; i < min; i++)
        {
            if (v1.Name[i] < v2.Name[i])
                return -1;
            else if (v1.Name[i] > v2.Name[i])
                return 1;
        }

        if (v1.Name.Length < v2.Name.Length)
            return -1;
        else if (v1.Name.Length > v2.Name.Length)
            return 1;

        if (v1.ModelID < v2.ModelID)
            return -1;
        else if (v1.ModelID > v2.ModelID)
            return 1;

        return 0;
    }
}
