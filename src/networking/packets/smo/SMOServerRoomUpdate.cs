using System;
using System.Collections.Generic;
using System.IO;



namespace StepmaniaServer
{
    /// <summary>
    /// packet for the servers login response
    /// </summary>
    class SMOServerRoomUpdate : SMOPacket
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
            get { return (int)SMOServerCommand.Login; }
            set { _smocommand = value; }
        }

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        public override void Read(BinaryReader binaryReader) { }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data)
        {
            MemoryStream packetStream = null;
            byte updateType;

            // update the 'title' of a room
            // this is also known as entering a room
            if((string)data["update"] == "title")
            {
                // load room information
                updateType = 0x00;
                string roomTitle = (string)data["roomTitle"];
                string roomDescription = (string)data["roomDescription"];
                byte roomType = ((bool)data["isGame"]) ? (byte)0x01 : (byte)0x00;
                byte allowSubroom = ((bool)data["allowSubroom"]) ? (byte)0x01 : (byte)0x00;

                // calculate length of packet
                Length = 7 + roomTitle.Length + roomDescription.Length;
                packetStream = new MemoryStream(Length + 4);

                // write data to memory stream
                PacketUtils.WriteLength(packetStream, Length);
                PacketUtils.WriteByte(packetStream, (byte)SMServerCommand.SMOnlinePacket);
                PacketUtils.WriteByte(packetStream, (byte)SMOServerCommand.RoomUpdate);
                PacketUtils.WriteByte(packetStream, updateType);
                PacketUtils.WriteNTString(packetStream, roomTitle);
                PacketUtils.WriteNTString(packetStream, roomDescription);
                PacketUtils.WriteByte(packetStream, roomType);
                PacketUtils.WriteByte(packetStream, allowSubroom);
            }
            // update the list of rooms
            else if ((string)data["update"] == "rooms")
            {
                // load information of all the rooms
                updateType = 0x01;
                int numberRooms = (int)data["numberRooms"];
                List<Tuple<string, string>> rooms = (List<Tuple<string, string>>)data["rooms"];

                // calculate packet length
                int totalPacketLength = 4;
                foreach (Tuple<string, string> room in rooms)
                {
                    totalPacketLength += room.Item1.Length;
                    totalPacketLength += room.Item2.Length;
                    totalPacketLength += 2;
                }

                Length = totalPacketLength;
                packetStream = new MemoryStream(Length + 4);

                // write to memory stream
                PacketUtils.WriteLength(packetStream, Length);
                PacketUtils.WriteByte(packetStream, (byte)SMServerCommand.SMOnlinePacket);
                PacketUtils.WriteByte(packetStream, (byte)SMOServerCommand.RoomUpdate);
                PacketUtils.WriteByte(packetStream, updateType);
                PacketUtils.WriteByte(packetStream, (byte)numberRooms);

                // write info for each room
                foreach (Tuple<string, string> room in rooms)
                {
                    PacketUtils.WriteNTString(packetStream, room.Item1);
                    PacketUtils.WriteNTString(packetStream, room.Item2);
                }
            }

            // convert memorystream into bytearray
            byte[] packetPayload = packetStream.GetBuffer();

            // send bytearray to client
            logger.Trace("Sending Room Update: {payload}", PacketUtils.ByteArrayToString(packetPayload));
            binaryWriter.Write(packetPayload);
            binaryWriter.Flush();
        }
    }
}
