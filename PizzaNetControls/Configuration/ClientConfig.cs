using PizzaNetCommon.DTOs;
using System;
using System.Xml.Serialization;

namespace PizzaNetControls.Configuration
{
    public class ClientConfig : AbstractConfig
    {
        public static string CONFIGNAME = "configuration.xml";

        public ClientConfig()
        {
            ServerAddress = 
                "http://localhost:60499/PizzaService.svc";
            //"https://localhost:44300/PizzaService.svc";
            User = new UserDTO();
        }

        [XmlIgnore]
        public UserDTO User { get; set; }
        [XmlIgnore]
        public string Password { get; set; }

        public string ServerAddress { get; set; }

        private static ClientConfig cfg = null;

        public static ClientConfig getConfig()
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
    }
}
