using Data.Structures.Account;
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
    public class AccountDAO : BaseDAO
    {
        private MySqlConnection AccountDAOConnection;

        public AccountDAO(string conStr) : base(conStr)
        {            
            Stopwatch stopwatch = Stopwatch.StartNew();
            AccountDAOConnection = new MySqlConnection(conStr);
            AccountDAOConnection.Open();

            stopwatch.Stop();
            Log.Info("DAO: AccountDAO Initialized with {0} Accounts in {1}s"
            , LoadTotalAccounts()
            , (stopwatch.ElapsedMilliseconds / 1000.0).ToString("0.00"));
        }

        public Account LoadAccount(string accountname)
        {
            string SqlQuery = "SELECT * FROM `accounts` WHERE `accountname` = ?accountname";
            MySqlCommand SqlCommand = new MySqlCommand(SqlQuery, AccountDAOConnection);
            SqlCommand.Parameters.AddWithValue("?accountname", accountname);
            MySqlDataReader AccountReader = SqlCommand.ExecuteReader();

            Account account = new Account();
            if (AccountReader.HasRows)
            {
                while (AccountReader.Read())
                {
                    account.AccountId = AccountReader.GetInt32(0);
                    account.AccountName = AccountReader.GetString(1);
                    account.AccountPassword = AccountReader.GetString(2);
                    account.AccountEmail = AccountReader.GetString(3);
                    account.AccessLevel = (byte)AccountReader.GetInt32(4);
                    account.Membership = AccountReader.GetInt32(5);
                    account.isGM = AccountReader.GetBoolean(6);
                    account.LastOnlineUtc = AccountReader.GetInt64(7);
                    account.Coins = (int)AccountReader.GetInt32(8);

                }
            }
            AccountReader.Close();
            return (account.AccountName == "") ? null : account;
        }

        //registering accounts, seems not used?!?
        public bool SaveAccount(Account account)
        {
            string SqlQuery = "INSERT INTO `accounts` (`accountname`,`password`) VALUES(?accountname, ?password);";
            MySqlCommand SqlCommand = new MySqlCommand(SqlQuery, AccountDAOConnection);
            SqlCommand.Parameters.AddWithValue("?accountname", account.AccountName);
            SqlCommand.Parameters.AddWithValue("?password", account.AccountPassword);

            try
            {
                SqlCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Log.ErrorException("DAO: SAVE ACCOUNT ERROR!", e);
            }
            return false;
        }

        public int LoadTotalAccounts()
        {
            string SQL = "SELECT COUNT(*) FROM `accounts`";
            MySqlCommand cmd = new MySqlCommand(SQL, AccountDAOConnection);
            MySqlDataReader LoadAccountCountReader = cmd.ExecuteReader();

            int count = 0;
            while (LoadAccountCountReader.Read())
            {
                count = LoadAccountCountReader.GetInt32(0);
            }

            LoadAccountCountReader.Close();

            return count;
        }

    }
}
