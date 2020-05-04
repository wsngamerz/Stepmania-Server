using System;
using System.IO;



namespace StepmaniaServer
{
    // a magic class which is super helpful for writing and parsing packets
    class PacketUtils
    {
        // reads a single byte
        public static byte ReadByte(BinaryReader binaryReader)
        {
            return binaryReader.ReadByte();
        }

        // writes a single byte
        public static void WriteByte(MemoryStream memoryStream, byte data)
        {
            byte[] rawData = new byte[] { data };
            memoryStream.Write(rawData);
        }

        // writes an array of bytes
        public static void WriteBytes(MemoryStream memoryStream, byte[] data) {
            memoryStream.Write(data);
        }
        
        // reads the length section of a packet
        // first 4 bytes of a packet in LittleEndian format
        // which means that the bytes are flipped
        public static int ReadLength(BinaryReader binaryReader)
        {
            byte[] rawLength = binaryReader.ReadBytes(4);
            Array.Reverse(rawLength);
            
            return Convert.ToInt32(BitConverter.ToUInt32(rawLength));
        }

        // writes the 4 byte length of a packet and correctly flips
        // for little endian format
        public static void WriteLength(MemoryStream memoryStream, int data)
        {
            byte[] rawLength = BitConverter.GetBytes(Convert.ToUInt32(data));
            Array.Reverse(rawLength);

            memoryStream.Write(rawLength);
        }

        // reads a null terminated string
        public static string ReadNTString(BinaryReader binaryReader)
        {
            string ntString = "";
            byte readByte = 0x00;

            do
            {
                readByte = binaryReader.ReadByte();
                if (readByte != 0x00)
                {
                    ntString += Convert.ToChar(readByte);
                }
            } while (readByte != 0x00);

            return ntString;
        }

        // writes a null terminated string
        public static void WriteNTString(MemoryStream memoryStream, string data)
        {
            byte[] rawString = new byte[data.Length + 1];

            for (int i = 0; i < data.Length; i++)
            {
                rawString[i] = (byte)data[i];
            }
            rawString[data.Length] = 0x00;

            memoryStream.Write(rawString);
        }

        // utility function to convert an array of bytes into a human readable
        // hexadecimal string
        public static string ByteArrayToString(byte[] data) {
            return BitConverter.ToString(data).Replace("-", " ");
        }
    }
}
