﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class IntakeManifold : TableMetadata
{
    public override string LabelPrefix { get; } = "in_";

    public IntakeManifold(SpecDBFolder folderType)
    {
        Columns.Add(new ColumnMetadata("torquemodifier", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("torquemodifier2", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("torquemodifier3", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("shiftlimit", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("revlimit", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("rpmeffect", DBColumnType.Byte));
    }
}
