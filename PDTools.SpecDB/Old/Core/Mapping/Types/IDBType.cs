using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;
namespace SpecDBOld.Mapping.Types
{
    public interface IDBType
    {
        void Serialize(BinaryStream bs);
    }
}
