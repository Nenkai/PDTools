using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;

using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class EntrySet
    {
        public EntryGenerate EntryGenerate { get; set; } = new EntryGenerate();
        public List<Entry> Entries { get; set; } = new List<Entry>();

        public bool IsDefault()
        {
            return EntryGenerate.IsDefault() && Entries.Count == 0;
        }

        public void ParseFromXml(XmlNode node)
        {
            foreach (XmlNode entryNode in node.ChildNodes)
            {
                switch (entryNode.Name)
                {
                    case "entry_generate":
                        EntryGenerate.ParseFromXml(entryNode);
                        break;

                    case "entry":
                        var entry = new Entry();
                        entry.ReadFromXml(entryNode);
                        Entries.Add(entry);
                        break;

                }
            }
        }

        public void WriteToXml(XmlWriter xml)
        {
            if (!EntryGenerate.IsDefault())
            {
                xml.WriteStartElement("entry_generate");
                EntryGenerate.WriteToXml(xml);
                xml.WriteEndElement();
            }

            foreach (var entry in Entries)
            {
                xml.WriteStartElement("entry");
                entry.WriteToXml(xml);
                xml.WriteEndElement();
            }
        }

        public void Deserialize(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5_E5_00_2F && magic != 0xE6_E6_00_2F)
                throw new System.IO.InvalidDataException($"Entry set magic did not match - Got {magic.ToString("X8")}, expected 0xE6E6002F");

            int version = reader.ReadInt32();
            EntryGenerate.Deserialize(ref reader);

            int entries = reader.ReadInt32();
            for (int i = 0; i < entries; i++)
            {
                EntryBase entry = new EntryBase();
                entry.Deserialize(ref reader);
            }
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_00_2F);
            bs.WriteUInt32(1_00);

            EntryGenerate.Serialize(ref bs);

            bs.WriteInt32(Entries.Count);
            foreach (var entry in Entries)
                entry.Serialize(ref bs);
        }
    }
}
