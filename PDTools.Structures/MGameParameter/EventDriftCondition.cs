using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class EventDriftCondition
    {
        public DriftModeType DriftMode { get; set; }
        public byte LaunchSpeed { get; set; }
        public float LaunchV { get; set; }
        public List<EventDriftSection> Sections { get; set; } = new List<EventDriftSection>();

        public void ParseDriftCondition(XmlNode driftCondNode)
        {
            foreach (XmlNode node in driftCondNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "drift_mode_type": // Not actually present in xmls - parse it anyway
                        DriftMode = node.ReadValueEnum<DriftModeType>(); break;

                    case "launch_speed":
                        LaunchSpeed = node.ReadValueByte(); break;

                    case "launch_v":
                        LaunchV = node.ReadValueSingle(); break;

                    case "section":
                        var section = new EventDriftSection();
                        ParseSection(section, node);
                        Sections.Add(section);
                        break;
                }
            }
        }

        private void ParseSection(EventDriftSection section, XmlNode node)
        {
            foreach (XmlNode secNode in node.ChildNodes)
            {
                switch (secNode.Name)
                {
                    case "start":
                        section.StartV = secNode.ReadValueSingle(); break;

                    case "finish":
                        section.FinishV = secNode.ReadValueSingle(); break;
                }
            }
        }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E8_DC_CF);
            bs.WriteUInt32(1_01); // Version

            bs.WriteSByte((sbyte)DriftMode);
            bs.WriteByte(LaunchSpeed);
            bs.WriteSingle(LaunchV);

            bs.WriteByte((byte)Sections.Count);
            for (int i = 0; i < Sections.Count; i++)
            {
                bs.WriteSingle(Sections[i].StartV);
                bs.WriteSingle(Sections[i].FinishV);
            }
        }
    }

    public class EventDriftSection
    {
        public float StartV { get; set; }
        public float FinishV { get; set; }
    }

    public enum DriftModeType
    {
        NONE,
        FREELAP,
        FREESECTION,
        ONELAP,
        SECTION,
        USER_V,
    }
}
