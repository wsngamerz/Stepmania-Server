using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    // The first packet sent to the server by the client
    class SMClientHello : Packet
    {
        private int _length;
        private int _command;
        private Dictionary<string, object> _data;

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

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        public override void Read(BinaryReader binaryReader)
        {
            // Client protocol Version:
            // 1 - Stepmania 3.9
            // 2 - Stepmania 3.95
            // 3 - Stepmania 4 alphas, sm-ssc v1.0-1.2.5, Stepmania 5
            // 4 - Stepmania 5 alphas?
            ClientProtocolVersion = PacketUtils.ReadByte(binaryReader);

            // stepmania build name
            ClientBuild = PacketUtils.ReadNTString(binaryReader);

            // save information
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("clientProtocolVersion", ClientProtocolVersion);
            data.Add("clientBuild", ClientBuild);
            Data = data;

            logger.Trace("Recieved Hello from client {build} protocol {protocol}", ClientBuild, ClientProtocolVersion);
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }
}
