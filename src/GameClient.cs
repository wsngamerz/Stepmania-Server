using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

using NLog;



namespace StepmaniaServer
{
    /// <summary>
    /// Holds methods and functions relating to the game client
    /// </summary>
    class GameClient
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static Config config = new Config();

        private TcpClient tcpClient;
        private NetworkStream tcpStream;
        private BinaryReader tcpReader;
        private BinaryWriter tcpWriter;

        public GameRoom CurrentRoom;
        public SMScreen CurrentScreen;
        public string ClientBuild;
        public int ClientProtocolVersion;

        public int NumberPlayers;
        public User Player1;
        public User Player2;
        public string Player1Name;
        public string Player2Name;

        /// <summary>
        /// the game client is a representation of a connection to the server
        /// </summary>
        /// <param name="clientConnection"></param>
        public GameClient(TcpClient clientConnection)
        {
            tcpClient = clientConnection;
            tcpStream = clientConnection.GetStream();
            tcpReader = new BinaryReader(tcpStream);
            tcpWriter = new BinaryWriter(tcpStream);
        }

        /// <summary>
        /// check for new packets sent by the client
        /// </summary>
        public void Update()
        {
            if (tcpClient.Available > 0)
            {
                // read the packet that has been recieved
                Packet recievedPacket = ReadPacket();

                // decide what to do with each packet command
                switch((SMClientCommand)recievedPacket.Command)
                {
                    case SMClientCommand.Ping:
                        break;

                    case SMClientCommand.PingR:
                        logger.Trace("Recieved a ping response");
                        break;

                    case SMClientCommand.Hello:
                        ClientBuild = (string)recievedPacket.Data["clientBuild"];
                        ClientProtocolVersion = (int)recievedPacket.Data["clientProtocolVersion"];

                        logger.Trace("Connection request from client {clientInfo} build {build} protocol {protocol}", tcpClient.Client.RemoteEndPoint.ToString(), ClientBuild, ClientProtocolVersion);

                        SendHello();
                        break;
                    
                    case SMClientCommand.GameStartRequest:
                        CurrentRoom.ClientReady();
                        break;
                    
                    case SMClientCommand.GameOverNotice:
                        break;
                    
                    case SMClientCommand.GameStatusUpdate:
                        break;
                    
                    case SMClientCommand.StyleUpdate:
                        NumberPlayers = (int)recievedPacket.Data["numPlayers"];
                        
                        object player1Name;
                        object player2Name;
                        
                        recievedPacket.Data.TryGetValue("player1Name", out player1Name);
                        recievedPacket.Data.TryGetValue("player2Name", out player2Name);
                        
                        Player1Name = (string)player1Name;
                        Player2Name = (string)player2Name;
                        break;
                    
                    case SMClientCommand.ChatMessage:
                        break;
                    
                    case SMClientCommand.RequestStartGame:
                        string songTitle = (string)recievedPacket.Data["requestedSongTitle"];
                        string songSubitle = (string)recievedPacket.Data["requestedSongSubtitle"];
                        string songArtist = (string)recievedPacket.Data["requestedSongArtist"];
                        CurrentRoom.StartGame(ServerRequestStartGame.BlindlyPlay, songTitle, songArtist, songSubitle);
                        break;

                    case SMClientCommand.ScreenChanged:
                        CurrentScreen = (SMScreen)recievedPacket.Data["screenStatus"];
                        HandleScreenChange();
                        break;
                    
                    case SMClientCommand.PlayerOptions:
                        break;
                    
                    case SMClientCommand.SMOnlinePacket:
                        SMOPacket smoPacket = (SMOPacket)recievedPacket;
                        HandleSMOPacket(smoPacket);
                        break;
                    
                    case SMClientCommand.XMLPacket:
                        break;
                }
            }
        }

