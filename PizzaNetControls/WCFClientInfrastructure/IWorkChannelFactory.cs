using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetWorkClient.WCFClientInfrastructure;

namespace PizzaNetControls.WCFClientInfrastructure
{
    public interface IWorkChannelFactory
    {
        IWorkChannel GetWorkChannel();
    }
}
