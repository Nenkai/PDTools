using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData.Memory;

using PDTools.Enums;

namespace PDTools.Structures;

public class DbCode
{
    public int Code { get; set; } = -1;
    public int TableId { get; set; } = -1;

    public DbCode(int code, int tableId)
    {
        Code = code;
        TableId = tableId;
    }

    public DbCode()
    {

    }

    public static DbCode Unpack(ref SpanReader sr)
    {
        var code = new DbCode();
        code.Code = sr.ReadInt32();
        code.TableId = sr.ReadInt32();
        return code;
    }

    public void Pack(ref SpanWriter sw)
    {
        sw.WriteInt32(Code);
        sw.WriteInt32(TableId);
    }
}
