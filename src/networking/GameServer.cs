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
            // create the server
            tcpListener = new TcpListener(IPAddress.Parse(config.Get("/config/game-server/ip", "0.0.0.0")), Convert.ToInt32(config.Get("/config/game-server/port", "8765")));
            
            // set the timeout options which are in config file
            tcpListener.Server.ReceiveTimeout = Convert.ToInt32(config.Get("/config/game-server/timeout", "1000"));
            tcpListener.Server.SendTimeout = Convert.ToInt32(config.Get("/config/game-server/timeout", "1000"));
            
            // start the server
            tcpListener.Start();
            logger.Info("Server starting on {ip}:{port}", IPAddress.Parse(config.Get("/config/game-server/ip", "0.0.0.0")), Convert.ToInt32(config.Get("/config/game-server/port", "8765")));

            // kickstart the thread which handles each of the connected game clients
            gameClientThread = new Thread(GameClientThread);
            gameClientThread.Start();

            // start the main server loop
            while (true)
            {
                // if a tcp request is recieved, assume its a client and accept it
                TcpClient newTcpClient = tcpListener.AcceptTcpClient();

                // get some basic info about the client
                string clientInformation = newTcpClient.Client.RemoteEndPoint.ToString();
                logger.Debug("New client connected {information}", clientInformation);

                // TODO: Implement ban checking etc.

                // add the client to the list of clients that the game thread will handle
                GameClient newGameClient = new GameClient(newTcpClient);
                gameClients.Add(newGameClient);
            }
        }

        private static void GameClientThread()
        {
            // game client thread loop
            while (true)
            {
                // loop through all clients and update them periodically
                foreach (GameClient gameClient in gameClients)
                {
                    gameClient.Update();
                }

                // NOTE: should probably calculate the time taken for the update and
                //       subtract it from the sleep time.
                Thread.Sleep(50); // update 20 times per second [20 tps]
            }
        }
    }
}
