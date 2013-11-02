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

namespace PizzaNetControls
{
    /// <summary>
    /// Interaction logic for OrderIngredientForm.xaml
    /// </summary>
    public partial class OrderIngredientForm : Window
    {
        private IngredientMonitor im = new IngredientMonitor();

        public ObservableCollection<Ingredient> Ingredients { get; set; }

        public OrderIngredientForm()
        {
            DataContext = this;
            InitializeComponent();
            Ingredients = new ObservableCollection<Ingredient>();
        }

        public OrderIngredientForm(ObservableCollection<Ingredient> ingredients)
        {
            DataContext = this;
            Ingredients = ingredients;
            InitializeComponent();
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
    }
}
