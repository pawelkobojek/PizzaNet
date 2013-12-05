using PizzaNetControls.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Views
{
    public class BaseView
    {
        public IWorker Worker { get; private set; }

        public BaseView(IWorker worker)
        {
            Worker = worker;
        }
    }
}
