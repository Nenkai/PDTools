using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Syroot.BinaryData;

namespace PDTools.GT4ElfBuilderTool
{
    public class ElfBuilder
    {
        public const int BssSize = 0x800000;

        // Section addresses
        // Compared with GT4 First Preview ELF (proper elf) to find sections

        // GT4O_US:
        // - ctors: 0x65F4F0 to 0x65FA28
        // - dtors: 0x65FA28 to 0x65FBA0
        // - REGINFO: 0x65FBA0 to 0x65FBBC
        // - .erx_lib: 0x6DF648 to 0x6E0CA0
        // - .erx_stub: 0x6E0CA0 to 0x6E0D80
        // - .rodata: 0x6E0D80 to 0x731182
        // - .rdata: 0x731182 to 0x732200
        // - .eh_frame: 0x732200 to 0x734000
        // - .gcc_except_table: 0x734000 to 734200
        // - .sbss: 0x734200 to 0x734700
        // VTables: 0x6A417C to 0x6DF1D0

        // GT4_EU
        // - ctors: 0x616F24 to 0x6174F4
        // - dtors: 0x6174F4 to 0x6179FC
        // - REGINFO: 0x6179F8 to 0x617A14
        // - .erx_lib: 0x68A418 to 0x68B9A0
        // - .erx_stub: 0x68B9A0 to 0x68BA80
        // - .rodata: 0x68BA80 to 0x6D3280
        // - .rdata: 0x6D3280 to 0x6D3280
        // VTables at 0x659A24 to 0x68A418

        // GT3_EU (SCES-50294)
        // - ctors: 0x2902AC to 0x290464
        // - dtors: 0x29046C to 0x29052C
        // - data: 

        public void BuildFromInfo(string fileName, GTImageLoader file)
        {
            using var ms = new FileStream(fileName, FileMode.Create);
            using var bs = new BinaryStream(ms);

            // e_ident
            bs.WriteByte(0x7F);
            bs.WriteString("ELF", StringCoding.Raw);
            bs.WriteByte((byte)ElfEnums.Bits.B32);
            bs.WriteByte((byte)ElfEnums.Endian.Le);
            bs.WriteByte((byte)1); // Version current
            bs.WriteByte((byte)(ElfEnums.OsAbi)0);
            bs.WriteByte(0); // Abi Version
            bs.Position += 6; // Pad
            bs.WriteByte(0); // nident size

            bs.WriteInt16((short)ElfEnums.e_type32_e.ET_EXEC);
            bs.WriteInt16((short)ElfEnums.Machine.Mips);
            bs.WriteInt32(1); // EV Version
            bs.WriteInt32(file.EntryPoint); // Entrypoint


            bs.WriteInt32(52); // phOffset write later
            bs.WriteInt32(0); // shOffset write later
            bs.WriteInt32(546455553); // flags
            bs.WriteInt16(52); // Elf header size
            bs.WriteInt16(0x20); // Program header size 
            bs.WriteInt16((short)file.Segments.Count);
            bs.WriteInt16(40); // Section header entry size
            bs.WriteInt16(0); // section header count - writen later
            bs.WriteInt16(0); // string table section index - writen later

            // Heuristic to find .data for older games than GT4 where only 1 segment is provided, by attempting to find the ctor table
            if (file.Segments.Count == 1)
            {
                var fullSegment = file.Segments[0];

                using var segms = new MemoryStream(file.Segments[0].Data);
                using var segbs = new BinaryStream(segms);

                int counter = 0;
                uint last = 0;
                for (int i = 0; i < fullSegment.Data.Length; i += 4)
                {
                    uint val = segbs.ReadUInt32();
                    if (val > last && val < 0x800000)
                    {
                        counter++;

                        // Vtables are typically increasing offsets, 10 should be enough
                        if (counter >= 10)
                        {
                            // Potentially found .data, adjust segments

                            int dataOffset = (int)(segms.Position - (counter * sizeof(int)));
                            int oldFullSize = fullSegment.Size;
                            int dataSize = oldFullSize - dataOffset;

                            file.Segments.Add(new ElfSegment()
                            {
                                Name = ".data",
                                OffsetInElf = fullSegment.OffsetInElf + dataOffset,
                                Data = fullSegment.Data.AsSpan(dataOffset, dataSize).ToArray(),
                                Size = dataSize,
                                TargetOffset = fullSegment.TargetOffset + dataOffset
                            });

                            // Readjust full segment to just be .text
                            fullSegment.Size = dataOffset;
                            fullSegment.Data = fullSegment.Data.AsSpan(0, fullSegment.Size).ToArray();
                            break;
                        }

                        last = val;
                    }
                    else
                    {
                        counter = 0;
                        last = 0;
                    }
                }
            }

            // Write program headers
            WriteProgramHeaders(bs, file);

            int sh_num = WriteSectionHeaders(bs, file, out int shOffset);
            bs.Position = 0x20;
            bs.WriteInt32((int)shOffset);

            bs.Position = 0x2C;
            bs.WriteInt16((short)file.Segments.Count);

            bs.Position = 0x30;
            bs.WriteInt16((short)sh_num);
            bs.WriteInt16((short)(sh_num - 1));
        }

