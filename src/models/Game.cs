using System;



namespace StepmaniaServer
{
    public class Game
    {
        public static string Id { get; set; }
        public static bool Active { get; set; }

        public virtual Song Song { get; set; }
        public virtual Room Room { get; set; }

        public static DateTime EndAt { get; set; }
        public static DateTime CreatedAt { get; set; }
        public static DateTime UpdatedAt { get; set; }
    }
}
