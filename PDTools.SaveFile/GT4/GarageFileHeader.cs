using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.IO;

using Syroot.BinaryData.Memory;

using PDTools.Crypto;
using PDTools.Hashing;
using PDTools.Structures.PS2;

namespace PDTools.SaveFile.GT4;

public class GarageFile
{
    public const int GarageFileHeaderSize = 0x40;

    public const int GarageCarSizeAligned_Retail = 0x500;
    public const int GarageCarSizeAligned_GT4O = 0x540;

    public const int GarageCarSize_Retail = 0x4C0; // Used for CRC

    public int GarageCarSizeAligned { get; set; }
    public int GarageCarSize { get; set; }

    public bool UseOldRandomUpdateCrypto { get; set; } = true;

    public ulong Money { get; set; }
    public int Key1 { get; set; }
    public int Key2 { get; set; }
    public int LastSystemTimeMicrosecond { get; set; }
    public uint UniqueID { get; set; }
    public int SystemTimeMicrosecond { get; set; }

    public List<Memory<byte>> Cars { get; set; } = new List<Memory<byte>>(1000);

    public void CopyTo(GarageFile dest)
    {
        dest.GarageCarSizeAligned = GarageCarSizeAligned;
        dest.GarageCarSize = GarageCarSize;
        dest.UseOldRandomUpdateCrypto = UseOldRandomUpdateCrypto;

        dest.Money = Money;
        dest.Key1 = Key1;
        dest.Key2 = Key2;
        dest.LastSystemTimeMicrosecond = LastSystemTimeMicrosecond;
        dest.UniqueID = UniqueID;
        dest.SystemTimeMicrosecond = SystemTimeMicrosecond;

        for (var i = 0; i < 1000; i++)
        {
            byte[] data = new byte[Cars[i].Length];
            Cars[i].CopyTo(data);
            dest.Cars.Add(data);
        }
    }

    /// <summary>
    /// Loads the garage.
    /// </summary>
    /// <param name="save"></param>
    /// <param name="garageFilePath"></param>
    /// <param name="key"></param>
    /// <param name="useOldRandomUpdateCrypto"></param>
    public void Load(GT4Save save, string garageFilePath, uint key, bool useOldRandomUpdateCrypto)
    {
        UseOldRandomUpdateCrypto = useOldRandomUpdateCrypto;

        IdentifyGarageCarSize(save.Type);

        byte[] garageFile = File.ReadAllBytes(garageFilePath);
        decryptHeader(garageFile, key);

        ReadHeader(garageFile);

        for (uint i = 0; i < 1000; i++)
        {
            Memory<byte> carBuffer = garageFile.AsMemory(GarageFileHeaderSize + (GarageCarSizeAligned * (int)i), GarageCarSizeAligned);
            Cars.Add(carBuffer);
        }

#if DEBUG
        File.WriteAllBytes("garage_with_decrypted_header.bin", garageFile);
#endif
    }

    public void IdentifyGarageCarSize(GT4SaveType saveType)
    {
        if (GT4Save.IsGT4Online(saveType))
        {
            GarageCarSizeAligned = GarageCarSizeAligned_GT4O;
            GarageCarSize = GarageCarSize_Retail; // FIXME
        }
        else
        {
            GarageCarSizeAligned = GarageCarSizeAligned_Retail;
            GarageCarSize = GarageCarSize_Retail;
        }
    }

    /// <summary>
    /// Gets a car by index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public CarGarage? GetCar(uint index)
    {
        if (index >= 1000)
            return null;

        var carBuffer = Cars[(int)index];
        if (!DecryptCar(carBuffer, UniqueID, index))
            return null;

        SpanReader sr = new SpanReader(carBuffer.Span);
        sr.Position += 8; // Skip encrypt header

        CarGarage car = new CarGarage();
        car.Unpack(ref sr);

        return car;
    }

