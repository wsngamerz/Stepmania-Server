using System;
using System.Net;
using System.Net.Sockets;

using NLog;



namespace StepmaniaServer
{
    class GameServer
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static Config config = new Config();
        private TcpListener tcpListener;
        private GameRoomManager gameRoomManager;

        public static string Name = config.Get("/config/game-server/name", "A Stepmania Server");
        public static int ProtocolVersion = Convert.ToInt32(config.Get("/config/game-server/protocol", "128"));

        // the game server is a class that holds the actual game server and handles
        // accepting connections and passing them down through the server
        public GameServer()
        {
            // get the server config options
            IPAddress serverIp = IPAddress.Parse(config.Get("/config/game-server/ip", "0.0.0.0"));
            int serverPort = Convert.ToInt32(config.Get("/config/game-server/port", "8765"));
            int serverTimeout = Convert.ToInt32(config.Get("/config/game-server/timeout", "1000"));

            // start the tcp listener (the server)
            tcpListener = new TcpListener(serverIp, serverPort);
            tcpListener.Server.SendTimeout = serverTimeout;
            tcpListener.Server.ReceiveTimeout = serverTimeout;
            tcpListener.Start();

            // create game room manager
            gameRoomManager = new GameRoomManager();

            // start the server loop
            ServerLoop();
        }

        // constantly listen for connections
        private void ServerLoop()
        {
            while(true)
            {
                // accept a TCP client if one connects to the server
                TcpClient tcpClient = tcpListener.AcceptTcpClient();

                // pass TCP client to the game room manager
                gameRoomManager.AddClient(tcpClient);
            }
        }
    }
}
