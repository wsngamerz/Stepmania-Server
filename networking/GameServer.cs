using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using NLog;


namespace StepmaniaServer
{
    class GameServer
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static Config config = new Config();
        private static TcpListener tcpListener;
        private static List<GameClient> gameClients = new List<GameClient>();

        private static Thread gameClientThread;

        public static void Start()
        {
            logger.Info("Started game server thread");
            tcpListener = new TcpListener(IPAddress.Parse(config.Get("/config/server/ip", "0.0.0.0")), Convert.ToInt32(config.Get("/config/server/port", "8765")));
            tcpListener.Server.ReceiveTimeout = Convert.ToInt32(config.Get("/config/server/timeout", "1000"));
            tcpListener.Server.SendTimeout = Convert.ToInt32(config.Get("/config/server/timeout", "1000"));
            tcpListener.Start();
            logger.Info("Server starting on {ip}:{port}", IPAddress.Parse(config.Get("/config/server/ip", "0.0.0.0")), Convert.ToInt32(config.Get("/config/server/port", "8765")));

            gameClientThread = new Thread(GameClientThread);
            gameClientThread.Start();

            while (true)
            {
                TcpClient newTcpClient = tcpListener.AcceptTcpClient();

                string clientInformation = newTcpClient.Client.RemoteEndPoint.ToString();
                logger.Debug("New client connected {information}", clientInformation);

                // TODO: Implement ban checking etc.

                GameClient newGameClient = new GameClient(newTcpClient);
                gameClients.Add(newGameClient);
            }
        }

        private static void GameClientThread()
        {
            while (true)
            {
                // loop through all clients and update them periodically
                foreach (GameClient gameClient in gameClients)
                {
                    gameClient.Update();
                }

                Thread.Sleep(50); // update 20 times per second [20 tps]
            }
        }
    }
}
