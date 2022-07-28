﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Crypto.SimulationInterface
{
    public interface ISimulationInterfaceCryptor
    {
        void Decrypt(Span<byte> bytes);
    }
}
