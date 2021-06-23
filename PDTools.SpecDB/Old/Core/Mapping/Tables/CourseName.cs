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
    public class CourseName : TableMetadata
    {
        public CourseName(SpecDBFolder folderType, string locale)
        {
            Columns.Add(new ColumnMetadata("Name", DBColumnType.String, locale));
        }
    }
}
