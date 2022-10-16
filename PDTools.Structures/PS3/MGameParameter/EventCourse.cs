using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;
using PDTools.Compression;

namespace PDTools.Structures.PS3.MGameParameter
{
    public class EventCourse
    {
        public string CourseLabel { get; set; } = "mini";
        public int CourseCode { get; set; }

        public int CourseLayoutNumber { get; set; }
        public short MapOffsetWorldX { get; set; }
        public short MapOffsetWorldY { get; set; }
        public short MapScale { get; set; }
        public bool IsOmodetoDifficulty { get; set; }

        public bool NeedsPopulating { get; set; } = true;

        public CustomCourse CustomCourse { get; set; }

        public List<Gadget> Gadgets { get; set; } = new List<Gadget>();

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("track");

            xml.WriteStartElement("course_code");
            xml.WriteAttributeString("label", CourseLabel);
            xml.WriteEndElement();

            if (CourseLabel.Equals("coursemaker") && CustomCourse != null)
            {
                xml.WriteElementInt("generated_course_id", 0);
                var ted = Convert.ToBase64String(PS2ZIP.Deflate(CustomCourse.Data));
                xml.WriteElementValue("edit_data", ted);
            }

            xml.WriteElementInt("course_layout_no", CourseLayoutNumber);
            xml.WriteElementBool("is_omedeto_difficulty", IsOmodetoDifficulty);
            xml.WriteElementInt("map_offset_world_x", MapOffsetWorldX);
            xml.WriteElementInt("map_offset_world_y", MapOffsetWorldY);
            xml.WriteElementInt("map_scale", MapScale);
            xml.WriteElementBool("use_generator", false);

            // GT5 Only
            xml.WriteStartElement("course_generator_param");
            {
                xml.WriteElementBool("use_random_seed", false);
                xml.WriteElementInt("seed", 0);
                xml.WriteElementValue("course_generator_kind", "GENERATOR_CIRCUIT");
                xml.WriteElementValue("course_generator_length_type", "LENGTH");
                xml.WriteElementInt("lengthy", 0);
                xml.WriteElementValue("course_name", "");
            }
            xml.WriteEndElement();

            if (Gadgets.Count > 0)
            {
                foreach (var gadget in Gadgets)
                    gadget.WriteToXml(xml);
            }

            xml.WriteEndElement();
        }

        public void ReadFromCache(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5_E5_C3_44 && magic != 0xE6_E6_C3_44)
                throw new System.IO.InvalidDataException($"Course magic did not match - Got {magic.ToString("X8")}, expected 0xE6E6C344");

            int version = reader.ReadInt32();

            int gadgetVersion = -1;
            if (version >= 1_01)
                gadgetVersion = reader.ReadInt32();

            CourseCode = (int)reader.ReadInt64();

            byte[] edit_data = reader.ReadByteArrayPrefixed();
            if (edit_data != null)
                CustomCourse = CustomCourse.Read(edit_data);

            CourseLayoutNumber = reader.ReadInt32();

            if (version <= 1_01)
                reader.ReadInt32();

            int gadgetCount = reader.ReadInt32();
            for (int i = 0; i < gadgetCount; i++)
            {
                Gadget gadget = new Gadget();
                gadget.ReadFromBuffer(ref reader, gadgetVersion);
            }

            if (version >= 1_02)
            {
                MapOffsetWorldX = reader.ReadInt16();
                MapOffsetWorldY = reader.ReadInt16();
                MapScale = reader.ReadInt16();
                IsOmodetoDifficulty = reader.ReadBool();
                reader.ReadByte();

                if (version >= 1_03)
                    reader.ReadInt64(); // generated_course_id
            }
            else
            {
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadSingle();
                reader.ReadInt32();
                reader.ReadInt32();

                reader.ReadString4();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                    reader.ReadSingle();

                reader.ReadInt32();

                MapOffsetWorldX = reader.ReadInt16();
                MapOffsetWorldY = reader.ReadInt16();
                MapScale = reader.ReadInt16();
                IsOmodetoDifficulty = reader.ReadBool();
                reader.ReadByte();
            }
        }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_C3_44);
            bs.WriteInt32(1_03); // Version
            bs.WriteInt32(1_01); // Gadget Version
            bs.WriteUInt64((ulong)CourseCode);

            if (CourseLabel.Equals("coursemaker") && CustomCourse != null)
            {
                // Figure out how data is written
            }
            else
                bs.WriteInt32(0);

            bs.WriteInt32(CourseLayoutNumber);

            bs.WriteInt32(Gadgets.Count);
            foreach (var gadget in Gadgets)
                gadget.WriteToBuffer(ref bs);

            bs.WriteInt16(MapOffsetWorldX);
            bs.WriteInt16(MapOffsetWorldY);
            bs.WriteInt16(MapScale);
            bs.WriteBool(IsOmodetoDifficulty);
            bs.WriteByte(0); // field_0x3b
            bs.WriteUInt64(0); // Generated Course ID
        }

        public void ReadEventCourse(XmlNode node)
        {
            foreach (XmlNode trackNode in node.ChildNodes)
            {
                switch (trackNode.Name)
                {
                    case "course_code":
                        CourseLabel = trackNode.Attributes["label"].Value; break;

                    case "edit_data":
                        CustomCourse = CustomCourse.Read(Convert.FromBase64String(trackNode.ReadValueString())); break;

                    case "course_layout_no":
                        CourseLayoutNumber = trackNode.ReadValueInt(); break;

                    case "map_offset_world_x":
                        MapOffsetWorldX = trackNode.ReadValueShort(); break;

                    case "map_offset_world_y":
                        MapOffsetWorldY = trackNode.ReadValueShort(); break;

                    case "map_scale":
                        MapScale = trackNode.ReadValueShort(); break;

                    case "gadget":
                        var gadget = new Gadget();
                        gadget.ReadGadgetNode(trackNode);
                        Gadgets.Add(gadget); break;
                }
            }
        }
    }
}
