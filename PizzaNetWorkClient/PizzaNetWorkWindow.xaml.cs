using System;
using System.Collections;
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
using System.Windows.Threading;
using PizzaNetControls;
using PizzaNetDataModel;
using PizzaNetDataModel.Model;
using PizzaNetDataModel.Repository;
using PizzaNetDataModel.Monitors;
using PizzaNetControls.Workers;
using System.Threading;
using System.Reflection;
using System.ComponentModel;
using PizzaNetControls.Dialogs;
using PizzaNetControls.Controls;
using PizzaNetControls.Common;

namespace PizzaNetWorkClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PizzaNetWorkWindow : Window
    {
        public PizzaNetWorkWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.worker.Lock = this.tabControl;
        }
        
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.OriginalSource is TabControl) || !this.IsLoaded)
                return;

            if (StockTab.IsSelected)
            {
                stockViewModel.StockView.RefreshStockItems();
            }

            if (OrdersTab.IsSelected)
            {
                ordersViewModel.WorkOrdersView.RefreshCurrentOrders();
            }

            if (RecipiesTab.IsSelected)
            {
                recipiesViewModel.RecipiesView.RefreshRecipies();
            }
        }
    }
}
