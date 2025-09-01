using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;

namespace PDTools.Utils.Tests;

[TestClass]
public sealed class BitStreamTests
{
    private readonly byte[] TestBytes = [0, 1, 2, 3, 4];

    [TestMethod]
    public void TestReadBitCheckPosition()
    {
        BitStream testRead = new BitStream(BitStreamMode.Read, TestBytes);

        testRead.ReadBits(1);
        Assert.AreEqual(0, testRead.Position);
        Assert.AreEqual(7u, testRead.BitCounter);
    }

    [TestMethod]
    public void TestReadOob()
    {
        Assert.ThrowsException<IndexOutOfRangeException>(() =>
        {
            BitStream testRead = new BitStream(BitStreamMode.Read, TestBytes);
            testRead.Position = 5;
            Assert.AreEqual(5, testRead.Position);

            testRead.ReadBits(1);
        });

        Assert.ThrowsException<IndexOutOfRangeException>(() =>
        {
            BitStream testRead = new BitStream(BitStreamMode.Read, TestBytes);
            testRead.SeekToBit(5 * 8);
            Assert.AreEqual(5, testRead.Position);

            testRead.ReadBits(1);
        });
    }

    [TestMethod]
    public void TestReadSeekToBitAndRead()
    {
        BitStream testRead = new BitStream(BitStreamMode.Read, TestBytes);

        testRead.SeekToBit(4);
        Assert.AreEqual(0, testRead.Position);
        Assert.AreEqual(4u, testRead.BitCounter);

        testRead.SeekToBit(8);
        Assert.AreEqual(1, testRead.Position);
        Assert.AreEqual(8u, testRead.BitCounter);

        testRead.SeekToBit(12);
        Assert.AreEqual(1, testRead.Position);
        Assert.AreEqual(4u, testRead.BitCounter);

        byte[] test2 = [0, 1 << 6];
        testRead = new BitStream(BitStreamMode.Read, test2);
        testRead.SeekToBit(9);
        Assert.AreEqual(true, testRead.ReadBoolBit());
    }

    [TestMethod]
    public void TestReadByteCheckPosition()
    {
        BitStream testRead = new BitStream(BitStreamMode.Read, TestBytes);
        testRead.ReadBits(1);
        Assert.AreEqual(7u, testRead.BitCounter);

        testRead.ReadBits(7);
        Assert.AreEqual(1, testRead.Position);
        Assert.AreEqual(8u, testRead.BitCounter);

        testRead.ReadBits(1);
        Assert.AreEqual(1, testRead.Position);
        Assert.AreEqual(7u, testRead.BitCounter);

        testRead.AlignToNextByte();
        Assert.AreEqual(2, testRead.Position);
        Assert.AreEqual(8u, testRead.BitCounter);
    }

    [TestMethod]
    public void TestReadSeek()
    {
        BitStream testRead = new BitStream(BitStreamMode.Read, TestBytes);
        testRead.ReadBits(4);

        testRead.Position = 3;
        Assert.AreEqual(3, testRead.Position);
        Assert.AreEqual(8u, testRead.BitCounter);
    }

    [TestMethod]
    public void TestReadGetBitPosition()
    {
        BitStream testRead = new BitStream(BitStreamMode.Read, TestBytes);
        testRead.ReadBits(4);
        Assert.AreEqual(4, testRead.GetBitPosition());

        testRead.ReadBits(4);
        Assert.AreEqual(8, testRead.GetBitPosition());

        testRead.ReadByte();
        Assert.AreEqual(16, testRead.GetBitPosition());

        for (int i = 7; i < 9; i++)
        {
            testRead.SeekToBit(i);
            Assert.AreEqual(i, testRead.GetBitPosition());
        }

        testRead.SeekToByte(TestBytes.Length);
        Assert.AreEqual(TestBytes.Length * BitStream.Byte_Bits, testRead.GetBitPosition());
    }

