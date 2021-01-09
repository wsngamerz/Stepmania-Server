using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    /// <summary>
    /// The packet sent when a player selects a song or in response to whether a client has the song
    /// </summary>
    class SMClientRequestStartGame : Packet
    {
        private int _length;
        private int _command;
        private Dictionary<string, object> _data;

        public ClientRequestStartGameStatus Status;
        public string RequestedSongTitle;
        public string RequestedSongArtist;
        public string RequestedSongSubtitle;

        public override int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public override int Command
        {
            get { return (int)SMClientCommand.RequestStartGame; }
            set { _command = value; }
        }

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        public override void Read(BinaryReader binaryReader)
        {
            Status = (ClientRequestStartGameStatus)PacketUtils.ReadByte(binaryReader);
            RequestedSongTitle = PacketUtils.ReadNTString(binaryReader);
            RequestedSongArtist = PacketUtils.ReadNTString(binaryReader);
            RequestedSongSubtitle = PacketUtils.ReadNTString(binaryReader);

            // save information
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("status", Status);
            data.Add("requestedSongTitle", RequestedSongTitle);
            data.Add("requestedSongArtist", RequestedSongArtist);
            data.Add("requestedSongSubtitle", RequestedSongSubtitle);
            Data = data;

            logger.Trace("Recieved RequestStartGame from client - Status: {status}, Title: {title}, Subtitle: {subtitle}, Artist: {artist}", Status, RequestedSongTitle, RequestedSongSubtitle, RequestedSongArtist);
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }

    public enum ClientRequestStartGameStatus
    {
        UserHasSong = 0,
        UserDoesntHaveSong = 1,
        RequestStart = 2
    }
}
