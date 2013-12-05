using PizzaNetDataModel.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class IngredientsList : UserControl, INotifyPropertyChanged
    {
        public IngredientsList()
        {
            InitializeComponent();
            this.DataContext = this;
            this.IngredientsCollection = new ObservableCollection<IngredientsListItem>();
        }

        public IngredientsList(OrderDetail od) : this()
        {
            OrderDetail = od;
            foreach(var item in od.Ingredients)
            {
                IngredientsCollection.Add(new IngredientsListItem() { OrderIngredient = item, Size = od.Size });
            }
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

        public ObservableCollection<IngredientsListItem> IngredientsCollection { get; set; }

        private OrderDetail _od;
        public OrderDetail OrderDetail
        {
            get { return _od; }
            set { _od = value; NotifyPropertyChanged("OrderDetail"); }
        }
    }
}
