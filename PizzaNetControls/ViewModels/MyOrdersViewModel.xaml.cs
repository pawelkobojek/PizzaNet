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
using PizzaNetControls.Workers;

namespace PizzaNetControls.ViewModels
{
    /// <summary>
    /// Interaction logic for MyOrdersViewModel.xaml
    /// </summary>
    public partial class MyOrdersViewModel : UserControl, INotifyPropertyChanged
    {
        public MyOrdersViewModel()
        {
            this.DataContext = this;
            InitializeComponent();
            this.Loaded += MyOrdersViewModel_Loaded;
        }

        bool initialize = false;
        void MyOrdersViewModel_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            if (!initialize)
            {
                this.MyOrdersView = new MyOrdersView(Worker);
                initialize = true;
            }
        }

        public IWorker Worker
        {
            get { return (IWorker)GetValue(WorkerProperty); }
            set { SetValue(WorkerProperty, value); }
        }

        public static readonly DependencyProperty WorkerProperty =
            DependencyProperty.Register("Worker", typeof(IWorker), typeof(MyOrdersViewModel), new UIPropertyMetadata());

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
                NotifyPropertyChanged("MyOrdersView");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void GotFocusAction()
        {
            MyOrdersView.RefreshCurrentOrders();
        }

        private void ordersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ordersListView.SelectedIndex < 0)
                return;
            MyOrdersView.OrderSelectionChanged(ordersListView.SelectedIndex);
        }

        private void ingredientsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pizzasListView.SelectedIndex < 0)
                return;

            MyOrdersView.PizzaSelectionChanged(pizzasListView.SelectedIndex);
        }

        public bool LostFocusAction()
        {
            return true;
        }
    }
}
