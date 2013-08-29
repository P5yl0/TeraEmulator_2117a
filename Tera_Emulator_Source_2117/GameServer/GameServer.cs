using System;
using System.Diagnostics;
using System.Threading;
using Communication;
using Communication.Interfaces;
using Communication.Logic;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using Network;
using Tera.Services;
using Utils;
using Data.DAO;

namespace Tera
{
    internal class GameServer : Global
    {
        public static TcpServer TcpServer;

        public static void Main()
        {
            Console.Title = "Tera-Project C# Emulator";
            Console.CancelKeyPress += CancelEventHandler;

            try
            {
                RunServer();
            }
            catch (Exception ex)
            {
                Log.FatalException("Can't start server!", ex);
                return;
            }

            MainLoop();
            StopServer();
            Process.GetCurrentProcess().Kill();
        }

        private static void RunServer()
        {
            Data.Data.DataPath = "data/";

            Stopwatch sw = Stopwatch.StartNew();

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            Console.WriteLine("----===== Tera-Project C# GameServer Emulator =====----\n\n");
            Console.WriteLine("Starting Game Server!\n"
                + "-------------------------------------------");

            TcpServer = new TcpServer("*", Config.GetServerPort(), Config.GetServerMaxCon());
            Connection.SendAllThread.Start();

            OpCodes.Init();
            Console.WriteLine("OpCodes - Revision:" + OpCodes.Version + " EU initialized!\n"
                + "-------------------------------------------\n");

            #region global_components

            //services
            FeedbackService = new FeedbackService();
            AccountService = new AccountService();
            PlayerService = new PlayerService();
            MapService = new MapService();
            ChatService = new ChatService();
            VisibleService = new VisibleService();
            ControllerService = new ControllerService();
            CraftService = new CraftService();
            ItemService = new ItemService();
            AiService = new AiService();
            GeoService = new GeoService();
            StatsService = new StatsService();
            ObserverService = new ObserverService();
            AreaService = new AreaService();
            TeleportService = new TeleportService();
            PartyService = new PartyService();
            SkillsLearnService = new SkillsLearnService();
            CraftLearnService = new CraftLearnService();
            GuildService = new GuildService();
            EmotionService = new EmotionService();
            RelationService = new RelationService();
            DuelService = new DuelService();
            StorageService = new StorageService();
            TradeService = new TradeService();
            MountService = new MountService();

            //engines
            ActionEngine = new ActionEngine.ActionEngine();
            AdminEngine = new AdminEngine.AdminEngine();
            SkillEngine = new SkillEngine.SkillEngine();
            QuestEngine = new QuestEngine.QuestEngine();

            #endregion

            GlobalLogic.ServerStart("SERVER=" + Config.GetDatabaseHost() + ";DATABASE=" + Config.GetDatabaseName() + ";UID=" + Config.GetDatabaseUser() + ";PASSWORD=" + Config.GetDatabasePass() + ";PORT=" + Config.GetDatabasePort() + ";charset=utf8");

            Console.WriteLine("-------------------------------------------\n"
                + "Loading Tcp Service.\n"
                + "-------------------------------------------");

            TcpServer.BeginListening();

            sw.Stop();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine(" Server start in {0}", (sw.ElapsedMilliseconds / 1000.0).ToString("0.00s"));
            Console.WriteLine("-------------------------------------------");
        }

        private static void StopServer()
        {
            TcpServer.ShutdownServer();
            GlobalLogic.OnServerShutdown();
        }

        protected static void UnhandledException(Object sender, UnhandledExceptionEventArgs args)
        {
            Log.FatalException("UnhandledException", (Exception)args.ExceptionObject);

            ShutdownServer();

            while (true)
                Thread.Sleep(1);
        }

        protected static void CancelEventHandler(object sender, ConsoleCancelEventArgs args)
        {
            ShutdownServer();

            while (true)
                Thread.Sleep(1);
        }

    }
}