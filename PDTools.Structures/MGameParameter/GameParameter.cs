using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.IO;

using Syroot.BinaryData;

using PDTools.Utils;
using System.Text;

namespace PDTools.Structures.MGameParameter
{
    public class GameParameter
    {
        public ulong FolderId { get; set; }
        public List<Event> Events { get; set; } = new List<Event>();
        public OnlineRoomParameter OnlineRoom { get; set; } = new OnlineRoomParameter();
        public Reward SeriesRewards { get; set; } = new Reward();
        public Information SeriesInformation { get; set; } = new Information();
        public EditorInfo EditorInfo { get; set; } = new EditorInfo();
        public bool Championship { get; set; }
        public bool Arcade { get; set; }

        public const int XMLVersionLatest = 106;

        /// <summary>
        /// Writes the structure to XML. Note: Unsorted XML nodes.
        /// </summary>
        /// <param name="xml"></param>
        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("GameParameter"); xml.WriteAttributeString("version", XMLVersionLatest.ToString());
            xml.WriteElementULong("folder_id", FolderId);

            xml.WriteStartElement("events");
            foreach (var evnt in Events)
                evnt.WriteToXml(xml);
            xml.WriteEndElement();

            // online_room

            if (!SeriesRewards.IsDefault())
            {
                xml.WriteStartElement("series_reward");
                SeriesRewards.WriteToXml(xml);
                xml.WriteEndElement();
            }

            if (!SeriesInformation.IsDefault())
            {
                xml.WriteStartElement("series_information");
                SeriesInformation.WriteToXml(xml);
                xml.WriteEndElement();
            }

            xml.WriteElementBool("championship", Championship);
            xml.WriteElementBool("arcade", Arcade);

            xml.WriteEndElement();
        }

        /// <summary>
        /// Writes to Xml. Nodes will be sorted alphabetically.
        /// </summary>
        /// <param name="writer"></param>
        public void WriteToXmlSorted(XmlWriter writer)
        {
            using (var ms = new MemoryStream())
            {
                using (var tempWriter = XmlWriter.Create(ms))
                {
                    WriteToXml(tempWriter);
                }

                ms.Position = 0;
                XmlDocument sortedDoc = new XmlDocument();
                sortedDoc.Load(ms);

                SortXml(sortedDoc["GameParameter"]);
                sortedDoc["GameParameter"].WriteTo(writer);
            }
        }

        private static void SortXml(XmlNode node)
        {
            // Load the child elements into a collection.
            List<XmlNode> childElements = new List<XmlNode>();

            foreach (XmlNode childNode in node.ChildNodes)
            {
                childElements.Add(childNode);

                // Call recursively if the child is not a leaf.
                if (childNode.HasChildNodes)
                {
                    SortXml(childNode);
                }
            }

            node.RemoveAll();

            // Re-add the child elements (sorted).
            foreach (var childNode in childElements.OrderBy(element => element.Name))
            {
                // Make sure folder_id isn't at the very bottom
                if (childNode.Name == "folder_id")
                {
                    node.InsertBefore(childNode, childElements[1]);
                    continue;
                }

                node.AppendChild(childNode);
            }
        }

        public void ParseFromXmlDocument(XmlDocument doc)
        {
            var nodes = doc["xml"]["GameParameter"];
            foreach (XmlNode childNode in nodes)
            {
                switch (childNode.Name)
                {
                    case "folder_id":
                        FolderId = childNode.ReadValueULong(); break;

                    case "events":
                        foreach (XmlNode eventNode in childNode.SelectNodes("event"))
                        {
                            var newEvent = new Event();
                            newEvent.ParseFromXml(eventNode);
                            newEvent.FixInvalidNodesIfNeeded();
                            Events.Add(newEvent);
                        }
                        break;

                    case "online_room":
                        throw new NotImplementedException("Implement online_room parsing!");
                    case "series_reward":
                        SeriesRewards.ParseFromXml(childNode); break;
                    case "series_information":
                        SeriesInformation.ParseFromXml(childNode); break;
                    case "editor_info":
                        EditorInfo.ParseFromXml(childNode); break;
                    case "championship":
                        Championship = childNode.ReadValueBool(); break;
                    case "arcade":
                        Arcade = childNode.ReadValueBool(); break;
                }
            }
        }

        public void ReadFromBuffer(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5E54F17 && magic != 0xE6E64F17)
                ;

            int version = reader.ReadInt32();
            FolderId = reader.ReadUInt64();
            int event_index = reader.ReadInt32();
            int event_count = reader.ReadInt32();

            Events = new List<Event>(event_count);
            for (int i = 0; i < event_count; i++)
            {
                Event evnt = new Event();
                evnt.ReadFromCache(ref reader);
                Events.Add(evnt);
            }
        }

        public void ReadFromBuffer(Span<byte> buffer)
        {
            var reader = new BitStream(BitStreamMode.Read, buffer);
            ReadFromBuffer(ref reader);
        }

        public byte[] Serialize()
        {
            BitStream bs = new BitStream(BitStreamMode.Write, 1024);
            bs.WriteUInt32(0xE6_E6_4F_17);
            bs.WriteInt32(1_01); // Version
            bs.WriteUInt64(FolderId);
            bs.WriteInt32(0); // Event Index;

            bs.WriteInt32(Events.Count);
            foreach (var evnt in Events)
                evnt.WriteToCache(ref bs);

            OnlineRoom.WriteToCache(ref bs);
            SeriesRewards.Serialize(ref bs);
            SeriesInformation.Serialize(ref bs);
            EditorInfo.Serialize(ref bs);
            bs.WriteBool(Championship);
            bs.WriteBool(Arcade); // arcade
            bs.WriteBool(false); // keep_sequence
            bs.WriteByte((byte)LaunchContext.NONE); // launch_context

            bs.WriteUInt32(0xE6_E6_4F_18); // End Magic Terminator

            return bs.GetSpan().ToArray();
        }

        public byte[] SerializeToSTStruct()
        {
            // We could use a PD struct utility for all this, but eh, effort, its just two fields
            using (var ms = new MemoryStream())
            using (var bs = new BinaryStream(ms, ByteConverter.Big))
            {
                bs.WriteByte(0x0E); // PD Struct version
                bs.Position += 4; // Write symbols later
                bs.WriteByte(0x0A);
                bs.WriteByte(9);
                bs.WriteInt32(2);
                bs.WriteByte(7);
                bs.WriteInt16(6);

                bs.Position += 4; // Write GP Size later

                var gp = Serialize();

                bs.WriteBytes(gp);
                bs.WriteByte(7);
                bs.WriteByte(1);
                bs.WriteByte(3);
                bs.WriteInt32(101); // GP VERSION

                long symbolsOffset = bs.Position;
                bs.WriteByte(2);
                bs.WriteByte(4); bs.WriteString("data", StringCoding.Raw);
                bs.WriteByte(7); bs.WriteString("version", StringCoding.Raw);

                bs.Position = 1;
                bs.WriteUInt32((uint)symbolsOffset);

                bs.Position = 14;
                bs.WriteInt32(gp.Length);

                return ms.ToArray();
            }
        }

        public enum LaunchContext
        {
            NONE,
            ACACDEMY,
        }
    }
}
