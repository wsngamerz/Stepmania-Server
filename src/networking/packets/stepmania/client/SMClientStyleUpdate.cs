using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    /// <summary>
    /// This is sent when a style is chosen.
    /// !IMPORTANT! This command is unrelated to Server Command 0x06 (006). !IMPORTANT! 
    /// </summary>
    class SMClientStyleUpdate : Packet
    {
        private int _length;
        private int _command;
        private Dictionary<string, object> _data;

        public int NumPlayers; // players on system
        public string Player1Name;
        public string Player2Name;

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
            Dictionary<string, object> data = new Dictionary<string, object>();

            // number of enabled players (1 or 2) on the client
            NumPlayers = PacketUtils.ReadByte(binaryReader);
            data.Add("numPlayers", NumPlayers);
            
            for (int i = 0; i < NumPlayers; i++)
            {
                // the player number (0: P1 or 1: P2)
                int playerNumber = PacketUtils.ReadByte(binaryReader);

                // The player name for the specified player number
                if (playerNumber == 0)
                {
                    Player1Name = PacketUtils.ReadNTString(binaryReader);
                    data.Add("player1Name", Player1Name);
                }
                else
                {
                    Player2Name = PacketUtils.ReadNTString(binaryReader);
                    data.Add("player2Name", Player2Name);
                }
            }

            Data = data;

            logger.Trace("Recieved Style Update - Players: {players}, P1Name: {player1Name}, P2Name: {player2Name}", NumPlayers, Player1Name, Player2Name);
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }
}
