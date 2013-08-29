using Network.Server;
namespace Network.Client
{
    public class CpRequestCharacterCreate : ARecvPacket
    {
        public override void Read()
        {
        }

        public override void Process()
        {
            //0x93EC - 2113 EU > send to char creator
            new SendPacket("EC9301").Send(Connection);
        }
    }
}