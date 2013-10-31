using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Worker
{
    public class WorkFinishedEventArgs : EventArgs
    {
        public object Result { get; set; }
        public object Parameter { get; set; }

        public WorkFinishedEventArgs()
        {
        }

        public WorkFinishedEventArgs(object result, object param)
        {
            Result = result;
            Parameter = param;
        }
    }
}
