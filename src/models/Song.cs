using System;
using System.Collections.Generic;



namespace StepmaniaServer
{
    public class Song
    {
        public static string Id { get; set; }
        public static string Title { get; set; }
        public static string Subtitle { get; set; }
        public static string Artist { get; set; }

        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<SongStatistic> SongStatistics { get; set; }
        
        public static DateTime CreatedAt { get; set; }
        public static DateTime UpdatedAt { get; set; }
    }
}
