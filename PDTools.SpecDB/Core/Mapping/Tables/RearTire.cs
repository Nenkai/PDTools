﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class RearTire : TableMetadata
{
    public override string LabelPrefix { get; } = "rt_";

    public RearTire(SpecDBFolder folderType)
    {
        Columns.Add(new ColumnMetadata("tiresize", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("tirecompound0", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("tirecompound1", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("tirecompound2", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("tireforcevol0", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("tireforcevol1", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("tireforcevol2", DBColumnType.Key));

        Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));

        if (folderType >= SpecDBFolder.GT5_JP3009)
        {
            Columns.Add(new ColumnMetadata("tireDrainageLevel", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("tireSpring_Auto", DBColumnType.Byte));
        }
    }
}
