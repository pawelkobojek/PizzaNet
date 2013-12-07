using PizzaNetControls.Configuration;
using PizzaNetControls.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Views
{
    public class ClientSettingsView : BaseView
    {
        public ClientSettingsView(IWorker worker) : base(worker)
        {
            
        }
        
        // TODO User property
        
        //private ClientConfig _config;
        //public ClientConfig Config
        //{
        //    get { return _config; }
        //    set { _config = value; NotifyPropertyChanged("Config"); }
        //}

        internal void SaveConfig()
        {
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                // MODIFIED Config.Save(ClientConfig.CONFIGNAME, typeof(ClientConfig));
                return null;
            }, null));
        }

        public void Load()
        {
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                //MODIFIED return ClientConfig.getConfig();
                return null;
            }, (s, args) =>
            {
                //MODIFIED Config = args.Result as ClientConfig;
            }));
        }
    }
}
