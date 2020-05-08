using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using EmbedIO;
using NLog;



namespace StepmaniaServer
{
    class WebServer
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        private static Config config = new Config();

        // start the web server
        public WebServer()
        {
            // setup logging
            SetupLogging();

            // get server config options
            logger.Trace("Initialising Web Server");
            string serverIp = config.Get("/config/web-server/ip", "localhost");
            string serverPort = config.Get("/config/web-server/port", "8080");
            string serverUrl = String.Format("http://{0}:{1}", serverIp, serverPort);

            // run the server async
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Task.WaitAll(
                    RunWebServerAsync(serverUrl, cts.Token)
                );
            }

            logger.Trace("WebServer stopped");
        }

        private static void SetupLogging()
        {
            // unregister the default logger and register our own
            // logger to use NLog instead of SWAN
            Swan.Logging.Logger.UnregisterLogger<Swan.Logging.ConsoleLogger>();
            Swan.Logging.Logger.RegisterLogger<WebServerLogger>();
        }

        // creates and configures the webs server
        private static EmbedIO.WebServer CreateWebServer(string url)
        {
            // the path to our web client
            string webServerFiles = Path.Combine(Directory.GetCurrentDirectory(), "Web");
            logger.Trace("Web server files: {location}", webServerFiles);

            // create the web server object
            EmbedIO.WebServer webServer = new EmbedIO.WebServer(o => o
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO)
            ).WithLocalSessionManager()
            .WithStaticFolder("/", webServerFiles, true);

            webServer.StateChanged += (s, e) => logger.Trace("WebServer New State - {state}", e.NewState);

            return webServer;
        }

        private static async Task RunWebServerAsync(string url, CancellationToken cancellationToken)
        {
            logger.Trace("Starting Web Server");
            EmbedIO.WebServer server = CreateWebServer(url);
            await server.RunAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    // the custom logger which wraps the NLog logging system in the Swan logging format
    // so that the application only technically outputs 1 logging format
    public class WebServerLogger : Swan.Logging.ILogger
    {
        // NLog
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        // only use a LogLevel of Info as EmbedIO outputs way too much Debug information
        public Swan.Logging.LogLevel LogLevel { get; set; } = Swan.Logging.LogLevel.Info;

        // forwards all logging attempts to NLogs logging functions
        public void Log(Swan.Logging.LogMessageReceivedEventArgs logEvent)
        {
            string message = logEvent.Message;

            switch(logEvent.MessageType)
            {
                case Swan.Logging.LogLevel.Debug:
                    logger.Debug(message);
                    break;

                case Swan.Logging.LogLevel.Error:
                    logger.Error(message);
                    break;

                case Swan.Logging.LogLevel.Fatal:
                    logger.Fatal(message);
                    break;

                case Swan.Logging.LogLevel.Info:
                    logger.Info(message);
                    break;

                case Swan.Logging.LogLevel.Trace:
                    logger.Trace(message);
                    break;

                case Swan.Logging.LogLevel.Warning:
                    logger.Warn(message);
                    break;
            }
        }

        // a method that has to be here?
        public void Dispose() {}
    }
}
