using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using PDTools.Utils;
using PDTools.Enums;
using System.Xml;

namespace PDTools.Structures.MGameParameter
{
    public class Replay
    {
        public string LocalPath { get; set; }

        public string Url { get; set; }

        public string DemoDataPath { get; set; }

        /// <summary>
        /// Not exposed in XMLs. Defaults to false.
        /// </summary>
        public bool UploadVideo { get; set; } = false;

        /// <summary>
        /// Not exposed in XMLs. Defaults to false.
        /// </summary>
        public bool ExportVideo { get; set; } = false;

        /// <summary>
        /// Not exposed in XMLs. Defaults to false.
        /// </summary>
        public bool DataLogger { get; set; } = false;

        /// <summary>
        /// Defaults to <see cref="ReplayRecordingQuality.EXTRA_HIGH"/>.
        /// </summary>
        public ReplayRecordingQuality ReplayRecordingQuality { get; set; } = ReplayRecordingQuality.EXTRA_HIGH;

        /// <summary>
        /// Defaults to false.
        /// </summary>
        public bool AutoSave { get; set; } = false;

        /// <summary>
        /// Not exposed in XMLs. Defaults to 576.
        /// </summary>
        public int VideoFormat { get; set; } = 576;

        /// <summary>
        /// Not exposed in XMLs. Defaults to 2.
        /// </summary>
        public int AudioFormat { get; set; } = 2;

        public bool IsDefault()
        {
            var defaultReplay = new Replay();
            return LocalPath == defaultReplay.LocalPath &&
                Url == defaultReplay.Url &&
                DemoDataPath == defaultReplay.Url &&
                UploadVideo == defaultReplay.UploadVideo &&
                ExportVideo == defaultReplay.ExportVideo &&
                DataLogger == defaultReplay.DataLogger &&
                ReplayRecordingQuality == defaultReplay.ReplayRecordingQuality &&
                AutoSave == defaultReplay.AutoSave &&
                VideoFormat == defaultReplay.VideoFormat &&
                AudioFormat == defaultReplay.AudioFormat;
        }

        public void CopyTo(Replay other)
        {
            other.LocalPath = LocalPath;
            other.Url = Url;
            other.DemoDataPath = DemoDataPath;
            other.UploadVideo = UploadVideo;
            other.ExportVideo = ExportVideo;
            other.DataLogger = DataLogger;
            other.ReplayRecordingQuality = ReplayRecordingQuality;
            other.AutoSave = AutoSave;
            other.VideoFormat = VideoFormat;
            other.AudioFormat = AudioFormat;
        }

        public void WriteToXml(XmlWriter xml)
        {
            xml.WriteElementValue("local_path", LocalPath);
            xml.WriteElementValue("url", Url);
            xml.WriteElementValue("demo_data_path", DemoDataPath);
            xml.WriteElementValue("replay_recording_quality", ReplayRecordingQuality.ToString());
            xml.WriteElementBool("auto_save", AutoSave);
        }

        public void ParseFromXml(XmlNode replayNode)
        {
            foreach (XmlNode node in replayNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "local_path":
                        LocalPath = node.ReadValueString(); break;
                    case "url":
                        Url = node.ReadValueString(); break;
                    case "demo_data_path":
                        DemoDataPath = node.ReadValueString(); break;
                    case "replay_recording_quality":
                        ReplayRecordingQuality = node.ReadValueEnum<ReplayRecordingQuality>(); break;
                    case "auto_save":
                        AutoSave = node.ReadValueBool(); break;
                }
            }
        }

        public void Deserialize(ref BitStream bs)
        {
            uint magic = bs.ReadUInt32();
            if (magic != 0x_E6_E6_C1_D0)
                throw new InvalidDataException("Unexpected Replay magic");

            uint version = bs.ReadUInt32();
            LocalPath = bs.ReadString4Aligned();
            Url = bs.ReadString4Aligned();
            DemoDataPath = bs.ReadString4Aligned();
            UploadVideo = bs.ReadBool();
            ExportVideo = bs.ReadBool();
            DataLogger = bs.ReadBool();
            UploadVideo = bs.ReadBool();
            ReplayRecordingQuality = (ReplayRecordingQuality)bs.ReadSByte();
            AutoSave = bs.ReadBool();
            VideoFormat = bs.ReadInt32();
            AudioFormat = bs.ReadInt32();
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteUInt32(0x_E6_E6_C1_D0);
            bs.WriteUInt32(1_00); // Version

            bs.WriteNullStringAligned4(LocalPath);
            bs.WriteNullStringAligned4(Url);
            bs.WriteNullStringAligned4(DemoDataPath);
            bs.WriteBool(UploadVideo);
            bs.WriteBool(ExportVideo);
            bs.WriteBool(DataLogger);
            bs.WriteSByte((sbyte)ReplayRecordingQuality);
            bs.WriteBool(AutoSave);
            bs.WriteInt32(VideoFormat);
            bs.WriteInt32(AudioFormat);
        }

    }
}
