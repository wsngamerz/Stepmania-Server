using System;
using System.Collections.Generic;
using System.IO;

using NLog;



namespace StepmaniaServer
{
    // packet sent to the client by the server to check whether
    // the client is still connected properly
    class SMServerPing : Packet
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
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
            get { return (int)SMServerCommand.Ping; }
            set { _command = value; }
        }

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        public override void Read(BinaryReader binaryReader) { }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data)
        {
            // calculate the length of the packet
            Length = 1;

            // NOTE: A ping packet has no payload so only the length of the
            //       packet and the packet command are sent (5 bytes total)

            // create a memorystream to hold the packet to send
            MemoryStream packetStream = new MemoryStream(Length + 4);
            PacketUtils.WriteLength(packetStream, Length);
            PacketUtils.WriteByte(packetStream, (byte)Command);

            // convert MemoryStream to byte array
            byte[] packetPayload = packetStream.GetBuffer();

            // send the byte array
            try
            {
                binaryWriter.Write(packetPayload);
                binaryWriter.Flush();
            }
            catch (Exception e)
            {
                // if there is an error, it probably means that the client has disconnected but
                // log it as a warning anyways just in case an error that was not expected occurs
                logger.Warn(e, "Error sending ping, client disconnected?");
            }
        }
    }
}
