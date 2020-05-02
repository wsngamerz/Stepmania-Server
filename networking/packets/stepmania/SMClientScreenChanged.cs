using System.Collections.Generic;
using System.IO;


namespace StepmaniaServer
{
    class SMClientScreenChanged : Packet
    {
        private int _length;
        private int _command;

        private int ScreenStatus;

        public override int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public override int Command
        {
            get { return (int)SMClientCommand.ScreenChanged; }
            set { _command = value; }
        }

        public override void Read(BinaryReader binaryReader)
        {
            ScreenStatus = PacketUtils.ReadByte(binaryReader);
            logger.Trace("Recieved Screen Change - Screen: {screenStatus}", ScreenStatus);
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }
}
