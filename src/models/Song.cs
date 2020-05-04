using System;
using System.Collections.Generic;



namespace StepmaniaServer
{
    public class Song
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Artist { get; set; }

        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<Room> ActiveRooms { get; set; }
        public virtual ICollection<SongStatistic> SongStatistics { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
