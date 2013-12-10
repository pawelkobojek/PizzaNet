using PizzaNetDataModel.Model;
using System;
using System.Collections.Generic;
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
using PizzaNetCommon.DTOs;

namespace PizzaNetControls
{
    /// <summary>
    /// Interaction logic for IngredientsListItem.xaml
    /// </summary>
    public partial class IngredientsListItem : UserControl, INotifyPropertyChanged
    {
        public IngredientsListItem()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public IngredientsListItem(SizeDTO size)
        {
            this.Size = size;
        }

        private OrderIngredientDTO _ingr;
        public OrderIngredientDTO OrderIngredient
        {
            get { return _ingr; }
            set { _ingr = value; NotifyPropertyChanged("OrderIngredient"); NotifyPropertyChanged("Weight"); }
        }

        public int Weight
        {
            get { return (int)(_ingr.Quantity*((_size!=null) ? _size.SizeValue : 0)); }
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

        private SizeDTO _size;
        public SizeDTO Size 
        {
            get { return _size; }
            set { _size = value; NotifyPropertyChanged("Size"); NotifyPropertyChanged("Weight"); }
        }
    }
}
