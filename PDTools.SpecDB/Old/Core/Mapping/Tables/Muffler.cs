using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

using SpecDBOld.Core;
namespace SpecDBOld.Mapping.Tables
{
    public class Muffler : TableMetadata
    {
        public override string LabelPrefix { get; } = "mu_";

        public Muffler(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("torquemodifier", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("torquemodifier2", DBColumnType.Short));

            if (folderType >= SpecDBFolder.GT5_JP3009)
                Columns.Add(new ColumnMetadata("torquemodifier3", DBColumnType.Short));

            Columns.Add(new ColumnMetadata("Price", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("category", DBColumnType.Byte));

            if (folderType >= SpecDBFolder.GT5_JP3009)
            {
                Columns.Add(new ColumnMetadata("shiftlimit", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("revlimit", DBColumnType.Byte));
                Columns.Add(new ColumnMetadata("rpmeffect", DBColumnType.Byte));
            }
        }
    }
}
