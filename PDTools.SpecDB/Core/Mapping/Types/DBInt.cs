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
    [DebuggerDisplay("Int - {Value}")]
    public class DBInt : IDBType, INotifyPropertyChanged
    {
        private int _value;
        public int Value
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
            => bs.WriteInt32(Value);

        public event PropertyChangedEventHandler PropertyChanged;
        public DBInt(int value)
            => Value = value;

        public override string ToString()
            => _value.ToString();
    }
}
