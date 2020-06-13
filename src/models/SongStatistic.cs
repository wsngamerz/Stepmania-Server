using System;
using System.Collections.Generic;



namespace StepmaniaServer
{
    public enum Grade
    {
        AAAA = 0,
        AAA = 1,
        AA = 2,
        A = 3,
        B = 4,
        C = 5,
        D = 6,
        F = 7
    }

    public enum Difficulty
    {
        Beginner = 0,
        Easy = 1,
        Medium = 2,
        Hard = 3,
        Expert = 4
    }
    
    public class SongStatistic
    {
        public string Id { get; set; }
        
        public virtual Game Game { get; set; }
        public virtual Song Song { get; set; }
        public virtual User User { get; set; }

        public int HitMine { get; set; }
        public int AvoidMine { get; set; }
        public int Miss { get; set; }
        public int W5 { get; set; }
        public int W4 { get; set; }
        public int W3 { get; set; }
        public int W2 { get; set; }
        public int W1 { get; set; }
        public int LetGo { get; set; }
        public int Held { get; set; }

        public int MaxCombo { get; set; }
        public string Options { get; set; }
        public int Score { get; set; }
        public Grade Grade { get; set; }
        public Difficulty Difficulty { get; set; }
        public int Percentage { get; set; }
        public int Duration { get; set; }

        public virtual ICollection<SongUpdate> Updates { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
