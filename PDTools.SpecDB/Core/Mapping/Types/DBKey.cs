using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using Syroot.BinaryData;
using System.IO;

namespace PDTools.SpecDB.Core.Mapping.Types;

[DebuggerDisplay("Key - {Value}")]
public class DBKey : IDBType, INotifyPropertyChanged
{
    public int _id;
    public int _tableIndex;

    public string Value
    {
        get => ToString();
        set
        {
            ParseKeyFromString(value);
            NotifyPropertyChanged(nameof(Value));
        }
    }

    private void NotifyPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public DBKey(string keyStr)
    {
        ParseKeyFromString(keyStr);
    }

    public DBKey(int tableIndex, int id)
    {
        _tableIndex = tableIndex;
        _id = id;
    }

    public DBKey(DBKey key)
    {
        _tableIndex = key._tableIndex;
        _id = key._id;
    }

    private void ParseKeyFromString(string value)
    {
        string[] spl = value.Split(":");
        if (spl.Length < 2)
            throw new ArgumentException("Invalid db key string");

        string tableId = spl[0];
        string id = spl[1];
        if (spl[0].StartsWith("Table"))
        {
            tableId = spl[0]["Table".Length..];
        }

        _tableIndex = int.Parse(tableId);
        _id = int.Parse(id);
    }

    public void Serialize(BinaryStream bs)
    {
        if (bs.ByteConverter == Syroot.BinaryData.ByteConverter.Big)
        {
            bs.WriteInt32(_tableIndex);
            bs.WriteInt32(_id);
        }
        else
        {
            bs.WriteInt32(_id);
            bs.WriteInt32(_tableIndex);
        }
    }

    public override string ToString()
        => $"Table{_tableIndex}:{_id}";
}
