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

public class EngineBalance : TableMetadata
{
    public override string LabelPrefix => "eb_";

    public EngineBalance(SpecDBFolder folderType)
    {
        Columns.Add(new ColumnMetadata("torqueModifier", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("torqueModifier2", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
    }
}
