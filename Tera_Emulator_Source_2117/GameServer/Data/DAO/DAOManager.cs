using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DAO
{
    public static class DAOManager
    {
        public static AccountDAO accountDAO;
        public static GuildDAO guildDAO;
        public static InventoryDAO inventoryDAO;
        public static PlayerDAO playerDAO;
        public static QuestDAO questDAO;
        public static SkillsDAO skillDAO;

        public static void Initialize(string constr)
        {
            accountDAO = new AccountDAO(constr);
            guildDAO = new GuildDAO(constr);
            inventoryDAO = new InventoryDAO(constr);
            playerDAO = new PlayerDAO(constr);
            questDAO = new QuestDAO(constr);
            skillDAO = new SkillsDAO(constr);

            Console.WriteLine("-------------------------------------------");
        }
    }
}
