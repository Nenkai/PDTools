using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using PDTools.Utils;

namespace PDTools.Structures.MGameParameter;

public class Gadget
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public int KindDBId { get; set; }
    public List<float> Postures { get; set; } = [];

    public void CopyTo(Gadget other)
    {
        other.X = X;
        other.Y = Y;
        other.Z = Z;
        other.KindDBId = KindDBId;

        for (int i = 0; i < Postures.Count; i++)
            other.Postures.Add(Postures[i]);
    }

    public void ReadGadgetNode(XmlNode node)
    {
        foreach (XmlNode childNode in node)
        {
            switch (childNode.Name)
            {
                case "x":
                    X = childNode.ReadValueSingle(); break;
                case "y":
                    Y = childNode.ReadValueSingle(); break;
                case "z":
                    Z = childNode.ReadValueSingle(); break;

                case "kind_db_id":
                    KindDBId = childNode.ReadValueInt(); break;

                case "posture":
                    ReadPostureNode(childNode); break;
            }
        }
    }

    public void ReadPostureNode(XmlNode node)
    {
        var nodes = node.SelectNodes("param");
        if (nodes is not null)
        {
            foreach (XmlNode param in nodes)
                Postures.Add(param.ReadValueSingle());
        }
    }

    public void WriteToXml(XmlWriter xml)
    {
        xml.WriteStartElement("gadget");
        {
            xml.WriteElementInt("kind_db_id", KindDBId);
            xml.WriteElementFloat("x", X);
            xml.WriteElementFloat("y", Y);
            xml.WriteElementFloat("z", Z);

            if (Postures.Count > 0)
            {
                xml.WriteStartElement("posture");
                foreach (var value in Postures)
                    xml.WriteElementFloat("value", value);
                xml.WriteEndElement();
            }
        }
        xml.WriteEndElement();
    }

    public void ReadFromBuffer(ref BitStream reader, int version)
    {
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        Z = reader.ReadSingle();

        if (version >= 1_01)
            KindDBId = reader.ReadByte();

        int postureCount = reader.ReadInt32();
        for (int i = 0; i < postureCount; i++)
            Postures.Add(reader.ReadSingle());
    }

    public void WriteToBuffer(ref BitStream bs)
    {
        bs.WriteSingle(X);
        bs.WriteSingle(Y);
        bs.WriteSingle(Z);
        bs.WriteInt32(KindDBId);

        bs.WriteInt32(Postures.Count);
        foreach (var param in Postures)
            bs.WriteSingle(param);
    }
}
