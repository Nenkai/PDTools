using System;
using System.Collections.Generic;
using System.Linq;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.Shapes;

public class MDL3ShapeKeyComparer : IComparer<MDL3ShapeKey>
{
    private static readonly MDL3ShapeKeyComparer _default = new MDL3ShapeKeyComparer();
    public static MDL3ShapeKeyComparer Default => _default;

    public int Compare(MDL3ShapeKey value1, MDL3ShapeKey value2)
    {
        MDL3ShapeKey v1 = value1;
        MDL3ShapeKey v2 = value2;

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

        if (v1.ShapeID < v2.ShapeID)
            return -1;
        else if (v1.ShapeID > v2.ShapeID)
            return 1;

        return 0;
    }
}