    public void PushCar(CarGarage garageCar, uint key, uint carIndex)
    {
        if (carIndex >= 1000)
            throw new IndexOutOfRangeException("Can't push a car to index > 1000.");

        Memory<byte> buffer = Cars[(int)carIndex];
        buffer.Span.Clear();

        SpanWriter sw = new SpanWriter(buffer.Span);
        sw.Position = 8;
        garageCar.Pack(ref sw, GarageCarSizeAligned == GarageCarSizeAligned_GT4O);
        EncryptCarBuffer(buffer, key, carIndex);
    }

    /// <summary>
    /// Save the garage file to the file system.
    /// </summary>
    /// <param name="fileName"></param>
    public void Save(string fileName)
    {
        byte[] buffer = new byte[GarageFileHeaderSize + (GarageCarSizeAligned * 1000)];
        SpanWriter sw = new SpanWriter(buffer);
        sw.WriteUInt32((uint)(Money >> 32));
        sw.WriteUInt32((uint)Money);

        for (var i = 0; i < 9; i++)
            sw.WriteInt32(0);

        sw.WriteInt32(Key1);
        sw.WriteInt32(Key2);
        sw.WriteInt32(LastSystemTimeMicrosecond);
        sw.WriteUInt32(UniqueID);
        sw.WriteInt32(SystemTimeMicrosecond);

        encryptHeader(buffer.AsMemory(0, GarageFileHeaderSize));

        for (var i = 0; i < 1000; i++)
            sw.WriteBytes(Cars[i].Span);

        File.WriteAllBytes(fileName, buffer);
    }

    /// <summary>
    /// Reads the header.
    /// </summary>
    /// <param name="buffer"></param>
    private void ReadHeader(Span<byte> buffer)
    {
        SpanReader sr = new SpanReader(buffer);
        Money = (ulong)sr.ReadUInt32() << 32 | sr.ReadUInt32();

        for (var i = 0; i < 9; i++)
            sr.ReadInt32();

        Key1 = sr.ReadInt32();
        Key2 = sr.ReadInt32();
        LastSystemTimeMicrosecond = sr.ReadInt32();
        UniqueID = sr.ReadUInt32();
        SystemTimeMicrosecond = sr.ReadInt32();
    }

    /// <summary>
    /// Decrypts the garage file header.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="key"></param>
    public void decryptHeader(Memory<byte> buffer, uint key)
    {
        Span<uint> hdr = MemoryMarshal.Cast<byte, uint>(buffer.Span);

        // Keep original key around; this will be changed by RandomUpdateOld1
        uint ogKey = key;

        int seed1 = SharedCrypto.RandomUpdateOld1(ref key, UseOldRandomUpdateCrypto);
        int seed2 = SharedCrypto.RandomUpdateOld1(ref key, UseOldRandomUpdateCrypto);

        // reverse shuffle decrypt bytes
        var rand = new MTRandom((uint)seed2);
        SharedCrypto.reverse_shufflebit(buffer, 0x40, rand); 

        uint ciph = (uint)(hdr[15] ^ seed1);
        hdr[15] = ciph;

        // Decrypt data
        rand = new MTRandom(ogKey + ciph);
        for (int i = 0; i < 0x3C; i++)
            buffer.Span[i] ^= (byte)rand.getInt32();
    }

    /// <summary>
    /// Encrypts the garage file header.
    /// </summary>
    /// <param name="buffer"></param>
    public void encryptHeader(Memory<byte> buffer)
    {
        Span<uint> hdr = MemoryMarshal.Cast<byte, uint>(buffer.Span);

        uint key = hdr[14];
        var rand = new MTRandom(key + hdr[15]);
        
        for (int i = 0; i < 0x3C; i++)
            buffer.Span[i] ^= (byte)rand.getInt32();

        hdr[15] ^= (uint)SharedCrypto.RandomUpdateOld1(ref key, UseOldRandomUpdateCrypto);
        var updated = SharedCrypto.RandomUpdateOld1(ref key, UseOldRandomUpdateCrypto);

        rand = new MTRandom((uint)updated);
        SharedCrypto.shufflebit(buffer, 0x40, rand); // Shuffle encrypt
    }

