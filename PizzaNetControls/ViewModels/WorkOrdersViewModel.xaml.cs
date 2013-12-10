using PizzaNetControls.Views;
using PizzaNetControls.Workers;
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
    /// Interaction logic for WorkOrdersViewModel.xaml
    /// </summary>
    public partial class WorkOrdersViewModel : UserControl, INotifyPropertyChanged
    {
        public WorkOrdersViewModel()
        {
            this.DataContext = this;
            InitializeComponent();
            this.Loaded += WorkOrdersViewModel_Loaded;
        }

        bool initialized = false;

        void WorkOrdersViewModel_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            if (!initialized)
            {
                this.WorkOrdersView = new WorkOrdersView(Worker);
                //MODIFIED WorkOrdersView.RefreshOrders();
                initialized = true;
            }
        }

        private WorkOrdersView _vo;
        public WorkOrdersView WorkOrdersView
        {
            get { return _vo; }
            set { _vo = value; NotifyPropertyChanged("WorkOrdersView"); }
        }

        public IWorker Worker
        {
            get { return (IWorker)GetValue(WorkerProperty); }
            set { SetValue(WorkerProperty, value); }
        }

        public static readonly DependencyProperty WorkerProperty =
            DependencyProperty.Register("Worker", typeof(IWorker), typeof(WorkOrdersViewModel), new UIPropertyMetadata());

        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ordersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != ordersListView) return;
            if (ordersListView.SelectedIndex < 0) return;
            _vo.ChangeOrderSelection(ordersListView.SelectedIndex);
        }

        private void pizzasListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != pizzasListView) return;
            if (pizzasListView.SelectedIndex < 0) return;
            _vo.ChangePizzaSelection(pizzasListView.SelectedIndex);
        }

        private void ButtonSetInRealisation_Click(object sender, RoutedEventArgs e)
        {
            if (ordersListView.SelectedIndex < 0) return;
            _vo.SetOrderInRealization((OrdersRow)ordersListView.SelectedItem);
        }

        private void ButtonSetDone_Click(object sender, RoutedEventArgs e)
        {
            if (ordersListView.SelectedIndex < 0) return;
            _vo.SetOrderDone((OrdersRow)ordersListView.SelectedItem);
        }

        private void ButtonRemoveOrder_Click(object sender, RoutedEventArgs e)
        {
            if (ordersListView.SelectedIndex < 0) return;
            _vo.RemoveOrder(ordersListView.SelectedIndex);
        }

        public void GotFocusAction()
        {
            this._vo.SetAutoRefresh(true);
            this._vo.RefreshCurrentOrders();
        }

        public bool LostFocusAction()
        {
            this._vo.SetAutoRefresh(false);
            return true;
        }
    }
}
