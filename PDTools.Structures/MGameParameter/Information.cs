using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PDTools.Utils;

namespace PDTools.Structures.MGameParameter
{
    public class Information
    {
        public LocalizeText Title { get; set; } = new LocalizeText();
        public LocalizeText OneLineTitle { get; set; } = new LocalizeText();
        public LocalizeText Description { get; set; } = new LocalizeText();
        public LocalizeText AdvancedNotice { get; set; } = new LocalizeText();
        public LocalizeText RegistrationNotice { get; set; } = new LocalizeText();
        public ushort NarrationID { get; set; }

        public string LogoImagePath { get; set; }
        public byte LogoImageLayout { get; set; }
        public byte[] LogoImageBuffer { get; set; }
        public string LogoOtherInfo { get; set; }

        public string FlierImagePath { get; set; }
        public byte[] FlierImageBuffer { get; set; }
        public string FlierOtherInfo { get; set; }

        public string RaceLabel { get; set; }
        public ushort RaceInfoMinute { get; set; }

        public bool IsDefault()
        {
            var defaultInformation = new Information();
            return Title.IsDefault() &&
                OneLineTitle.IsDefault() &&
                Description.IsDefault() &&
                AdvancedNotice.IsDefault() &&
                RegistrationNotice.IsDefault() &&
                NarrationID == defaultInformation.NarrationID &&
                LogoImagePath == defaultInformation.LogoImagePath &&
                LogoImageLayout == defaultInformation.LogoImageLayout &&
                LogoImageBuffer is null &&
                LogoOtherInfo == defaultInformation.LogoOtherInfo &&
                FlierImagePath == defaultInformation.FlierImagePath &&
                FlierImageBuffer is null &&
                FlierOtherInfo == defaultInformation.FlierOtherInfo &&
                RaceLabel == defaultInformation.RaceLabel &&
                RaceInfoMinute == defaultInformation.RaceInfoMinute;
        }

        public void CopyTo(Information other)
        {
            Title.CopyTo(other.Title);
            OneLineTitle.CopyTo(other.OneLineTitle);
            Description.CopyTo(other.Description);
            AdvancedNotice.CopyTo(other.AdvancedNotice);
            RegistrationNotice.CopyTo(other.RegistrationNotice);
            other.NarrationID = NarrationID;
            other.LogoImagePath = LogoImagePath;

            if (LogoImageBuffer != null)
            {
                other.LogoImageBuffer = new byte[LogoImageBuffer.Length];
                LogoImageBuffer.AsSpan().CopyTo(other.LogoImageBuffer);
            }

            other.LogoOtherInfo = LogoOtherInfo;
            other.FlierImagePath = FlierImagePath;

            if (FlierImageBuffer != null)
            {
                other.FlierImageBuffer = new byte[FlierImageBuffer.Length];
                FlierImageBuffer.AsSpan().CopyTo(other.FlierImageBuffer);
            }

            other.FlierOtherInfo = FlierOtherInfo;
            other.RaceLabel = RaceLabel;
            other.RaceInfoMinute = RaceInfoMinute;
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteStartElement("title");
            if (!Title.IsDefault())
                Title.WriteToXml(xml);
            xml.WriteEndElement();

            xml.WriteStartElement("one_line_title");
            if (!OneLineTitle.IsDefault())
                OneLineTitle.WriteToXml(xml);
            xml.WriteEndElement();

            xml.WriteStartElement("description");
            if (!Description.IsDefault())
                Description.WriteToXml(xml);
            xml.WriteEndElement();

            xml.WriteStartElement("advanced_notice");
            if (!AdvancedNotice.IsDefault())
                AdvancedNotice.WriteToXml(xml);
            xml.WriteEndElement();

            xml.WriteStartElement("registration_notice");
            if (!RegistrationNotice.IsDefault())
                RegistrationNotice.WriteToXml(xml);
            xml.WriteEndElement();

            if (NarrationID != 0)
                xml.WriteElementInt("narration_id", NarrationID);

            if (!string.IsNullOrEmpty(LogoImagePath))
                xml.WriteElementValue("logo_image_path", LogoImagePath);

            if (LogoImageLayout != 0)
                xml.WriteElementInt("logo_image_layout", LogoImageLayout);

            if (LogoImageBuffer != null)
                xml.WriteElementValue("logo_image_buffer", Convert.ToBase64String(LogoImageBuffer));
            xml.WriteElementValue("logo_other_info", LogoOtherInfo);

            if (!string.IsNullOrEmpty(FlierImagePath))
                xml.WriteElementValue("flier_image_path", FlierImagePath);

            if (FlierImageBuffer != null)
                xml.WriteElementValue("flier_image_buffer", Convert.ToBase64String(FlierImageBuffer));

            xml.WriteElementValue("flier_other_info", FlierOtherInfo);

            if (!string.IsNullOrEmpty(RaceLabel))
                xml.WriteElementValue("race_label", RaceLabel);

            if (RaceInfoMinute != 0)
                xml.WriteElementUInt("race_info_minute", RaceInfoMinute);
        }

        public void ParseFromXml(XmlNode node)
        {
            foreach (XmlNode informationNode in node.ChildNodes)
            {
                switch (informationNode.Name)
                {
                    case "title":
                        Title.ReadFromXml(informationNode);
                        break;

                    case "one_line_title":
                        OneLineTitle.ReadFromXml(informationNode);
                        break;

                    case "description":
                        Description.ReadFromXml(informationNode);
                        break;

                    case "advanced_notice":
                        Description.ReadFromXml(informationNode);
                        break;

                    case "registration_notice":
                        RegistrationNotice.ReadFromXml(informationNode);
                        break;

                    case "narration_id":
                        NarrationID = informationNode.ReadValueUShort(); break;
                    case "logo_image_path":
                        LogoImagePath = informationNode.ReadValueString(); break;
                    case "logo_image_layout":
                        LogoImageLayout = informationNode.ReadValueByte(); break;
                    case "logo_image_buffer":
                        LogoImageBuffer = Convert.FromBase64String(informationNode.ReadValueString()); break;
                    case "logo_other_info":
                        LogoOtherInfo = informationNode.ReadValueString(); break;
                    case "flier_image_path":
                        FlierImagePath = informationNode.ReadValueString(); break;
                    case "flier_image_buffer":
                        FlierImageBuffer = Convert.FromBase64String(informationNode.ReadValueString()); break;
                    case "flier_other_info":
                        FlierOtherInfo = informationNode.ReadValueString(); break;
                }
            }
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_9F_40);
            bs.WriteUInt32(1_02); // Version

            Title.Serialize(ref bs);
            OneLineTitle.Serialize(ref bs);
            Description.Serialize(ref bs);
            AdvancedNotice.Serialize(ref bs);
            RegistrationNotice.Serialize(ref bs);

            bs.WriteUInt16(NarrationID);
            bs.WriteUInt16(RaceInfoMinute);

            bs.WriteNullStringAligned4(LogoImagePath);
            bs.WriteByte(LogoImageLayout);
            bs.WriteByteData(LogoImageBuffer, withPrefixLength: true);
            bs.WriteNullStringAligned4(FlierImagePath);
            bs.WriteByteData(FlierImageBuffer, withPrefixLength: true);

            bs.WriteNullStringAligned4(RaceLabel);
        }
    }
}
