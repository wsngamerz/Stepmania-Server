using System.Collections.Generic;



namespace StepmaniaServer
{
    class GameRoom
    {
        Room room = null;
        List<GameClient> roomClients = new List<GameClient>();

        public GameRoom(Room _room = null)
        {
            if (_room == null)
            {
                _room = new Room();
                _room.Name = "Fake Room";
                _room.Description = "A dummy room that all clients are added to before joining an actual room";
            }

            room = _room;
        }

        public void AddClient(GameClient client)
        {
            roomClients.Add(client);
        }

        public void RemoveClient(GameClient client)
        {
            roomClients.Remove(client);
        }
    }
}
