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
    [DebuggerDisplay("Bool - {Value}")]
    public class DBLong : IDBType, INotifyPropertyChanged
    {
        private long _value;
        public long Value
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
            => bs.WriteInt64(_value);

        public event PropertyChangedEventHandler PropertyChanged;
        public DBLong(long value)
            => Value = value;

        public override string ToString()
            => _value.ToString();
    }
}
