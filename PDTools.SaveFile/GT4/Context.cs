using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

using PDTools.Utils;
namespace PDTools.SaveFile.GT4
{
    public class ContextGT4 : IGameSerializeBase
    {
        public string MajorProject { get; set; }
        public string MajorPage { get; set; }
        public int CurrentStackSize { get; set; }
        public byte[] Stack { get; set; }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteStringFix(MajorProject, 0x20);
            sw.WriteStringFix(MajorPage, 0x20);
            sw.WriteInt32(CurrentStackSize);
            sw.WriteBytes(Stack);

            sw.Align(GT4Save.ALIGNMENT);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            MajorProject = sr.ReadFixedString(32);
            MajorPage = sr.ReadFixedString(32);
            CurrentStackSize = sr.ReadInt32();
            Stack = sr.ReadBytes(0x100);

            sr.Align(GT4Save.ALIGNMENT);
        }
    }
}