    /* Major note regarding the garage file encryption (more specifically: cars):
     * To begin with: each car is encrypted, normally with a key tweaked with the car index.
     * 
     * When the game first creates the garage file, it encrypts the header normally, and it mallocs the size of 1000 car entries all initialized to 0. 
     * The key only is taken as a seed, RandomUpdateOld1 + rand value is used only, no shuffling.
     * It happens in one go until the end of the file - 0x40 to the end. There is no CRC.
     * 
     * When car slots are in use, the method below with the car index are used. 
     * For documentation purposes, the method CreateEntriesEncrypt below describes how it's done.
     * 
     * This is done so that even if no data is actually stored in the garage, garbage is generated that cannot be decrypted
     * because: 
     * - it doesn't care about the input buffer, it's not xorred against it even if it's not 0's. It's only creating garbage anyway.
     * - when the first slot is used by valid data, it then means that the initial state is compromised anyway since it started at 0x40 (after header length).
     * 
     * Basically just obfuscation >_<
     */

    private void CreateGarageData(uint uniqueIdKey)
    {
        byte[] data = new byte[GarageFileHeaderSize + (1000 * GarageCarSizeAligned)];

        var rand = new MTRandom(uniqueIdKey);
        for (var i = 0x40; i < data.Length; i++)
            data[i] = (byte)(rand.getInt32() ^ (byte)SharedCrypto.RandomUpdateOld1(ref uniqueIdKey, UseOldRandomUpdateCrypto));

        // Header creation would go here
    }

    /* This one is the one that decrypts cars normally when the slots are in use. */
    private bool DecryptCar(Memory<byte> carBuffer, uint uniqueIdKey, uint carIndex)
    {
        // Reverse shuffle decrypt whole blob
        var rand = new MTRandom(uniqueIdKey + carIndex);
        SharedCrypto.reverse_shufflebit(carBuffer, GarageCarSizeAligned, rand);

        uint time = BinaryPrimitives.ReadUInt32LittleEndian(carBuffer.Span);
        uint crc = BinaryPrimitives.ReadUInt32LittleEndian(carBuffer.Span[4..]);

        // Decrypt data
        uint seed = crc;
        rand = new MTRandom(uniqueIdKey ^ time);
        Span<uint> bufInts = MemoryMarshal.Cast<byte, uint>(carBuffer.Span[8..]);
        for (var i = 0; i < (GarageCarSizeAligned - 8) / 4; i++)
            bufInts[i] = (bufInts[i] + (uint)SharedCrypto.RandomUpdateOld1(ref seed, UseOldRandomUpdateCrypto)) ^ (uint)rand.getInt32();

        if (crc == CRC32.crc32_0x77073096(carBuffer.Span[8..], GarageCarSize))
            return true;

        return false;
    }

    /// <summary>
    /// Decrypts a car buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="key"></param>
    /// <param name="carIndex"></param>
    private void EncryptCarBuffer(Memory<byte> buffer, uint key, uint carIndex)
    {
        // Header
        uint time = (uint)new Random().Next(); // Normally PDISTD::GetSystemTimeMicroSecond();
        uint crc = CRC32.crc32_0x77073096(buffer[8..].Span, GarageCarSize);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer[0..].Span, time);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer[4..].Span, crc);

        // Encrypt data
        var rand = new MTRandom(key ^ time);
        Span<uint> bufInts = MemoryMarshal.Cast<byte, uint>(buffer.Span);
        uint seed = crc;
        for (var i = 2; i < GarageCarSizeAligned / sizeof(int); i++)
            bufInts[i] = (bufInts[i] ^ (uint)rand.getInt32()) - (uint)SharedCrypto.RandomUpdateOld1(ref seed, UseOldRandomUpdateCrypto);

        // Shuffle encrypt whole buffer
        rand = new MTRandom(key + carIndex);
        SharedCrypto.shufflebit(buffer, GarageCarSizeAligned, rand);
    }
}
