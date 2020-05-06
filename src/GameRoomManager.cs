using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

using NLog;



namespace StepmaniaServer
{
    class GameRoomManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static Config config = new Config();
        private List<GameRoom> gameRooms = new List<GameRoom>();
        private Thread roomManagerThread;
        
        // the game room manager handles all of the rooms and handles switching
        // users between the rooms
        public GameRoomManager()
        {
            // create the 'Dummy Room' that all clients initially get added to
            // and add it to the list of rooms
            GameRoom dummyRoom = new GameRoom();
            dummyRoom.RoomManager = this;
            gameRooms.Add(dummyRoom);

            // create the game room manger thread
            roomManagerThread = new Thread(RoomManagerThread);
            roomManagerThread.Name = "GameRoomManager Thread";
            roomManagerThread.Start();
        }

        // updates all of the rooms
        public void RoomManagerThread()
        {
            while (true)
            {
                // loop through all rooms and call their update methods
                foreach (GameRoom gameRoom in gameRooms)
                {
                    gameRoom.Update();
                }

                // sleep for 50ms which creates an approximate 20tps
                // tps -> ticks per second
                Thread.Sleep(50);
            }
        }

        // add a client to the 'network' of rooms by adding it to the dummy
        // room
        public void AddClient(TcpClient tcpClient)
        {
            // create a game client from the TCP client
            GameClient gameClient = new GameClient(tcpClient);

            // add the game client to the dummy room
            gameRooms[0].AddGameClient(gameClient);
        }

        // move clients between rooms
        public void MoveClient(GameClient gameClient, GameRoom roomFrom, GameRoom roomTo)
        {

        }
    }
}
