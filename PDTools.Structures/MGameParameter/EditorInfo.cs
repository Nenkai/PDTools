using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;

namespace PDTools.Structures.MGameParameter;

public class EditorInfo
{
    public int PspMode { get; set; }

    public bool IsDefault()
    {
        var defaultEditorInfo = new EditorInfo();
        return PspMode != defaultEditorInfo.PspMode;
    }

    public void CopyTo(EditorInfo other)
    {
        other.PspMode = PspMode;
    }

    public void ParseFromXml(XmlNode node)
    {
        foreach (XmlNode secNode in node.ChildNodes)
        {
            switch (secNode.Name)
            {
                case "psp_mode":
                    PspMode = secNode.ReadValueInt(); break;
            }
        }
    }

    public void WriteToXml(XmlWriter xml)
    {
        xml.WriteElementInt("psp_mode", PspMode);
    }

    public void Serialize(ref BitStream bs)
    {
        bs.WriteUInt32(0xE6_E6_A0_7D);
        bs.WriteUInt32(1_00); // Version

        bs.WriteInt32(PspMode);
    }
}
