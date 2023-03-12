using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Buffers.Binary;

using Syroot.BinaryData;
using Syroot.BinaryData.Memory;
using Syroot.BinaryData.Core;

namespace PDTools.Files.Models.ModelSet3.FVF
{
    public class MDL3FVFElementDefinition
    {
        /// <summary>
        /// Name of the field within the flexible vertex.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Location of the field within the flexible vertex.
        /// </summary>
        public byte StartOffset { get; set; }

        /// <summary>
        /// Count of data elements within the flexible vertex.
        /// </summary>
        public byte ElementCount { get; set; }

        /// <summary>
        /// Data Type within the flexible vertex.
        /// </summary>
        public CELL_GCM_VERTEX_TYPE FieldType { get; set; }

        public byte ArrayIndex { get; set; }

        public static MDL3FVFElementDefinition FromStream(BinaryStream bs, long baseMdlPos, uint mdl3Version)
        {
            MDL3FVFElementDefinition def = new MDL3FVFElementDefinition();
            uint nameOffset = bs.ReadUInt32();
            def.StartOffset = bs.Read1Byte();
            def.ElementCount = bs.Read1Byte();
            def.FieldType = (CELL_GCM_VERTEX_TYPE)bs.Read1Byte();
            def.ArrayIndex = bs.Read1Byte();

            bs.Position = baseMdlPos + nameOffset;
            def.Name = bs.ReadString(StringCoding.ZeroTerminated);
            return def;
        }

        public Vector3 GetFVFFieldVector3(Span<byte> buffer)
        {
            float v1 = 0, v2 = 0, v3 = 0;

            if (ElementCount != 3)
                throw new InvalidOperationException("Expected 3 elements for Vector3");

            SpanReader sr = new SpanReader(buffer, Endian.Big); // Fix me..
            sr.Position = StartOffset;

            if (FieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_F)
            {
                v1 = sr.ReadSingle();
                v2 = sr.ReadSingle();
                v3 = sr.ReadSingle();
            }
            else if (FieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_S1)
            {
                v1 = sr.ReadUInt16() * (1f / short.MaxValue);
                v2 = sr.ReadUInt16() * (1f / short.MaxValue);
                v3 = sr.ReadUInt16() * (1f / short.MaxValue);
            }
            else if (FieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_UB)
            {
                v1 = sr.ReadByte() * (1f / sbyte.MaxValue);
                v2 = sr.ReadByte() * (1f / sbyte.MaxValue);
                v3 = sr.ReadByte() * (1f / sbyte.MaxValue);
            }
            else if (FieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_SF)
            {
                var bytes = sr.ReadBytes(2);
                var bytes2 = sr.ReadBytes(2);
                var bytes3 = sr.ReadBytes(2);
                v1 = (float)(sr.Endian == Endian.Big ? BinaryPrimitives.ReadHalfBigEndian(bytes) : BinaryPrimitives.ReadHalfLittleEndian(bytes));
                v2 = (float)(sr.Endian == Endian.Big ? BinaryPrimitives.ReadHalfBigEndian(bytes2) : BinaryPrimitives.ReadHalfLittleEndian(bytes2));
                v3 = (float)(sr.Endian == Endian.Big ? BinaryPrimitives.ReadHalfBigEndian(bytes3) : BinaryPrimitives.ReadHalfLittleEndian(bytes3));
            }
            else
            {
                throw new NotImplementedException($"Unimplemented field type {FieldType}");
            }

            return new Vector3(v1, v2, v3);
        }

        public Vector2 GetFVFFieldVector2(Span<byte> buffer)
        {
            float v1 = 0, v2 = 0;

            SpanReader sr = new SpanReader(buffer, Endian.Big); // Fix me..
            sr.Position = StartOffset;

            if (FieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_F)
            {
                if (ElementCount == 4)
                    ; // TODO: Check whats up with this, GT6 PS3 tracks uses 4 elements for map12 sometimes

                v1 = sr.ReadSingle();
                v2 = sr.ReadSingle();
            }
            else if (FieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_S1)
            {
                v1 = sr.ReadUInt16() * (1f / short.MaxValue);
                v2 = sr.ReadUInt16() * (1f / short.MaxValue);
            }
            else if (FieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_UB)
            {
                v1 = sr.ReadByte() * (1f / byte.MaxValue);
                v2 = sr.ReadByte() * (1f / byte.MaxValue);
            }
            else if (FieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_SF)
            {
                if (ElementCount == 4)
                    ; // TODO: Check whats up with this, GT5 PS3 tracks uses 4 elements for map12 sometimes

                var bytes = sr.ReadBytes(2);
                var bytes2 = sr.ReadBytes(2);
                v1 = (float)(sr.Endian == Endian.Big ? BinaryPrimitives.ReadHalfBigEndian(bytes) : BinaryPrimitives.ReadHalfLittleEndian(bytes));
                v2 = (float)(sr.Endian == Endian.Big ? BinaryPrimitives.ReadHalfBigEndian(bytes2) : BinaryPrimitives.ReadHalfLittleEndian(bytes2));
            }
            else
            {
                throw new NotImplementedException($"Unimplemented field type {FieldType}");
            }

            return new Vector2(v1, v2);
        }

        public static int GetSize()
        {
            return 0x08;
        }

        public override string ToString()
        {
            return $"{Name} (Start: 0x{StartOffset:X2}, Type: {FieldType}, {ElementCount} elements, {ArrayIndex})";
        }
    }

    public enum CELL_GCM_VERTEX_TYPE
    {
        /// <summary>
        /// Normalized short
        /// </summary>
        CELL_GCM_VERTEX_S1 = 1,

        /// <summary>
        /// Float
        /// </summary>
        CELL_GCM_VERTEX_F = 2,

        /// <summary>
        /// Half Float
        /// </summary>
        CELL_GCM_VERTEX_SF = 3,

        /// <summary>
        /// Unsigned byte
        /// </summary>
        CELL_GCM_VERTEX_UB = 4,

        /// <summary>
        /// Short
        /// </summary>
        CELL_GCM_VERTEX_S32K = 5,

        /// <summary>
        /// Vector, 10 11 11 bits
        /// </summary>
        CELL_GCM_VERTEX_CMP = 6,

        CELL_GCM_VERTEX_UB256 = 7,
    }
}
