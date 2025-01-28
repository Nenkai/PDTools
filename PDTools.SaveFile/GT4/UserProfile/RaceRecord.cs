using Syroot.BinaryData.Memory;

using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.SaveFile.GT4.UserProfile;

public class RaceRecord : IGameSerializeBase<RaceRecord>
{
    public const int MAX_RECORDS = 768;
    public RaceRecordUnit[] Records { get; set; } = new RaceRecordUnit[MAX_RECORDS];

    public void CopyTo(RaceRecord dest)
    {
        for (var i = 0; i < MAX_RECORDS; i++)
        {
            dest.Records[i] = new RaceRecordUnit();
            Records[i].CopyTo(dest.Records[i]);
        }
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    {
        for (var i = 0; i < MAX_RECORDS; i++)
            Records[i].Pack(save, ref sw);

        sw.Align(GT4Save.ALIGNMENT);
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        for (var i = 0; i < MAX_RECORDS; i++)
        {
            Records[i] = new RaceRecordUnit();
            Records[i].Unpack(save, ref sr);
        }

        sr.Align(GT4Save.ALIGNMENT);
    }
}
