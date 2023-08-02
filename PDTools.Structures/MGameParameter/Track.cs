using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;
using PDTools.Compression;
using PDTools.Structures.PS3;

namespace PDTools.Structures.MGameParameter
{
    public class Track
    {
        /// <summary>
        /// Defaults to -1 (No code)
        /// </summary>
        public int CourseCode { get; set; } = -1;
        public string CourseLabel { get; set; } = "mini";

        /// <summary>
        /// GT6 Only. Defaults to 0
        /// </summary>
        public ulong GeneratedCourseID { get; set; } = 0;

        /// <summary>
        /// Defaults to 0
        /// </summary>
        public int CourseLayoutNumber { get; set; } = 0;

        /// <summary>
        /// For GT5
        /// </summary>
        public bool UseGenerator { get; set; }

        /// <summary>
        /// For GT5
        /// </summary>
        public CourseGeneratorParam CourseGeneratorParam { get; set; } = new CourseGeneratorParam();

        public List<Gadget> Gadgets { get; set; } = new List<Gadget>();

        /// <summary>
        /// Defaults to 0
        /// </summary>
        public short MapOffsetWorldX { get; set; }

        /// <summary>
        /// Defaults to 0
        /// </summary>
        public short MapOffsetWorldY { get; set; }

        /// <summary>
        /// Defaults to 0
        /// </summary>
        public short MapScale { get; set; }

        /// <summary>
        /// Defaults to false
        /// </summary>
        public bool IsOmodetoDifficulty { get; set; }

        /// <summary>
        /// For GT5
        /// </summary>
        public string CoursePathway { get; set; }

        /// <summary>
        /// For GT6. Custom TED. Must be PS2ZIP'ed.
        /// </summary>
        public byte[] EditData { get; set; }


        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("course_code");
            xml.WriteAttributeString("label", CourseLabel);
            xml.WriteEndElement();

            xml.WriteElementULong("generated_course_id", GeneratedCourseID);
            xml.WriteElementInt("course_layout_no", CourseLayoutNumber);
            xml.WriteElementBool("use_generator", UseGenerator);

            CourseGeneratorParam?.WriteToXml(xml);

            foreach (var gadget in Gadgets)
                gadget.WriteToXml(xml);

            xml.WriteElementValue("course_pathway", CoursePathway);
            xml.WriteElementInt("map_offset_world_x", MapOffsetWorldX);
            xml.WriteElementInt("map_offset_world_y", MapOffsetWorldY);
            xml.WriteElementInt("map_scale", MapScale);
            xml.WriteElementBool("is_omedeto_difficulty", IsOmodetoDifficulty);

            if (EditData != null)
                xml.WriteElementValue("edit_data", Convert.ToBase64String(EditData));
        }

        public void Deserialize(ref BitStream reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0xE5_E5_C3_44 && magic != 0xE6_E6_C3_44)
                throw new System.IO.InvalidDataException($"Course magic did not match - Got {magic.ToString("X8")}, expected 0xE6E6C344");

            int version = reader.ReadInt32();

            int gadgetVersion = -1;
            if (version >= 1_01)
                gadgetVersion = reader.ReadInt32();

            CourseCode = (int)reader.ReadInt64();
            EditData = reader.ReadByteArrayPrefixed();

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
                    GeneratedCourseID = reader.ReadUInt64();
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

        public void Serialize(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_C3_44);
            bs.WriteInt32(1_03); // Version
            bs.WriteInt32(1_01); // Gadget Version
            bs.WriteUInt64((ulong)CourseCode);

            bs.WriteByteData(EditData, withPrefixLength: true);

            bs.WriteInt32(CourseLayoutNumber);

            bs.WriteInt32(Gadgets.Count);
            foreach (var gadget in Gadgets)
                gadget.WriteToBuffer(ref bs);

            bs.WriteInt16(MapOffsetWorldX);
            bs.WriteInt16(MapOffsetWorldY);
            bs.WriteInt16(MapScale);
            bs.WriteBool(IsOmodetoDifficulty);
            bs.WriteByte(0); // field_0x3b
            bs.WriteUInt64(GeneratedCourseID); // Generated Course ID
        }

        public void ParseFromXml(XmlNode node)
        {
            foreach (XmlNode trackNode in node.ChildNodes)
            {
                switch (trackNode.Name)
                {
                    case "course_code":
                        CourseLabel = trackNode.Attributes["label"].Value; break;

                    case "edit_data":
                        EditData = Convert.FromBase64String(trackNode.ReadValueString()); break;

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
