using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Worker
{
    public class WorkerTask
    {
        public delegate object RunnableTask(object[] args);
        public delegate void WorkFinishedHandler(object sender, WorkFinishedEventArgs e);

        public WorkerTask()
        {
        }

        public WorkerTask(RunnableTask task, WorkFinishedHandler handler, params object[] args)
        {
            this.Task = task;
            this.Handler = handler;
            this.Arguments = args;
        }

        public RunnableTask Task { get; set; }
        public WorkFinishedHandler Handler { get; set; }
        public object[] Arguments { get; set; }
        public object Result { get; set; }
    }
}
