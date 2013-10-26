using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PizzaNetClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PizzaNetWindow : Window
    {
        public PizzaNetWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.IngredientsCollection = new ObservableCollection<PizzaNetControls.IngredientsRow>();
            this.RecipesCollection = new ObservableCollection<PizzaNetControls.RecipeControl>();

            #region ExampleData
            var c = new PizzaNetControls.IngredientsRow();
            c.IngredientName = "Ingredient1";
            c.Background = new SolidColorBrush(Colors.LightSalmon);
            this.IngredientsCollection.Add(c);
            for (int i = 0; i < 10; i++)
            {
                c = new PizzaNetControls.IngredientsRow();
                c.IngredientName = "Mozzarella Cheese";
                c.IngredientQuantity = 100;
                c.Background = new SolidColorBrush(Colors.LightGreen);
                this.IngredientsCollection.Add(c);
            }

            PizzaNetControls.RecipeControl d;
            for (int i = 0; i < 10; i++)
            {
                d = new PizzaNetControls.RecipeControl();
                d.RecipeName = "MyRecipeName";
                d.Prices = new PizzaNetControls.PriceData() { PriceLow = 10, PriceMed = 20, PriceHigh = 30 };
                d.Ingredients = new List<string>() { "Mozarella Cheese", "Mushrooms", "Ingredient3" };
                d.Width = 300;
                this.RecipesCollection.Add(d);
            }
            #endregion
        }

        public ObservableCollection<PizzaNetControls.IngredientsRow> IngredientsCollection { get; set; }
        public ObservableCollection<PizzaNetControls.RecipeControl> RecipesCollection { get; set; }
    }
}
