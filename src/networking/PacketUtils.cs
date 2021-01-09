using System;
using System.IO;



namespace StepmaniaServer
{
    /// <summary>
    /// a util class which is super helpful for writing and parsing packets
    /// </summary>
    class PacketUtils
    {
        /// <summary>
        /// reads a single byte
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        public static byte ReadByte(BinaryReader binaryReader)
        {
            return binaryReader.ReadByte();
        }

        /// <summary>
        /// writes a single byte
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <param name="data"></param>
        public static void WriteByte(MemoryStream memoryStream, byte data)
        {
            byte[] rawData = new byte[] { data };
            memoryStream.Write(rawData);
        }

        /// <summary>
        /// reads an array of bytes
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] ReadBytes(BinaryReader binaryReader, int count)
        {
            return binaryReader.ReadBytes(count);
        }

        // writes an array of bytes
        public static void WriteBytes(MemoryStream memoryStream, byte[] data) {
            memoryStream.Write(data);
        }
        
        /// <summary>
        /// reads the length section of a packet first 4 bytes of a packet in LittleEndian format which means that the
        /// bytes are flipped
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        public static int ReadLength(BinaryReader binaryReader)
        {
            byte[] rawLength = binaryReader.ReadBytes(4);
            Array.Reverse(rawLength);
            
            return Convert.ToInt32(BitConverter.ToUInt32(rawLength));
        }

        /// <summary>
        /// writes the 4 byte length of a packet and correctly flips for little endian format
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <param name="data"></param>
        public static void WriteLength(MemoryStream memoryStream, int data)
        {
            byte[] rawLength = BitConverter.GetBytes(Convert.ToUInt32(data));
            Array.Reverse(rawLength);

            memoryStream.Write(rawLength);
        }

        /// <summary>
        /// reads a null terminated string
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
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

        /// <summary>
        /// writes a null terminated string
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <param name="data"></param>
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

        /// <summary>
        /// utility function to convert an array of bytes into a human readable hexadecimal string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] data) {
            return BitConverter.ToString(data).Replace("-", " ");
        }
    }
}
