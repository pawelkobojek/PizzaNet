using PizzaNetControls.Views;
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

namespace PizzaNetControls.ViewModels
{
    /// <summary>
    /// Interaction logic for MyOrdersViewModel.xaml
    /// </summary>
    public partial class MyOrdersViewModel : UserControl
    {
        public MyOrdersViewModel()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private MyOrdersView _vo;
        public MyOrdersView MyOrdersView
        {
            get
            {
                return _vo;
            }
            set
            {
                _vo = value;
                _vo.NotifyPropertyChanged("MyOrdersView");
            }
        }
    }
}
