using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace PDTools.Utils;

public static class XmlUtils
{
    public static void WriteElementValue(this XmlWriter xml, string localName, string value)
    {
        xml.WriteStartElement(localName);
        xml.WriteAttributeString("value", value);
        xml.WriteEndElement();
    }

    public static void WriteEmptyElement(this XmlWriter xml, string localName)
    {
        xml.WriteStartElement(localName);
        xml.WriteEndElement();
    }

    public static void WriteElementBool(this XmlWriter xml, string localName, bool value)
        => WriteElementValue(xml, localName, value ? "1" : "0");

    public static void WriteElementInt(this XmlWriter xml, string localName, int value)
        => WriteElementValue(xml, localName, value.ToString());

    public static void WriteElementIntIfNotDefault(this XmlWriter xml, string localName, int value, int defaultValue = -1)
    {
        if (value != defaultValue)
            WriteElementValue(xml, localName, value.ToString());
    }

    public static void WriteElementUInt(this XmlWriter xml, string localName, uint value)
        => WriteElementValue(xml, localName, value.ToString());

    public static void WriteElementULong(this XmlWriter xml, string localName, ulong value)
        => WriteElementValue(xml, localName, value.ToString());

    public static void WriteElementFloat(this XmlWriter xml, string localName, float value)
        => WriteElementValue(xml, localName, value.ToString());

    public static void WriteElementEnumInt<T>(this XmlWriter xml, string localName, T value) where T : Enum
        => WriteElementValue(xml, localName, ((int)(object)value).ToString());


    public static T ReadValueEnum<T>(this XmlNode node) where T : Enum
    {
        var val = node.Attributes["value"].Value;
        if (string.IsNullOrEmpty(val))
            return default;

        return (T)Enum.Parse(typeof(T), node.Attributes["value"].Value);
    }

    private static bool TryGetValueAttribute(XmlNode node, out XmlAttribute attr)
    {
        attr = null;
        if (node.Attributes.Count > 0)
        {
            attr = node.Attributes["value"];
            return attr != null;
        }

        return false;
    }

    public static string ReadValueString(this XmlNode node)
    {
        if (TryGetValueAttribute(node, out XmlAttribute attr))
            return attr.Value;

        return null;
    }

    public static uint ReadValueUInt(this XmlNode node)
    {
        if (TryGetValueAttribute(node, out XmlAttribute attr))
        {
            if (uint.TryParse(attr.Value, out uint value))
                return value;
        }

        return 0;
    }

    public static int ReadValueInt(this XmlNode node)
    {
        if (TryGetValueAttribute(node, out XmlAttribute attr))
        {
            if (int.TryParse(attr.Value, out int value))
                return value;
        }

        return 0;
    }

    public static byte ReadValueByte(this XmlNode node)
    {
        if (TryGetValueAttribute(node, out XmlAttribute attr))
        {
            if (byte.TryParse(attr.Value, out byte value))
                return value;
        }

        return 0;
    }

    public static sbyte ReadValueSByte(this XmlNode node)
    {
        if (TryGetValueAttribute(node, out XmlAttribute attr))
        {
            if (sbyte.TryParse(attr.Value, out sbyte value))
                return value;
        }

        return 0;
    }

    public static float ReadValueSingle(this XmlNode node)
    {
        if (TryGetValueAttribute(node, out XmlAttribute attr))
        {
            if (float.TryParse(attr.Value, out float value))
                return value;
        }

        return 0;
    }

    public static short ReadValueShort(this XmlNode node)
    {
        if (TryGetValueAttribute(node, out XmlAttribute attr))
        {
            if (short.TryParse(attr.Value, out short value))
                return value;
        }

        return 0;
    }

    public static ushort ReadValueUShort(this XmlNode node)
    {
        if (TryGetValueAttribute(node, out XmlAttribute attr))
        {
            if (ushort.TryParse(attr.Value, out ushort value))
                return value;
        }

        return 0;
    }

    public static ulong ReadValueULong(this XmlNode node)
    {
        if (TryGetValueAttribute(node, out XmlAttribute attr))
        {
            if (ulong.TryParse(attr.Value, out ulong value))
                return value;
        }

        return 0;
    }

    public static bool ReadValueBool(this XmlNode node)
       => ReadValueInt(node) == 1;
}
