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

namespace PizzaNetControls
{
    /// <summary>
    /// Interaction logic for RecipeControl.xaml
    /// </summary>
    public partial class RecipeControl : UserControl, INotifyPropertyChanged
    {
        public RecipeControl()
        {
            InitializeComponent();
            this.DataContext = this;
            RecipeName = "Unknown";
            Ingredients = new List<string>() { "Ingredient" };
            Prices = new PriceData() { PriceLow = 0, PriceMed = 0, PriceHigh = 0 };
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

        private string _recipeName = "";
        public string RecipeName
        {
            get { return _recipeName; }
            set { _recipeName = value; NotifyPropertyChanged("RecipeName"); }
        }

        private ICollection<string> _ingredients;
        public ICollection<string> Ingredients
        {
            get { return _ingredients; }
            set { _ingredients = value; NotifyPropertyChanged("Ingredients"); }
        }

        private PriceData _prices;
        public PriceData Prices
        {
            get { return _prices; }
            set { _prices = value; NotifyPropertyChanged("Prices"); }
        }
    }
}
