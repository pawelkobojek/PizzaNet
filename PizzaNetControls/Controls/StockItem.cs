using PizzaNetDataModel.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Controls
{
    public partial class StockItem : INotifyPropertyChanged
    {
        public StockItem(Ingredient t)
        {
            this._ingredient = t;
        }

        private Ingredient _ingredient;
        public Ingredient Ingredient
        {
            get { return _ingredient; }
            set { _ingredient = value; NotifyPropertyChanged("Ingredient"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
