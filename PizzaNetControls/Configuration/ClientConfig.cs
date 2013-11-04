using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Configuration
{
    public class ClientConfig : AbstractConfig
    {
        public const string CONFIGNAME = "configuration.xml";

        public ClientConfig()
        {
            UserAddress = "";
            UserPhone = 0;
        }

        public ClientConfig(string userAddress, int userPhone)
        {
            UserAddress = userAddress;
            UserPhone = userPhone;
        }

        public string UserAddress { get; set; }
        public int UserPhone { get; set; }

        public static ClientConfig getConfig()
        {
            ClientConfig cfg;
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
