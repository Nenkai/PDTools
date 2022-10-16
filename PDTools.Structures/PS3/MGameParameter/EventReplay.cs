using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Utils;

namespace PDTools.Structures.PS3.MGameParameter
{
    public class EventReplay
    {
        public string LocalPath { get; set; }
        public string Url { get; set; }
        public string DemoDataPath { get; set; }
        public bool UploadVideo { get; set; }
        public bool ExportVideo { get; set; }
        public bool DataLogger { get; set; }
        public sbyte ReplayRecordingQuality { get; set; }
        public bool AutoSave { get; set; }
        public int VideoFormat { get; set; } = 576;
        public int AudioFormat { get; set; } = 2;

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0x_E6_E6_C1_D0);
            bs.WriteUInt32(1_00); // Version

            bs.WriteNullStringAligned4(LocalPath);
            bs.WriteNullStringAligned4(Url);
            bs.WriteNullStringAligned4(DemoDataPath);
            bs.WriteBool(UploadVideo);
            bs.WriteBool(ExportVideo);
            bs.WriteBool(DataLogger);
            bs.WriteSByte(ReplayRecordingQuality);
            bs.WriteBool(AutoSave);
            bs.WriteInt32(VideoFormat);
            bs.WriteInt32(AudioFormat);
        }

    }
}
