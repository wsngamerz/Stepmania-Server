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

        /// <summary>
        /// the game room is an instance of a room which can host a game or will just keep a group of clients updated
        /// if it's the dummy room
        /// </summary>
        /// <param name="room">the room database entity</param>
        public GameRoom(Room room)
        {
            Room = room;
        }

        /// <summary>
        /// add a client to this specific room
        /// </summary>
        /// <param name="gameClient">the client</param>
        public void AddGameClient(GameClient gameClient)
        {
            gameClient.CurrentRoom = this;
            gameClients.Add(gameClient);
        }

        /// <summary>
        /// remove client from game room
        /// </summary>
        /// <param name="gameClient">the client</param>
        public void RemoveGameClient(GameClient gameClient)
        {
            gameClients.Remove(gameClient);
        }

        /// <summary>
        /// Start the game
        /// </summary>
        /// <param name="status">server start game status</param>
        /// <param name="title">the song title</param>
        /// <param name="artist">the song artist</param>
        /// <param name="subtitle">the song subtitle</param>
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

        /// <summary>
        /// Called when a client has said that it's ready
        /// </summary>
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

        /// <summary>
        /// update all of this rooms clients
        /// </summary>
        public void Update()
        {
            foreach (GameClient gameClient in gameClients.ToList())
            {
                gameClient.Update();
            }
        }
    }
}
