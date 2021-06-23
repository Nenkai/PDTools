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
    [DebuggerDisplay("UInt - {Value}")]
    public class DBUInt : IDBType, INotifyPropertyChanged
    {
        private uint _value;
        public uint Value
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
            => bs.WriteUInt32(Value);

        public event PropertyChangedEventHandler PropertyChanged;
        public DBUInt(uint value)
            => Value = value;

        public override string ToString()
            => _value.ToString();
    }
}
