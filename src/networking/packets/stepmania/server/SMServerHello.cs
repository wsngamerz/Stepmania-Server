using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    // packet sent to the client by the server in response to the client's
    // hello packet to introduce the server to the client.
    class SMServerHello : Packet
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
            // server protocol version
            //     1: SMLan (Old)
            //   128: SMOnline
            //   129: SMOnline with "better song identification using chart hashes"
            int serverProtocolVersion = (int)data["serverProtocolVersion"];

            // servers name
            string serverName = (string)data["serverName"];

            // server random key
            byte[] randomKey = (byte[])data["randomKey"];

            // calculate the length of the packet
            Length = 7 + serverName.Length;

            // create a memorystream to hold the packet to send
            MemoryStream packetStream = new MemoryStream(Length + 4);

            // write data to the memory stream
            PacketUtils.WriteLength(packetStream, Length);
            PacketUtils.WriteByte(packetStream, (byte)Command);
            PacketUtils.WriteByte(packetStream, (byte)serverProtocolVersion);
            PacketUtils.WriteNTString(packetStream, serverName);
            PacketUtils.WriteBytes(packetStream, randomKey);

            // convert MemoryStream to byte array
            byte[] packetPayload = packetStream.GetBuffer();

            // send the byte array
            binaryWriter.Write(packetPayload);
            binaryWriter.Flush();
        }
    }
}
