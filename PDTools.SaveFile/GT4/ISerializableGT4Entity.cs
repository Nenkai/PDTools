// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Text;
using Syroot.BinaryData.Memory;

namespace PDTools.SaveFile.GT4;

public interface IGameSerializeBase<T>
{
    void CopyTo(T obj);

    void Unpack(GT4Save save, ref SpanReader sr);
    void Pack(GT4Save save, ref SpanWriter sw);
}