    [TestMethod]
    public void TestReadBitsAlignCheckPosition()
    {
        BitStream testRead = new BitStream(BitStreamMode.Read, TestBytes);
        testRead.ReadBits(1);
        testRead.ReadBits(7);
        Assert.AreEqual(1, testRead.Position);

        testRead.ReadBits(1);
        Assert.AreEqual(1, testRead.Position);

        testRead.AlignToNextByte();
        Assert.AreEqual(2, testRead.Position);

        byte val = testRead.ReadByte();
        Assert.AreEqual(2, val);
    }

    [TestMethod]
    public void TestWriteCheckPosition()
    {
        BitStream testWrite = new BitStream(BitStreamMode.Write, new byte[5]);
        Assert.AreEqual(0, testWrite.Position);

        testWrite.WriteBits(1, 1);
        Assert.AreEqual(0, testWrite.Position);
        Assert.AreEqual(1u, testWrite.BitCounter);

        testWrite.WriteBits(1, 1);
        Assert.AreEqual(0, testWrite.Position);
        Assert.AreEqual(2u, testWrite.BitCounter);

        testWrite.WriteBits(1, 6);
        Assert.AreEqual(1, testWrite.Position);
        Assert.AreEqual(0u, testWrite.BitCounter);

        testWrite.WriteBits(1, 1);
        testWrite.AlignToNextByte();
        Assert.AreEqual(2, testWrite.Position);

        testWrite.WriteBits(1, 1);
        Assert.AreEqual(2, testWrite.Position);
        Assert.AreEqual(1u, testWrite.BitCounter);
    }

    [TestMethod]
    public void TestWrite()
    {
        byte[] buf = new byte[5];
        BitStream testWrite = new BitStream(BitStreamMode.Write, buf);
        Assert.AreEqual(0, testWrite.Position);

        testWrite.WriteInt16(0x1234);
        Assert.AreEqual(0x12, buf[0]);
        Assert.AreEqual(0x34, buf[1]);
    }

    [TestMethod]
    public void TestWriteWithAlignByte()
    {
        byte[] buf = new byte[5];
        BitStream testWrite = new BitStream(BitStreamMode.Write, buf);

        testWrite.Position = 2;
        Assert.AreEqual(2, testWrite.Position);
        Assert.AreEqual(0u, testWrite.BitCounter);

        testWrite.WriteBits(0, 1);
        testWrite.WriteBits(1, 1);
        testWrite.AlignToNextByte();
        Assert.AreEqual(3, testWrite.Position);
        Assert.AreEqual(0, testWrite.CurrentByte);

        // LSB
        Assert.AreEqual(0x40, buf[2]);
    }

    [TestMethod]
    public void TestWriteSpecificByte()
    {
        byte[] buf = new byte[5];
        BitStream testWrite = new BitStream(BitStreamMode.Write, buf);
        testWrite.Position = 4;
        Assert.AreEqual(4, testWrite.Position);

        testWrite.WriteByte(4);
        Assert.AreEqual(5, testWrite.Position);
        Assert.AreEqual(4, buf[4]);
    }

    [TestMethod]
    public void TestWriteWithSeeks()
    {
        byte[] buf = new byte[5];
        BitStream testWrite = new BitStream(BitStreamMode.Write, buf, BitStreamSignificantBitOrder.MSB); // We'll use MSB (little endian) for this test

        // Byte 0
        testWrite.WriteBits(1, 4);
        testWrite.Position = 4;
        testWrite.WriteBits(4, 4);
        Assert.AreEqual(1, buf[0]);

        // Byte 1
        testWrite.Position = 1;
        testWrite.WriteBits(2, 4);
        testWrite.AlignToNextByte();
        Assert.AreEqual(2, buf[1]);

        // Byte 3
        testWrite.Position = 2;
        testWrite.WriteBits(3, 4);
        testWrite.SeekToBit(3 * BitStream.Byte_Bits);
        Assert.AreEqual(3, buf[2]);

        // Byte 4
        testWrite.Position = 3;
        testWrite.WriteBits(3, 4);
        testWrite.SeekToBit((3 * BitStream.Byte_Bits) + 3); // Seek to same position
        Assert.AreEqual(3, buf[2]);

        testWrite.Position = 5;
        testWrite.AlignToNextByte();

        testWrite.Position = 0;
        Assert.AreEqual(1, testWrite.CurrentByte);

       
    }

