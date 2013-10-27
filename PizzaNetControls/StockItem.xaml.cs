using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PizzaNetDataModel.Model;

namespace PizzaNetControls
{
    /// <summary>
    /// Interaction logic for StockItem.xaml
    /// </summary>
    public partial class StockItem : UserControl, INotifyPropertyChanged
    {
        public StockItem()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public StockItem(Ingredient t)
        {
            StockItemName = t.Name;
            StockQuantity = t.StockQuantity;
            NormalWeight = t.NormalWeight;
            ExtraWeight = t.ExtraWeight;
            PricePerUnit = t.PricePerUnit;
        }

        private string _stockItemName = "";
        public string StockItemName
        {
            get { return _stockItemName; }
            set { _stockItemName = value; NotifyPropertyChanged("StockItemName"); }
        }

        private int _stockQuantity = 0;
        public int StockQuantity
        {
            get { return _stockQuantity; }
            set { _stockQuantity = value; NotifyPropertyChanged("StockQuantity"); }
        }

        private int _normalWeight = 0;
        public int NormalWeight
        {
            get { return _normalWeight; }
            set { _normalWeight = value; NotifyPropertyChanged("NormalWeight"); }
        }

        private int _extraWeight = 0;
        public int ExtraWeight
        {
            get { return _extraWeight; }
            set { _extraWeight = value; NotifyPropertyChanged("ExtraWeight"); }
        }

        private decimal _pricePerUnit = 0;
        public decimal PricePerUnit
        {
            get { return _pricePerUnit; }
            set { _pricePerUnit = value; NotifyPropertyChanged("PricePerUnit"); }
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
