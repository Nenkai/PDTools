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
    [DebuggerDisplay("Float - {Value}")]
    public class DBFloat : IDBType, INotifyPropertyChanged
    {
        private float _value;
        public float Value
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
            => bs.WriteSingle(_value);

        public event PropertyChangedEventHandler PropertyChanged;
        public DBFloat(float value)
            => Value = value;

        public override string ToString()
            => _value.ToString();
    }
}