        private void WriteProgramHeaders(BinaryStream bs, GTImageLoader file)
        {
            long cPos = bs.Position;
            bs.Position = 0x1000;

            long segOffset = bs.Position;
            foreach (ElfSegment segment in file.Segments)
            {
                segment.OffsetInElf = segOffset;
                bs.WriteBytes(segment.Data);

                segOffset += segment.Data.Length;
            }


            long lastPos = bs.Position;
            bs.Position = cPos;

            foreach (ElfSegment segment in file.Segments)
            {
                if (segment.Name == ".text")
                {
                    // .text 
                    bs.WriteInt32((int)ElfEnums.PhType.Load);
                    bs.WriteInt32((int)segment.OffsetInElf);
                    bs.WriteInt32(segment.TargetOffset); // Virtual address
                    bs.WriteInt32(segment.TargetOffset); // Physical address
                    bs.WriteInt32(segment.Size); // File length
                    bs.WriteInt32(segment.Size); // Ram length
                    bs.WriteInt32(7); // Flags, PF_Read_Write_Exec
                    bs.WriteInt32(0x1000); // Align
                }
                else if (segment.Name == ".data")
                {
                    // .data
                    bs.WriteInt32((int)ElfEnums.PhType.Load);
                    bs.WriteInt32((int)segment.OffsetInElf);
                    bs.WriteInt32(segment.TargetOffset); // Virtual address
                    bs.WriteInt32(segment.TargetOffset); // Physical address
                    bs.WriteInt32(segment.Size); // File length
                    bs.WriteInt32(segment.Size); // Ram length
                    bs.WriteInt32(6); // Flags, PF_Read_Write
                    bs.WriteInt32(0x1000); // Align
                }
                else if (segment.Name == ".reginfo")
                {
                    // .reginfo
                    bs.WriteInt32((int)ElfEnums.PhType.PT_LOPROC);
                    bs.WriteInt32((int)segment.OffsetInElf);
                    bs.WriteInt32(segment.TargetOffset); // Virtual address
                    bs.WriteInt32(segment.TargetOffset); // Physical address
                    bs.WriteInt32(segment.Size); // File length
                    bs.WriteInt32(segment.Size); // Ram length
                    bs.WriteInt32(4); // Flags, PF_Read
                    bs.WriteInt32(4); // Align
                }
            }

            bs.Position = lastPos;
        }

