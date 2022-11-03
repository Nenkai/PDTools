using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.GTPatcher
{
    public class Breakpoint
    {
        public int Index { get; }
        public ulong Offset { get; }

        public Breakpoint(int index, ulong offset)
        {
            Index = index;
            Offset = offset;
        }
    }
}
