using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    /// <summary>
    /// The packet sent after loading has been done by the client
    /// </summary>
    class SMClientGameStartRequest : Packet
    {
        private int _length;
        private int _command;
        private Dictionary<string, object> _data;

        public int PrimaryPlayerMeter;
        public int PrimaryPlayerDifficulty;
        public string PrimaryPlayerOptions;

        public int SecondaryPlayerMeter;
        public int SecondaryPlayerDifficulty;
        public string SecondaryPlayerOptions;
        
        public string SongTitle;
        public string SongSubtitle;
        public string SongArtist;
        public string SongOptions;

        public string CourseTitle;
        public int StartPosition;


        public override int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public override int Command
        {
            get { return (int)SMClientCommand.GameStartRequest; }
            set { _command = value; }
        }

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        public override void Read(BinaryReader binaryReader)
        {
            // read in these data points as bytes before splitting into their
            // respective MSN and LSN values
            int playerMeters = (int)PacketUtils.ReadByte(binaryReader);
            PrimaryPlayerMeter = playerMeters / 16;
            SecondaryPlayerMeter = playerMeters % 16;

            int playerDifficulties = (int)PacketUtils.ReadByte(binaryReader);
            PrimaryPlayerDifficulty = playerDifficulties / 16;
            SecondaryPlayerDifficulty = playerDifficulties % 16;

            int startPos = (int)PacketUtils.ReadByte(binaryReader);
            StartPosition = startPos / 16;

            SongTitle = PacketUtils.ReadNTString(binaryReader);
            SongSubtitle = PacketUtils.ReadNTString(binaryReader);
            SongArtist = PacketUtils.ReadNTString(binaryReader);
            CourseTitle = PacketUtils.ReadNTString(binaryReader);
            SongOptions = PacketUtils.ReadNTString(binaryReader);
            PrimaryPlayerOptions = PacketUtils.ReadNTString(binaryReader);
            SecondaryPlayerOptions = PacketUtils.ReadNTString(binaryReader);

            // save information
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("primaryPlayerMeter", PrimaryPlayerMeter);
            data.Add("SecondaryPlayerMeter", SecondaryPlayerMeter);
            data.Add("primaryPlayerDifficulty", PrimaryPlayerDifficulty);
            data.Add("SecondaryPlayerDifficulty", SecondaryPlayerDifficulty);
            data.Add("startPosition", StartPosition);
            data.Add("songTitle", SongTitle);
            data.Add("songSubtitle", SongSubtitle);
            data.Add("songArtist", SongArtist);
            data.Add("courseTitle", CourseTitle);
            data.Add("songOptions", SongOptions);
            data.Add("primaryPlayerOptions", PrimaryPlayerOptions);
            data.Add("secondaryPlayerOptions", SecondaryPlayerOptions);
            Data = data;

            logger.Trace("Recieved Game start request: Startpos: {pos}, Course Title: {coursetitle}", StartPosition, CourseTitle);
            logger.Trace("[Song] Title: {title}, Subtitle: {subtitle}, Artist: {artist}, Options: {opts}", SongTitle, SongSubtitle, SongArtist, SongOptions);
            logger.Trace("[Primary] Meter: {meter}, Difficulty: {diff}, Options: {opts}", PrimaryPlayerMeter, PrimaryPlayerDifficulty, PrimaryPlayerOptions);
            logger.Trace("[Secondary] Meter: {meter}, Difficulty: {diff}, Options: {opts}", SecondaryPlayerMeter, SecondaryPlayerDifficulty, SecondaryPlayerOptions);
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }
}
