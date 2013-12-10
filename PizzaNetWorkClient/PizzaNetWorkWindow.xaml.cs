using PizzaNetControls.Dialogs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            LastSelected = OrdersTab;
        }

        public TabItem LastSelected { get; set; }
        public bool IsSelectionChanging { get; set; }

        private void PizzaNetWindowClass_Loaded(object sender, RoutedEventArgs e)
        {
            FocusManager.SetFocusedElement(this, loginDialog);
            loginDialog.ModalDialogHidden += loginDialog_ModalDialogHidden;
            loginDialog.Show();
        }

        void loginDialog_ModalDialogHidden(object sender, ModalDialog.ModalDialogEventArgs e)
        {
            if (!loginDialog.DialogResult)
                this.Close();
            ordersViewModel.GotFocusAction();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.OriginalSource is TabControl) || !this.IsLoaded)
                return;

            if (IsSelectionChanging)
            {
                IsSelectionChanging = false;
                return;
            }

            if (LastSelected == OrdersTab)
            {
                if (!ordersViewModel.LostFocusAction())
                {
                    IsSelectionChanging = true;
                    tabControl.SelectedIndex = 0;
                    e.Handled = true;
                    return;
                }
            }

            if (LastSelected==StockTab)
            {
                if (!stockViewModel.LostFocusAction())
                {
                    IsSelectionChanging = true;
                    tabControl.SelectedIndex = 1;
                    e.Handled = true;
                    return;
                }

            }

            if (StockTab.IsSelected)
            {

                stockViewModel.GotFocusAction();
                LastSelected = StockTab;
            }

            if (OrdersTab.IsSelected)
            {
                ordersViewModel.GotFocusAction();
                LastSelected = OrdersTab;
            }

            if (RecipiesTab.IsSelected)
            {
                //TODO uncomment refresh recipiesViewModel.RecipiesView.RefreshRecipies();
                LastSelected = RecipiesTab;
            }

            if (UsersTab.IsSelected)
            {
                //TODO probably some loading data
                LastSelected = UsersTab;
            }
        }
    }
}