        /// <summary>
        /// read the tcp stream and return the packet
        /// </summary>
        /// <returns>Packet</returns>
        private Packet ReadPacket()
        {
            // read the packet length and packet command from stream
            int packetLength = PacketUtils.ReadLength(tcpReader);
            byte packetCommand = PacketUtils.ReadByte(tcpReader);
            logger.Trace("Recieved {packet} length {length}", (SMClientCommand)packetCommand, packetLength);

            // get the packet recieved
            Packet packetRecieved = PacketFactory.GetPacket(packetCommand);
            packetRecieved.Length = packetLength;
            packetRecieved.Command = packetCommand;

            // parse the payload and return the packet
            packetRecieved.Read(tcpReader);
            return packetRecieved;
        }

        /// <summary>
        /// decide what to do with the smo packet
        /// </summary>
        /// <param name="packet">SMOPacket</param>
        private void HandleSMOPacket(SMOPacket packet)
        {
            switch((SMOClientCommand)packet.SMOCommand)
            {
                case SMOClientCommand.Login:
                    LoginUser(packet);
                    break;
                
                case SMOClientCommand.EnterRoom:
                    bool isEnter = (bool)packet.Data["isEnter"];

                    if (isEnter)
                    {
                        string roomName = (string)packet.Data["enterRoomName"];
                        string roomPassword = (string)packet.Data["enterRoomPassword"];
                        Room room = StepmaniaServer.dbContext.Rooms.Where(s => s.Name == roomName).SingleOrDefault();
                        bool successfullyEntered = CurrentRoom.RoomManager.EnterRoom(room, roomPassword, this);

                        if (successfullyEntered)
                        {
                            SendRoomEntered(roomName, room.Description);
                        }
                    }
                    break;
                
                case SMOClientCommand.CreateRoom:
                    string name = (string)packet.Data["newRoomName"];
                    string description = (string)packet.Data["newRoomDescription"];
                    string password = (string)packet.Data["newRoomPassword"];
                    
                    CurrentRoom.RoomManager.CreateRoom(name, description, password);
                    SendRoomList();
                    break;
                
                case SMOClientCommand.RoomInfo:
                    break;
            }
        }

        /// <summary>
        /// Internal changes that need to happen as a result of a screen change
        /// </summary>
        private void HandleScreenChange()
        {
            switch (CurrentScreen)
            {
                case SMScreen.EnteredScreenNetRoom:
                    logger.Trace("Client entered room selection screen");
                    SendRoomList();
                    CurrentRoom.RoomManager.MoveToDummyRoom(this);
                    break;
            }
        }

