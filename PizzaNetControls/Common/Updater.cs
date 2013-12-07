using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Monitors;
using PizzaNetDataModel.Model;
using System.Windows;
using PizzaNetControls.Workers;

namespace PizzaNetControls.Common
{
    public static class Updater<M, E>
        where M : IMonitor<E>
        where E : Entity
    {
        public static void Update(IWorker worker, M monitor, E entity)
        {
            if (!monitor.IsMonitoring()) return;
            if (!monitor.HasStateChanged(entity)) return;
            worker.EnqueueTask(new WorkerTask((args) =>
                {
                    monitor.Update(entity);
                    return null;
                }, null, null));
        }
    }
}
