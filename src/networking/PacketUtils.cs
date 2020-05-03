using System;
using System.IO;


namespace StepmaniaServer
{
    class PacketUtils
    {
        public static byte ReadByte(BinaryReader binaryReader)
        {
            return binaryReader.ReadByte();
        }

        public static void WriteByte(MemoryStream memoryStream, byte data)
        {
            byte[] rawData = new byte[] { data };
            memoryStream.Write(rawData);
        }

        public static void WriteBytes(MemoryStream memoryStream, byte[] data) {
            memoryStream.Write(data);
        }

        public static int ReadLength(BinaryReader binaryReader)
        {
            byte[] rawLength = binaryReader.ReadBytes(4);
            Array.Reverse(rawLength);
            
            return Convert.ToInt32(BitConverter.ToUInt32(rawLength));
        }

        public static void WriteLength(MemoryStream memoryStream, int data)
        {
            byte[] rawLength = BitConverter.GetBytes(Convert.ToUInt32(data));
            Array.Reverse(rawLength);

            memoryStream.Write(rawLength);
        }

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

        public static string ByteArrayToString(byte[] data) {
            return BitConverter.ToString(data).Replace("-", " ");
        }
    }
}
