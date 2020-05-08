using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    // The SMOnline Packet is a wrapper of sorts for the SMOnline protocol
    class SMClientSMOnline : SMOPacket
    {
        private int _length;
        private int _command;
        private int _smocommand;
        private Dictionary<string, object> _data;

        // the sub-protocol command ID
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

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }


        public override void Read(BinaryReader binaryReader)
        {
            // The SMOCommand is the first byte of the payload
            SMOnlineCommand = PacketUtils.ReadByte(binaryReader);
            SMOCommand = SMOnlineCommand;

            // decide which smo command to parse the rest of the payload for
            // NOTE: At the moment, it is easier to parse all of the clients
            //       SMO commands within a single class rather than create
            //       another packet system for SMO Client packets
            switch (SMOnlineCommand)
            {
                // Client sent Login Information for a Login Attempt
                case (int)SMOClientCommand.Login:
                    // Player Number
                    int playerNumber = PacketUtils.ReadByte(binaryReader);
                    
                    // Whether the password is encrypted
                    bool isEncrypted = PacketUtils.ReadByte(binaryReader) == 1 ? true : false;
                    
                    // username
                    string username = PacketUtils.ReadNTString(binaryReader);
                    
                    // password
                    string password = PacketUtils.ReadNTString(binaryReader);

                    // store all of the parsed data in the Data dictionary
                    Dictionary<string, object> loginData = new Dictionary<string, object>();
                    loginData.Add("playerNumber", playerNumber);
                    loginData.Add("isEncrypted", isEncrypted);
                    loginData.Add("username", username);
                    loginData.Add("password", password);
                    Data = loginData;

                    logger.Trace("[SMO] Login - Player Number: {playerNum} - Encrypted: {encrypted} - Username: {username} - Password: {password}", playerNumber, isEncrypted, username, password);
                    break;
                
                // Client entering/exiting room
                case (int)SMOClientCommand.EnterRoom:
                    // Whether is entering or exiting
                    bool isEnter = PacketUtils.ReadByte(binaryReader) == 1 ? true : false;

                    // room name (empty if exiting)
                    string enterRoomName = PacketUtils.ReadNTString(binaryReader);

                    // room password (empty if unused)
                    string enterRoomPassword = null;
                    if (Length > (4 + enterRoomName.Length)) {
                        enterRoomPassword = PacketUtils.ReadNTString(binaryReader);
                    }

                    // store all of the parsed data in the Data dictionary
                    Dictionary<string, object> enterRoomData = new Dictionary<string, object>();
                    enterRoomData.Add("isEnter", isEnter);
                    enterRoomData.Add("enterRoomName", enterRoomName);
                    enterRoomData.Add("enterRoomPassword", enterRoomPassword);
                    Data = enterRoomData;

                    logger.Trace("[SMO] {enterExit} Room - Name: {roomName} - Password: {roomPassword}", isEnter ? "Entering" : "Leaving", enterRoomName, enterRoomPassword);
                    break;
                
                // client creates a new room
                case (int)SMOClientCommand.CreateRoom:
                    // room type (game [no subrooms], normal [with subrooms])
                    bool isGame = PacketUtils.ReadByte(binaryReader) == 1 ? true : false;

                    // room title
                    string newRoomName = PacketUtils.ReadNTString(binaryReader);

                    // room description
                    string newRoomDescription = PacketUtils.ReadNTString(binaryReader);

                    // TODO: Handle an empty password as it crashes if the password is empty
                    
                    // room password (blank if no password)
                    string newRoomPassword = null;
                    if (Length > (5 + newRoomName.Length + newRoomDescription.Length)) {
                        newRoomPassword = PacketUtils.ReadNTString(binaryReader);
                    }

                    // store all of the parsed data in the Data dictionary
                    Dictionary<string, object> createRoomData = new Dictionary<string, object>();
                    createRoomData.Add("isGame", isGame);
                    createRoomData.Add("newRoomName", newRoomName);
                    createRoomData.Add("newRoomDescription", newRoomDescription);
                    createRoomData.Add("newRoomPassword", newRoomPassword);
                    Data = createRoomData;

                    logger.Trace("[SMO] Create Room - Name: {roomName} - Description: {roomDescription} - Password: {roomPassword}", newRoomName, newRoomDescription, newRoomPassword);
                    break;
                
                // Client is requesting room information
                case (int)SMOClientCommand.RoomInfo:
                    // room name
                    string roomName = PacketUtils.ReadNTString(binaryReader);

                    Dictionary<string, object> roomInfoData = new Dictionary<string, object>();
                    roomInfoData.Add("roomName", roomName);
                    Data = roomInfoData;

                    logger.Trace("[SMO] Room Info - Name: {roomName}", roomName);
                    break;
            }
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }
}
