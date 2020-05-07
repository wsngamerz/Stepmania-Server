using System.Collections.Generic;
using System.Linq;

using NLog;



namespace StepmaniaServer
{
    class GameRoom
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static Config config = new Config();
        private List<GameClient> gameClients = new List<GameClient>();

        public GameRoomManager RoomManager;
        public Room Room;
        
        // the game room is an instance of a room which can host a game or will
        // just keep a group of clients updated if its the dummy room
        public GameRoom(Room room)
        {
            Room = room;
        }

        // add a client to this specific room
        public void AddGameClient(GameClient gameClient)
        {
            gameClient.CurrentRoom = this;
            gameClients.Add(gameClient);
        }

        public void RemoveGameClient(GameClient gameClient)
        {
            gameClients.Remove(gameClient);
        }

        // update all of this rooms clients
        public void Update()
        {
            foreach (GameClient gameClient in gameClients.ToList())
            {
                gameClient.Update();
            }
        }
    }
}
