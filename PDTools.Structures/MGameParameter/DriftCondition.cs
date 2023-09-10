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
    public class DriftCondition
    {
        /// <summary>
        /// Not exposed in XMLs - Defaults to <see cref="DriftModeType.NONE"/>.
        /// </summary>
        public DriftModeType DriftModeType { get; set; } = DriftModeType.NONE;

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        public byte LaunchSpeed { get; set; } = 0;

        /// <summary>
        /// Defaults to 0.0.
        /// </summary>
        public float LaunchV { get; set; } = 0;

        public List<DriftSection> Sections { get; set; } = new List<DriftSection>();

        public bool IsDefault()
        {
            var defaultDriftCondition = new DriftCondition();
            return DriftModeType == defaultDriftCondition.DriftModeType &&
                LaunchSpeed == defaultDriftCondition.LaunchSpeed &&
                LaunchV == defaultDriftCondition.LaunchV &&
                Sections.Count == 0;
        }

        public void CopyTo(DriftCondition other)
        {
            other.DriftModeType = DriftModeType;
            other.LaunchSpeed = LaunchSpeed;
            other.LaunchV = LaunchV;

            foreach (var section in Sections)
            {
                var newSection = new DriftSection();
                section.CopyTo(newSection);
                other.Sections.Add(section);
            }
        }

        public void ParseFromXml(XmlNode driftCondNode)
        {
            foreach (XmlNode node in driftCondNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "drift_mode_type": // Not actually present in xmls - parse it anyway
                        DriftModeType = node.ReadValueEnum<DriftModeType>(); break;

                    case "launch_speed":
                        LaunchSpeed = node.ReadValueByte(); break;

                    case "launch_v":
                        LaunchV = node.ReadValueSingle(); break;

                    case "section":
                        var section = new DriftSection();
                        section.ParseFromXml(node);
                        Sections.Add(section);
                        break;
                }
            }
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteElementInt("launch_speed", LaunchSpeed);
            xml.WriteElementFloat("launch_v", LaunchV);

            xml.WriteStartElement("section");
            for (int i = 0; i < Sections.Count; i++)
                Sections[i].WriteToXml(xml);
            xml.WriteEndElement();
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E8_DC_CF);
            bs.WriteUInt32(1_01); // Version

            bs.WriteSByte((sbyte)DriftModeType);
            bs.WriteByte(LaunchSpeed);
            bs.WriteSingle(LaunchV);

            bs.WriteByte((byte)Sections.Count);
            for (int i = 0; i < Sections.Count; i++)
                Sections[i].Serialize(ref bs);
        }
    }
}
