using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Configuration
{
    public class UsersConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("users", IsRequired = true)]
        public UsersCollection Users { get; set; }
    }
}
