using System;
using System.Collections.Generic;
using System.Linq;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.Meshes
{
    public class MDL3MeshKeyComparer : IComparer<MDL3MeshKey>
    {
        private static readonly MDL3MeshKeyComparer _default = new MDL3MeshKeyComparer();
        public static MDL3MeshKeyComparer Default => _default;

        public int Compare(MDL3MeshKey value1, MDL3MeshKey value2)
        {
            MDL3MeshKey v1 = value1;
            MDL3MeshKey v2 = value2;

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

            if (v1.MeshID < v2.MeshID)
                return -1;
            else if (v1.MeshID > v2.MeshID)
                return 1;

            return 0;
        }
    }
}
