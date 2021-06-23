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
    public class DefaultParam : TableMetadata
    {
        public override string LabelPrefix { get; } = "dp_";

        public DefaultParam(SpecDBFolder folderType)
        {
            Columns.Add(new ColumnMetadata("torquevol", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("finalgearDF", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("rideheightDFF", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("rideheightDFR", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("springrateDFF", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("springrateDFR", DBColumnType.Short));
            Columns.Add(new ColumnMetadata("cIDFF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("cIDFR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("LSDparamFDF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("LSDparam2FDF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("LSDparam3FDF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("LSDparamRDF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("LSDparam2RDF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("LSDparam3RDF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("maxspeedDF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("camberDFF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("camberDFR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("toeDFF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("toeDFR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("dampF1BDFF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("dampF2BDFF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("dampF1RDFF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("dampF2RDFF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("dampF1BDFR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("dampF2BDFR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("dampF1RDFR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("dampF2RDFR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("stabilizerDFF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("stabilizerDFR", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("VSCDF", DBColumnType.Byte));
            Columns.Add(new ColumnMetadata("TCSDF", DBColumnType.Byte));
        }
    }
}
