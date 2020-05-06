using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    // packet sent to the client by the server in response to a requesting of a game start
    class SMServerRequestStartGame : Packet
    {
        private int _length;
        private int _command;
        private Dictionary<string, object> _data;

        public override int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public override int Command
        {
            get { return (int)SMServerCommand.RequestStartGame; }
            set { _command = value; }
        }

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        public override void Read(BinaryReader binaryReader) { }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data)
        {
            ServerRequestStartGame messageType = (ServerRequestStartGame)data["messageType"];
            string songTitle = (string)data["songTitle"];
            string songArtist = (string)data["songArtist"];
            string songSubtitle = (string)data["songSubtitle"];

            // calculate the length of the packet
            Length = 5 + songTitle.Length + songArtist.Length + songSubtitle.Length;

            // create a memorystream to hold the packet to send
            MemoryStream packetStream = new MemoryStream(Length + 4);

            // write data to the memory stream
            PacketUtils.WriteLength(packetStream, Length);
            PacketUtils.WriteByte(packetStream, (byte)Command);
            PacketUtils.WriteByte(packetStream, (byte)messageType);
            PacketUtils.WriteNTString(packetStream, songTitle);
            PacketUtils.WriteNTString(packetStream, songArtist);
            PacketUtils.WriteNTString(packetStream, songSubtitle);

            // convert MemoryStream to byte array
            byte[] packetPayload = packetStream.GetBuffer();
            logger.Trace("Packet {command} sending {payload}", Command, PacketUtils.ByteArrayToString(packetPayload));

            // send the byte array
            binaryWriter.Write(packetPayload);
            binaryWriter.Flush();
        }
    }

    public enum ServerRequestStartGame
    {
        CHECK_CLIENT_HAS_SONG = 0,
        CHECK_CLIENT_HAS_SONG_SCROLL = 1,
        CHECK_CLIENT_HAS_SONG_SCROLL_PLAY = 2,
        BLINDLY_PLAY = 3
    }
}
