using System;
using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    // Updates game info for each step
    class SMClientGameStatusUpdate : Packet
    {
        private int _length;
        private int _command;
        private Dictionary<string, object> _data;

        public int PlayerNumber;
        public int StepId;
        public int ProjectedGrade;
        public int Score;
        public int Combo;
        public int Health;
        public int TimingOffset;

        public override int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public override int Command
        {
            get { return (int)SMClientCommand.GameStatusUpdate; }
            set { _command = value; }
        }

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        public override void Read(BinaryReader binaryReader)
        {
            // read in data as ints before 'splitting bits'
            int statusPt1 = (int)PacketUtils.ReadByte(binaryReader);
            int statusPt2 = (int)PacketUtils.ReadByte(binaryReader);

            byte[] score = PacketUtils.ReadBytes(binaryReader, 4);
            byte[] combo = PacketUtils.ReadBytes(binaryReader, 2);
            byte[] health = PacketUtils.ReadBytes(binaryReader, 2);
            byte[] offset = PacketUtils.ReadBytes(binaryReader, 2);

            Array.Reverse(score);
            Array.Reverse(combo);
            Array.Reverse(health);
            Array.Reverse(offset);

            PlayerNumber = statusPt1 / 16;
            StepId = statusPt1 % 16;
            ProjectedGrade = statusPt2 / 16;

            Score = BitConverter.ToInt32(score);
            Combo = BitConverter.ToInt16(combo);
            Health = BitConverter.ToInt16(health);
            TimingOffset = BitConverter.ToInt16(offset);

            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("playerNumber", PlayerNumber);
            data.Add("stepId", StepId);
            data.Add("projectedGrade", (Grade) ProjectedGrade);
            data.Add("score", Score);
            data.Add("combo", Combo);
            data.Add("health", Health);
            data.Add("timingOffset", TimingOffset);
            Data = data;

            logger.Trace("Recieved Game status update");
            logger.Trace(string.Join(", ", Data));
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }
}
