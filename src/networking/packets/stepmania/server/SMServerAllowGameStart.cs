using System;
using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    /// <summary>
    /// packet sent to the client by the server to check whether the client is still connected properly
    /// </summary>
    class SMServerAllowGameStart : Packet
    {
        private int _length;
        private int _command;
        private Dictionary<string, object> _data;

        public override int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public override int Command
        {
            get { return (int)SMServerCommand.AllowGameStart; }
            set { _command = value; }
        }

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        public override void Read(BinaryReader binaryReader) { }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data)
        {
            // calculate the length of the packet
            Length = 1;

            // create a memorystream to hold the packet to send
            MemoryStream packetStream = new MemoryStream(Length + 4);
            PacketUtils.WriteLength(packetStream, Length);
            PacketUtils.WriteByte(packetStream, (byte)Command);

            // convert MemoryStream to byte array
            byte[] packetPayload = packetStream.GetBuffer();
            logger.Trace("Sending allow game start");

            // send the byte array
            binaryWriter.Write(packetPayload);
            binaryWriter.Flush();
        }
    }
}
