using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class ArcadeInfo : TableMetadata
{
    public override string LabelPrefix { get; } = "";

    public ArcadeInfo(SpecDBFolder folderType)
    {
        Columns.Add(new ColumnMetadata("NormalRace0", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("NormalRace1", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("NormalRace2", DBColumnType.Key));

        Columns.Add(new ColumnMetadata("DirtRace0", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("DirtRace1", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("DirtRace2", DBColumnType.Key));

        Columns.Add(new ColumnMetadata("DirtTunedCarCode", DBColumnType.Key));
        Columns.Add(new ColumnMetadata("SnowTunedCarCode", DBColumnType.Key));

        Columns.Add(new ColumnMetadata("TireSelect", DBColumnType.Bool));
    }
}
