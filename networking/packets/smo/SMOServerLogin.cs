using System.Collections.Generic;
using System.IO;


namespace StepmaniaServer
{
    class SMOServerLogin : SMOPacket
    {
        private int _length;
        private int _command;
        private int _smocommand;

        public override int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public override int Command
        {
            get { return (int)SMServerCommand.SMOnlinePacket; }
            set { _command = value; }
        }

        public override int SMOCommand
        {
            get { return (int)SMOServerCommand.Login; }
            set { _smocommand = value; }
        }

        public override void Read(BinaryReader binaryReader) { }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data)
        {
            bool isSuccess = (bool)data["success"];
            string loginResponse = (string)data["loginResponse"];

            Length = 4 + loginResponse.Length;
            MemoryStream packetStream = new MemoryStream(Length + 4);

            PacketUtils.WriteLength(packetStream, Length);
            PacketUtils.WriteByte(packetStream, (byte)Command);
            PacketUtils.WriteByte(packetStream, (byte)SMOCommand);
            PacketUtils.WriteByte(packetStream, (byte)(isSuccess ? 0 : 1));
            PacketUtils.WriteNTString(packetStream, loginResponse);

            byte[] packetPayload = packetStream.GetBuffer();

            logger.Trace("Sending Login Response: {payload}", PacketUtils.ByteArrayToString(packetPayload));

            binaryWriter.Write(packetPayload);
        }
    }
}
