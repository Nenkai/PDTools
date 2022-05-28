using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using PDTools.Crypto;

namespace GTToolsSharp.Encryption
{
    public class ChaCha20Stream : Stream
    {
        public ChaCha20 _chacha;
        public Stream BaseStream { get; set; }
        public long _BasePos;

        public ChaCha20Stream(Stream baseStream, byte[] key, byte[] iv)
        {
            BaseStream = baseStream;
            _BasePos = BaseStream.Position; 
            _chacha = new ChaCha20(key, iv, 0);
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => BaseStream.Length;

        public override long Position { get => BaseStream.Position; set => BaseStream.Position = value; }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ulong cryptoPos = (ulong)(BaseStream.Position - _BasePos);

            BaseStream.Read(buffer, 0, count);
            _chacha.DecryptBytes(buffer, count, cryptoPos);
            return count;
        }

        public void SetBasePosition(long basePos)
        {
            _BasePos = basePos;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
