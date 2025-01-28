using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3;

public class MDL3TextureKeyComparer : IComparer<MDL3TextureKey>
{
    private static readonly MDL3TextureKeyComparer _default = new MDL3TextureKeyComparer();
    public static MDL3TextureKeyComparer Default => _default;

    public int Compare(MDL3TextureKey value1, MDL3TextureKey value2)
    {
        MDL3TextureKey v1 = value1;
        MDL3TextureKey v2 = value2;

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

        if (v1.TextureID < v2.TextureID)
            return -1;
        else if (v1.TextureID > v2.TextureID)
            return 1;

        return 0;
    }
}
