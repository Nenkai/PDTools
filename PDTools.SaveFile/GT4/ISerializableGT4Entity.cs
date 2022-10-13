using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4
{
    public interface IGameSerializeBase
    {
        void Unpack(GT4Save save, ref SpanReader sr);
        void Pack(GT4Save save, ref SpanWriter sw);
    }
}
