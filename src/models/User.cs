using System;
using System.Collections.Generic;



namespace StepmaniaServer
{
    public enum UserStatus
    {
        SPECTATOR = 0,
        ROOMSELECTION = 1,
        MUSICSELECTION = 2,
        OPTIONS = 3,
        EVALUATION = 4
    }

    public class User
    {
        public string Id { get; set;  }
        public string Name { get; set; }
        public string Userame { get; set; }
        public string Password { get; set; }
        public string SMPassword { get; set; }
        public string Email { get; set; }
        public int Rank { get; set; }
        public int Xp { get; set; }
        public string LastIp { get; set; }
        public string ClientVersion { get; set; }
        public string ClientName { get; set; }
        public bool Online { get; set; }
        public UserStatus Status { get; set; }

        public virtual Room CurrentRoom { get; set; }
        public virtual ICollection<SongStatistic> SongStatistics { get; set; }
        public virtual ICollection<Ban> Bans { get; set; }
        
        public static DateTime CreatedAt { get; set; }
        public static DateTime UpdatedAt { get; set; }
    }
}
