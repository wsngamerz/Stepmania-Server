using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

using NLog;



namespace StepmaniaServer
{
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

        // the game client is a representation of a connection to the server
        public GameClient(TcpClient clientConnection)
        {
            tcpClient = clientConnection;
            tcpStream = clientConnection.GetStream();
            tcpReader = new BinaryReader(tcpStream);
            tcpWriter = new BinaryWriter(tcpStream);
        }

        // check for new packets sent by the client
        public void Update()
        {
            if (tcpClient.Available > 0)
            {
                // read the packet that has been recieved
                Packet recievedPacket = ReadPacket();

                // decide what to do with each packet command
                switch((SMClientCommand)recievedPacket.Command)
                {
                    case SMClientCommand.Hello:
                        ClientBuild = (string)recievedPacket.Data["clientBuild"];
                        ClientProtocolVersion = (int)recievedPacket.Data["clientProtocolVersion"];

                        logger.Trace("Connection request from client {clientInfo} build {build} protocol {protocol}", tcpClient.Client.RemoteEndPoint.ToString(), ClientBuild, ClientProtocolVersion);

                        SendHello();
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

                    case SMClientCommand.ScreenChanged:
                        CurrentScreen = (SMScreen)recievedPacket.Data["screenStatus"];
                        break;
                    
                    case SMClientCommand.SMOnlinePacket:
                        break;
                }
            }
        }

        // read the tcp stream and return the packet
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

        // send a hello to the client
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
    }
}
