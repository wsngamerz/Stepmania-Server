using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    // packet for the servers login response
    class SMOServerLogin : SMOPacket
    {
        private int _length;
        private int _command;
        private int _smocommand;
        private Dictionary<string, object> _data;

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

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        public override void Read(BinaryReader binaryReader) { }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data)
        {
            // approval status
            bool isSuccess = (bool)data["success"];

            // login response (plaintext)
            string loginResponse = (string)data["loginResponse"];

            // calculate length
            Length = 4 + loginResponse.Length;
            MemoryStream packetStream = new MemoryStream(Length + 4);

            // populate memorystream with packet
            PacketUtils.WriteLength(packetStream, Length);
            PacketUtils.WriteByte(packetStream, (byte)Command);
            PacketUtils.WriteByte(packetStream, (byte)SMOCommand);
            PacketUtils.WriteByte(packetStream, (byte)(isSuccess ? 0 : 1));
            PacketUtils.WriteNTString(packetStream, loginResponse);

            // convert to bytearray
            byte[] packetPayload = packetStream.GetBuffer();

            // send packet
            binaryWriter.Write(packetPayload);
            binaryWriter.Flush();
        }
    }
}
