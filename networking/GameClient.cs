using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

using NLog;


namespace StepmaniaServer
{
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
            tcpStream.ReadTimeout = Convert.ToInt32(config.Get("/config/server/timeout", "1000"));
            tcpReader = new BinaryReader(tcpStream);
            tcpWriter = new BinaryWriter(tcpStream);
        }

        public void Update()
        {
            if (tcpClient.Available > 0)
            {
                int packetLength = PacketUtils.ReadLength(tcpReader);
                byte command = PacketUtils.ReadByte(tcpReader);

                Packet packet = PacketFactory.GetPacket(command);
                packet.Length = packetLength;
                packet.Command = command;
                packet.Read(tcpReader);

                switch (command)
                {
                    case (int)SMClientCommand.Hello:
                        // send a server hello back
                        Dictionary<string, object> smHelloData = new Dictionary<string, object>();
                        Packet smHelloPacket = new SMServerHello();

                        smHelloData.Add("serverProtocolVersion", Convert.ToInt32(config.Get("/config/server/protocol", "128")));
                        smHelloData.Add("serverName", config.Get("/config/server/name", "A Stepmania Server"));
                        smHelloData.Add("randomKey", new byte[] { 0, 0, 0, 0 });

                        smHelloPacket.Write(tcpWriter, smHelloData);
                        tcpWriter.Flush();

                        break;
                    
                    case (int)SMClientCommand.SMOnlinePacket:
                        SMOPacket smoPacket = (SMOPacket)packet;
                        int smoCommand = smoPacket.SMOCommand;

                        switch(smoCommand)
                        {
                            case (int)SMOClientCommand.Login:
                                // no auth rn
                                Dictionary<string, object> smoLoginData = new Dictionary<string, object>();
                                Packet smoLoginPacket = new SMOServerLogin();

                                smoLoginData.Add("success", true);
                                smoLoginData.Add("loginResponse", "Successfully logged into server");

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