        private int WriteSectionHeaders(BinaryStream bs, GTImageLoader file, out int shOffset)
        {
            long shstrTabOffset = bs.Position;
            Dictionary<string, long> dir = new Dictionary<string, long>();
            dir.Add("", bs.Position - shstrTabOffset);
            bs.WriteString("", StringCoding.ZeroTerminated);

            foreach (ElfSegment seg in file.Segments)
            {
                dir.Add(seg.Name, bs.Position - shstrTabOffset);
                bs.WriteString(seg.Name, StringCoding.ZeroTerminated);
            }

            dir.Add(".bss", bs.Position - shstrTabOffset);
            bs.WriteString(".bss", StringCoding.ZeroTerminated);

            dir.Add(".shstrtab", bs.Position - shstrTabOffset);
            bs.WriteString(".shstrtab", StringCoding.ZeroTerminated);

            long shstrtabLen = bs.Position - shstrTabOffset;
            bs.Align(0x04);

            shOffset = (int)bs.Position;
            bs.WriteInt32((int)ElfEnums.SectionHeaderIdxSpecial.Undefined);
            bs.WriteInt32(0); // Type
            bs.WriteInt32(0); // Flags
            bs.WriteInt32(0); // Addr
            bs.WriteInt32(0); // Offset
            bs.WriteInt32(0); // Size
            bs.WriteInt32(0); // Link
            bs.WriteInt32(0); // Info
            bs.WriteInt32(0); // Addralign
            bs.WriteInt32(0); // EntSize

            int bssOffset;
            int bssElfOffset;

            foreach (ElfSegment segment in file.Segments)
            {
                if (segment.Name == ".text")
                {
                    // .text 
                    bs.WriteInt32((int)dir[".text"]);
                    bs.WriteInt32(1); // Type
                    bs.WriteInt32(6); // Flags
                    bs.WriteInt32(file.Segments[0].TargetOffset); // Addr
                    bs.WriteInt32((int)file.Segments[0].OffsetInElf); // Offset
                    bs.WriteInt32(file.Segments[0].Size); // Size
                    bs.WriteInt32(0); // Link
                    bs.WriteInt32(0); // Info
                    bs.WriteInt32(0x40); // Addralign
                    bs.WriteInt32(0); // EntSize
                }
                else if (segment.Name == ".data")
                {
                    // .data
                    bs.WriteInt32((int)dir[".data"]);
                    bs.WriteInt32(1); // Type
                    bs.WriteInt32(2); // Flags
                    bs.WriteInt32(segment.TargetOffset); // Addr
                    bs.WriteInt32((int)segment.OffsetInElf); // Offset
                    bs.WriteInt32(segment.Size); // Size
                    bs.WriteInt32(0); // Link
                    bs.WriteInt32(0); // Info
                    bs.WriteInt32(0x40); // Addralign
                    bs.WriteInt32(0); // EntSize
                }
                else if (segment.Name == ".reginfo")
                {
                    // .reginfo
                    bs.WriteInt32((int)dir[".reginfo"]);
                    bs.WriteInt32(1879048198); // Type
                    bs.WriteInt32(2); // Flags
                    bs.WriteInt32(segment.TargetOffset); // Addr
                    bs.WriteInt32((int)segment.OffsetInElf); // Offset
                    bs.WriteInt32(segment.Size); // Size
                    bs.WriteInt32(0); // Link
                    bs.WriteInt32(0); // Info
                    bs.WriteInt32(4); // Addralign
                    bs.WriteInt32(1); // EntSize
                }
            }

            bssOffset = (int)file.Segments[^1].TargetOffset + file.Segments[^1].Size;
            bssElfOffset = (int)file.Segments[^1].OffsetInElf + file.Segments[^1].Size;

            bs.WriteInt32((int)dir[".bss"]);
            bs.WriteInt32(8); // Type
            bs.WriteInt32(3); // Flags
            bs.WriteInt32(bssOffset); // Addr
            bs.WriteInt32(bssElfOffset); // Offset
            bs.WriteInt32(BssSize); // Size
            bs.WriteInt32(0); // Link
            bs.WriteInt32(0); // Info
            bs.WriteInt32(0x40); // Addralign
            bs.WriteInt32(0); // EntSize

            bs.WriteInt32((int)dir[".shstrtab"]);
            bs.WriteInt32(3); // Type
            bs.WriteInt32(0); // Flags
            bs.WriteInt32(0); // Addr
            bs.WriteInt32((int)shstrTabOffset); // Offset
            bs.WriteInt32((int)shstrtabLen); // Size
            bs.WriteInt32(0); // Link
            bs.WriteInt32(0); // Info
            bs.WriteInt32(1); // Addralign
            bs.WriteInt32(0); // EntSize

            return dir.Count;
        }
    }
}
