using Communication.Logic;

namespace Network.Client
{
    public class CpRequestCharacterList : ARecvPacket
    {
        public override void Read()
        {
        }

        public override void Process()
        {
            AccountLogic.GetCharacterList(Connection);
        }
    }
}