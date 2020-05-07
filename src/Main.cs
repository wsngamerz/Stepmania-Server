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

        public static StepmaniaContext dbContext;
        public static bool isRunning = true;

        static void Main(string[] args)
        {
            SetupLogging();
            logger.Info("StepmaniaServer ALPHA [{servername}] Starting", config.Get("/config/game-server/name", "Unknown Server Name"));

            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "MainThread";
            }

            SetupDatabases();

            // start GameServer thread
            gameServerThread = new Thread(() => new GameServer());
            gameServerThread.Name = "GameServer Thread";
            gameServerThread.Start();

            // start WebServer thread if enabled
            if (config.Get("/config/web-server/enabled", "true") == "true")
            {
                webServerThread = new Thread(() => new WebServer());
                webServerThread.Name = "WebServer Thread";
                webServerThread.Start();
            }

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
            NLog.Config.LoggingConfiguration loggingConfiguration = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            string loggingFormat = @"[${date:format=HH\:mm\:ss}] [${callsite}] [${level}] ${message} ${exception}";

            // Logging config for log file
            NLog.Targets.FileTarget logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = config.Get("/config/logging/path", "${currentdir}/Logs/stepmaniaserver.log"),
                Layout = loggingFormat,
                ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Rolling,
                ArchiveEvery = NLog.Targets.FileArchivePeriod.Day
            };

            // Logging configuration for logging to console (IN COLOUR!!!)
            NLog.Targets.ColoredConsoleTarget logconsole = new NLog.Targets.ColoredConsoleTarget("logconsole")
            {
                Layout = loggingFormat
            };

            LogLevel loggingLevel = LogLevel.Info;
            switch (config.Get("/config/logging/level"))
            {
                case "Trace":
                    loggingLevel = LogLevel.Trace;
                    break;
                
                case "Debug":
                    loggingLevel = LogLevel.Debug;
                    break;
                
                case "Info":
                    loggingLevel = LogLevel.Info;
                    break;
                
                case "Warning":
                    loggingLevel = LogLevel.Warn;
                    break;
                
                case "Error":
                    loggingLevel = LogLevel.Error;
                    break;
                
                case "Fatal":
                    loggingLevel = LogLevel.Fatal;
                    break;
            }

            // Apply the rules!
            loggingConfiguration.AddRule(loggingLevel, LogLevel.Fatal, logconsole);
            loggingConfiguration.AddRule(loggingLevel, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = loggingConfiguration;

            logger.Trace("Setting up logging");
        }

        private static void SetupDatabases()
        {
            logger.Trace("Setting up databases");
            dbContext = new StepmaniaContext();
            // Create database if not exists
            dbContext.Database.EnsureCreated();

            // Save Changes
            dbContext.SaveChanges();
        }
    }
}
