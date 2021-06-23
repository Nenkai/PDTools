using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;
using Syroot.BinaryData.Core;

namespace PDTools.Utils
{
    /// <summary>
    /// Represents a binary string table that Polyphony Digital uses to save space within files.
    /// </summary>
    public class OptimizedStringTable
    {
        /// <summary>
        /// Position within the string table
        /// </summary>
        public int _currentPos;

        private int _alignment = -1;
        /// <summary>
        /// Gets or sets the string alignment. No alignment by default.
        /// </summary>
        public int Alignment
        {
            get => _alignment;
            set
            {
                if (value % 2 != 0)
                    throw new Exception("Alignment must be a power of two");
                _alignment = value;
            }
        }

        /// <summary>
        /// Whether the string offsets returned are relative to the base position rather than stream position.
        /// </summary>
        public bool IsRelativeOffsets { get; set; }

        /// <summary>
        /// Whether if the strings should be null terminated. On by default.
        /// </summary>
        public bool NullTerminated { get; set; } = true;

        private StringCoding _stringCoding = StringCoding.Raw;

        /// <summary>
        /// Encoding for strings. Defaults to UTF8.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Whether if the strings are prefixed. No prefix by default.
        /// </summary>
        public StringCoding StringCoding
        {
            get => _stringCoding;
            set
            {
                if (value == StringCoding.ZeroTerminated)
                    throw new InvalidOperationException("Zero-terminated string coding enum type is not supported, use the property 'NullTerminated' instead.\n" +
                        "This will allow you to have both a prefixed and zero terminated string, in cases of binary searching.");
                else if (value == StringCoding.VariableByteCount)
                    throw new InvalidOperationException("Variable byte count is not supported.");

                _stringCoding = value;
            }

        }

        public Dictionary<string, int> StringMeta = new Dictionary<string, int>();


        /// <summary>
        /// Adds a string to the string table.
        /// </summary>
        public void AddString(string str)
        {
            if (!StringMeta.ContainsKey(str))
            {
                StringMeta.Add(str, _currentPos);

                switch (_stringCoding)
                {
                    case StringCoding.ByteCharCount:
                        _currentPos++; break;
                    case StringCoding.Int16CharCount:
                        _currentPos += 2; break;
                    case StringCoding.Int32CharCount:
                        _currentPos += 4; break;
                }
                _currentPos += Encoding.GetByteCount(str);

                if (NullTerminated)
                    _currentPos++;

                if (_alignment != -1)
                {
                    var newPos = (-_currentPos % _alignment + _alignment) % _alignment;
                    _currentPos += newPos;
                }
            }
        }

        /// <summary>
        /// Saves the string table into a main stream.
        /// This updates the underlaying table holding the offsets to match the main stream.
        /// </summary>
        public void SaveStream(BinaryStream bs)
        {
            int basePos = (int)bs.Position;
            foreach (var strEntry in StringMeta)
            {
                switch (_stringCoding)
                {
                    case StringCoding.ByteCharCount:
                        bs.WriteByte((byte)(Encoding.GetByteCount(strEntry.Key) + (NullTerminated ? 1 : 0))); break;
                    case StringCoding.Int16CharCount:
                        bs.WriteInt16((short)(Encoding.GetByteCount(strEntry.Key) + (NullTerminated ? 1 : 0))); break;
                    case StringCoding.Int32CharCount:
                        bs.WriteInt32(Encoding.GetByteCount(strEntry.Key) + (NullTerminated ? 1 : 0)); break;
                }

                if (NullTerminated)
                    bs.WriteString(strEntry.Key, StringCoding.ZeroTerminated);
                else
                    bs.WriteString(strEntry.Key, StringCoding.Raw);

                bs.Align(_alignment);
            }

            if (!IsRelativeOffsets)
            {
                // Update the offsets - kinda inefficient way to do it
                foreach (var strEntry in StringMeta.Keys.ToList())
                    StringMeta[strEntry] += basePos;
            }
        }

        /// <summary>
        /// Gets the offset of a string within the binary table.
        /// Save stream should've already been called.
        /// </summary>
        public int GetStringOffset(string str)
        {
            return StringMeta[str];
        }
    }
}