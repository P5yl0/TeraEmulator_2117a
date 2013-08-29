using Network.Server;
using Utils;
namespace Network.Client
{
    public class CpSystemInfo : ARecvPacket
    {
        public override void Read()
        {
            //shifts
            short OsNameShift = (short)ReadH();
            //Log.Info("OsNameShift: " + OsNameShift);
            short CpuNameShift = (short)ReadH();
            //Log.Info("CpuNameShift: " + CpuNameShift);
            short GpuNameShift = (short)ReadH();
            //Log.Info("GpuNameShift: " + GpuNameShift);

            //ToDo but not needed!
        }

        public override void Process()
        {
            //2117 EU
            ////not needed to start but added PlanetDB ?!?
            new SendPacket("91E50E00A54200000000000050006C0061006E0065007400440042005F00570033000000").Send(Connection);

        }
    }
}