using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using Syroot.BinaryData;

namespace PDTools.SpecDB.Core.Mapping.Types
{
    [DebuggerDisplay("Byte - {Value}")]
    public class DBByte : IDBType, INotifyPropertyChanged
    {
        private byte _value;
        public byte Value
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
            => bs.WriteByte(_value);

        public event PropertyChangedEventHandler PropertyChanged;
        public DBByte(byte value)
            => Value = value;

        public override string ToString()
            => _value.ToString();
    }
}
