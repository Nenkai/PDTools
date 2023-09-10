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
    /// <summary>
    /// GT6 Only
    /// </summary>
    public class DriftSection
    {
        public float StartV { get; set; }
        public float FinishV { get; set; }

        public void CopyTo(DriftSection other)
        {
            other.StartV = StartV;
            other.FinishV = FinishV;
        }

        public void ParseFromXml(XmlNode node)
        {
            foreach (XmlNode secNode in node.ChildNodes)
            {
                switch (secNode.Name)
                {
                    case "start":
                        StartV = secNode.ReadValueSingle(); break;

                    case "finish":
                        FinishV = secNode.ReadValueSingle(); break;
                }
            }
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteElementFloat("start", StartV);
            xml.WriteElementFloat("finish", FinishV);
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteSingle(StartV);
            bs.WriteSingle(FinishV);
        }
    }
}
