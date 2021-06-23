using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;

using SpecDBOld.Mapping.Types;

namespace SpecDBOld.Mapping
{
    [DebuggerDisplay("{ColumnData.Count} columns")]
    public class SpecDBRowData : INotifyPropertyChanged
    {
        private int _id;
        public int ID
        {
            get => _id;
            set
            {
                _id = value;
                NotifyPropertyChanged("ID");
            }
        }
    

        private string _label;
        public string Label
        {
            get => _label;
            set
            {
                _label = value;
                NotifyPropertyChanged("Label");
            }
        }

        public List<IDBType> ColumnData { get; set; } = new List<IDBType>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
