using System;
using System.Collections.Generic;
using System.Linq;

using PDTools.Files.Models.ModelSet3.Wing;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3
{
    public class MDL3WingKeyComparer : IComparer<MDL3WingKey>
    {
        private static readonly MDL3WingKeyComparer _default = new MDL3WingKeyComparer();
        public static MDL3WingKeyComparer Default => _default;

        public int Compare(MDL3WingKey value1, MDL3WingKey value2)
        {
            MDL3WingKey v1 = value1;
            MDL3WingKey v2 = value2;

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

            if (v1.WingDataID < v2.WingDataID)
                return -1;
            else if (v1.WingDataID > v2.WingDataID)
                return 1;

            return 0;
        }
    }
}
