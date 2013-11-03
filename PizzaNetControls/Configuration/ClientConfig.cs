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
            UserPhone = "";
        }

        public ClientConfig(string userAddress, string userPhone)
        {
            UserAddress = userAddress;
            UserPhone = userPhone;
        }

        public string UserAddress { get; set; }
        public string UserPhone { get; set; }
    }
}
