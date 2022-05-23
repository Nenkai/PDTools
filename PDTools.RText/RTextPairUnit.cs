using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.RText
{
    public class RTextPairUnit
    {
        public int ID { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }

        public RTextPairUnit(int id, string label, string value)
        {
            ID = id;
            Label = label;
            Value = value;
        }
    }
}
