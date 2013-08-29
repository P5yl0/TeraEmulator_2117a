using Communication.Logic;
using Utils;

namespace Network.Client
{
    public class CpCheckName : ARecvPacket
    {
        protected string Name;
        protected short Type;

        public override void Read()
        {
            //C06F 01000C000A0000000C0000001600 0100 0000 66006C006F006600660079000000
            ReadB(14);
            Type = (short)ReadH();
            ReadH();
            Name = ReadS();
        }

        public override void Process()
        {
            PlayerLogic.CheckName(Connection, Name, Type);
        }
    }
}