using PizzaNetDataModel.Model;
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
    public partial class PizzaNetClientWindow : Window
    {
        public PizzaNetClientWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.IngredientsCollection = new ObservableCollection<PizzaNetControls.IngredientsRow>();
            this.RecipesCollection = new ObservableCollection<PizzaNetControls.RecipeControl>();

            #region ExampleData
            var c = new PizzaNetControls.IngredientsRow(new Ingredient()
            {
                Name = "Ingredient1",
                NormalWeight = 200,
                ExtraWeight = 300
            });
            this.IngredientsCollection.Add(c);
            for (int i = 0; i < 10; i++)
            {
                c = new PizzaNetControls.IngredientsRow(new Ingredient()
                {
                    Name = "Mozzarella Cheese",
                    NormalWeight = 100,
                    ExtraWeight = 200
                });
                c.CurrentQuantity = c.Ingredient.NormalWeight;
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
