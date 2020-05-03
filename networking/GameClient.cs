using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

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
                                logger.Debug("Login attempt recieved -> {username}:{password}", smoPacket.Data["username"], smoPacket.Data["password"]);
                                
                                // DEV: Remember to provide some actual authentication here in the future
                                logger.Warn("Login is currently aunauthenticated, beware!");

                                // create a response packet
                                Dictionary<string, object> smoLoginData = new Dictionary<string, object>();
                                Packet smoLoginPacket = new SMOServerLogin();

                                // set the response to a successful login
                                smoLoginData.Add("success", true);
                                smoLoginData.Add("loginResponse", "Successfully logged into server");

                                // send the packet
                                smoLoginPacket.Write(tcpWriter, smoLoginData);
                                tcpWriter.Flush();
                                break;
                            
                            case (int)SMOClientCommand.EnterRoom:
                                break;
                            
                            case (int)SMOClientCommand.CreateRoom:
                                break;
                            
                            case (int)SMOClientCommand.RoomInfo:
                                break;
                        }
                        break;
                }
            }
        }
    }
}
