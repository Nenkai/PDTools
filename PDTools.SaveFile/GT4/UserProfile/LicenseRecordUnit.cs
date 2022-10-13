using System;
using System.Collections.Generic;
using System.Text;

using PDTools.Enums.PS2;

using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4.UserProfile
{
    public class LicenseRecordUnit : IGameSerializeBase
    {
        public LicenseEntryUnit[] Entries { get; set; } = new LicenseEntryUnit[10];
        public int[] SectorTimes = new int[16];

        public void Pack(GT4Save save, ref SpanWriter sw)
        {
            for (var i = 0; i < Entries.Length; i++)
                Entries[i].Pack(save, ref sw);

            for (var i = 0; i < SectorTimes.Length; i++)
                sw.WriteInt32(SectorTimes[i]);
        }

        public void Unpack(GT4Save save, ref SpanReader sr)
        {
            for (var i = 0; i < Entries.Length; i++)
            {
                Entries[i] = new LicenseEntryUnit();
                Entries[i].Unpack(save, ref sr);
            }

            for (var i = 0; i < SectorTimes.Length; i++)
                SectorTimes[i] = sr.ReadInt32();
        }
    }
}
