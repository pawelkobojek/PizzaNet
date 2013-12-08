using System;
using System.Xml.Serialization;

namespace PizzaNetControls.Configuration
{
    public class ClientConfig : AbstractConfig
    {
        public static string CONFIGNAME = "configuration.xml";

        public ClientConfig()
        {
            ServerAddress = "http://localhost:60499/PizzaService.svc";
            UserAddress = "";
            Login = "";
            Password = "";
            Phone = 0;
            Rights = 0;
        }

        [XmlIgnore]
        public string Login { get; set; }
        [XmlIgnore]
        public string Password { get; set; }
        [XmlIgnore]
        public string UserAddress { get; set; }
        [XmlIgnore]
        public int Phone { get; set; }
        [XmlIgnore]
        public int Rights { get; set; }

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
