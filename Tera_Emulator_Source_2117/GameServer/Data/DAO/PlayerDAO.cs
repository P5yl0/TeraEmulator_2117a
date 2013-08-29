using Data.Enums;
using Data.Structures.Player;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Data.DAO
{
    public class PlayerDAO : BaseDAO
    {
        private MySqlConnection PlayerDAOConnection;

        public PlayerDAO(string conStr) : base(conStr)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            PlayerDAOConnection = new MySqlConnection(conStr);
            PlayerDAOConnection.Open();

            stopwatch.Stop();
            Log.Info("DAO: PlayerDAO Initialized with {0} Players in {1}s"
            , LoadTotalPlayers()
            , (stopwatch.ElapsedMilliseconds / 1000.0).ToString("0.00"));

        }

        public List<Player> LoadAccountPlayers(string accountname)
        {
            string SqlQuery = "SELECT * FROM `player` WHERE accountname=?accountname AND playerdeleted=?delete";
            MySqlCommand SqlCommand = new MySqlCommand(SqlQuery, PlayerDAOConnection);
            SqlCommand.Parameters.AddWithValue("?accountname", accountname);
            SqlCommand.Parameters.AddWithValue("?delete", 0);
            MySqlDataReader AccountReader = SqlCommand.ExecuteReader();

            List<Player> players = new List<Player>();
            if (AccountReader.HasRows)
            {
                while (AccountReader.Read())
                {
                    Player player = new Player()
                    {
                        PlayerId = AccountReader.GetInt32(0),
                        AccountName = AccountReader.GetString(1),
                        PlayerLevel = AccountReader.GetInt32(2),
                        PlayerExp = AccountReader.GetInt64(3),
                        PlayerExpRecoverable = AccountReader.GetInt64(4),
                        PlayerMount = AccountReader.GetInt32(5),
                        PlayerGuildAccepted = (byte)AccountReader.GetInt16(6),
                        PlayerPraiseGiven = (byte)AccountReader.GetInt16(7),
                        PlayerLastPraise = AccountReader.GetInt32(8),
                        PlayerCurrentBankSection = AccountReader.GetInt32(9),
                        PlayerCreationDate = AccountReader.GetInt32(10),
                        PlayerLastOnlineUtc = AccountReader.GetInt32(11)
                    };
                    players.Add(player);
                }
            }
            AccountReader.Close();

            foreach (var player in players)
            {
                SqlQuery = "SELECT * FROM playerdata WHERE PlayerId=?id";
                SqlCommand = new MySqlCommand(SqlQuery, PlayerDAOConnection);
                SqlCommand.Parameters.AddWithValue("?id", player.PlayerId);
                AccountReader = SqlCommand.ExecuteReader();
                if (AccountReader.HasRows)
                {
                    while (AccountReader.Read())
                    {
                        player.PlayerData = new PlayerData()
                        {
                            Name = AccountReader.GetString(1),
                            Gender = (Gender)Enum.Parse(typeof(Gender), AccountReader.GetString(2)),
                            Race = (Race)Enum.Parse(typeof(Race), AccountReader.GetString(3)),
                            Class = (PlayerClass)Enum.Parse(typeof(PlayerClass), AccountReader.GetString(4)),
                            Data = Funcs.HexToBytes(AccountReader.GetString(5)),
                            Details1 = Funcs.HexToBytes(AccountReader.GetString(6)),
                            Details2 = Funcs.HexToBytes(AccountReader.GetString(7)),
                        };

                        player.Position = new Structures.World.WorldPosition()
                        {
                            MapId = AccountReader.GetInt32(8),
                            X = AccountReader.GetFloat(9),
                            Y = AccountReader.GetFloat(10),
                            Z = AccountReader.GetFloat(11),
                            Heading = AccountReader.GetInt16(12)
                        };
                    }
                }
                AccountReader.Close();
            }


            return players;
        }

        public int SaveNewPlayer(Player player)
        {
            string SqlQuery = "INSERT INTO `player` "
            + "(`AccountName`,`PlayerLevel`,`PlayerExp`,`PlayerExpRecoverable`,`PlayerMount`,`PlayerGuildAccepted`,`PlayerPraiseGiven`,`PlayerLastPraise`,`PlayerCurrentBankSection`,`PlayerCreationDate`,`PlayerLastOnlineUtc`,`PlayerDeleted`) "
            + "VALUES (?accountname, ?level, ?exp, ?exprecover, ?mount, ?gaccept, ?praisgive, ?lastpraise, ?bank, ?credate, ?lastonline, ?delete); SELECT LAST_INSERT_ID();";
            MySqlCommand SqlCommand = new MySqlCommand(SqlQuery, PlayerDAOConnection);
            SqlCommand.Parameters.AddWithValue("?accountname", player.AccountName.ToLower());
            SqlCommand.Parameters.AddWithValue("?level", player.PlayerLevel);
            SqlCommand.Parameters.AddWithValue("?exp", player.PlayerExp);
            SqlCommand.Parameters.AddWithValue("?exprecover", player.PlayerExpRecoverable);
            SqlCommand.Parameters.AddWithValue("?mount", player.PlayerMount);
            SqlCommand.Parameters.AddWithValue("?gaccept", player.PlayerGuildAccepted);
            SqlCommand.Parameters.AddWithValue("?praisgive", player.PlayerPraiseGiven);
            SqlCommand.Parameters.AddWithValue("?lastpraise", player.PlayerLastPraise);
            SqlCommand.Parameters.AddWithValue("?bank", player.PlayerCurrentBankSection);
            SqlCommand.Parameters.AddWithValue("?credate", player.PlayerCreationDate);
            SqlCommand.Parameters.AddWithValue("?lastonline", player.PlayerLastOnlineUtc);
            SqlCommand.Parameters.AddWithValue("?delete", 0);

            int pid;
            try
            {
                pid = Convert.ToInt32(SqlCommand.ExecuteScalar());
            }
            catch (MySqlException ex)
            {
                Log.ErrorException("Player Save Error!", ex);
                return 0;
            }

            SqlQuery = "INSERT INTO playerdata "
            + "(`PlayerId`,`PlayerName`,`PlayerGender`,`PlayerRace`,`PlayerClass`,`PlayerData`,`PlayerDetails1`,`PlayerDetails2`,`PlayerMapId`,`PlayerX`,`PlayerY`,`PlayerZ`,`PlayerH`) "
            + "VALUES (?pid, ?name, ?gender, ?race, ?class, ?data, ?details1, ?details2, ?mapid, ?x, ?y, ?z, ?h);";
            SqlCommand = new MySqlCommand(SqlQuery, PlayerDAOConnection);
            SqlCommand.Parameters.AddWithValue("?pid", pid);
            SqlCommand.Parameters.AddWithValue("?name", player.PlayerData.Name);
            SqlCommand.Parameters.AddWithValue("?gender", player.PlayerData.Gender.ToString());
            SqlCommand.Parameters.AddWithValue("?race", player.PlayerData.Race.ToString());
            SqlCommand.Parameters.AddWithValue("?class", player.PlayerData.Class.ToString());
            SqlCommand.Parameters.AddWithValue("?data", Funcs.BytesToHex(player.PlayerData.Data));
            SqlCommand.Parameters.AddWithValue("?details1", Funcs.BytesToHex(player.PlayerData.Details1));
            SqlCommand.Parameters.AddWithValue("?details2", Funcs.BytesToHex(player.PlayerData.Details2));
            SqlCommand.Parameters.AddWithValue("?mapid", player.Position.MapId);
            SqlCommand.Parameters.AddWithValue("?x", player.Position.X);
            SqlCommand.Parameters.AddWithValue("?y", player.Position.Y);
            SqlCommand.Parameters.AddWithValue("?z", player.Position.Z);
            SqlCommand.Parameters.AddWithValue("?h", player.Position.Heading);

            try
            {
                SqlCommand.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Log.ErrorException("PlayerData Save Error!", ex);
                return 0;
            }

            return pid;
        }

        public void RemovePlayer(int playerId)
        {
            string SqlQuery = "UPDATE `player` SET `playerdeleted`=1 WHERE `PlayerId`=?pid";
            MySqlCommand SqlCommand = new MySqlCommand(SqlQuery, PlayerDAOConnection);
            SqlCommand.Parameters.AddWithValue("?pid", playerId);

            try
            {
                Log.Info("Player Reqlinquished On PID : {0}", playerId);
                SqlCommand.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                Log.ErrorException("Player Relinquish Failed!", e);
                return;
            }
        }

        public void UpdatePlayer(Player player)
        {
            string SqlQuery = "UPDATE `player` SET"
            + "`AccountName`=?accountname,`PlayerLevel`=?lvl,`PlayerExp`=?exp,`PlayerExpRecoverable`=?exprecover,`PlayerMount`=?mount,`PlayerGuildAccepted`=?gaccept,`PlayerPraiseGiven`=?praisgive,`PlayerLastPraise`=?lastpraise,`PlayerCurrentBankSection`=?bank,`PlayerCreationDate`=?credate,`PlayerLastOnlineUtc`=?lastonline "
            + "WHERE PlayerId=?pid";
            MySqlCommand SqlCommand = new MySqlCommand(SqlQuery, PlayerDAOConnection);
            SqlCommand.Parameters.AddWithValue("?accountname", player.AccountName.ToLower());
            SqlCommand.Parameters.AddWithValue("?lvl", player.PlayerLevel);
            SqlCommand.Parameters.AddWithValue("?exp", player.PlayerExp);
            SqlCommand.Parameters.AddWithValue("?exprecover", player.PlayerExpRecoverable);
            SqlCommand.Parameters.AddWithValue("?mount", player.PlayerMount);
            SqlCommand.Parameters.AddWithValue("?gaccept", player.PlayerGuildAccepted);
            SqlCommand.Parameters.AddWithValue("?praisgive", player.PlayerPraiseGiven);
            SqlCommand.Parameters.AddWithValue("?lastpraise", player.PlayerLastPraise);
            SqlCommand.Parameters.AddWithValue("?bank", player.PlayerCurrentBankSection);
            SqlCommand.Parameters.AddWithValue("?credate", player.PlayerCreationDate);
            SqlCommand.Parameters.AddWithValue("?lastonline", player.PlayerLastOnlineUtc);
            SqlCommand.Parameters.AddWithValue("?pid", player.PlayerId);

            try
            {
                SqlCommand.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Log.ErrorException("Player Update Error!", ex);
                return;
            }

            SqlQuery = "UPDATE playerdata SET "
            + "`PlayerData`=?data,`PlayerDetails1`=?details1,`PlayerDetails2`=?details2,`PlayerMapId`=?mapid,`PlayerX`=?x,`PlayerY`=?y,`PlayerZ`=?z,`PlayerH`=?h WHERE `PlayerId`=?pid";
            SqlCommand = new MySqlCommand(SqlQuery, PlayerDAOConnection);
            SqlCommand.Parameters.AddWithValue("?data", Funcs.BytesToHex(player.PlayerData.Data));
            SqlCommand.Parameters.AddWithValue("?details1", Funcs.BytesToHex(player.PlayerData.Details1));
            SqlCommand.Parameters.AddWithValue("?details2", Funcs.BytesToHex(player.PlayerData.Details2));//2113 Add
            SqlCommand.Parameters.AddWithValue("?mapid", player.Position.MapId);
            SqlCommand.Parameters.AddWithValue("?x", player.Position.X);
            SqlCommand.Parameters.AddWithValue("?y", player.Position.Y);
            SqlCommand.Parameters.AddWithValue("?z", player.Position.Z);
            SqlCommand.Parameters.AddWithValue("?h", player.Position.Heading);
            SqlCommand.Parameters.AddWithValue("?pid", player.PlayerId);

            try
            {
                SqlCommand.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Log.ErrorException("Player Data Update Error!", ex);
                return;
            }
        }

        public int LoadTotalPlayers()
        {
            string SqlQuery = "SELECT COUNT(*) FROM `player`";
            MySqlCommand SqlCommand = new MySqlCommand(SqlQuery, PlayerDAOConnection);
            MySqlDataReader LoadACharacterCountReader = SqlCommand.ExecuteReader();

            int count = 0;
            while (LoadACharacterCountReader.Read())
            {
                count = LoadACharacterCountReader.GetInt32(0);
            }

            LoadACharacterCountReader.Close();

            return count;
        }
    }
}
