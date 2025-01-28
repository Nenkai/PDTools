using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using Syroot.BinaryData;

namespace PDTools.SpecDB.Core.Mapping.Types;

[DebuggerDisplay("Bool - {Value}")]
public class DBBool : IDBType, INotifyPropertyChanged
{
    private bool _value;
    public bool Value
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
        => bs.WriteBoolean(_value);

    public event PropertyChangedEventHandler PropertyChanged;
    public DBBool(bool value)
        => Value = value;

    public override string ToString()
        => _value.ToString();
}
