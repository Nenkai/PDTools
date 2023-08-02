using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PDTools.Enums;


using PDTools.Utils;
using Syroot.BinaryData;

namespace PDTools.Structures.MGameParameter
{
    /// <summary>
    /// For GT5
    /// </summary>
    public class CourseGeneratorParamSection
    {
        public float Width { get; set; }
        public float Curvy { get; set; }
        public float Sharpness { get; set; }
        public float BankAngularity { get; set; }

        public void ReadCourseGeneratorSectionNode(XmlNode node)
        {
            foreach (XmlNode childNode in node)
            {
                switch (childNode.Name)
                {
                    case "width":
                        Width = childNode.ReadValueSingle(); break;

                    case "curvy":
                        Curvy = childNode.ReadValueSingle(); break;

                    case "sharpness":
                        Sharpness = childNode.ReadValueSingle(); break;

                    case "bank_angularity":
                        BankAngularity = childNode.ReadValueSingle(); break;

                }
            }
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("section");
            {
                xml.WriteElementFloat("width", Width);
                xml.WriteElementFloat("curvy", Curvy);
                xml.WriteElementFloat("sharpness", Sharpness);
                xml.WriteElementFloat("bank_angularity", BankAngularity);
            }
            xml.WriteEndElement();
        }
    }
}
