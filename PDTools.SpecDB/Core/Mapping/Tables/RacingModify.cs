using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class RacingModify : TableMetadata
{
    public override string LabelPrefix { get; } = "rm_";

    public RacingModify(SpecDBFolder folderType)
    {
        if (folderType >= SpecDBFolder.GT5_JP3009)
        {
            Columns.Add(new ColumnMetadata("HasRM", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("NewCarCode", DBColumnType.Int));
            Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
        }
        else
        {
            // Chassis data, GT5 retail moved them to CHASSIS
            Columns.Add(new ColumnMetadata("?", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("chassisTreadF", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("chassisThreadR", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("chassisWidth", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("chassisDWidth", DBColumnType.Short));

            Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));

            Columns.Add(new ColumnMetadata("cd", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("clMINF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("clMAXF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("clDFF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("clMINR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("clMAXR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("clDFR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("wheeloffsetF", DBColumnType.SByte));
            Columns.Add(new ColumnMetadata("wheeloffsetR", DBColumnType.SByte));
            Columns.Add(new ColumnMetadata("susarmF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("susarmR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("?", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("HasActiveWing", DBColumnType.Bool));
            Columns.Add(new ColumnMetadata("ActiveWingType", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("ActiveWingVelocity1", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("ActiveWingVelocity2", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("ActiveWingMovingSpeed", DBColumnType.Byte));
        }
    }
}
