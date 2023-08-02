using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;
using PDTools.Enums;

namespace PDTools.Structures.MGameParameter
{
    public class LocalizeText
    {
        public Dictionary<Language, string> Texts { get; set; } = new Dictionary<Language, string>();

        public void SetText(Language locale, string title)
        {
            Texts[locale] = title;
        }

        public bool IsDefault()
        {
            return Texts.Count == 0;
        }

        public void WriteToXml(XmlWriter xml)
        {
            foreach (var text in Texts)
                xml.WriteElementString(text.Key.ToString(), text.Value);
        }

        public void ReadFromXml(XmlNode node)
        {
            foreach (XmlNode textNode in node.ChildNodes)
            {
                if (Enum.TryParse(textNode.Name, out Language language))
                {
                    if (!Texts.ContainsKey(language))
                        Texts.Add(language, textNode.InnerText);
                }
            }
        }

        public void Serialize(ref BitStream bs)
        {
            // Game will always use Language.MAX for writing - verified in decompilation (GT6 EU DISC 1.22 0x334074)
            bs.WriteUInt32(1_00);
            bs.WriteByte((byte)Language.MAX);
            bs.WriteSByte(-1); // Unk

            for (int i = 0; i < (int)Language.MAX; i++)
            {
                if (i < Texts.Count)
                    bs.WriteNullStringAligned4(Texts.ElementAt(i).Value);
                else
                    bs.WriteNullStringAligned4(string.Empty);
            }
        }
    }
}