        /// <summary>
        /// Handle logins and authentication
        /// </summary>
        /// <param name="packet">Login SMO Packet</param>
        private void LoginUser(SMOPacket packet)
        {
            string username = (string)packet.Data["username"];
            string password = (string)packet.Data["password"];

            User existingUser = StepmaniaServer.dbContext.Users.Where(s => s.Username == username).SingleOrDefault();

            if (existingUser == null)
            {
                // register new account
                User newUser = new User()
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = username,
                    SMPassword = password
                };

                StepmaniaServer.dbContext.Add<User>(newUser);
                StepmaniaServer.dbContext.SaveChanges();

                // save user and send client a response
                SetUser(username, newUser);
                SendLoginResponse(true, "Successfully registered new account");
            }
            else
            {
                // attempt a login
                if (password == existingUser.SMPassword)
                {
                    // successful login
                    SetUser(username, existingUser);
                    SendLoginResponse(true, "Successfully logged in");
                }
                else {
                    // unsuccessful login
                    SendLoginResponse(false, "Incorrect password");
                }
            }
        }

        /// <summary>
        /// Distinguish player1 and player2 internally
        /// </summary>
        /// <param name="username">username sent by client</param>
        /// <param name="user">user in database</param>
        private void SetUser(string username, User user)
        {
            if (username == Player1Name)
            {
                Player1 = user;
                logger.Info("[Player 1] {username} successfully logged in", username);
            }
            else {
                Player2 = user;
                logger.Info("[Player 2] {username} successfully logged in", username);
            }
        }

        /// <summary>
        /// send a hello to the client
        /// </summary>
        private void SendHello()
        {
            // create a packet and a dictionary to store packet data
            Packet packet = new SMServerHello();
            Dictionary<string, object> packetData = new Dictionary<string, object>();

            // add data to packet to send
            packetData.Add("serverProtocolVersion", GameServer.ProtocolVersion);
            packetData.Add("serverName", GameServer.Name);
            packetData.Add("randomKey", new byte[] { 0, 0, 0, 0 });

            // send packet
            packet.Write(tcpWriter, packetData);
        }

        /// <summary>
        /// allows the game to start
        /// </summary>
        public void SendAllowGameStart()
        {
            // create a packet
            Packet packet = new SMServerAllowGameStart();

            // send packet
            packet.Write(tcpWriter, new Dictionary<string, object>());
        }

        /// <summary>
        /// send requests start game
        /// </summary>
        /// <param name="status">game status</param>
        /// <param name="title">the song's title</param>
        /// <param name="artist">the song's artist</param>
        /// <param name="subtitle">the song's subtitle</param>
        public void SendRequestStartGame(ServerRequestStartGame status, string title, string artist, string subtitle)
        {
            // create a packet and a dictionary to store packet data
            Packet packet = new SMServerRequestStartGame();
            Dictionary<string, object> packetData = new Dictionary<string, object>();

            // add data to packet to send
            packetData.Add("messageType", status);
            packetData.Add("songTitle", title);
            packetData.Add("songSubtitle", subtitle);
            packetData.Add("songArtist", artist);

            // send packet
            packet.Write(tcpWriter, packetData);
        }

        /// <summary>
        /// send login response
        /// </summary>
        /// <param name="isSuccess">whether the login was a success</param>
        /// <param name="loginResponse">a message to send alongside the status</param>
        private void SendLoginResponse(bool isSuccess, string loginResponse)
        {
            // create a packet and a dictionary to store packet data
            SMOPacket packet = new SMOServerLogin();
            Dictionary<string, object> packetData = new Dictionary<string, object>();

            // add data to packet to send
            packetData.Add("isSuccess", isSuccess);
            packetData.Add("loginResponse", loginResponse);

            // send packet
            packet.Write(tcpWriter, packetData);
        }

        /// <summary>
        /// send list of rooms
        /// </summary>
        private void SendRoomList()
        {
            List<Room> rooms = StepmaniaServer.dbContext.Rooms.ToList();
            List<Tuple<string, string>> formattedRooms = new List<Tuple<string, string>>();

            // pull all required data and place in a List of Tuples
            foreach (Room room in rooms)
            {
                string roomName = room.Name;
                string roomDescription = room.Description;

                formattedRooms.Add(new Tuple<string, string>(roomName, roomDescription));
            }

            // create a packet and a dictionary to store packet data
            SMOPacket packet = new SMOServerRoomUpdate();
            Dictionary<string, object> packetData = new Dictionary<string, object>();

            // add data to packet to send
            packetData.Add("update", "rooms");
            packetData.Add("numberRooms", formattedRooms.Count);
            packetData.Add("rooms", formattedRooms);

            // send packet
            packet.Write(tcpWriter, packetData);
        }

        /// <summary>
        /// send room entered
        /// </summary>
        /// <param name="name">name of the room</param>
        /// <param name="description">room description</param>
        private void SendRoomEntered(string name, string description)
        {
            // create a packet and a dictionary to store packet data
            SMOPacket packet = new SMOServerRoomUpdate();
            Dictionary<string, object> packetData = new Dictionary<string, object>();

            // add data to packet to send
            packetData.Add("update", "title");
            packetData.Add("roomTitle", name);
            packetData.Add("roomDescription", description);
            packetData.Add("isGame", true);
            packetData.Add("allowSubroom", false);

            //send packet
            packet.Write(tcpWriter, packetData);
        }
    }
}
