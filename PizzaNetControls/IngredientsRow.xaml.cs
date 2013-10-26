using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PizzaNetControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    [ContentProperty("Children")]
    public partial class IngredientsRow : UserControl
    {
        public IngredientsRow()
        {
            InitializeComponent();
        }

        public string IngredientName
        {
            get { return (string)GetValue(IngredientNameProperty); }
            set { SetValue(IngredientNameProperty, value); }
        }

        public static readonly DependencyProperty IngredientNameProperty =
            DependencyProperty.Register("IngredientName", typeof(string), typeof(IngredientsRow), new UIPropertyMetadata("Ingredient"));



        public int IngredientQuantity
        {
            get { return (int)GetValue(IngredientQuantityProperty); }
            set { SetValue(IngredientQuantityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IngredientQuantity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IngredientQuantityProperty =
            DependencyProperty.Register("IngredientQuantity", typeof(int), typeof(IngredientsRow), new UIPropertyMetadata(0));


    }
}
