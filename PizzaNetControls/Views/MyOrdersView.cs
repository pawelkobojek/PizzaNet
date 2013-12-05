using PizzaNetControls.Workers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Views
{
    public class MyOrdersView : BaseView
    {
        public MyOrdersView(IWorker worker) : base(worker)
        {
        }
    }
}
