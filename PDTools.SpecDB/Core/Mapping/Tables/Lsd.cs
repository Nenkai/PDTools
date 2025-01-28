using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Mapping.Tables;

public class Lsd : TableMetadata
{
    public override string LabelPrefix { get; } = "ls_";

    public Lsd(SpecDBFolder folderType)
    {
        Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
        Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("difftypeF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparamFMIN", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparamFMAX", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparamFDF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam2F", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam2FMIN", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam2FMAX", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam2FDF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam3F", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam3FMIN", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam3FMAX", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam3FDF", DBColumnType.Byte));

        Columns.Add(new ColumnMetadata("difftypeR", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparamRMIN", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparamRMAX", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparamRDF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam2R", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam2RMIN", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam2RMAX", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam2RDF", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam3F", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam3RMIN", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam3RMAX", DBColumnType.Byte));
        Columns.Add(new ColumnMetadata("LSDparam3RDF", DBColumnType.Byte));


    }
}
