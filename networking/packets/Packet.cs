using System.Collections.Generic;
using System.IO;

using NLog;


namespace StepmaniaServer
{
    abstract class Packet
    {
        public static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public abstract int Length { get; set; }
        public abstract int Command { get; set; }

        public abstract void Read(BinaryReader binaryReader);
        public abstract void Write(BinaryWriter binaryWriter, Dictionary<string, object> data);
    }

    abstract class SMOPacket : Packet
    {
        public abstract int SMOCommand { get; set; }
    }

    class UnknownPacket : Packet
    {
        private int _length;
        private int _command;

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

        public override void Read(BinaryReader binaryReader)
        {
            Payload = binaryReader.ReadBytes(_length - 1);
            logger.Trace("Recieved packet {command} of length {length} containing {payload}", _command, _length, PacketUtils.ByteArrayToString(Payload));
        }

        public override void Write(BinaryWriter binaryWriter, Dictionary<string, object> data) { }
    }

    class PacketFactory
    {
        public static Packet GetPacket(int command)
        {
            switch (command)
            {
                case (int)SMClientCommand.Hello:
                    return new SMClientHello();

                case (int)SMClientCommand.StyleUpdate:
                    return new SMClientStyleUpdate();

                case (int)SMClientCommand.ScreenChanged:
                    return new SMClientScreenChanged();

                case (int)SMClientCommand.SMOnlinePacket:
                    return new SMClientSMOnline();

                default:
                    return new UnknownPacket();
            }
        }
    }
}
