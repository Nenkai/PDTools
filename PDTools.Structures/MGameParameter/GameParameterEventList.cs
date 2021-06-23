using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class GameParameterEventList
    {
        public string Title { get; set; } = "Folder Title";
        public string Description { get; set; } = "Folder Description";
        public int StarsNeeded { get; set; }
        public bool IsChampionship { get; set; }

        public List<string> eventIds = new List<string>();
        public EventCategory Category { get; set; }

        // TODO: Move these globals somewhere else
        public static List<string> LocaliseLanguages { get; set; } = new List<string>();
        public static List<EventCategory> EventCategories = new List<EventCategory>();

        public GameParameterEventList()
        {
            Category = new EventCategory("", 1000);
        }

        public void WriteToXML(GameParameter parent, string dir, int eventRaceIDStart, string eventType, bool minify)
        {
            using (var xml = XmlWriter.Create(Path.Combine(dir, $"{parent.FolderFileName}.xml"), new XmlWriterSettings() { Indent = !minify }))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("event_list");
                xml.WriteStartElement("event");

                xml.WriteStartElement("title");
                foreach (string lang in LocaliseLanguages)
                {
                    xml.WriteStartElement(lang);
                    xml.WriteString(Title);
                    xml.WriteEndElement();
                }
                    
                xml.WriteEndElement();

                xml.WriteStartElement("description");
                foreach (string lang in LocaliseLanguages)
                {
                    xml.WriteStartElement(lang);
                    xml.WriteString(Description);
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();

                xml.WriteStartElement("copy");
                foreach (string lang in LocaliseLanguages)
                    xml.WriteEmptyElement(lang);

                xml.WriteEndElement();

                xml.WriteEmptyElement("ranking_list");
                xml.WriteStartElement("id"); xml.WriteString(parent.FolderId.ToString()); xml.WriteEndElement();
                xml.WriteEmptyElement("voucher");
                xml.WriteStartElement("registration"); xml.WriteString(0.ToString()); xml.WriteEndElement();
                xml.WriteEmptyElement("bg_image");
                xml.WriteEmptyElement("icon_image");
                xml.WriteEmptyElement("folder_image");
                xml.WriteStartElement("event_type"); xml.WriteString( EventCategories.Find(x => x.name == eventType ).typeID.ToString() ); xml.WriteEndElement();
                xml.WriteStartElement("gameitem_type"); xml.WriteString(0.ToString()); xml.WriteEndElement();
                xml.WriteStartElement("gameitem_category"); xml.WriteString(0.ToString()); xml.WriteEndElement();
                xml.WriteStartElement("gameitem_id"); xml.WriteString(0.ToString()); xml.WriteEndElement();
                xml.WriteEmptyElement("gameitem_value");
                xml.WriteStartElement("dlc_flag"); xml.WriteString(0.ToString()); xml.WriteEndElement();
                xml.WriteStartElement("star"); xml.WriteString(parent.Events.Sum(e => e.Rewards.Stars).ToString()); xml.WriteEndElement();
                xml.WriteStartElement("need_star"); xml.WriteString(StarsNeeded.ToString()); xml.WriteEndElement();
                xml.WriteStartElement("championship_value"); xml.WriteString(IsChampionship ? "1" : "0"); xml.WriteEndElement();
                xml.WriteEmptyElement("need_folder_id");

                parent.OrderEventIDs();
                xml.WriteStartElement("event_id_list");
                xml.WriteString(string.Join(",", parent.Events.Select(e => e.EventID)));
                xml.WriteEndElement();

                xml.WriteStartElement("argument1"); xml.WriteString((-1).ToString()); xml.WriteEndElement();
                xml.WriteEmptyElement("argument2");
                xml.WriteStartElement("argument3"); xml.WriteString(0.ToString()); xml.WriteEndElement();
                xml.WriteEmptyElement("argument4");

                xml.WriteEndElement();
                xml.WriteEndElement();
            }
        }

        public void ParseEventList(GameParameter parent, XmlDocument doc)
        {
            ParseEventText(doc);
            ParseEventData(parent, doc);
        }

        private void ParseEventText(XmlDocument doc)
        {
            foreach (XmlNode node in doc.DocumentElement.ChildNodes[0])
            {
                switch (node.Name)
                {
                    case "arcade":
                    case "championship":
                    case "editor_info":
                    case "events":
                        throw new InvalidDataException("Could not load folder info - this is an event list, not a folder xml, import it using \"Import Event List (GT5)\"");

                    // Use GB only without localisation for the time being
                    case "title":
                        foreach (XmlNode titleNode in node.ChildNodes)
                        {
                            if (titleNode.Name == "GB")
                                Title = titleNode.InnerText;
                        }
                        break;

                    case "description":
                        foreach (XmlNode descNode in node.ChildNodes)
                        {
                            if (descNode.Name == "GB")
                                Description = descNode.InnerText;
                        }
                        break;
                }
            }
        }

        private void ParseEventData(GameParameter parent, XmlDocument doc)
        {
            foreach (XmlNode node in doc.DocumentElement.ChildNodes[0])
            {
                switch (node.Name)
                {
                    case "id":
                        parent.FolderId = ulong.Parse(node.InnerText);
                        break;

                    case "event_type":
                        Category = new EventCategory("", int.Parse(node.InnerText));
                        break;

                    case "star":
                        //Stars = int.Parse(node.InnerText); // We don't need it
                        break;

                    case "need_star":
                        StarsNeeded = int.Parse(node.InnerText);
                        break;

                    case "championship_value":
                        IsChampionship = node.InnerText == "1";
                        break;

                    case "event_id_list":
                        foreach (string id in node.InnerText.Split(','))
                            eventIds.Add(id);
                        break;
                }
            }

        }
    }
}
