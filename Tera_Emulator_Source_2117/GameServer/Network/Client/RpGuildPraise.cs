using Network.Server;
using Utils;

namespace Network.Client
{
    public class RpGuildPraise : ARecvPacket
    {
        protected string GuildName;

        public override void Read()
        {
            ReadC(); // 7
            ReadC(); // 0
            ReadC(); // 1
            GuildName = ReadS();
        }

        public override void Process()
        {
            if(GuildName == "")
                return;

            if(Connection.Player.PlayerLastPraise != -1 && Funcs.GetRoundedUtc() - Connection.Player.PlayerLastPraise > 86400)
            {
                Connection.Player.PlayerLastPraise = -1;
                Connection.Player.PlayerPraiseGiven = 0;
            }


            if(Connection.Player.PlayerPraiseGiven >= 3)
            {
                new SpSystemNotice("Sorry, but you've exceeded the limit of 3 praises per day.").Send(Connection);
                return;
            }

            Connection.Player.PlayerPraiseGiven++;
            Connection.Player.PlayerLastPraise = Funcs.GetRoundedUtc();

            Communication.Global.GuildService.PraiseGuild(GuildName);
            Communication.Global.GuildService.SendServerGuilds(Connection.Player, 1);
            SystemMessages.YouPraiseGuildNowYouHavePraisesLeft(GuildName, 3 - Connection.Player.PlayerPraiseGiven).Send(Connection.Player);
        }
    }
}
