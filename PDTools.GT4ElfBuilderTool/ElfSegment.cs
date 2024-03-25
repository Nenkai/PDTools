using System;
using System.Collections.Generic;
using System.Text;

namespace PDTools.GT4ElfBuilderTool
{
    public class ElfSegment
    {
        public string Name { get; set; }

        public int TargetOffset { get; set; }
        public int Size { get; set; }

        public byte[] Data { get; set; }

        /// <summary>
        /// Used for serializing
        /// </summary>
        public long OffsetInElf { get; set; }
    }
}
