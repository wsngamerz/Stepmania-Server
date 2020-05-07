using System;
using System.Collections.Generic;
using System.Linq;
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
            GameRoom dummyRoom = new GameRoom(new Room());
            dummyRoom.RoomManager = this;
            dummyRoom.Room.Name = "Dummy Room";
            gameRooms.Add(dummyRoom);

            // add all rooms to room manager
            List<Room> rooms = StepmaniaServer.dbContext.Rooms.ToList();
            foreach (Room room in rooms)
            {
                GameRoom gameRoom = new GameRoom(room);
                gameRooms.Add(gameRoom);
            }

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
                foreach (GameRoom gameRoom in gameRooms.ToList())
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
            logger.Trace("Client moving from {from} to {to}", roomFrom.Room.Name, roomTo.Room.Name);
            // remove from old room
            roomFrom.RemoveGameClient(gameClient);

            // add to new game room
            roomTo.AddGameClient(gameClient);
            gameClient.CurrentRoom = roomTo;
            gameClient.CurrentRoom.RoomManager = this;
        }

        public void MoveToDummyRoom(GameClient gameClient)
        {
            GameRoom dummyRoom = gameRooms[0];
            MoveClient(gameClient, gameClient.CurrentRoom, dummyRoom);
        }

        // create new room
        public void CreateRoom(string name, string description, string password)
        {
            // TODO: Check that room doesnt actually exist in db already
            Room newRoom = new Room()
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Description = description,
                Password = password
            };

            StepmaniaServer.dbContext.Add<Room>(newRoom);
            StepmaniaServer.dbContext.SaveChanges();

            GameRoom newGameRoom = new GameRoom(newRoom);

            gameRooms.Add(newGameRoom);
        }

        // enter a room
        public bool EnterRoom(Room roomEnter, string password, GameClient client)
        {
            GameRoom gameRoomEnter = gameRooms.Where(s => s.Room.Name == roomEnter.Name).SingleOrDefault();

            if (roomEnter.Password == password)
            {
                logger.Trace("Moving client to new room");
                MoveClient(client, client.CurrentRoom, gameRoomEnter);
                return true;
            }
            else
            {
                logger.Trace("Unsuccessful move as password incorrect");
                return false;
            }
        }

        // get room information
        public Room GetRoom(string name)
        {
            return StepmaniaServer.dbContext.Rooms.Where(s => s.Name == name).SingleOrDefault();
        }
    }
}
