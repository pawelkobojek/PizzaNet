using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Workers
{
    public class WorkFinishedEventArgs : EventArgs
    {
        public object Result { get; set; }
        public object[] Arguments { get; set; }

        public WorkFinishedEventArgs()
        {
        }

        public WorkFinishedEventArgs(object result, object[] arguments)
        {
            Result = result;
            Arguments = arguments;
        }
    }
}
