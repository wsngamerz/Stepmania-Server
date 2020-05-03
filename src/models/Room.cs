using System;
using System.Collections.Generic;



namespace StepmaniaServer
{
    public class Room
    {
        public static string Id { get; set; }
        public static string Name { get; set; }
        public static string Description { get; set; }
        public static string Password { get; set; }
        public static string Motd { get; set; }
        public static int MaxUsers { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Game> Games { get; set; }
        public virtual Song ActiveSong { get; set; }
        
        public static DateTime CreatedAt { get; set; }
        public static DateTime UpdatedAt { get; set; }
    }
}
