using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;

using Syroot.BinaryData;

namespace PDTools.SpecDB.Core.Mapping.Types;

[DebuggerDisplay("Short - {Value}")]
public class DBShort : IDBType, INotifyPropertyChanged
{
    private short _value;
    public short Value
    {
        get => _value;
        set
        {
            _value = value;
            NotifyPropertyChanged(nameof(Value));
        }
    }

    private void NotifyPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Serialize(BinaryStream bs)
        => bs.WriteInt16(Value);

    public event PropertyChangedEventHandler PropertyChanged;
    public DBShort(short value)
        => Value = value;

    public override string ToString()
        => _value.ToString();
}
