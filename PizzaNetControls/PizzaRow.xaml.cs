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
using PizzaNetDataModel.Model;

namespace PizzaNetControls
{
    /// <summary>
    /// Interaction logic for PizzaRow.xaml
    /// </summary>
    public partial class PizzaRow : UserControl, INotifyPropertyChanged
    {
        public PizzaRow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public PizzaRow(OrderDetail orderDetail) : this()
        {
            this._orderDetail = orderDetail;
        }

        private OrderDetail _orderDetail = new OrderDetail();
        public OrderDetail OrderDetail
        {
            get { return _orderDetail; }
            set { _orderDetail = value; NotifyPropertyChanged("OrderDetail"); }
        }

        public void NotifyAll()
        {
            NotifyPropertyChanged("OrderDetail");
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
