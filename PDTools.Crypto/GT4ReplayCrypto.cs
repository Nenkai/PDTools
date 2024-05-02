using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace PDTools.Crypto
{
    public class GT4ReplayCrypto
    {
        public const float Mult1 = 0.22973239f;
        public const float Mult2 = 0.69584757f;

        /// <summary>
        /// Do not use. Does not work due to PS2 floats.
        /// </summary>
        /// <param name="encReplay"></param>
        /// <returns></returns>
        public static bool DecryptReplay(byte[] encReplay)
        {
            if (SharedCrypto.EncryptUnit_Decrypt(encReplay, encReplay.Length, 0, 0.22973239f, 0.69584757f, useMt: false, bigEndian: false, randomUpdateOld1_OldVersion: false) == -1)
                return false;

            // Read Serialize header
            // Names are from GTHD 1668.elf (with asserts)
            Span<byte> serializeHeader = encReplay.AsSpan(0x08);
            int header_size = BinaryPrimitives.ReadInt32LittleEndian(serializeHeader);
            uint data_size = BinaryPrimitives.ReadUInt32LittleEndian(serializeHeader.Slice(0x04));
            int minor_version = BinaryPrimitives.ReadInt16LittleEndian(serializeHeader.Slice(0x08));
            int major_version = BinaryPrimitives.ReadInt16LittleEndian(serializeHeader.Slice(0x0A)); // GT4: 4.0, GT4O: 5.0
            uint encode_param = BinaryPrimitives.ReadUInt32LittleEndian(serializeHeader.Slice(0x0C)); // aka CRC
            // _type_name[0] = v4 + 0x10

            Span<byte> data = serializeHeader.Slice(header_size);
            // NOTE: Data cannot be decoded. PS2 float fail - Might be intentional from PDI
            // CRC32_PS2_Float(data, data, dataSize, encode_param);

            return false;
        }

        /*
        /// <summary>
        /// THIS FUNCTION DOES NOT WORK - LOGIC IS CORRECT - BUT PS2 FLOATS PROBLEMS
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="length"></param>
        /// <param name="crc"></param>
        public static void CRC32_PS2_Float(Span<byte> input, Span<byte> output, uint length, uint crc)
        {
            float currentFloat = BitConverter.Int32BitsToSingle((int)(crc & 0xFFFFFF | 0x3F000000));

            for (var i = 0; i < length; i++)
            {
                int currentVal = BitConverter.SingleToInt32Bits(currentFloat);
                byte inByte = input[i];
                output[i] = (byte)(inByte ^ (currentVal >> 16 & 0xFF) ^ (currentVal >> 8 & 0xFF) ^ (currentVal & 0xFF));
                currentFloat = (FloatTable[inByte] * currentFloat) / BitConverter.Int32BitsToSingle( (ShufTable[(byte)(currentVal & 0xFF)] << 16) | (ShufTable[(currentVal >> 8) & 0xFF] << 8) | (ShufTable[(currentVal >> 16) & 0xFF]) | 0x3F000000);

            }

            // CRC check should be here
        }*/

        private static float[] FloatTable = new float[256]
        {
            1.001955f, 1.0058651f, 1.0097752f, 1.0136852f, 1.0175953f,
            1.0215054f, 1.0254154f, 1.0293255f, 1.0332355f, 1.0371456f,
            1.0410557f, 1.0449657f, 1.0488758f, 1.0527859f, 1.0566959f,
            1.060606f, 1.0645161f, 1.0684261f, 1.0723362f, 1.0762463f,
            1.0801563f, 1.0840664f, 1.0879765f, 1.0918865f, 1.0957966f,
            1.0997066f, 1.1036167f, 1.1075268f, 1.1114368f, 1.1153469f,
            1.119257f, 1.123167f, 1.1270772f, 1.1309873f, 1.1348974f,
            1.1388074f, 1.1427175f, 1.1466275f, 1.1505376f, 1.1544477f,
            1.1583577f, 1.1622678f, 1.1661779f, 1.1700879f, 1.173998f,
            1.1779081f, 1.1818181f, 1.1857282f, 1.1896383f, 1.1935483f,
            1.1974584f, 1.2013685f, 1.2052785f, 1.2091886f, 1.2130986f,
            1.2170087f, 1.2209188f, 1.2248288f, 1.2287389f, 1.232649f,
            1.236559f, 1.2404691f, 1.2443792f, 1.2482892f, 1.2521994f,
            1.2561095f, 1.2600195f, 1.2639296f, 1.2678397f, 1.2717497f,
            1.2756598f, 1.2795699f, 1.2834799f, 1.28739f, 1.2913001f,
            1.2952101f, 1.2991202f, 1.3030303f, 1.3069403f, 1.3108504f,
            1.3147604f, 1.3186705f, 1.3225806f, 1.3264906f, 1.3304007f,
            1.3343108f, 1.3382208f, 1.3421309f, 1.346041f, 1.349951f,
            1.3538611f, 1.3577712f, 1.3616812f, 1.3655913f, 1.3695014f,
            1.3734114f, 1.3773216f, 1.3812317f, 1.3851417f, 1.3890518f,
            1.3929619f, 1.3968719f, 1.400782f, 1.4046921f, 1.4086021f,
            1.4125122f, 1.4164222f, 1.4203323f, 1.4242424f, 1.4281524f,
            1.4320625f, 1.4359726f, 1.4398826f, 1.4437927f, 1.4477028f,
            1.4516128f, 1.4555229f, 1.459433f, 1.463343f, 1.4672531f,
            1.4711632f, 1.4750732f, 1.4789833f, 1.4828933f, 1.4868034f,
            1.4907135f, 1.4946235f, 1.4985336f, 1.5024438f, 1.5063539f,
            1.5102639f, 1.514174f, 1.518084f, 1.5219941f, 1.5259042f,
            1.5298142f, 1.5337243f, 1.5376344f, 1.5415444f, 1.5454545f,
            1.5493646f, 1.5532746f, 1.5571847f, 1.5610948f, 1.5650048f,
            1.5689149f, 1.572825f, 1.576735f, 1.5806451f, 1.5845551f,
            1.5884652f, 1.5923753f, 1.5962853f, 1.6001954f, 1.6041055f,
            1.6080155f, 1.6119256f, 1.6158357f, 1.6197457f, 1.6236558f,
            1.627566f, 1.631476f, 1.6353861f, 1.6392962f, 1.6432062f,
            1.6471163f, 1.6510264f, 1.6549364f, 1.6588465f, 1.6627566f,
            1.6666666f, 1.6705767f, 1.6744868f, 1.6783968f, 1.6823069f,
            1.686217f, 1.690127f, 1.6940371f, 1.6979471f, 1.7018572f,
            1.7057673f, 1.7096773f, 1.7135874f, 1.7174975f, 1.7214075f,
            1.7253176f, 1.7292277f, 1.7331377f, 1.7370478f, 1.7409579f,
            1.7448679f, 1.748778f, 1.7526882f, 1.7565982f, 1.7605083f,
            1.7644184f, 1.7683284f, 1.7722385f, 1.7761486f, 1.7800586f,
            1.7839687f, 1.7878788f, 1.7917888f, 1.7956989f, 1.7996089f,
            1.803519f, 1.8074291f, 1.8113391f, 1.8152492f, 1.8191593f,
            1.8230693f, 1.8269794f, 1.8308895f, 1.8347995f, 1.8387096f,
            1.8426197f, 1.8465297f, 1.8504398f, 1.8543499f, 1.8582599f,
            1.86217f, 1.86608f, 1.8699901f, 1.8739002f, 1.8778104f,
            1.8817204f, 1.8856305f, 1.8895406f, 1.8934506f, 1.8973607f,
            1.9012707f, 1.9051808f, 1.9090909f, 1.9130009f, 1.916911f,
            1.9208211f, 1.9247311f, 1.9286412f, 1.9325513f, 1.9364613f,
            1.9403714f, 1.9442815f, 1.9481915f, 1.9521016f, 1.9560117f,
            1.9599217f, 1.9638318f, 1.9677418f, 1.9716519f, 1.975562f,
            1.979472f, 1.9833821f, 1.9872922f, 1.9912022f, 1.9951123f,
            1.9990224f,
        };

        public static byte[] ShufTable = new byte[256]
        {
            0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10,
            0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0, 0x08, 0x88,
            0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58,
            0xD8, 0x38, 0xB8, 0x78, 0xF8, 0x04, 0x84, 0x44, 0xC4,
            0x24, 0xA4, 0x64, 0xE4, 0x14, 0x94, 0x54, 0xD4, 0x34,
            0xB4, 0x74, 0xF4, 0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC,
            0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C,
            0xFC, 0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2,
            0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2, 0x0A,
            0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A,
            0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA, 0x06, 0x86, 0x46,
            0xC6, 0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6,
            0x36, 0xB6, 0x76, 0xF6, 0x0E, 0x8E, 0x4E, 0xCE, 0x2E,
            0xAE, 0x6E, 0xEE, 0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE,
            0x7E, 0xFE, 0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61,
            0xE1, 0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1,
            0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 0x19,
            0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9, 0x05, 0x85,
            0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95, 0x55,
            0xD5, 0x35, 0xB5, 0x75, 0xF5, 0x0D, 0x8D, 0x4D, 0xCD,
            0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D,
            0xBD, 0x7D, 0xFD, 0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3,
            0x63, 0xE3, 0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73,
            0xF3, 0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB,
            0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB, 0x07,
            0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 0x17, 0x97,
            0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7, 0x0F, 0x8F, 0x4F,
            0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F, 0x5F, 0xDF,
            0x3F, 0xBF, 0x7F, 0xFF,
        };
    }
}
