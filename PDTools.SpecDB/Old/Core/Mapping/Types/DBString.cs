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
    [DebuggerDisplay("String - '{Value}' ({StringIndex}, {FileName})")]
    public class DBString : IDBType, INotifyPropertyChanged
    {
        public int StringIndex { get; set; }
        public string FileName { get; set; }

        private string _value;
        public string Value
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void Serialize(BinaryStream bs)
            => bs.WriteInt32(StringIndex);

        public DBString(int stringIndex, string fileName)
        {
            StringIndex = stringIndex;
            FileName = fileName;
        }

        public override string ToString()
            => _value.ToString();

    }
}
