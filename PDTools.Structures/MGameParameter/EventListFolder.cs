using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

using PDTools.Utils;
using PDTools.Enums;
using PDTools.Enums.PS3;

namespace PDTools.Structures.MGameParameter
{
    // This is parsed by EventRace2.decodeOfflineEventList
    public class EventListFolder
    {
        public Dictionary<Country, string> Title { get; set; } = new Dictionary<Country, string>();
        public Dictionary<Country, string> Description { get; set; } = new Dictionary<Country, string>();
        public Dictionary<Country, string> OneLineTitle { get; set; } = new Dictionary<Country, string>();

        /// <summary>
        /// Folder ID.
        /// </summary>
        public int Id { get; set; }

        public int Voucher { get; set; }
        public int Registration { get; set; }
        public string BgImage { get; set; }
        public string IconImage { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime NewDate { get; set; }
        public int Type { get; set; }
        public GameItemType GameItemType { get; set; }
        public GameItemCategory GameItemCategory { get; set; }
        public int GameItemID { get; set; }
        public string GameItemValue { get; set; }
        public bool DLCFlag { get; set; }
        public List<ulong> EventIDList { get; set; } = new List<ulong>();

        // Anything else is NOT parsed by the game, GT5 or GT6

        public EventListFolder()
        {
            foreach (Country country in Enum.GetValues(typeof(Country)))
            {
                if (country == Country.NumOfCountries)
                    break;

                Title.Add(country, string.Empty);
                Description.Add(country, string.Empty);
            }
        }

        public void WriteToXML(XmlWriter xml)
        {
            xml.WriteStartElement("event_list");
            {
                xml.WriteStartElement("event");
                {
                    xml.WriteStartElement("title");
                    foreach (var lang in Title)
                    {
                        xml.WriteStartElement(lang.Key.ToString());
                        xml.WriteString(lang.Value);
                        xml.WriteEndElement();
                    }
                    xml.WriteEndElement();

                    xml.WriteStartElement("description");
                    foreach (var lang in Description)
                    {
                        xml.WriteStartElement(lang.Key.ToString());
                        xml.WriteString(lang.Value);
                        xml.WriteEndElement();
                    }
                    xml.WriteEndElement();

                    xml.WriteStartElement("copy");
                    foreach (var lang in OneLineTitle)
                    {
                        xml.WriteStartElement(lang.Key.ToString());
                        xml.WriteString(lang.Value);
                        xml.WriteEndElement();
                    }
                    xml.WriteEndElement();

                    //xml.WriteEmptyElement("ranking_list");
                    xml.WriteStartElement("id"); xml.WriteString(Id.ToString()); xml.WriteEndElement();
                    xml.WriteStartElement("voucher"); xml.WriteString(Voucher.ToString()); xml.WriteEndElement();
                    xml.WriteStartElement("registration"); xml.WriteString(Registration.ToString()); xml.WriteEndElement();

                    xml.WriteStartElement("bg_image");
                    if (!string.IsNullOrEmpty(BgImage))
                        xml.WriteString(BgImage.ToString());
                    xml.WriteEndElement();

                    xml.WriteStartElement("icon_image");
                    if (!string.IsNullOrEmpty(IconImage))
                        xml.WriteString(IconImage.ToString());
                    xml.WriteEndElement();

                    xml.WriteStartElement("event_type"); xml.WriteString(Type.ToString()); xml.WriteEndElement();
                    xml.WriteStartElement("gameitem_type"); xml.WriteString(((int)GameItemType).ToString()); xml.WriteEndElement();
                    xml.WriteStartElement("gameitem_category"); xml.WriteString(((int)GameItemCategory).ToString()); xml.WriteEndElement();
                    xml.WriteStartElement("gameitem_id"); xml.WriteString(GameItemID.ToString()); xml.WriteEndElement();
                    xml.WriteStartElement("gameitem_value"); xml.WriteString(GameItemValue); xml.WriteEndElement();
                    xml.WriteStartElement("dlc_flag"); xml.WriteString((DLCFlag ? 1 : 0).ToString()); xml.WriteEndElement();

                    xml.WriteStartElement("event_id_list");
                    xml.WriteString(string.Join(",", EventIDList));
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }

        public void ParseEventText(XmlNode parentNode)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "arcade":
                    case "championship":
                    case "editor_info":
                    case "events":
                        throw new InvalidDataException("Could not load folder info - this is an event list, not a folder xml, import it using \"Import Event List (GT5)\"");

                    case "title":
                        foreach (XmlNode titleNode in node.ChildNodes)
                        {
                            if (Enum.TryParse(titleNode.Name, out Country country))
                                Title[country] = titleNode.InnerText;
                        }
                        break;

                    case "description":
                        foreach(XmlNode descNode in node.ChildNodes)
                        {
                            if (Enum.TryParse(descNode.Name, out Country country))
                                Description[country] = descNode.InnerText;
                        }
                        break;

                    case "copy":
                        foreach (XmlNode oneLineTitleNode in node.ChildNodes)
                        {
                            if (Enum.TryParse(oneLineTitleNode.Name, out Country country))
                                OneLineTitle[country] = oneLineTitleNode.InnerText;
                        }
                        break;

                    case "id":
                        if (int.TryParse(node.InnerText, out int id))
                            Id = id;
                        break;

                    case "event_type":
                        if (int.TryParse(node.InnerText, out int event_type))
                            Type = event_type;
                        break;

                    case "gameitem_type":
                        if (int.TryParse(node.InnerText, out int gameitem_type))
                            GameItemType = (GameItemType)gameitem_type;
                        break;

                    case "gameitem_category":
                        if (int.TryParse(node.InnerText, out int gameitem_category))
                            GameItemCategory = (GameItemCategory)gameitem_category;
                        break;

                    case "gameitem_id":
                        if (int.TryParse(node.InnerText, out int gameitem_id))
                            GameItemID = gameitem_id;
                        break;

                    case "gameitem_value":
                        GameItemValue = node.InnerText;
                        break;

                    case "dlc_flag":
                        if (int.TryParse(node.InnerText, out int dlc_flag))
                            DLCFlag = dlc_flag == 1;
                        break;

                    case "voucher":
                        if (int.TryParse(node.InnerText, out int voucher))
                            Voucher = voucher;
                        break;

                    case "registration":
                        if (int.TryParse(node.InnerText, out int registration))
                            Registration = registration;
                        break;

                    case "bg_image":
                        BgImage = node.InnerText;
                        break;

                    case "icon_image":
                        IconImage = node.InnerText;
                        break;

                    case "event_id_list":
                        foreach (string idStr in node.InnerText.Split(','))
                        {
                            if (ulong.TryParse(idStr, out ulong eventId))
                                EventIDList.Add(eventId);
                        }
                        break;
                }
            }
        }
    }
}
