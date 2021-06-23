using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;

using Syroot.BinaryData;
namespace SpecDBOld.Mapping.Types
{
    [DebuggerDisplay("UShort - {Value}")]
    public class DBUShort : IDBType, INotifyPropertyChanged
    {
        private ushort _value;
        public ushort Value
        {
            get => _value;
            set
            {
                _value = value;
                NotifyPropertyChanged("Value");
            }
        }

        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Serialize(BinaryStream bs)
            => bs.WriteUInt16(Value);

        public event PropertyChangedEventHandler PropertyChanged;
        public DBUShort(ushort value)
            => Value = value;

        public override string ToString()
            => _value.ToString();
    }
}
