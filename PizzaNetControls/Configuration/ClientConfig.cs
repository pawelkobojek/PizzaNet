using PizzaNetCommon.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PizzaNetControls.Configuration
{
    public class ClientConfig : AbstractConfig, IXmlSerializable
    {
        public static string CONFIGNAME = "configuration.xml";

        public ClientConfig()
        {
            ServerAddress = "https://localhost:44300/PizzaService.svc";
            User = new User();
        }

        public User User { get; set; }

        public string ServerAddress { get; set; }

        private static ClientConfig cfg = null;
        private static string readingUserName = null;
        private static readonly object syncRoot = new object();

        public static ClientConfig getConfig(string userName)
        {
            cfg = null;
            lock (syncRoot)
            {
                readingUserName = userName;
                cfg = readConfig();
                readingUserName = null;
            }
            return cfg;
        }

        public static ClientConfig getConfig()
        {
            lock (syncRoot)
            {
                return readConfig();
            }
        }

        private static ClientConfig readConfig()
        {
            if (cfg != null) return cfg;
            try
            {
                cfg = (ClientConfig)AbstractConfig.Read(ClientConfig.CONFIGNAME, typeof(ClientConfig));
            }
            catch (InvalidOperationException)
            {
                cfg = new ClientConfig();
                cfg.Save(ClientConfig.CONFIGNAME, typeof(ClientConfig));
            }
            catch (System.IO.IOException)
            {
                cfg = new ClientConfig();
                cfg.Save(ClientConfig.CONFIGNAME, typeof(ClientConfig));
            }
            return cfg;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ClientConfig")
            {
                while (reader.Read())
                {
                    if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ServerAddress")
                    {
                        ServerAddress = reader.ReadElementContentAsString();
                    }
                    else if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "User")
                    {
                        if (readingUserName!=null && reader["Email"]==readingUserName)
                        {
                            User = new User();
                            User.ReadXml(reader);
                        }
                    }
                }
            }
        }

        public void Save()
        {
            this.Save(ClientConfig.CONFIGNAME, typeof(ClientConfig));
        }

        public override void Save(string filename, Type type)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filename);
            }
            catch(Exception)
            {
                base.Save(filename, type);
                return;
            }
            XmlNode ccfg = doc.SelectSingleNode("//ClientConfig");
            List<XmlNode> nodesToRemove = new List<XmlNode>();
            List<XmlNode> nodesToInsert = new List<XmlNode>();
            bool UserInserted = false;
            foreach(XmlNode n in ccfg.ChildNodes)
            {
                if (n.Name == "User" && n.Attributes["Email"].Value == this.User.Email)
                {
                    nodesToRemove.Add(n);
                    nodesToInsert.Add(doc.ImportNode(this.User.getNode(),true));
                    UserInserted = true;
                }
                else if (n.Name == "ServerAddress")
                {
                    n.InnerText = this.ServerAddress;
                }
            }
            if (!UserInserted)
                nodesToInsert.Add(doc.ImportNode(this.User.getNode(), true));
            foreach(var n in nodesToRemove)
                ccfg.RemoveChild(n);
            foreach (var n in nodesToInsert)
                ccfg.AppendChild(n);
            doc.Save(filename);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("ServerAddress",ServerAddress);
        }
    }
}
