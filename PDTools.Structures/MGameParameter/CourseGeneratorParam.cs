using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PDTools.Enums;

using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    /// <summary>
    /// For GT5
    /// </summary>
    public class CourseGeneratorParam
    {
        public uint Seed { get; set; } = 0;
        public bool UseRandomSeed { get; set; } = false;
        public float LengthY { get; set; } = 0;
        public CourseGeneratorKind CourseGeneratorKind { get; set; } = CourseGeneratorKind.GENERATOR_CIRCUIT;
        public CourseGeneratorLengthType CourseGeneratorLengthType { get; set; } = CourseGeneratorLengthType.LENGTH;
        public string CourseName { get; set; }

        public List<CourseGeneratorParamSection> Sections { get; set; } = new List<CourseGeneratorParamSection>();
        public bool FirstVersionIncompatible { get; set; }

        public void ParseFromXml(XmlNode node)
        {
            foreach (XmlNode childNode in node)
            {
                switch (childNode.Name)
                {
                    case "seed":
                        Seed = childNode.ReadValueUInt(); break;
                    case "use_random_seed":
                        UseRandomSeed = childNode.ReadValueBool(); break;
                    case "lengthy":
                        LengthY = childNode.ReadValueSingle(); break;

                    case "course_generator_kind":
                        CourseGeneratorKind = childNode.ReadValueEnum<CourseGeneratorKind>(); break;

                    case "course_generator_length_type":
                        CourseGeneratorLengthType = childNode.ReadValueEnum<CourseGeneratorLengthType>(); break;

                    case "course_name":
                        CourseName = childNode.ReadValueString(); break;

                    case "section":
                        var section = new CourseGeneratorParamSection();
                        section.ReadCourseGeneratorSectionNode(childNode);
                        Sections.Add(section);
                        break;

                    case "first_version_incompatible":
                        FirstVersionIncompatible = childNode.ReadValueBool();
                        break;
                }
            }
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("course_generator_param");
            {
                xml.WriteElementUInt("seed", Seed);
                xml.WriteElementBool("use_random_seed", UseRandomSeed);
                xml.WriteElementFloat("lengthy", LengthY);
                xml.WriteElementValue("course_generator_kind", CourseGeneratorKind.ToString());
                xml.WriteElementValue("course_generator_length_type", CourseGeneratorLengthType.ToString());
                xml.WriteElementValue("course_name", CourseName);

                foreach (var section in Sections)
                    section.WriteToXml(xml);
            }
            xml.WriteEndElement();
        }
    }
}
