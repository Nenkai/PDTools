﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables
{
    public class Wing : TableMetadata
    {
        public Wing(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("?", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("?", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("?", DBColumnType.Short));
        }
    }
}