    [TestMethod]
    public void TestWriteSeekWriteCheckOob()
    {
        BitStream testWrite = new BitStream(BitStreamMode.Write, new byte[5]);
        
        Assert.ThrowsException<IndexOutOfRangeException>(() =>
        {
            BitStream testWrite = new BitStream(BitStreamMode.Write, new byte[5]);
            testWrite.Position = 5;
            testWrite.WriteBits(1, 1);
        });
    }

    [TestMethod]
    public void TestWriteArray()
    {
        BitStream testWrite = new BitStream(BitStreamMode.Write, new byte[5]);
        Assert.AreEqual(0u, testWrite.BitCounter);

        testWrite.WriteByteData([0xFF, 0xFF, 0xFF, 0xFF, 0xFF]);
        Assert.IsTrue(testWrite.GetBuffer().ToArray().All(e => e == 0xFF));
        Assert.AreEqual(5, testWrite.GetSpanToCurrentPosition().Length);
    }

    [TestMethod]
    public void TestWriteDynamicArray()
    {
        int offset = 1024;
        BitStream testDynamicStream = new BitStream();
        testDynamicStream.SeekToByte(offset);
        Assert.AreEqual(testDynamicStream.Position, offset);

        testDynamicStream.SeekToBit(offset * BitStream.Byte_Bits);
        Assert.AreEqual(testDynamicStream.Position, offset);

        testDynamicStream.WriteBoolBit(true);
        Assert.AreEqual(testDynamicStream.Length, offset + 1);

        Span<byte> span = testDynamicStream.GetSpanFromCurrentPosition(); // Get here -> end
        Assert.AreEqual(span.Length, offset); // Source buffer should have been doubled, therefore we get a span of the same length.
        Assert.AreEqual(testDynamicStream.SourceBuffer.Length, offset * 2);

        testDynamicStream.AlignToNextByte();
        span = testDynamicStream.GetSpanToCurrentPosition(); // Get start -> here
        Assert.AreEqual(span.Length, offset + 1); // We advanced by one
    }

    [TestMethod]
    public void TestWriteCheckLength()
    {
        int offset = 1024;
        BitStream testDynamicStream = new BitStream();
        testDynamicStream.SeekToByte(offset);

        for (int i = offset; i < offset + 256; i++)
        {
            testDynamicStream.WriteBits(4, 4);
            Assert.AreEqual(testDynamicStream.Length, i + 1);
            testDynamicStream.WriteBits(4, 4);
            Assert.AreEqual(testDynamicStream.Length, i + 1);
        }
    }

    [TestMethod]
    public void TestWriteMSBLSB()
    {
        Span<byte> buf = stackalloc byte[8];
        BitStream lsbWriter = new BitStream(BitStreamMode.Write, buf, BitStreamSignificantBitOrder.LSB);
        lsbWriter.WriteUInt64(0x123456789ABCDEF0);
        Assert.AreEqual(0x123456789ABCDEF0, BinaryPrimitives.ReadInt64BigEndian(buf));

        BitStream msbWriter = new BitStream(BitStreamMode.Write, buf, BitStreamSignificantBitOrder.MSB);
        msbWriter.WriteUInt64(0x123456789ABCDEF0);
        Assert.AreEqual(0x123456789ABCDEF0, BinaryPrimitives.ReadInt64LittleEndian(buf));
    }
}
