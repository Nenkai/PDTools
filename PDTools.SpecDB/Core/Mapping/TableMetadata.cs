using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Syroot.BinaryData.Memory;
using Syroot.BinaryData.Core;

using PDTools.SpecDB.Core.Mapping.Types;

namespace PDTools.SpecDB.Core.Mapping;

[DebuggerDisplay("{Columns.Count} Columns")]
public abstract class TableMetadata
{
    public virtual string LabelPrefix { get; } = string.Empty;

    public List<ColumnMetadata> Columns { get; set; } = [];

    private ColumnMetadata _categoryColumn;
    public ColumnMetadata CategoryColumn { get; private set; }

    public (RowData Row, bool ReadAll) ReadRow(Span<byte> rowData, Endian endian)
    {
        var sr = new SpanReader(rowData, endian);
        var row = new RowData();
        foreach (var columnMeta in Columns)
        {
            switch (columnMeta.ColumnType)
            {
                case DBColumnType.Bool:
                    row.ColumnData.Add(new DBBool(sr.ReadBoolean())); break;
                case DBColumnType.Byte:
                    row.ColumnData.Add(new DBByte(sr.ReadByte())); break;
                case DBColumnType.SByte:
                    row.ColumnData.Add(new DBSByte(sr.ReadSByte())); break;
                case DBColumnType.Short:
                    row.ColumnData.Add(new DBShort(sr.ReadInt16())); break;
                case DBColumnType.UShort:
                    row.ColumnData.Add(new DBUShort(sr.ReadUInt16())); break;
                case DBColumnType.Int:
                    row.ColumnData.Add(new DBInt(sr.ReadInt32())); break;
                case DBColumnType.UInt:
                    row.ColumnData.Add(new DBUInt(sr.ReadUInt32())); break;
                case DBColumnType.Long:
                    row.ColumnData.Add(new DBLong(sr.ReadInt64())); break;
                case DBColumnType.Float:
                    row.ColumnData.Add(new DBFloat(sr.ReadSingle())); break;
                case DBColumnType.String:
                    row.ColumnData.Add(new DBString(sr.ReadInt32(), columnMeta.StringFileName)); break;
                default:
                    break;
            }
        }

        return (row, sr.IsEndOfSpan);
    }

    public ColumnMetadata GetCategoryColumn()
    {
        _categoryColumn ??= Columns.Find(c => c.ColumnName.Equals("category", StringComparison.OrdinalIgnoreCase));
        return _categoryColumn;
    }

    public int GetColumnSize()
    {
        int length = 0;
        foreach (var column in Columns)
        {
            switch (column.ColumnType)
            {
                case DBColumnType.Bool:
                case DBColumnType.Byte:
                case DBColumnType.SByte:
                    length++; break;
                case DBColumnType.Short:
                case DBColumnType.UShort:
                    length += 2; break;
                case DBColumnType.Int:
                case DBColumnType.UInt:
                case DBColumnType.Float:
                case DBColumnType.String:
                    length += 4; break;
                case DBColumnType.Long:
                case DBColumnType.Key:
                    length += 8; break;
            }
        }
        return length;
    }
}

public class ColumnMetadata
{
    public string ColumnName { get; set; }
    public DBColumnType ColumnType { get; set; }
    public string StringFileName { get; set; }

    public int ColumnIndex { get; set; }

    public ColumnMetadata(string columnName, DBColumnType columnType)
    {
        ColumnName = columnName;
        ColumnType = columnType;
    }

    public ColumnMetadata(string columnName, DBColumnType columnType, string stringFileName)
    {
        ColumnName = columnName;
        ColumnType = columnType;
        StringFileName = stringFileName;
    }
}

public enum DBColumnType
{
    Bool,
    Byte,
    SByte,
    Short,
    UShort,
    Int,
    UInt,
    String,
    Long,
    Float,

    Key,
}
