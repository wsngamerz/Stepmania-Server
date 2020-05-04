using System;
using System.Collections.Generic;
using System.IO;


namespace StepmaniaServer
{
    // packet sent to the client by the server in response to the client's
    // hello packet to introduce the server to the client.
    class SMServerPing : Packet
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
            get { return (int)SMServerCommand.Hello; }
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
            logger.Trace("Sending Ping, {payload}", PacketUtils.ByteArrayToString(packetPayload));

            // send the byte array
            try
            {
                binaryWriter.Write(packetPayload);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error sending ping, client disconnected?");
            }
        }
    }
}
