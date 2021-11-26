using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files
{
    public struct Tri
    {
        public ushort A;
        public ushort B;
        public ushort C;

        public Tri(ushort a, ushort b, ushort c)
        {
            A = a;
            B = b;
            C = c;
        }
    }
}
