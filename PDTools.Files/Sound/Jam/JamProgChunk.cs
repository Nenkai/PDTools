using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Sound.Jam;

// Game code refers to this as "JamSplitChunk"
// JAM original documents refer to this as "SplitBlock"
public class JamProgChunk
{
    // 0xFF = has ALL notes provided for a certain range
    // notes in ranges can still be empty
    public byte CountOrFlag { get; set; }

    public byte Volume { get; set; }
    public byte field_0x02 { get; set; }
    public byte field_0x03 { get; set; }
    public byte field_0x04 { get; set; }
    public byte capacityMaybe { get; set; }
    public byte FullRangeMin { get; set; }
    public byte FullRangeMax { get; set; }

    public List<JamSplitChunk> SplitChunks { get; set; } = [];

    public void Read(BinaryStream bs)
    {
        CountOrFlag = bs.Read1Byte();
        Volume = bs.Read1Byte();
        field_0x02 = bs.Read1Byte();
        field_0x03 = bs.Read1Byte();
        field_0x04 = bs.Read1Byte();
        capacityMaybe = bs.Read1Byte();
        FullRangeMin = bs.Read1Byte();
        FullRangeMax = bs.Read1Byte();

        if (CountOrFlag == 0xFF)
        {
            int cnt = (FullRangeMax - FullRangeMin) + 1;
            for (int i = 0; i < cnt; i++)
            {
                var splitChunk = new JamSplitChunk();
                splitChunk.Read(bs);
                SplitChunks.Add(splitChunk);
            }
        }
        else
        {
            int cnt = (CountOrFlag & 0x0F) + 1;
            for (int i = 0; i < cnt; i++)
            {
                var splitChunk = new JamSplitChunk();
                splitChunk.Read(bs);
                SplitChunks.Add(splitChunk);
            }
        }
    }
}

public class JamSplitChunk
{
    public byte NoteMin { get; set; }
    public byte NoteMax { get; set; }
    public byte BaseNoteMaybe { get; set; }
    public byte field_0x03 { get; set; }

    /* ?? = 0x01
     * SetNoiseShiftFrequency = 0x02, // SE only
       PitchModulateSpeedAndDepth = 0x20,
       ??? = 0x40
       Reverb = 0x80,

       possibly more unknown
    */
    public byte Flags { get; set; }

    /// <summary>
    /// Multiply by 0x10 for sample data offset starting from bd offset
    /// </summary>
    public uint SsaOffset { get; set; }

    public short field_0x08 { get; set; }
    public short field_0x0A { get; set; }
    public short field_0x0C { get; set; }
    public byte field_0x0E { get; set; }

    /// <summary>
    /// 0x7F = no lfo in use
    /// </summary>
    public byte LfoTableIndex { get; set; }

    public void Read(BinaryStream bs)
    {
        NoteMin = bs.Read1Byte();
        NoteMax = bs.Read1Byte();
        BaseNoteMaybe = bs.Read1Byte();
        field_0x03 = bs.Read1Byte();
        Flags = bs.Read1Byte();
        SsaOffset = (uint)((bs.Read1Byte() << 16) | bs.ReadUInt16()); // Game code refers to the offset to audio as Ssa
        field_0x08 = bs.ReadInt16();
        field_0x0A = bs.ReadInt16();
        field_0x0C = bs.ReadInt16();
        field_0x0E = bs.Read1Byte();
        LfoTableIndex = bs.Read1Byte();
    }

    public byte[] GetData(BinaryStream bs)
    {
        // Size of vag is not provided, we must find it using vag flags
        long basePos = bs.Position;
        long absoluteSsaOffset = basePos + (SsaOffset * 0x10);
        bs.Position = absoluteSsaOffset;

        int sampleIndex = 1;
        while (bs.Position < bs.Length)
        {
            byte decodingCoef = bs.Read1Byte();
            byte flag = bs.Read1Byte();
            if (flag == 1 || flag == 3)
                break;

            sampleIndex++;

            bs.Position += 0x0E;
        }

        bs.Position = absoluteSsaOffset;
        return bs.ReadBytes(0x10 * sampleIndex);
    }

    public override string ToString()
    {
        return $"{NoteMin}->{NoteMax}";
    }
}
