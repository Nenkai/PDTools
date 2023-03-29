using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.Option
{
    public class PhysicalMonitorSize : IGameSerializeBase<PhysicalMonitorSize>
    {
        public float MonitorSize1 { get; set; }
        public float MonitorSize2 { get; set; }
        public float MonitorSize3 { get; set; }
        public float MonitorSize4 { get; set; }
        public float MonitorSize5 { get; set; }
        public float MonitorSize6 { get; set; }
        public float MonitorSize7 { get; set; }
        public float MonitorSize8 { get; set; }
        public float MonitorSize9 { get; set; }
        public float MonitorSize10 { get; set; }
        public float MonitorSize11 { get; set; }

        public void CopyTo(PhysicalMonitorSize dest)
        {
            dest.MonitorSize1 = MonitorSize1;
            dest.MonitorSize2 = MonitorSize2;
            dest.MonitorSize3 = MonitorSize3;
            dest.MonitorSize4 = MonitorSize4;
            dest.MonitorSize5 = MonitorSize5;
            dest.MonitorSize6 = MonitorSize6;
            dest.MonitorSize7 = MonitorSize7;
            dest.MonitorSize8 = MonitorSize8;
            dest.MonitorSize9 = MonitorSize9;
            dest.MonitorSize10 = MonitorSize10;
            dest.MonitorSize11 = MonitorSize11;
        }

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            sw.WriteSingle(MonitorSize1);
            sw.WriteSingle(MonitorSize2);
            sw.WriteSingle(MonitorSize3);
            sw.WriteSingle(MonitorSize4);
            sw.WriteSingle(MonitorSize5);
            sw.WriteSingle(MonitorSize6);
            sw.WriteSingle(MonitorSize7);
            sw.WriteSingle(MonitorSize8);
            sw.WriteSingle(MonitorSize9);
            sw.WriteSingle(MonitorSize10);
            sw.WriteSingle(MonitorSize1);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            MonitorSize1 = sr.ReadSingle();
            MonitorSize2 = sr.ReadSingle();
            MonitorSize3 = sr.ReadSingle();
            MonitorSize4 = sr.ReadSingle();
            MonitorSize5 = sr.ReadSingle();
            MonitorSize6 = sr.ReadSingle();
            MonitorSize7 = sr.ReadSingle();
            MonitorSize8 = sr.ReadSingle();
            MonitorSize9 = sr.ReadSingle();
            MonitorSize10 = sr.ReadSingle();
            MonitorSize11 = sr.ReadSingle();

        }
    }
}
