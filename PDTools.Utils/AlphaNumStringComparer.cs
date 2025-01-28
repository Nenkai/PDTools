using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Utils;

public class AlphaNumStringComparer : IComparer<string>
{
    private static readonly AlphaNumStringComparer _default = new AlphaNumStringComparer();
    public static AlphaNumStringComparer Default => _default;

    public int Compare(string value1, string value2)
    {
        string v1 = value1;
        string v2 = value2;

        int min = v1.Length > v2.Length ? v2.Length : v1.Length;
        for (int i = 0; i < min; i++)
        {
            if (v1[i] < v2[i])
                return -1;
            else if (v1[i] > v2[i])
                return 1;
        }
        if (v1.Length < v2.Length)
            return -1;
        else if (v1.Length > v2.Length)
            return 1;

        return 0;
    }
}
