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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PizzaNetControls
{
    /// <summary>
    /// Interaction logic for IngredientsListRow.xaml
    /// </summary>
    public partial class IngredientsList : UserControl
    {
        public IngredientsList()
        {
            InitializeComponent();
            this.DataContext = this;
            this.IngredientsCollection = new ObservableCollection<IngredientsListItem>();
        }

        public IngredientsList(ObservableCollection<IngredientsRow> ingredients) : this()
        {
            foreach(var item in ingredients)
            {
                IngredientsCollection.Add(new IngredientsListItem() { Ingredient = item.Ingredient, Quantity = item.CurrentQuantity });
            }
        }

        public ObservableCollection<IngredientsListItem> IngredientsCollection { get; set; }
    }
}
