using System;



namespace StepmaniaServer
{
    public class Ban
    {
        public static string Id { get; set; }
        public static string Ip { get; set; }
        public static bool Permanent { get; set; }
        public static bool Server { get; set; }

        public virtual User User { get; set; }
        public virtual Room Room { get; set; }

        public static DateTime EndAt { get; set; }
        public static DateTime CreatedAt { get; set; }
        public static DateTime UpdatedAt { get; set; }
    }
}
