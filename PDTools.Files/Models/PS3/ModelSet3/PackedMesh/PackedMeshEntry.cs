﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh;

public class PackedMeshEntry
{
    public short StructDeclarationID { get; set; }
    public short ElementBitLayoutDefinitionID { get; set; }

    public PackedMeshEntryData Data { get; set; }

    public static PackedMeshEntry FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        PackedMeshEntry entry = new();

        entry.StructDeclarationID = bs.ReadInt16();
        entry.ElementBitLayoutDefinitionID = bs.ReadInt16();
        short unk_0x04 = bs.ReadInt16();
        byte countOfUnk = bs.Read1Byte();
        byte unk_0x07 = bs.Read1Byte();
        int unkOffset_0x08 = bs.ReadInt32();
        float unk_0x0C = bs.ReadSingle();
        int dataTocOffset = bs.ReadInt32();
        // TODO read rest
        bs.Position += 0x18;

        if (dataTocOffset != 0)
        {
            bs.Position = mdlBasePos + dataTocOffset;
            entry.Data = PackedMeshEntryData.FromStream(bs, mdlBasePos, mdl3VersionMajor);
        }

        return entry;
    }

    public static int GetSize()
    {
        return 0x30;
    }
}
