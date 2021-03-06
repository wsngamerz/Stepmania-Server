using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    /// <summary>
    /// sent when the user joins a room or the user changes their options
    /// </summary>
    class SMClientPlayerOptions : Packet
    {
        private int _length;
        private int _command;
        private Dictionary<string, object> _data;

        public string Player1Options;
        public string Player2Options;

        public override int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public override int Command
        {
            get { return (int)SMClientCommand.PlayerOptions; }
            set { _command = value; }
        }

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        public override void Read(BinaryReader binaryReader)
        {
            // read in the player options
            Player1Options = PacketUtils.ReadNTString(binaryReader);
            Player2Options = PacketUtils.ReadNTString(binaryReader);

            // save them in a dictionary
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("player1Options", Player1Options);
            data.Add("player2Options", Player2Options);
            Data = data;
            
            logger.Trace("Recieved PlayerOptions: [1: {p1o}] [2: {p2o}]", Player1Options, Player2Options);
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }
}
