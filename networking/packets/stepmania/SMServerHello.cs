using System.Collections.Generic;
using System.IO;


namespace StepmaniaServer
{
    class SMServerHello : Packet
    {
        private int _length;
        private int _command;

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

        public override void Read(BinaryReader binaryReader) { }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data)
        {
            int serverProtocolVersion = (int)data["serverProtocolVersion"];
            string serverName = (string)data["serverName"];
            byte[] randomKey = (byte[])data["randomKey"];

            Length = 7 + serverName.Length;
            MemoryStream packetStream = new MemoryStream(Length + 4);

            PacketUtils.WriteLength(packetStream, Length);
            PacketUtils.WriteByte(packetStream, (byte)Command);
            PacketUtils.WriteByte(packetStream, (byte)serverProtocolVersion);
            PacketUtils.WriteNTString(packetStream, serverName);
            PacketUtils.WriteBytes(packetStream, randomKey);

            byte[] packetPayload = packetStream.GetBuffer();

            logger.Trace("Packet {command} sending {payload}", Command, PacketUtils.ByteArrayToString(packetPayload));

            binaryWriter.Write(packetPayload);
        }
    }
}
