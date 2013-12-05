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

namespace PizzaNetControls
{
    /// <summary>
    /// Interaction logic for OrdersRow.xaml
    /// </summary>
    public partial class OrdersRow : UserControl, INotifyPropertyChanged
    {
        public OrdersRow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public OrdersRow(Order order) : this()
        {
            this._order = order;
        }

        private Order _order = new Order() { State = new State() };
        public Order Order
        {
            get { return _order; }
            set { _order = value; NotifyPropertyChanged("Order"); }
        }

        public void Update()
        {
            NotifyPropertyChanged("Order");
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
    }
}
