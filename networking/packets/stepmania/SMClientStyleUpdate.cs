using System.Collections.Generic;
using System.IO;


namespace StepmaniaServer
{
    class SMClientStyleUpdate : Packet
    {
        private int _length;
        private int _command;

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

        public override void Read(BinaryReader binaryReader)
        {
            NumPlayers = PacketUtils.ReadByte(binaryReader);
            PlayerNum = PacketUtils.ReadByte(binaryReader);
            PlayerName = PacketUtils.ReadNTString(binaryReader);
            logger.Trace("Recieved Style Update - Players: {players} ID: {playerID} Name: {playerName}", NumPlayers, PlayerNum, PlayerName);
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }
}
