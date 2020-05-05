using System;
using System.Collections.Generic;



namespace StepmaniaServer
{
    public enum RoomStatus
    {
        Normal = 0,
        Unused = 1,
        Playing = 2,
        Stage1 = 3,
        Stage2 = 4
    }

    public class Room
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public string Motd { get; set; }
        public int MaxUsers { get; set; }
        public RoomStatus Status { get; set; }

        public virtual ICollection<Ban> Bans { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Game> Games { get; set; }
        public virtual Song ActiveSong { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
