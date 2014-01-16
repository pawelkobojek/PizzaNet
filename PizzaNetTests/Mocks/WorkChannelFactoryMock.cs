using PizzaNetControls.WCFClientInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetTests.Mocks
{
    public class WorkChannelFactoryMock : IWorkChannelFactory
    {
        public PizzaNetWorkClient.WCFClientInfrastructure.IWorkChannel GetWorkChannel()
        {
            return new WorkChannelMock();
        }
    }
}
