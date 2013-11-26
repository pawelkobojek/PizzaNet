using PizzaNetControls.Workers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Views
{
    public class MyOrdersView : BaseView, INotifyPropertyChanged
    {
        public MyOrdersView(IWorker worker) : base(worker)
        {
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
