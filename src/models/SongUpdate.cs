using System;



namespace StepmaniaServer {
    public enum StepId {
        HitMine = 1,
        AvoidMine = 2,
        Miss = 3,
        W5 = 4,
        W4 = 5,
        W3 = 6,
        W2 = 7,
        W1 = 8,
        LetGo = 9,
        Held = 10
    }

    public class SongUpdate {
        public string Id { get; set; }

        public virtual SongStatistic SongStatistic { get; set; }

        public StepId Step { get; set; }
        public Grade PedictedGrade { get; set; }
        public int Score { get; set; }
        public int Combo { get; set; }
        public int Health { get; set; }
        public int TimingOffset { get; set; }
    }
}
