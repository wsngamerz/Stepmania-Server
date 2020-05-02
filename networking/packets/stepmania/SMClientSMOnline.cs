using System.Collections.Generic;
using System.IO;


namespace StepmaniaServer
{
    class SMClientSMOnline : SMOPacket
    {
        private int _length;
        private int _command;
        private int _smocommand;

        public int SMOnlineCommand;

        public override int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public override int Command
        {
            get { return (int)SMClientCommand.SMOnlinePacket; }
            set { _command = value; }
        }
        
        public override int SMOCommand
        {
            get { return _smocommand; }
            set { _smocommand = value; }
        }


        public override void Read(BinaryReader binaryReader)
        {
            SMOnlineCommand = PacketUtils.ReadByte(binaryReader);
            SMOCommand = SMOnlineCommand;

            switch (SMOnlineCommand)
            {
                case (int)SMOClientCommand.Login:
                    int playerNumber = PacketUtils.ReadByte(binaryReader);
                    bool isEncrypted = PacketUtils.ReadByte(binaryReader) == 1 ? true : false;
                    string username = PacketUtils.ReadNTString(binaryReader);
                    string password = PacketUtils.ReadNTString(binaryReader);

                    logger.Trace("[SMO] Login - Player Number: {playerNum} - Encrypted: {encrypted} - Username: {username} - Password: {password}", playerNumber, isEncrypted, username, password);
                    break;
                
                case (int)SMOClientCommand.EnterRoom:
                    bool isEnter = PacketUtils.ReadByte(binaryReader) == 1 ? true : false;
                    string enterRoomName = PacketUtils.ReadNTString(binaryReader);
                    string enterRoomPassword = PacketUtils.ReadNTString(binaryReader);

                    logger.Trace("[SMO] {enterExit} Room - Name: {roomName} - Password: {roomPassword}", isEnter ? "Entering" : "Leaving", enterRoomName, enterRoomPassword);
                    break;
                
                case (int)SMOClientCommand.CreateRoom:
                    bool isGame = PacketUtils.ReadByte(binaryReader) == 1 ? true : false;
                    string newRoomName = PacketUtils.ReadNTString(binaryReader);
                    string newRoomDescription = PacketUtils.ReadNTString(binaryReader);
                    string newRoomPassword = PacketUtils.ReadNTString(binaryReader);

                    logger.Trace("[SMO] Create Room - Name: {roomName} - Description: {roomDescription} - Password: {roomPassword}", newRoomName, newRoomDescription, newRoomPassword);
                    break;
                
                case (int)SMOClientCommand.RoomInfo:
                    string roomName = PacketUtils.ReadNTString(binaryReader);

                    logger.Trace("[SMO] Room Info - Name: {roomName}", roomName);
                    break;
            }
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }
}
