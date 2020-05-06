using System.Collections.Generic;

using NLog;



namespace StepmaniaServer
{
    class GameRoom
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static Config config = new Config();
        private List<GameClient> gameClients = new List<GameClient>();

        public GameRoomManager RoomManager;
        
        // the game room is an instance of a room which can host a game or will
        // just keep a group of clients updated if its the dummy room
        public GameRoom()
        {

        }

        // add a client to this specific room
        public void AddGameClient(GameClient gameClient)
        {
            gameClient.CurrentRoom = this;
            gameClients.Add(gameClient);
        }

        // update all of this rooms clients
        public void Update()
        {
            foreach (GameClient gameClient in gameClients)
            {
                gameClient.Update();
            }
        }
    }
}
