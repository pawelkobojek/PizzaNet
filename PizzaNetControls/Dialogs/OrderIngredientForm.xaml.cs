using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PizzaNetDataModel.Model;
using PizzaNetDataModel.Monitors;
using PizzaNetDataModel.Repository;
using System.ComponentModel;
using System.Text.RegularExpressions;
using PizzaNetControls.Common;

namespace PizzaNetControls.Dialogs
{
    /// <summary>
    /// Interaction logic for OrderIngredientForm.xaml
    /// </summary>
    public partial class OrderIngredientForm : Window, INotifyPropertyChanged
    {
        private IngredientMonitor im = new IngredientMonitor();

        private ObservableCollection<Ingredient> _ingr;
        public ObservableCollection<Ingredient> Ingredients 
        {
            get { return _ingr; }
            set { _ingr = value; NotifyPropertyChanged("Ingredients"); }
        }

        public OrderIngredientForm()
        {
            InitializeComponent();
            DataContext = this;
            Ingredients = new ObservableCollection<Ingredient>();
        }

        public OrderIngredientForm(Window owner, ObservableCollection<Ingredient> ingredients) : this()
        {
            Ingredients = ingredients;
        }

        private void ButtonOrder_Click(object sender, RoutedEventArgs e)
        {
            Ingredient ing = (Ingredient)this.listBox.SelectedItem;
            Console.WriteLine(ing.Name);
            int quantity = int.Parse(textQuantity.Text);

            ing.StockQuantity += quantity;
            Updater<IngredientMonitor, Ingredient>.Update(this, im, ing);
            Console.WriteLine("Updated");
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            im.StartMonitor((Ingredient)listBox.SelectedItem);
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

        private void textQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumber(e.Text);
        }

        private static bool IsNumber(string text)
        {
            Regex regex = new Regex("^[0-9]+$");
            return regex.IsMatch(text);
        }
    }
}
