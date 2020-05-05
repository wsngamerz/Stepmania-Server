using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Linq;

using NLog;



namespace StepmaniaServer
{
    // represents a connected client
    class GameClient
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static Config config = new Config();

        public TcpClient tcpClient;
        public string clientInformation;
        public int updates;

        public User user = null;
        public Room currentRoom = null;

        private BinaryReader tcpReader;
        private BinaryWriter tcpWriter;
        private NetworkStream tcpStream;

        public GameClient(TcpClient client)
        {
            // store client information and TCP class
            tcpClient = client;
            clientInformation = tcpClient.Client.RemoteEndPoint.ToString();
            updates = 0;
            tcpStream = tcpClient.GetStream();

            // set config options
            tcpStream.ReadTimeout = Convert.ToInt32(config.Get("/config/game-server/timeout", "1000"));
            tcpReader = new BinaryReader(tcpStream);
            tcpWriter = new BinaryWriter(tcpStream);
        }

        public void Update()
        {
            // increment the number of updates
            updates++;

            // if client has sent data that has been recieved by the server
            if (tcpClient.Available > 0)
            {
                // read the length of the packet (first 4 bytes)
                int packetLength = PacketUtils.ReadLength(tcpReader);
                // read the command of the packet (next 1 byte)
                byte command = PacketUtils.ReadByte(tcpReader);

                // gets the class which handles that specific packet
                Packet packet = PacketFactory.GetPacket(command);
                packet.Length = packetLength;
                packet.Command = command;

                // get the packet class to parse the rest of the payload of the packet
                packet.Read(tcpReader);

                // decide what to do for each kind of packet
                switch (command)
                {
                    case (int)SMClientCommand.Hello:
                        // if recieved a hello from the client, that means that the client wants to
                        // connect to respond with server information
                        Dictionary<string, object> smHelloData = new Dictionary<string, object>();
                        Packet smHelloPacket = new SMServerHello();

                        // add data to packet to send
                        smHelloData.Add("serverProtocolVersion", Convert.ToInt32(config.Get("/config/game-server/protocol", "128")));
                        smHelloData.Add("serverName", config.Get("/config/game-server/name", "A Stepmania Server"));
                        // TODO: add this random key to the config file
                        smHelloData.Add("randomKey", new byte[] { 0, 0, 0, 0 });

                        // send the packet
                        smHelloPacket.Write(tcpWriter, smHelloData);
                        tcpWriter.Flush();
                        break;
                    
                    case (int)SMClientCommand.ScreenChanged:
                        switch((SMScreen)packet.Data["screenStatus"])
                        {
                            case SMScreen.ExitedScreenNetSelectMusic:
                                ChangeStatus(UserStatus.None);
                                break;

                            case SMScreen.EnteredScreenNetSelectMusic:
                                ChangeStatus(UserStatus.MusicSelection);
                                break;
                            
                            case SMScreen.NotSent:
                                ChangeStatus(UserStatus.None);
                                break;
                            
                            case SMScreen.EnteredOptionsScreen:
                                ChangeStatus(UserStatus.Options);
                                break;
                            
                            case SMScreen.ExitedEvaluationScreen:
                                ChangeStatus(UserStatus.None);
                                break;
                            
                            case SMScreen.EnteredEvaluationScreen:
                                ChangeStatus(UserStatus.Evaluation);
                                break;
                            
                            case SMScreen.ExitedScreenNetRoom:
                                ChangeStatus(UserStatus.None);
                                break;
                            
                            case SMScreen.EnteredScreenNetRoom:
                                if (user.CurrentRoom != null)
                                {
                                    user.CurrentRoom = null;
                                }
                                
                                ChangeStatus(UserStatus.RoomSelection);

                                UpdateRoomList();
                                break;
                        }
                        break;
                    
                    case (int)SMClientCommand.SMOnlinePacket:
                        // if the packet is a StepMania Online packet, do a bit more work as it
                        // is basically an encapsulated packet

                        SMOPacket smoPacket = (SMOPacket)packet;
                        // get the SMO Command (Seperate to a regular packet command)
                        int smoCommand = smoPacket.SMOCommand;

                        // decide what to do for each SMO command
                        switch(smoCommand)
                        {
                            case (int)SMOClientCommand.Login:
                                // Recieved if the user wants to login

                                // NOTE: Password is recieved as either:
                                //       1. MD5 Hash
                                //       2. MD5(MD5 hash + salt) where the salt is a plaintext base10 string
                                logger.Trace("Login attempt recieved -> {username}:{password}", smoPacket.Data["username"], smoPacket.Data["password"]);

                                // create a response packet
                                Dictionary<string, object> smoLoginData = new Dictionary<string, object>();
                                Packet smoLoginPacket = new SMOServerLogin();

                                // check if user exists in the database
                                // if it does, attempt a login, if it doesn't, create an account with that password
                                User existingUser = StepmaniaServer.dbContext.Users.Where(s => s.Username == (string)smoPacket.Data["username"]).SingleOrDefault();
                                logger.Trace("User: {u}", existingUser);

                                // if the username isnt taken, presume a registration attempt
                                if (existingUser == null)
                                {
                                    logger.Trace("User does not exist so persuming that this is a new registration");
                                    
                                    // create a user
                                    User newUser = new User()
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        Username = (string)smoPacket.Data["username"],
                                        SMPassword = (string)smoPacket.Data["password"]
                                    };

                                    // add it to database
                                    StepmaniaServer.dbContext.Add<User>(newUser);

                                    // apply changes
                                    StepmaniaServer.dbContext.SaveChanges();

                                    // set the response to a successful login
                                    smoLoginData.Add("success", true);
                                    smoLoginData.Add("loginResponse", "Successfully registered on the server");

                                    user = StepmaniaServer.dbContext.Users.Where(s => s.Id == newUser.Id).SingleOrDefault();
                                    logger.Info("User {username} has registered an account", newUser.Username);
                                }
                                // if the username exists in database, presume a login attempt
                                else
                                {
                                    logger.Trace("User exists, attempting login");

                                    if ((string)smoPacket.Data["password"] == existingUser.SMPassword)
                                    {
                                        // set the response to a successful login
                                        smoLoginData.Add("success", true);
                                        smoLoginData.Add("loginResponse", "Successfully logged into server");

                                        user = existingUser;
                                        logger.Info("User {username} has successfully logged in", existingUser.Username);
                                    }
                                    else
                                    {
                                        // set the response to a unsuccessful login
                                        smoLoginData.Add("success", false);
                                        smoLoginData.Add("loginResponse", "Error, Incorrect password");

                                        logger.Warn("Incorrect login from user {username}", existingUser.Username);
                                    }
                                }


                                // send the packet
                                smoLoginPacket.Write(tcpWriter, smoLoginData);
                                tcpWriter.Flush();
                                break;
                            
                            case (int)SMOClientCommand.EnterRoom:
                                if ((bool)smoPacket.Data["isEnter"])
                                {
                                    // get the room the client wants to enter from database
                                    Room roomToEnter = StepmaniaServer.dbContext.Rooms.Where(s => s.Name == (string)smoPacket.Data["enterRoomName"]).SingleOrDefault();
                                    
                                    // catch blank password
                                    string roomToEnterPassword = roomToEnter.Password;
                                    if (roomToEnterPassword == null)
                                    {
                                        roomToEnterPassword = "";
                                    }

                                    // attempt authentication for room
                                    if (roomToEnterPassword == (string)smoPacket.Data["enterRoomPassword"])
                                    {
                                        logger.Trace("Correct password for room entered, allow room entry");

                                        // create a response packet
                                        Dictionary<string, object> smoEnterRoom = new Dictionary<string, object>();
                                        Packet smoEnterRoomPacket = new SMOServerRoomUpdate();

                                        // add the data
                                        smoEnterRoom.Add("update", "title");
                                        smoEnterRoom.Add("roomTitle", roomToEnter.Name);
                                        smoEnterRoom.Add("roomDescription", roomToEnter.Description);
                                        smoEnterRoom.Add("isGame", true); // TODO: Support 'chat' rooms
                                        smoEnterRoom.Add("allowSubroom", false);

                                        // write the packet
                                        smoEnterRoomPacket.Write(tcpWriter, smoEnterRoom);
                                        tcpWriter.Flush();

                                        // Add user to room
                                        user.CurrentRoom = roomToEnter;
                                        StepmaniaServer.dbContext.SaveChanges();
                                    }
                                    else
                                    {
                                        logger.Trace("Incorrect room password. Correct {correctPassword}, Entered: {enteredPassword}", roomToEnterPassword, smoPacket.Data["enterRoomPassword"]);
                                    }
                                }
                                break;
                            
                            case (int)SMOClientCommand.CreateRoom:
                                // create a room entity holding the information
                                Room newRoom = new Room()
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = (string)smoPacket.Data["newRoomName"],
                                    Description = (string)smoPacket.Data["newRoomDescription"],
                                    Password = (string)smoPacket.Data["newRoomPassword"],
                                    Status = RoomStatus.Normal
                                };

                                // add to database and apply changes
                                StepmaniaServer.dbContext.Add<Room>(newRoom);
                                StepmaniaServer.dbContext.SaveChanges();

                                // trigger an update for the client
                                UpdateRoomList();
                                break;
                            
                            case (int)SMOClientCommand.RoomInfo:
                                // get the room that the information is requested about
                                string roomName = (string)smoPacket.Data["roomName"];
                                Room room = StepmaniaServer.dbContext.Rooms.Where(s => s.Name == roomName).SingleOrDefault();

                                // if the room exists
                                if (room != null)
                                {
                                    // create a response packet
                                    Dictionary<string, object> smoRoomInfo = new Dictionary<string, object>();
                                    Packet smoRoomInfoPacket = new SMOServerRoomInfo();

                                    // get the players currently online
                                    List<string> playerNames = new List<string>();
                                    int numberPlayers = 0;
                                    if (room.Users != null)
                                    {
                                        // if there are players online
                                        numberPlayers = room.Users.Count;
                                        foreach (User user in room.Users)
                                        {
                                            playerNames.Add(user.Username);
                                        }
                                    }

                                    // if there is an active song/recent song
                                    if (room.ActiveSong != null)
                                    {
                                        smoRoomInfo.Add("lastSongTitle", room.ActiveSong.Title);
                                        smoRoomInfo.Add("lastSongSubtitle", room.ActiveSong.Subtitle);
                                        smoRoomInfo.Add("lastSongArtist", room.ActiveSong.Artist);
                                    }
                                    else
                                    {
                                        // otherwise send back 'blank' data
                                        smoRoomInfo.Add("lastSongTitle", "No Song Played");
                                        smoRoomInfo.Add("lastSongSubtitle", "N/A");
                                        smoRoomInfo.Add("lastSongArtist", "N/A");
                                    }

                                    smoRoomInfo.Add("numberPlayers", numberPlayers);
                                    smoRoomInfo.Add("maxPlayers", room.MaxUsers);
                                    smoRoomInfo.Add("playerNames", playerNames);

                                    // write packet to client
                                    smoRoomInfoPacket.Write(tcpWriter, smoRoomInfo);
                                    tcpWriter.Flush();
                                }
                                break;
                        }
                        break;
                }
            }
        }

        // used to send the client an updated list of available rooms
        private void UpdateRoomList()
        {
            logger.Trace("Sending list of rooms");

            // get all the rooms fom from the db in a list
            List<Room> rooms = StepmaniaServer.dbContext.Rooms.ToList();
            List<Tuple<string, string, byte, byte>> formattedRooms = new List<Tuple<string, string, byte, byte>>();

            // pull all required data and place in a List of Tuples
            foreach (Room room in rooms)
            {
                string roomName = room.Name;
                string roomDescription = room.Description;
                byte roomStatus = (byte)room.Status;
                byte roomPasswordless = (room.Password == null) ? (byte)0x01 : (byte)0x00;

                formattedRooms.Add(new Tuple<string, string, byte, byte>(roomName, roomDescription, roomStatus, roomPasswordless));
            }

            // create a response packet
            Dictionary<string, object> smoUpdateRoomList = new Dictionary<string, object>();
            Packet smoUpdateRoomListPacket = new SMOServerRoomUpdate();

            // add required info
            smoUpdateRoomList.Add("update", "rooms");
            smoUpdateRoomList.Add("numberRooms", formattedRooms.Count);
            smoUpdateRoomList.Add("rooms", formattedRooms);

            // send packet to client
            smoUpdateRoomListPacket.Write(tcpWriter, smoUpdateRoomList);
            tcpWriter.Flush();
        }

        // used to check whether the client is still connected
        public void SendPing()
        {
            // reset update counter
            updates = 0;

            // create a ping packet and send it
            Packet smoPingPacket = new SMServerPing();
            smoPingPacket.Write(tcpWriter, new Dictionary<string, object>());
            tcpWriter.Flush();
        }

        public void ChangeStatus(UserStatus userStatus)
        {
            if (user != null)
            {
                logger.Trace("User {username} Status: {status}", user.Username, userStatus);
                user.Online = userStatus != UserStatus.None;
                user.Status = userStatus;
                StepmaniaServer.dbContext.SaveChanges();
            }
        }

        // called if the client disconnects for any reason
        // basically a cleanup method
        public void Disconnected()
        {
            if (user != null)
            {
                user.Online = false;
                user.Status = UserStatus.None;
                user.CurrentRoom = null;
            }

            StepmaniaServer.dbContext.SaveChanges();
        }
    }
}
