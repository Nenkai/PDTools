﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Buffers.Binary;

namespace PDTools.Crypto.SimulationInterface
{
    /// <summary>
    /// Used to decrypt packets from GT6's Simulator Interface.
    /// </summary>
    public class SimulatorInterfaceCryptorGT6 : ISimulationInterfaceCryptor
    {
        private Salsa20 _salsa;

        public const string Key = "Simulator Interface Packet ver 0.0";

        public SimulatorInterfaceCryptorGT6()
        {
            _salsa = new Salsa20(Encoding.ASCII.GetBytes(Key), Key.Length);
        }

        public void Decrypt(Span<byte> bytes)
        {
            // Input should be 0x128

            // Reset offset
            _salsa.Set(0);

            int iv1 = BinaryPrimitives.ReadInt32BigEndian(bytes.Slice(0x40)); // GT6 will be big endian
            int iv2 = (int)(iv1 ^ 0xDEADBEAF);

            Span<byte> iv = stackalloc byte[8];
            BinaryPrimitives.WriteInt32LittleEndian(iv, iv2);
            BinaryPrimitives.WriteInt32LittleEndian(iv.Slice(4), iv1);
            _salsa.SetIV(iv);

            _salsa.Decrypt(bytes, bytes.Length);

            // Magic should be "G6S0" when decrypted 
        }
    }
}
