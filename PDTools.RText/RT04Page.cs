using System.Collections.Generic;
using System.IO;

using Syroot.BinaryData;

namespace PDTools.RText;

public class RT04Page : RTextPageBase
{
    public const int EntrySize = 0x0C;

    public RT04Page()
    {
        
    }

    public override void Read(BinaryStream reader)
    {
        var pageNameOffset = reader.ReadUInt32();
        var pairUnitCount = reader.ReadUInt32();
        var pairUnitOffset = reader.ReadUInt32();

        reader.BaseStream.Position = pageNameOffset;
        Name = reader.ReadString(StringCoding.ZeroTerminated);

        reader.BaseStream.Position += reader.BaseStream.Position % 0x10; // Padding with 0x5E

        for (int i = 0; i < pairUnitCount; i++)
        {
            reader.BaseStream.Position = pairUnitOffset + (i * EntrySize);
            int id = reader.ReadInt32();
            uint labelOffset = reader.ReadUInt32();
            uint valueOffset = reader.ReadUInt32();

            reader.BaseStream.Position = labelOffset;
            string label = reader.ReadString(StringCoding.ZeroTerminated);

            reader.BaseStream.Position = valueOffset;
            string value = reader.ReadString(StringCoding.ZeroTerminated);

            var pair = new RTextPairUnit(id, label, value);
            PairUnits.Add(label, pair);
        }
    }

    
    public override void Write(BinaryStream writer, int baseOffset, int baseDataOffset)
    {
        writer.BaseStream.Position = baseDataOffset;
        int baseNameOffset = (int)writer.BaseStream.Position;
        writer.WriteString(Name, StringCoding.Raw);
        writer.Align(0x04, grow: true);

        int pairUnitOffset = (int)writer.BaseStream.Position;

        // Proceed to write the string tree, skip the entry map for now
        writer.BaseStream.Position += EntrySize * PairUnits.Count;
        int lastStringPos = (int)writer.BaseStream.Position;

        // Write our strings
        int j = 0;
        foreach (var pair in PairUnits)
        {
            writer.BaseStream.Position = lastStringPos;

            int labelOffset = (int)writer.BaseStream.Position;
            writer.WriteString(pair.Value.Label, StringCoding.Raw);
            writer.Align(0x04, grow: true);

            int valueOffset = (int)writer.BaseStream.Position;
            writer.WriteString(pair.Value.Value, StringCoding.Raw);
            writer.Align(0x04, grow: true);

            lastStringPos = (int)writer.BaseStream.Position;

            // Write the offsets
            writer.BaseStream.Position = pairUnitOffset + (j * RT04Page.EntrySize);
            writer.Write(pair.Value.ID);
            writer.Write(labelOffset);
            writer.Write(valueOffset);

            j++;
        }

        // Finish page toc entry
        writer.BaseStream.Position = baseOffset;
        writer.Write(baseNameOffset);
        writer.Write(PairUnits.Count);
        writer.Write(pairUnitOffset);
        writer.Write(0x5E5E5E5E); // Padding

        // Seek to the end of it
        writer.BaseStream.Position = writer.BaseStream.Length;
    }
}
