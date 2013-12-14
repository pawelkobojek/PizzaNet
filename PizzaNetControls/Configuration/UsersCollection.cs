using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Configuration
{
    [ConfigurationCollection(typeof(ClientConfig.User), AddItemName = "user", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class UsersCollection : BaseConfigurationElementCollection<ClientConfig.User>
    {
        public ClientConfig.User Find(Func<ClientConfig.User,bool> predicate)
        {
            foreach(var u in this)
                if (predicate(u))
                    return u;
            return null;
        }
    }
}
