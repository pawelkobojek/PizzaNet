using System;
using System.Collections.Generic;
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
    public partial class RecipeControl : UserControl
    {
        public RecipeControl()
        {
            InitializeComponent();
        }

        public string RecipeName
        {
            get { return (string)GetValue(RecipeNameProperty); }
            set { SetValue(RecipeNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecipeName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecipeNameProperty =
            DependencyProperty.Register("RecipeName", typeof(string), typeof(RecipeControl), new UIPropertyMetadata("RecipeName"));



        public ICollection<string> Ingredients
        {
            get { return (ICollection<string>)GetValue(IngredientsProperty); }
            set { SetValue(IngredientsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ingredients.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IngredientsProperty =
            DependencyProperty.Register("Ingredients", typeof(ICollection<string>), typeof(RecipeControl), new UIPropertyMetadata(new List<string>()));

        public PriceData Prices
        {
            get { return (PriceData)GetValue(PricesProperty); }
            set { SetValue(PricesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Prices.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PricesProperty =
            DependencyProperty.Register("Prices", typeof(PriceData), typeof(RecipeControl), new UIPropertyMetadata(PriceData.Empty));
    }
}
