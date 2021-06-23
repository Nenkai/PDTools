using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class EventInformation
    {
        public bool NeedsPopulating { get; set; } = true;

        public Dictionary<string, string> Titles { get; set; }
        public Dictionary<string, string> OneLineTitles { get; set; }
        public Dictionary<string, string> Descriptions { get; set; }

        public static readonly List<string> LocaleCodes = new List<string>()
        {
            "JP","US","GB","FR","ES","DE","IT","NL","PT","RU","PL",
            "TR","KR","EL","TW","CN","DK","NO","SE","FI","CZ","HU","BP","MS",
        };

        public static readonly Dictionary<string, string> Locales = new Dictionary<string, string>()
        {
            { "JP", "Japanese" },
            { "US", "American" },
            { "GB", "British (Primary)" },
            { "FR", "French" },
            { "ES", "Spanish" },
            { "DE", "German" },
            { "IT", "Italian" },
            { "NL", "Dutch" },
            { "PT", "Portuguese" },
            { "RU", "Russian" },
            { "PL", "Polish" },
            { "TR", "Turkish" },
            { "KR", "Korean" },
            { "EL", "Greek" },

            { "TW", "Chinese (Taiwan) - Unused" }, // Non included
            { "CN", "Chinese - Unused" }, // Non included
            { "DK", "Danish - Unused" }, // Non included 
            { "NO", "Norwegian - Unused" }, // Non included
            { "SE", "Swedish - Unused" }, // Non included

            { "FI", "Finish" },
            { "CZ", "Czech" },
            { "HU", "Magyar (Hungary)" },
            { "BP", "Portuguese (Brazillian)" },
            { "MS", "Spanish (Mexican)" },
        };

        public EventInformation()
        {
            Titles = InitializeLocaleStrings();
            OneLineTitles = InitializeLocaleStrings();
            Descriptions = InitializeLocaleStrings();
        }

        public void SetTitle(string locale, string title)
        {
            Titles[locale] = title;
        }

        public void SetOneLineTitle(string locale, string title)
        {
            OneLineTitles[locale] = title;
        }

        public void SetDescription(string locale, string description)
        {
            Descriptions[locale] = description;
        }

        private Dictionary<string, string> InitializeLocaleStrings()
        {
            var localizedStrings = new Dictionary<string, string>();
            foreach (var locale in Locales)
                localizedStrings.Add(locale.Key, string.Empty);
            return localizedStrings;
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("information");
            {
                xml.WriteEmptyElement("advanced_notice");
                xml.WriteElementValue("flier_other_info", "");
                xml.WriteElementValue("logo_other_info", "");
                xml.WriteElementValue("race_info_minute", "");

                xml.WriteStartElement("title");
                WriteLocales(Titles);
                xml.WriteEndElement();

                xml.WriteStartElement("one_line_title");
                WriteLocales(OneLineTitles);
                xml.WriteEndElement();

                xml.WriteStartElement("description");
                WriteLocales(Descriptions);
                xml.WriteEndElement();
            } 
            xml.WriteEndElement();

            void WriteLocales(Dictionary<string, string> locales)
            {
                foreach (var title in locales)
                {
                    if (title.Key == "TW" || title.Key == "CN" || title.Key == "DK" || title.Key == "NO" || title.Key == "SE")
                        continue;

                    xml.WriteElementString(title.Key, title.Value);
                }
            }
        }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_9F_40);
            bs.WriteUInt32(1_02); // Version

            WriteLocaleListToCache(ref bs, Titles);
            WriteLocaleListToCache(ref bs, OneLineTitles);
            WriteLocaleListToCache(ref bs, Descriptions);
            WriteLocaleListToCache(ref bs, null); // advanced_notice
            WriteLocaleListToCache(ref bs, null); // registration_notice

            bs.WriteInt16(0); // narration_id
            bs.WriteInt16(0); // race_info_minute

            bs.WriteNullStringAligned4(string.Empty); // logo_image_path
            bs.WriteByte(0); // logo_image_layout
            bs.WriteInt32(0); // logo_image_buffer size
            bs.WriteNullStringAligned4(string.Empty); // logo_other_info

            bs.WriteNullStringAligned4(string.Empty); // flier_image_path
            bs.WriteInt32(0); // flier_image_buffer
            bs.WriteNullStringAligned4(string.Empty); // flier_other_info

            bs.WriteNullStringAligned4(string.Empty); // race_label
        }

        private void WriteLocaleListToCache(ref BitStream bs, Dictionary<string, string> list)
        {
            bs.WriteUInt32(1_00);
            bs.WriteByte(24); // Count
            bs.WriteSByte(-1); // Unk

            if (list is null)
            {
                for (int i = 0; i < 24; i++)
                    bs.WriteNullStringAligned4(string.Empty);
            }
            else
            {
                for (int i = 0; i < 24; i++)
                {
                    if (i >= 15 && i <= 19) // Ignored locales
                    {
                        bs.WriteInt32(0);
                        continue;
                    }

                    bs.WriteNullStringAligned4(list.ElementAt(i).Value);
                }
            }
        }

        public void ParseRaceInformation(Event evnt, XmlNode node)
        {
            foreach (XmlNode informationNode in node.ChildNodes)
            {
                switch (informationNode.Name)
                {
                    case "title":
                        foreach (XmlNode titleNode in informationNode.ChildNodes)
                        {
                            if (Titles.TryGetValue(titleNode.Name, out _))
                            {
                                if (titleNode.Name == "GB")
                                    evnt.Name = titleNode.InnerText;
                                Titles[titleNode.Name] = titleNode.InnerText;
                            } 
                        }
                        break;

                    case "description":
                        foreach (XmlNode descNode in informationNode.ChildNodes)
                        {
                            if (Descriptions.TryGetValue(descNode.Name, out _))
                                Descriptions[descNode.Name] = descNode.InnerText;
                        }
                        break;
                    case "one_line_title":
                        foreach (XmlNode descNode in informationNode.ChildNodes)
                        {
                            if (Descriptions.TryGetValue(descNode.Name, out _))
                                OneLineTitles[descNode.Name] = descNode.InnerText;
                        }
                        break;
                }
            }
        }
    }
}
