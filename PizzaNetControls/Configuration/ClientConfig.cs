using PizzaNetCommon.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PizzaNetControls.Configuration
{
    public static class ClientConfig
    {
        private static UsersDictionary Users { get; set; }
        private const string USERS = "users.xml";

        public static User CurrentUser { get; set; }

        public static User GetUser(string login)
        {
            LoadConfig();
            User res = null;
            if (Users.ContainsKey(login))
                res= Users[login];
            if (res==null)
            {
                res = new User() { Email = login };
                Users.Add(res.Email, res);
            }
            return res;
        }

        public static void LoadConfig()
        {
            System.Type t = typeof(UsersDictionary);
            Users = new UsersDictionary();
            if (File.Exists(USERS))
                try
                {
                    using (XmlTextReader reader = new XmlTextReader(USERS))
                    {
                        Users.ReadXml(reader);
                    }
                }
                catch(IOException)
                {
                    Users.Clear();
                }
                catch (XmlException)
                {
                    Users.Clear();
                }
                catch(InvalidOperationException)
                {
                    Users.Clear();
                }
            else Users = new UsersDictionary();
        }

        public static void Save()
        {
            if (Users == null) return;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            using(XmlWriter writer = XmlWriter.Create(USERS,settings))
            {
                Users.WriteXml(writer);
            }
        }
    }
}
