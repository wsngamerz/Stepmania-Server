using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    // sent when user enters / exits a network screen
    class SMClientScreenChanged : Packet
    {
        private int _length;
        private int _command;
        private Dictionary<string, object> _data;

        private SMScreen ScreenStatus;

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

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        public override void Read(BinaryReader binaryReader)
        {
            // Screen Status:
            //     0 -> exited ScreenNetSelectMusic
            //     1 -> entered ScreenNetSelectMusic
            //     2 -> not sent
            //     3 -> entered Options
            //     4 -> exited ScreenNetEvaluation
            //     5 -> entered ScreenNetEvaluation
            //     6 -> exited ScreenNetRoom
            //     7 -> entered ScreenNetRoom
            ScreenStatus = (SMScreen)PacketUtils.ReadByte(binaryReader);

            // save the current position of the client in a dictionary
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("screenStatus", ScreenStatus);
            Data = data;
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }
}
