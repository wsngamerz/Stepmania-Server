using System.Collections.Generic;
using System.IO;


namespace StepmaniaServer
{
    class SMClientHello : Packet
    {
        private int _length;
        private int _command;

        public int ClientProtocolVersion;
        public string ClientBuild;

        public override int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public override int Command
        {
            get { return (int)SMClientCommand.Hello; }
            set { _command = value; }
        }

        public override void Read(BinaryReader binaryReader)
        {
            ClientProtocolVersion = PacketUtils.ReadByte(binaryReader);
            ClientBuild = PacketUtils.ReadNTString(binaryReader);
            logger.Trace("Recieved Hello from client {build} protocol {protocol}", ClientBuild, ClientProtocolVersion);
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }
}
