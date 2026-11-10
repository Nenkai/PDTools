//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/PDTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class CarCustomInfo : TableMetadata
{
    public override string LabelPrefix { get; } = "";
    public CarCustomInfo(SpecDBFolder folderType)
    {
        Columns.Add(new ColumnMetadata("BitsA", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("BitsB", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("BitsC", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("BitsD", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("BitsE", DBColumnType.Int));
        Columns.Add(new ColumnMetadata("BitsF", DBColumnType.Int));
    }
}
