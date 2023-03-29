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
    [DebuggerDisplay("SByte - {Value}")]
    public class DBSByte : IDBType, INotifyPropertyChanged
    {
        private sbyte _value;
        public sbyte Value
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
            => bs.WriteSByte(_value);

        public event PropertyChangedEventHandler PropertyChanged;
        public DBSByte(sbyte value)
            => Value = value;

        public override string ToString()
            => _value.ToString();
    }
}
