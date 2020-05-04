using System;
using System.Collections.Generic;



namespace StepmaniaServer
{
    public class Game
    {
        public string Id { get; set; }
        public bool Active { get; set; }

        public virtual Room Room { get; set; }
        public virtual Song Song { get; set; }
        public virtual ICollection<SongStatistic> SongStatistics { get; set; }

        public DateTime EndAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
