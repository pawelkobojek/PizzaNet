using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Workers
{
    public class WorkerTask
    {
        public delegate object RunnableTask(object[] args);
        public delegate void WorkFinishedHandler(object sender, WorkFinishedEventArgs e);

        public event WorkFinishedHandler WorkFinished;

        public WorkerTask()
        {
        }

        public WorkerTask(RunnableTask task, WorkFinishedHandler handler, params object[] args)
        {
            this.Task = task;
            this.Arguments = args;
            if (handler!=null)
                this.WorkFinished += handler;
        }

        public RunnableTask Task { get; set; }
        public object[] Arguments { get; set; }
        public object Result { get; set; }

        public void Finish(object sender, WorkFinishedEventArgs e)
        {
            if (WorkFinished != null)
                WorkFinished(sender, e);
        }
    }
}
