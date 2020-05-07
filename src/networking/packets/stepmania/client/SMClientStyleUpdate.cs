using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    //!IMPORTANT! This command is unrelated to Server Command 0x06 (006). !IMPORTANT!
    // This is sent when a style is chosen.
    class SMClientStyleUpdate : Packet
    {
        private int _length;
        private int _command;
        private Dictionary<string, object> _data;

        private int NumPlayers; // players on system
        private int PlayerNum; // specified player
        private string PlayerName; // player name

        public override int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public override int Command
        {
            get { return (int)SMClientCommand.StyleUpdate; }
            set { _command = value; }
        }

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        public override void Read(BinaryReader binaryReader)
        {
            // number of enabled players (1 or 2) on the client
            NumPlayers = PacketUtils.ReadByte(binaryReader);

            // the player number (0: P1 or 1: P2)
            PlayerNum = PacketUtils.ReadByte(binaryReader);

            // The player name for the specified player number
            PlayerName = PacketUtils.ReadNTString(binaryReader);
            
            // store the data for packet recieved in the Data dictionary
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("numPlayers", NumPlayers);
            data.Add("playerNum", PlayerNum);
            data.Add("playerName", PlayerName);
            Data = data;

            logger.Trace("Recieved Style Update - Players: {players} ID: {playerID} Name: {playerName}", NumPlayers, PlayerNum, PlayerName);
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }
}
