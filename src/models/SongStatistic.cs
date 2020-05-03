using System;



namespace StepmaniaServer
{
    public enum SongStatisticGrade
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

    public enum SongStatisticDifficulty
    {
        BEGINNER = 0,
        EASY = 1,
        MEDIUM = 2,
        HARD = 3,
        EXPERT = 4
    }
    
    public class SongStatistic
    {
        public static string Id { get; set; }
        
        public virtual Song Song { get; set; }
        public virtual User User { get; set; }
        public virtual Game Game { get; set; }

        public static int HitMine { get; set; }
        public static int AvoidMine { get; set; }
        public static int Miss { get; set; }
        public static int Bad { get; set; }
        public static int Good { get; set; }
        public static int Great { get; set; }
        public static int Perfect { get; set; }
        public static int Flawless { get; set; }
        public static int NotHeld { get; set; }
        public static int Held { get; set; }

        public static int MaxCombo { get; set; }
        public static string Options { get; set; }
        public static int Score { get; set; }
        public static SongStatisticGrade Grade { get; set; }
        public static SongStatisticDifficulty Difficulty { get; set; }
        public static int Percentage { get; set; }
        public static int Duration { get; set; }

        public static DateTime CreatedAt { get; set; }
        public static DateTime UpdatedAt { get; set; }
    }
}
