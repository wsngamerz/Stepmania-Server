using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    // Sent in response to the server sending a ping to the client
    class SMClientPingR : Packet
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
            get { return (int)SMClientCommand.PingR; }
            set { _command = value; }
        }

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        // The ping response packet has no payload so no parsing takes place
        public override void Read(BinaryReader binaryReader) { }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }
}
