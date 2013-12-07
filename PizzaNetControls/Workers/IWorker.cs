using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Workers
{
    public interface IWorker
    {
        void EnqueueTask(WorkerTask task);

        event EventHandler<EventArgs> AllWorkDone;
    }
}
