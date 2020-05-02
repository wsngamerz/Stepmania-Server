using System;
using System.IO;
using System.Xml;



namespace StepmaniaServer
{
    public class Config
    {
        static Config instance = null;
        static readonly object padlock = new object();

        XmlDocument document;

        public Config()
        {
            if(!File.Exists("config.xml"))
            {
                Console.WriteLine("Unable to find Config File, creating new file");
                using (XmlWriter writer = XmlWriter.Create("config.xml"))
                {
                    writer.WriteStartElement("config");
                    writer.WriteStartElement("server");
                    writer.WriteElementString("ip", "0.0.0.0");
                    writer.WriteElementString("port", "8765");
                    writer.WriteElementString("name", "WSNStepzz");
                    writer.WriteElementString("timeout", "1000");
                    writer.WriteElementString("protocol", "128");
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

            document = new XmlDocument();
            document.Load("config.xml");
        }

        // ensure only 1 config class is ever created accross threads
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

            if ((result != null))
            {
                return result[0].InnerText;
            }
            else
            {
                return fallback;
            }
        }
    }
}
