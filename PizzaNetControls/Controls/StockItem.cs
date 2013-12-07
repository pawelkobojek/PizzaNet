using PizzaNetDataModel.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetCommon.DTOs;

namespace PizzaNetControls.Controls
{
    public partial class StockItem : INotifyPropertyChanged
    {
        public StockItem(Ingredient t)
        {
            this._ingredient = t;
        }

        public StockItem(StockIngredientDTO t)
        {
            this._ingredient = new Ingredient
            {
                ExtraWeight = t.ExtraWeight,
                IngredientID = t.IngredientID,
                Name = t.Name,
                NormalWeight = t.NormalWeight,
                PricePerUnit = t.PricePerUnit,
                StockQuantity = t.StockQuantity
            };
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
