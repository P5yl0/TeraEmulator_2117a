using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Utils;

namespace Tera
{
    class Config
    {
        //database
        public static string GetDatabaseHost()
        {
            string dbHost = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"configs/databaseConfig.xml");
                XmlNode node = doc.SelectSingleNode("database_configuration/database_Host");
                dbHost = node.InnerText;
            }
            catch (Exception /*ex*/)
            {
                Log.Fatal("Config: Database host NOT FOUND!!!");
            }
            return dbHost;
        }
        public static string GetDatabasePort()
        {
            string dbPort = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"configs/databaseConfig.xml");
                XmlNode node = doc.SelectSingleNode("database_configuration/database_Port");
                dbPort = node.InnerText;
            }
            catch (Exception /*ex*/)
            {
                Log.Fatal("Config: Database port NOT FOUND!!!");
            }
            return dbPort;
        }
        public static string GetDatabaseUser()
        {
            string dbUser = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"configs/databaseConfig.xml");
                XmlNode node = doc.SelectSingleNode("database_configuration/database_User");
                dbUser = node.InnerText;
            }
            catch (Exception /*ex*/)
            {
                Log.Fatal("Config: Database user NOT FOUND!!!");
            }
            return dbUser;
        }
        public static string GetDatabasePass()
        {
            string dbPass = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"configs/databaseConfig.xml");
                XmlNode node = doc.SelectSingleNode("database_configuration/database_Pass");
                dbPass = node.InnerText;
            }
            catch (Exception /*ex*/)
            {
                Log.Fatal("Config: Database pass NOT FOUND!!!");
            }
            return dbPass;
        }
        public static string GetDatabaseName()
        {
            string dbName = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"configs/databaseConfig.xml");
                XmlNode node = doc.SelectSingleNode("database_configuration/database_Name");
                dbName = node.InnerText;
            }
            catch (Exception /*ex*/)
            {
                Log.Fatal("Config: Database name NOT FOUND!!!");
            }
            return dbName;
        }

        //server
        public static String GetServerIp(String defaultValue = "127.0.0.1")
        {
            String ip = defaultValue;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"configs/networkConfig.xml");
                XmlNode node = doc.SelectSingleNode("network_configuration/network_IP");
                ip = node.InnerText;
            }
            catch (Exception /*ex*/)
            {
                Log.Fatal("Config: Network_IP NOT FOUND!!!");
            }
            return ip;
        }
        public static int GetServerPort(int defaultValue = 11101)
        {
            int port = defaultValue;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"configs/networkConfig.xml");
                XmlNode node = doc.SelectSingleNode("network_configuration/network_Port");
                port = Convert.ToInt32(node.InnerText);
            }
            catch (Exception /*ex*/)
            {
                Log.Fatal("Config: Network_Port NOT FOUND!!!");
            }
            return port;
        }
        public static int GetServerMaxCon(int defaultValue = 100)
        {
            int maxCon = defaultValue;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"configs/networkConfig.xml");
                XmlNode node = doc.SelectSingleNode("network_configuration/network_MaxCon");
                maxCon = Convert.ToInt32(node.InnerText);
            }
            catch (Exception /*ex*/)
            {
                Log.Fatal("Config: Network_MaxCon NOT FOUND!!!");
            }
            return maxCon;
        }

        //gameplay
        public static int GetLevelCap(int defaultValue = 60)
        {
            int levelCap = defaultValue;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"configs/serverConfig.xml");
                XmlNode node = doc.SelectSingleNode("server_configuration/level_Cap");
                levelCap = Convert.ToInt32(node.InnerText);
            }
            catch (Exception /*ex*/)
            {
                Log.Fatal("Config: level_Cap NOT FOUND!!!");
            }
            return levelCap;
        }
        public static int GetServerRates(int defaultValue = 1)
        {
            int serverRates = defaultValue;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"configs/serverConfig.xml");
                XmlNode node = doc.SelectSingleNode("server_configuration/server_Rates");
                serverRates = Convert.ToInt32(node.InnerText);
            }
            catch (Exception /*ex*/)
            {
                Log.Fatal("Config: server_Rates NOT FOUND!!!");
            }
            return serverRates;
        }
        public static String GetWelcomeMessage(String defaultValue = "Welcome")
        {
            String welcomeMessage = defaultValue;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"configs/serverConfig.xml");
                XmlNode node = doc.SelectSingleNode("server_configuration/welcome_Message");
                welcomeMessage = node.InnerText;
            }
            catch (Exception /*ex*/)
            {
                Log.Fatal("Config: welcome_Message NOT FOUND!!!");
            }
            return welcomeMessage;
        }
    
    }
}
