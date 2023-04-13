using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables
{
    public class ArcadeInfo : TableMetadata
    {
        public override string LabelPrefix { get; } = "";

        public ArcadeInfo(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("NormalRace0", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Race_TblID0", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("NormalRace1", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Race_TblID1", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("NormalRace2", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Race_TblID2", DBColumnType.Int));

            Columns.Add(new ColumnMetadata("DirtRace0", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Race_TblID3", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("DirtRace1", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Race_TblID4", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("DirtRace2", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("Race_TblID5", DBColumnType.Int));

            Columns.Add(new ColumnMetadata("DirtTunedCarCode", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("EnemyCar_TblID", DBColumnType.Int));

            Columns.Add(new ColumnMetadata("SnowTunedCarCode", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("EnemyCar_TblID2", DBColumnType.Int));

            Columns.Add(new ColumnMetadata("TireSelect", DBColumnType.Bool));
        }
    }
}
