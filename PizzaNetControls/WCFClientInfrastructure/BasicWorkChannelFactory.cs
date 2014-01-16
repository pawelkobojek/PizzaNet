using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetWorkClient.WCFClientInfrastructure;

namespace PizzaNetControls.WCFClientInfrastructure
{
    public class BasicWorkChannelFactory : IWorkChannelFactory
    {
        public PizzaNetWorkClient.WCFClientInfrastructure.IWorkChannel GetWorkChannel()
        {
            return new WorkChannel();
        }
    }
}
