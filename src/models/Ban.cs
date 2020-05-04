using System;



namespace StepmaniaServer
{
    public class Ban
    {
        public string Id { get; set; }
        public string Ip { get; set; }
        public bool Permanent { get; set; }
        public bool Server { get; set; }

        public virtual Room Room { get; set; }
        public virtual User User { get; set; }

        public DateTime EndAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
