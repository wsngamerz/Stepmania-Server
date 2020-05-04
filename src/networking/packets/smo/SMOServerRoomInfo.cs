using System.Collections.Generic;
using System.IO;


namespace StepmaniaServer
{
    // packet for the servers login response
    class SMOServerRoomInfo : SMOPacket
    {
        private int _length;
        private int _command;
        private int _smocommand;
        private Dictionary<string, object> _data;

        public override int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public override int Command
        {
            get { return (int)SMServerCommand.SMOnlinePacket; }
            set { _command = value; }
        }

        public override int SMOCommand
        {
            get { return (int)SMOServerCommand.RoomInfo; }
            set { _smocommand = value; }
        }

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        public override void Read(BinaryReader binaryReader) { }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data)
        {
            string lastSongTitle = (string)data["lastSongTitle"];
            string lastSongSubtitle = (string)data["lastSongSubtitle"];
            string lastSongArtist = (string)data["lastSongArtist"];
            int numberPlayers = (int)data["numberPlayers"];
            int maxPlayers = (int)data["maxPlayers"];
            List<string> playerNames = (List<string>)data["playerNames"];

            int packetLength = 7;
            
            packetLength += lastSongTitle.Length;
            packetLength += lastSongSubtitle.Length;
            packetLength += lastSongArtist.Length;

            foreach (string playerName in playerNames)
            {
                packetLength += playerName.Length;
                packetLength += 1;
            }

            // calculate length
            Length = packetLength;
            MemoryStream packetStream = new MemoryStream(Length + 4);

            // populate memorystream with packet
            PacketUtils.WriteLength(packetStream, Length);
            PacketUtils.WriteByte(packetStream, (byte)Command);
            PacketUtils.WriteByte(packetStream, (byte)SMOCommand);
            PacketUtils.WriteNTString(packetStream, lastSongTitle);
            PacketUtils.WriteNTString(packetStream, lastSongSubtitle);
            PacketUtils.WriteNTString(packetStream, lastSongArtist);
            PacketUtils.WriteByte(packetStream, (byte)numberPlayers);
            PacketUtils.WriteByte(packetStream, (byte)maxPlayers);

            foreach (string playerName in playerNames)
            {
                PacketUtils.WriteNTString(packetStream, playerName);
            }

            // convert to bytearray
            byte[] packetPayload = packetStream.GetBuffer();

            logger.Trace("Sending Room Info: {payload}", PacketUtils.ByteArrayToString(packetPayload));

            // send packet
            binaryWriter.Write(packetPayload);
        }
    }
}
