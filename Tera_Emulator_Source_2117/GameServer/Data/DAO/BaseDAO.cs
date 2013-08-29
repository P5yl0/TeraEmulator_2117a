using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Data.DAO
{
    public abstract class BaseDAO
    {
        private string ConnectionString = string.Empty;

        public static MySqlConnection Connection;

        public BaseDAO(string conStr)
        {
            this.ConnectionString = conStr;
            Connection = new MySqlConnection(this.ConnectionString);

            try
            {
                Connection.Open();
            }
            catch (Exception ex)
            {
                Log.ErrorException("Cannot connect to MySQL", ex);
            }
            finally
            {
                Connection.Close();
            }
        }
    }
}
