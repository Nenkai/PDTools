using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
namespace SpecDBOld.Core
{
    [DebuggerDisplay("{Id} ({Label})")]
    public class RowData
    {
        public int Id { get; set; }
        public string Label { get; set; }
    }
}
