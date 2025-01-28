using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class Steer : TableMetadata
{
    public override string LabelPrefix { get; } = "st_";

    public Steer(SpecDBFolder folderType)
    {
        Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("padgraphx1", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("padgraphx2", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("padgraphx3", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("padgraphx4", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("padgraphx5", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("padgraphx6", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("padgraphy1", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("padgraphy2", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("padgraphy3", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("padgraphy4", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("padgraphy5", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("padgraphy6", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("steergraphx1", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("steergraphx2", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("steergraphx3", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("steergraphx4", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("steergraphx5", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("steergraphx6", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("steergraphy1", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("steergraphy2", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("steergraphy3", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("steergraphy4", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("steergraphy5", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("steergraphy6", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("dirtgraphx1", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dirtgraphx2", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dirtgraphx3", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dirtgraphx4", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dirtgraphx5", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dirtgraphx6", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dirtgraphy1", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dirtgraphy2", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dirtgraphy3", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dirtgraphy4", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dirtgraphy5", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("dirtgraphy6", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("steerlimit", DBColumnType.Byte));

        if (folderType > SpecDBFolder.GT5_TRIAL_JP2704)
        {
            Columns.Add(new ColumnMetadata("steerlimitMin", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("steerlimitMax", DBColumnType.Byte));
        }

        if (folderType >= SpecDBFolder.GT5_JP3009)
            Columns.Add(new ColumnMetadata("steerMaxVisual", DBColumnType.Byte));
    }
}
