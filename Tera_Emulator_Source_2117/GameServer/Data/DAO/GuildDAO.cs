using MySql.Data.MySqlClient;
using Data.Enums.Item;
using Data.Structures.Account;
using Data.Structures.Player;
using Data.Structures.Guild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using System.Diagnostics;

namespace Data.DAO
{
    public class GuildDAO : BaseDAO
    {
        private MySqlConnection GuildDAOConnection;

        public GuildDAO(string conStr) : base(conStr)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            GuildDAOConnection = new MySqlConnection(conStr);
            GuildDAOConnection.Open();

            stopwatch.Stop();
            Log.Info("DAO: GuildDAO Initialized with {0} Guilds in {1}s"
            , LoadTotalGuilds()
            , (stopwatch.ElapsedMilliseconds / 1000.0).ToString("0.00"));
        }

        public bool SaveGuild(List<Player> players, string guildName)
        {
            string SQL = "INSERT INTO `guilds` "
                + "(guildname, guildlogo, level, creationdate) "
                + "VALUES(?name, ?logo, ?level, ?credate);";
            MySqlCommand cmd = new MySqlCommand(SQL, GuildDAOConnection);
            cmd.Parameters.AddWithValue("?name", guildName);
            cmd.Parameters.AddWithValue("?guildlogo", "");
            cmd.Parameters.AddWithValue("?level", "1");
            cmd.Parameters.AddWithValue("?credate", Funcs.GetRoundedUtc());

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Log.ErrorException("DAO: Guild Save Failed!", e);
                return false;
            }

            return true;
        }

        public void AddCharacterToGuild(Player player, Guild guild)
        {
            string SQL = "UPDATE guilds SET guildmembers = ?player WHERE guildname = ?gname";
            MySqlCommand cmd = new MySqlCommand(SQL, GuildDAOConnection);
            cmd.Parameters.AddWithValue("?player", player);
            cmd.Parameters.AddWithValue("?gname", guild.GuildName);
        }

        public void GuildHistoryAdd(string historyEvent)
        {

        }

        public int LoadTotalGuilds()
        {
            string SQL = "SELECT COUNT(*) FROM guilds";
            MySqlCommand cmd = new MySqlCommand(SQL, GuildDAOConnection);
            MySqlDataReader reader = cmd.ExecuteReader();

            int count = 0;
            while (reader.Read())
            {
                count = reader.GetInt32(0);
            }

            reader.Close();
            return count;
        }

        public bool AddGuildRank(Guild g, GuildMemberRank gmr)
        {
            string SQL = "INSERT INTO guild_ranks "
            + "(gid, rankprivileges, rankname) "
            + "VALUES(?gid, ?rankpriv, ?rankname);";

            // Are we GM
            if (gmr.RankName == "GuildMaster")
                gmr.RankPrivileges = 7;
            else
                gmr.RankPrivileges = 0;

            MySqlCommand cmd = new MySqlCommand(SQL, GuildDAOConnection);
            cmd.Parameters.AddWithValue("?gid", g.GuildId);
            cmd.Parameters.AddWithValue("?rankpriv", gmr.RankPrivileges);
            cmd.Parameters.AddWithValue("?rankname", gmr.RankName);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Log.ErrorException("DAO: Guild Rank Add Error!", e);
                return false;
            }
            return true;
        }
    }
}
