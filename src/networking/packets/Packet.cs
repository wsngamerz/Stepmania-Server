using System.Collections.Generic;
using System.IO;

using NLog;



namespace StepmaniaServer
{
    /// <summary>
    /// abstract class to represent a packet
    /// </summary>
    abstract class Packet
    {
        // Logger
        public static readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        // Packet length (first 4 bytes of a packet)
        public abstract int Length { get; set; }

        // Packet command (next 1 byte of packet)
        public abstract int Command { get; set; }

        // Packet data (will be populated after packet 'Read')
        public abstract Dictionary<string, object> Data { get; set; }

        // Methods to override when needed
        public abstract void Read(BinaryReader binaryReader);
        public abstract void Write(BinaryWriter binaryWriter, Dictionary<string, object> data);
    }

    /// <summary>
    /// abstract class that represents a SMO packet which has a bit more information than a regular packet
    /// </summary>
    abstract class SMOPacket : Packet
    {
        // SMO Command is another byte using the SMO 'Sub-Protocol'
        public abstract int SMOCommand { get; set; }
    }

    /// <summary>
    /// Unknown packet is used if a packet has yet to be implemented so it reads all the packet information inclding
    /// the length, command and the payload and outputs it in the log for development
    /// </summary>
    class UnknownPacket : Packet
    {
        private int _length;
        private int _command;
        private Dictionary<string, object> _data;

        private byte[] Payload;

        public override int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public override int Command
        {
            get { return _command; }
            set { _command = value; }
        }

        public override Dictionary<string, object> Data {
            get { return _data; }
            set { _data = value; }
        }

        public override void Read(BinaryReader binaryReader)
        {
            // read the remaining data after the length and the command bytes
            Payload = binaryReader.ReadBytes(_length - 1);

            // output to console as a byte array string
            logger.Trace("Recieved packet {command} of length {length} containing {payload}", _command, _length, PacketUtils.ByteArrayToString(Payload));
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }


    /// <summary>
    /// used to return the correct class based upon the packet command
    /// </summary>
    class PacketFactory
    {
        /// <summary>
        /// return the backed based upon the packet command
        /// </summary>
        /// <param name="command">packet command</param>
        /// <returns>packet relating to the command</returns>
        public static Packet GetPacket(int command)
        {
            switch (command)
            {
                case (int)SMClientCommand.PingR:
                    return new SMClientPingR();

                case (int)SMClientCommand.Hello:
                    return new SMClientHello();

                case (int)SMClientCommand.GameStartRequest:
                    return new SMClientGameStartRequest();

                case (int)SMClientCommand.GameStatusUpdate:
                    return new SMClientGameStatusUpdate();

                case (int)SMClientCommand.StyleUpdate:
                    return new SMClientStyleUpdate();

                case (int)SMClientCommand.RequestStartGame:
                    return new SMClientRequestStartGame();

                case (int)SMClientCommand.ScreenChanged:
                    return new SMClientScreenChanged();
                
                case (int)SMClientCommand.PlayerOptions:
                    return new SMClientPlayerOptions();

                case (int)SMClientCommand.SMOnlinePacket:
                    return new SMClientSMOnline();

                default:
                    return new UnknownPacket();
            }
        }
    }
}
