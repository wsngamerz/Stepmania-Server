using System;
using System.IO;
using System.Xml;

using NLog;


namespace StepmaniaServer
{
    public class Config
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        static Config instance = null;
        static readonly object padlock = new object();

        // the config file contents
        XmlDocument document;

        public Config()
        {
            if(!File.Exists("config.xml"))
            {
                // if the file doesn't already exist, create it and populate it with some
                // default values
                Console.WriteLine("Unable to find Config File, creating new file");
                // ensure that the xml file is readable by adding proper indentation
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "    ";

                using (XmlWriter writer = XmlWriter.Create("config.xml", settings))
                {
                    writer.WriteStartElement("config");

                    writer.WriteStartElement("game-server");
                    writer.WriteElementString("ip", "0.0.0.0");
                    writer.WriteElementString("port", "8765");
                    writer.WriteElementString("name", "WSNStepzz");
                    writer.WriteElementString("timeout", "1000");
                    writer.WriteElementString("protocol", "128");
                    writer.WriteEndElement();
                    
                    writer.WriteStartElement("web-server");
                    writer.WriteElementString("enabled", "true");
                    writer.WriteElementString("ip", "0.0.0.0");
                    writer.WriteElementString("port", "8080");
                    writer.WriteEndElement();

                    writer.WriteStartElement("database");
                    writer.WriteElementString("type", "sqlite");
                    writer.WriteElementString("file", "database.db");
                    writer.WriteElementString("username", "user");
                    writer.WriteElementString("password", "pass");
                    writer.WriteElementString("database", "stepmaniaserver");
                    writer.WriteElementString("host", "localhost");
                    writer.WriteElementString("port", "3306");
                    writer.WriteEndElement();
                    
                    writer.WriteEndElement();
                    writer.Flush();
                }
            }

            // load the xml file contents into the class
            document = new XmlDocument();
            document.Load("config.xml");
        }

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
