using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.Structures
{
    public struct DbCode
    {
        public int Code { get; set; }
        public int TableId { get; set; }

        public DbCode(int code, int tableId)
        {
            Code = code;
            TableId = tableId;
        }
    }
}
