using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Syroot.BinaryData;

namespace PDTools.GT4ElfBuilderTool
{
    // Warning: Dirty
    public class ElfBuilder
    {
        public const int BssSize = 0x200000;

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
            bs.WriteInt16(3);
            bs.WriteInt16(40); // Section header entry size
            bs.WriteInt16(6); // section header count
            bs.WriteInt16(5); // string table section index

            // Write program headers
            WriteProgramHeaders(bs, file);

            WriteSectionHeaders(bs, file, out int shOffset);
            bs.Position = 0x20;
            bs.WriteInt32((int)shOffset);
        }

        private void WriteProgramHeaders(BinaryStream bs, GTImageLoader file)
        {
            long cPos = bs.Position;

            bs.Position = 0x1000;
            long textOffset = bs.Position;
            file.Segments[1].OffsetInElf = textOffset;
            bs.WriteBytes(file.Segments[1].Data);
            bs.Align(0x1000, grow: true);

            long regInfoSectionOffset = bs.Position;
            file.Segments[0].OffsetInElf = regInfoSectionOffset;
            bs.WriteBytes(file.Segments[0].Data);
            bs.Align(4, grow: true);

            long dataSectionOffset = bs.Position;
            file.Segments[2].OffsetInElf = dataSectionOffset;
            bs.WriteBytes(file.Segments[2].Data);
            bs.Position += BssSize; // to fit bss?
            bs.Align(0x1000, grow: true);

            long lastPos = bs.Position;
            bs.Position = cPos;

            // .text 
            bs.WriteInt32((int)ElfEnums.PhType.Load);
            bs.WriteInt32((int)textOffset);
            bs.WriteInt32(file.Segments[1].TargetOffset); // Virtual address
            bs.WriteInt32(file.Segments[1].TargetOffset); // Physical address
            bs.WriteInt32(file.Segments[1].Size - 0x18); // File length
            bs.WriteInt32(file.Segments[1].Size - 0x18); // Ram length
            bs.WriteInt32(7); // Flags, PF_Read_Write_Exec
            bs.WriteInt32(0x1000); // Align

            // .reginfo
            bs.WriteInt32((int)ElfEnums.PhType.PT_LOPROC);
            bs.WriteInt32((int)regInfoSectionOffset);
            bs.WriteInt32(file.Segments[0].TargetOffset); // Virtual address
            bs.WriteInt32(file.Segments[0].TargetOffset); // Physical address
            bs.WriteInt32(file.Segments[0].Size); // File length
            bs.WriteInt32(file.Segments[0].Size); // Ram length
            bs.WriteInt32(4); // Flags, PF_Read
            bs.WriteInt32(4); // Align

            // .data
            bs.WriteInt32((int)ElfEnums.PhType.Load);
            bs.WriteInt32((int)dataSectionOffset);
            bs.WriteInt32(file.Segments[2].TargetOffset); // Virtual address
            bs.WriteInt32(file.Segments[2].TargetOffset); // Physical address
            bs.WriteInt32(file.Segments[2].Size); // File length
            bs.WriteInt32(file.Segments[2].Size + BssSize); // Ram length
            bs.WriteInt32(6); // Flags, PF_Read_Write
            bs.WriteInt32(0x1000); // Align

            bs.Position = lastPos;
        }

        private void WriteSectionHeaders(BinaryStream bs, GTImageLoader file, out int shOffset)
        {
            long shstrTabOffset = bs.Position;
            Dictionary<string, long> dir = new Dictionary<string, long>();
            dir.Add("", bs.Position - shstrTabOffset);
            bs.WriteString("", StringCoding.ZeroTerminated);

            dir.Add(".text", bs.Position - shstrTabOffset);
            bs.WriteString(".text", StringCoding.ZeroTerminated);

            dir.Add(".reginfo", bs.Position - shstrTabOffset);
            bs.WriteString(".reginfo", StringCoding.ZeroTerminated);

            dir.Add(".data", bs.Position - shstrTabOffset);
            bs.WriteString(".data", StringCoding.ZeroTerminated);

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

            bs.WriteInt32((int)dir[".text"]);
            bs.WriteInt32(1); // Type
            bs.WriteInt32(6); // Flags
            bs.WriteInt32(file.Segments[1].TargetOffset); // Addr
            bs.WriteInt32((int)file.Segments[1].OffsetInElf); // Offset
            bs.WriteInt32(file.Segments[1].Size); // Size
            bs.WriteInt32(0); // Link
            bs.WriteInt32(0); // Info
            bs.WriteInt32(0x40); // Addralign
            bs.WriteInt32(0); // EntSize

            bs.WriteInt32((int)dir[".reginfo"]);
            bs.WriteInt32(1879048198); // Type
            bs.WriteInt32(2); // Flags
            bs.WriteInt32(file.Segments[0].TargetOffset); // Addr
            bs.WriteInt32((int)file.Segments[0].OffsetInElf); // Offset
            bs.WriteInt32(file.Segments[0].Size); // Size
            bs.WriteInt32(0); // Link
            bs.WriteInt32(0); // Info
            bs.WriteInt32(4); // Addralign
            bs.WriteInt32(1); // EntSize

            bs.WriteInt32((int)dir[".data"]);
            bs.WriteInt32(1); // Type
            bs.WriteInt32(2); // Flags
            bs.WriteInt32(file.Segments[2].TargetOffset); // Addr
            bs.WriteInt32((int)file.Segments[2].OffsetInElf); // Offset
            bs.WriteInt32(file.Segments[2].Size); // Size
            bs.WriteInt32(0); // Link
            bs.WriteInt32(0); // Info
            bs.WriteInt32(0x40); // Addralign
            bs.WriteInt32(0); // EntSize

            bs.WriteInt32((int)dir[".bss"]);
            bs.WriteInt32(8); // Type
            bs.WriteInt32(3); // Flags
            bs.WriteInt32((int)file.Segments[2].TargetOffset + file.Segments[2].Size); // Addr
            bs.WriteInt32((int)file.Segments[2].OffsetInElf + file.Segments[2].Size); // Offset
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
        }
    }
}
