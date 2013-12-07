using PizzaNetControls.Dialogs;
using System.Windows;
using System.Windows.Controls;

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

        private void PizzaNetWindowClass_Loaded(object sender, RoutedEventArgs e)
        {
            loginDialog.ModalDialogHidden += loginDialog_ModalDialogHidden;
            loginDialog.Show();
        }

        void loginDialog_ModalDialogHidden(object sender, ModalDialog.ModalDialogEventArgs e)
        {
            if (!loginDialog.DialogResult)
                this.Close();
            // TODO uncomment ordersViewModel.WorkOrdersView.Load();
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

