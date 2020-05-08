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

        private int ClientsInGame = 0;
        private int ClientsReady = 0;

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

        // remove client from game room
        public void RemoveGameClient(GameClient gameClient)
        {
            gameClients.Remove(gameClient);
        }

        public void StartGame(ServerRequestStartGame status, string title, string artist, string subtitle)
        {
            // TODO: Perform verifications about whether song exists on clients
            ClientsInGame = gameClients.Count;
            ClientsReady = 0;

            foreach (GameClient gameClient in gameClients)
            {
                gameClient.SendRequestStartGame(status, title, artist, subtitle);
            }
        }

        public void ClientReady()
        {
            ClientsReady++;

            if (ClientsReady == ClientsInGame)
            {
                // allow start
                foreach (GameClient gameClient in gameClients)
                {
                    gameClient.SendAllowGameStart();
                }
                
                ClientsReady = 0;
            }
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
