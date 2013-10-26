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
            this.IngredientsCollection = new ObservableCollection<PizzaNetControls.IngredientsRow>();
            var c = new PizzaNetControls.IngredientsRow();
            var tb = new TextBlock();
            tb.Text="lololol";
            c.Children.Add(tb);
            this.IngredientsCollection.Add(c);
            this.DataContext = IngredientsCollection;
        }

        public ObservableCollection<PizzaNetControls.IngredientsRow> IngredientsCollection;
    }
}
