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

        private static TcpClient tcpClient;
        private static BinaryReader tcpReader;
        private static BinaryWriter tcpWriter;
        private static NetworkStream tcpStream;

        public GameClient(TcpClient client)
        {
            tcpClient = client;
            tcpStream = tcpClient.GetStream();
            tcpStream.ReadTimeout = Convert.ToInt32(config.Get("/config/game-server/timeout", "1000"));
            tcpReader = new BinaryReader(tcpStream);
            tcpWriter = new BinaryWriter(tcpStream);
        }

        public void Update()
        {
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
                            case SMScreen.EnteredScreenNetRoom:
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
                                break;
                            
                            case (int)SMOClientCommand.CreateRoom:
                                Room newRoom = new Room()
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = (string)smoPacket.Data["newRoomName"],
                                    Description = (string)smoPacket.Data["newRoomDescription"],
                                    Password = (string)smoPacket.Data["newRoomPassword"],
                                    Status = RoomStatus.NORMAL
                                };

                                StepmaniaServer.dbContext.Add<Room>(newRoom);
                                StepmaniaServer.dbContext.SaveChanges();

                                UpdateRoomList();
                                break;
                            
                            case (int)SMOClientCommand.RoomInfo:
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
    }
}
