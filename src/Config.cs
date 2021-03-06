using System;
using System.IO;
using System.Xml;

using NLog;



namespace StepmaniaServer
{
    /// <summary>
    /// Manage application wide configuration
    /// </summary>
    public class Config
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        static Config instance = null;
        static readonly object padlock = new object();

        // the config file contents
        XmlDocument document;

        // ensure only 1 config class is ever created accross threads
        // 'Singleton Class - should be thread safe?'
        public static Config Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Config();
                    }
                    return instance;
                }
            }
        }

        /// <summary>
        /// Manages application wide configuration values
        /// </summary>
        public Config()
        {
            if(!File.Exists("Data/config.xml"))
            {
                WriteXMLConfig();
            }

            // load the xml file contents into the class
            document = new XmlDocument();
            document.Load("Data/config.xml");
        }

        /// <summary>
        /// Create the default configuration file filled with the default values
        /// </summary>
        private void WriteXMLConfig()
        {
            Console.WriteLine("creating configuration file");

            // create the folder if it doesnt exist
            Directory.CreateDirectory("Data/");

            // ensure that the xml file is readable by adding proper indentation
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";

            using (XmlWriter writer = XmlWriter.Create("Data/config.xml", settings))
            {
                // start config options
                writer.WriteComment("Stepmainia Server aspires to be extremly configurable so here you go");
                writer.WriteStartElement("config");

                // start logging config options
                writer.WriteComment("Config options relating to logging");
                writer.WriteStartElement("logging");

                writer.WriteComment("level: The log level to output");
                writer.WriteElementString("level", "Info");

                writer.WriteComment("path: The path to the output log file");
                writer.WriteElementString("path", "${currentdir}/Logs/stepmaniaserver.log");

                writer.WriteEndElement();
                // end logging config

                // start game-server config options
                writer.WriteComment("Config options relating to the game server");
                writer.WriteStartElement("game-server");

                writer.WriteComment("ip: the ip to host the game server on");
                writer.WriteElementString("ip", "0.0.0.0");
                
                writer.WriteComment("port: the port to host the game server on (not recommended to change)");
                writer.WriteElementString("port", "8765");
                
                writer.WriteComment("name: The server name");
                writer.WriteElementString("name", "WSNStepzz");
                
                writer.WriteComment("timeout: The server timeout in ms");
                writer.WriteElementString("timeout", "1000");
                
                writer.WriteComment("protocol: The server protocol version (not recommended to change)");
                writer.WriteElementString("protocol", "128");
                
                writer.WriteEndElement();
                // end game-server config options
                
                // start web-server config options
                writer.WriteComment("Config options relating to the web server");
                writer.WriteStartElement("web-server");
                
                writer.WriteComment("enabled: [true/false] - whether to enable the embedded web server or not");
                writer.WriteElementString("enabled", "true");
                
                writer.WriteComment("ip: the ip to host the web server on");
                writer.WriteElementString("ip", "localhost");
                
                writer.WriteComment("port: the port to host the web server on");
                writer.WriteElementString("port", "8080");
                
                writer.WriteEndElement();
                // end web-server config options

                // start database config options
                writer.WriteComment("Config options relating to the database");
                writer.WriteStartElement("database");
                
                writer.WriteComment("type: Can currently be one of [sqlite, mysql]");
                writer.WriteElementString("type", "sqlite");
                
                writer.WriteComment("file: a file path used to store the sqlite database (SQLite Only)");
                writer.WriteElementString("file", "Data/database.db");
                
                writer.WriteComment("The following options are only used in database servers such as MySQL (Not SQLite)");
                writer.WriteComment("username: Username to the database server");
                writer.WriteElementString("username", "user");
                
                writer.WriteComment("password: Password to the database server");
                writer.WriteElementString("password", "pass");
                
                writer.WriteComment("database: The name of the database to use/create");
                writer.WriteElementString("database", "stepmaniaserver");
                
                writer.WriteComment("host: The host of the database server");
                writer.WriteElementString("host", "localhost");
                
                writer.WriteComment("port: The port of the database server");
                writer.WriteElementString("port", "3306");
                
                writer.WriteEndElement();
                // end database config options
                
                writer.WriteEndElement();
                // end config options

                // write to file
                writer.Flush();
            }
        }

        /// <summary>
        /// Returns a value stored in configuration
        /// </summary>
        /// <param name="path">xml path to the value</param>
        /// <param name="fallback">the value to return if the path is invalid or nor found</param>
        /// <returns>configuration value</returns>
        public string Get(string path, string fallback = "")
        {
            var result = document.SelectNodes(path);

            // if the path for a value exists
            // return it
            if ((result != null) && (result[0] != null))
            {
                return result[0].InnerText;
            }
            else
            {
                // otherwise, return the fallback value and log to the console as it probably
                // means that the option doesn't exist in the config file.
                logger.Trace("Using fallback value of {fallback} for {path}", fallback, path);
                return fallback;
            }
        }
    }
}
