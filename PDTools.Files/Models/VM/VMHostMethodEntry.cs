using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.VM
{
    public class VMHostMethodEntry
    {
        public string Name { get; set; }
        public short StorageDataSize { get; set; }
        public short StorageID { get; set; }

        public static VMHostMethodEntry FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            VMHostMethodEntry registerVal = new();

            int strOffset = bs.ReadInt32();
            registerVal.StorageDataSize = bs.ReadInt16();
            registerVal.StorageID = bs.ReadInt16();

            bs.Position = mdlBasePos + strOffset;

            // first will be empty so skip it
            registerVal.Name = bs.ReadString(StringCoding.ZeroTerminated);

            return registerVal;
        }

        public override string ToString()
        {
            return $"{Name} (ID: {StorageID}, Size: {StorageDataSize})";
        }
    }
}
