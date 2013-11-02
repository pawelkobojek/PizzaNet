using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Monitors;
using PizzaNetDataModel.Model;
using System.Windows;

namespace PizzaNetControls
{
    public static class Updater<M, E>
        where M : IMonitor<E>
        where E : Entity
    {
        public static void Update(Window owner, M monitor, E entity)
        {
            if (!monitor.IsMonitoring()) return;
            if (!monitor.HasStateChanged(entity)) return;
            var worker = new Worker.WorkerWindow(owner, (args) =>
                {
                    monitor.Update(entity);
                    return null;
                }, null, null);
            worker.ShowDialog();
        }
    }
}
