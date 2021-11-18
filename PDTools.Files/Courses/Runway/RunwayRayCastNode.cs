using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;
using System.Diagnostics;

namespace PDTools.Files.Courses.Runway
{
    public class RunwayRayCastNode
    {
        /// <summary>
        /// For branches
        /// </summary>
        public bool UnkBool { get; set; }

        /// <summary>
        /// For branches
        /// </summary>
        public RayCastAxis Axis { get; set; }

        /// <summary>
        /// For branches
        /// </summary>
        public float AxisValue { get; set; }

        /// <summary>
        /// For branches
        /// </summary>
        public RunwayRayCastNode Left { get; set; }

        /// <summary>
        /// For branches
        /// </summary>
        public RunwayRayCastNode Right { get; set; }

        /// <summary>
        /// For leaves
        /// </summary>
        public List<RunwayRayCastData> Data { get; set; } = new();

        /// <summary>
        /// For leaves
        /// </summary>
        public ushort Unk1 { get; set; }

        /// <summary>
        /// For leaves
        /// </summary>
        public ushort Unk2 { get; set; }

        public static RunwayRayCastNode TraverseRead(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
        {
            RunwayRayCastNode node = new RunwayRayCastNode();

            long basePos = bs.Position;

            uint bits = bs.ReadUInt32();
            uint nextOffset = 0;

            /* 2 bit axis
             * 1 bit is same
             * 29 bit offset is branch */
            node.UnkBool = ((bits >> 2) & 1) != 0;
            node.Axis = (RayCastAxis)(bits & 0x03);

            if (node.Axis == RayCastAxis.None)
            {
                node.Unk1 = (ushort)(bits >> 16);
                node.Unk2 = (ushort)((bits >> 3) & 0b11111_11111111); // 13 bits

                if (rwyVersionMajor >= 4)
                {
                    ushort dataCount = bs.ReadUInt16();
                    ushort[] dataOffsets = bs.ReadUInt16s(dataCount);

                    for (int i = 0; i < dataCount; i++)
                    {
                        if (dataOffsets[i] == 0)
                        {
                            node.Data.Add(new RunwayRayCastData());
                            continue;
                        }

                        bs.Position = basePos + dataOffsets[i];

                        var data = ReadEntryData(bs);
                        node.Data.Add(data);
                    }
                }
                else
                {
                    // Only one
                    var data = ReadEntryData(bs);
                    node.Data.Add(data);
                }
            }
            else
            {
                nextOffset = bits >> 3;
                node.AxisValue = bs.ReadSingle();
            }

            if (node.Axis != RayCastAxis.None)
            {
                bs.Position = basePos + 0x08;
                node.Left = TraverseRead(bs, rwyVersionMajor, rwyVersionMinor);
            }

            if (node.Axis != RayCastAxis.None && nextOffset != 0)
            {
                if (nextOffset == 0x08 && node.Left is not null) // Refers to same one
                {
                    node.Right = node.Left;
                }
                else
                {
                    bs.Position = basePos + nextOffset;
                    node.Right = TraverseRead(bs, rwyVersionMajor, rwyVersionMinor);
                }
            }

            return node;
        }

        private static RunwayRayCastData ReadEntryData(BinaryStream bs)
        {
            var data = new RunwayRayCastData();
            data.MainRoadTriIndex = bs.ReadUInt32();

            while (true)
            {
                uint value = 0;

                uint currentVal = (uint)bs.ReadByte();
                if (currentVal == 0)
                    break;

                if (currentVal < 0x80) // 1 Byte
                {
                    value = currentVal;
                }
                else if (currentVal < 0xC0) // 2 Byte
                {
                    value = ((currentVal << 8) & 0x3F00) | bs.Read1Byte();
                }
                else if (currentVal >= 0xC0) // 4 Byte
                {
                    value = ((currentVal << 24) & 0x1F000000) | ((uint)bs.Read1Byte() << 16) | ((uint)bs.Read1Byte() << 8) | bs.Read1Byte();
                }

                data.RoadTriIndices.Add(value);
            }

            return data;
        }

        public void ToStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
        {
            long basePos = bs.Position;
            long lastCurrentPos;

            bs.Position += 4;

            uint entryBits = 0;
            if (Axis == RayCastAxis.None)
            {
                entryBits |= ((uint)Unk1 << 16);
                entryBits |= (((uint)Unk2 << 3) & 0b1111_11111111);

                Debug.Assert(Data.Count <= ushort.MaxValue, $"Too much data to save? ({Data.Count} entries > {ushort.MaxValue})");

                if (rwyVersionMajor >= 4)
                {
                    bs.WriteUInt16((ushort)Data.Count);
                    bs.Position += Data.Count * sizeof(ushort);

                    List<ushort> offsets = new(Data.Count);

                    for (int i = 0; i < Data.Count; i++)
                    {
                        var data = Data[i];
                        if (data.MainRoadTriIndex is null)
                        {
                            offsets.Add(0);
                            continue;
                        }
                        else
                        {
                            long dataOffset = bs.Position - basePos;
                            Debug.Assert(dataOffset <= ushort.MaxValue, $"Data entry offset is too large ({dataOffset:X8} > {ushort.MaxValue:X8})");

                            offsets.Add((ushort)(bs.Position - basePos));

                            Data[i].ToStream(bs);
                        }
                    }

                    // Align entry
                    bs.Align(0x04, grow: true);
                    lastCurrentPos = bs.Position;

                    // Write offsets
                    bs.Position = basePos + 6;
                    for (int i = 0; i < offsets.Count; i++)
                        bs.WriteUInt16(offsets[i]);
                }
                else
                {
                    // Just one
                    Data[0].ToStream(bs);
                    bs.Align(0x04, grow: true);
                    lastCurrentPos = bs.Position;
                }
            }
            else
            {
                bs.WriteSingle(AxisValue);
                
                if (Left is not null)
                    Left.ToStream(bs, rwyVersionMajor, rwyVersionMinor);

                lastCurrentPos = bs.Position;
                long nextOffset = lastCurrentPos - basePos;
                if (Right is not null)
                {
                    if (Right == Left)
                    {
                        nextOffset = 0x08;
                    }
                    else
                    {
                        Right.ToStream(bs, rwyVersionMajor, rwyVersionMinor);
                        lastCurrentPos = bs.Position;
                    }

                    entryBits |= (uint)(nextOffset << 3);
                }

                entryBits |= (uint)Axis & 0x03;
            }

            entryBits |= (UnkBool ? 1u : 0u) << 2;

            bs.Position = basePos;
            bs.WriteUInt32(entryBits);

            bs.Position = lastCurrentPos;
        }
    }

    public enum RayCastAxis
    {
        None,
        X,
        Y,
        Z
    }
}
