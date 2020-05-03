using System;
using System.Threading;

using NLog;


namespace StepmaniaServer
{
    class StepmaniaServer
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static Config config = new Config();

        private static Thread gameServerThread;
        private static Thread webServerThread;

        public static bool isRunning = true;

        static void Main(string[] args)
        {
            SetupLogging();
            logger.Info("StepmaniaServer ALPHA [{servername}] Starting", config.Get("/config/game-server/name", "Unknown Server Name"));

            // start GameServer thread
            gameServerThread = new Thread(GameServer.Start);
            gameServerThread.Start();

            while (isRunning)
            {
                Thread.Sleep(500);
            }

            // on shutdown
            LogManager.Shutdown();
        }

        private static void SetupLogging()
        {
            // setup the configuration for logging
            NLog.Config.LoggingConfiguration config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            string loggingFormat = @"[${date:format=HH\:mm\:ss}] [${callsite}] [${level}] ${message} ${exception}";

            // Logging config for log file
            NLog.Targets.FileTarget logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = "${basedir}/log.txt",
                Layout = loggingFormat
            };

            // Logging configuration for logging to console (IN COLOUR!!!)
            NLog.Targets.ColoredConsoleTarget logconsole = new NLog.Targets.ColoredConsoleTarget("logconsole")
            {
                Layout = loggingFormat
            };

            // Apply the rules!
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = config;

            logger.Trace("Setting up logging");
        }
    }
}
