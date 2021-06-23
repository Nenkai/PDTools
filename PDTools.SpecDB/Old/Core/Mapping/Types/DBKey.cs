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
    [DebuggerDisplay("Key - {Value}")]
    public class DBKey : IDBType, INotifyPropertyChanged
    {
        private uint _id;
        public uint _tableIndex;

        public uint Value
        {
            get => _id;
            set
            {
                _id = value;
                NotifyPropertyChanged("Value");
            }
        }

        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Serialize(BinaryStream bs)
        {
            if (bs.ByteConverter == Syroot.BinaryData.ByteConverter.Big)
            {
                bs.WriteUInt32(_id);
                bs.WriteUInt32(_tableIndex);
            }
            else
            {
                bs.WriteUInt32(_tableIndex);
                bs.WriteUInt32(_id);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public DBKey(uint tableIndex, uint id)
        {
            _tableIndex = tableIndex;
            _id = id;
        }

        public override string ToString()
            => _id.ToString();
    }
}
